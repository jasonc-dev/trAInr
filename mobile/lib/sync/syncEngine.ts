/**
 * Sync Engine
 * Handles offline-first sync with the backend
 */

import { getDatabase } from "../db/database";
import { apiClient } from "../api/client";
import { v4 as uuidv4 } from "uuid";

export interface SyncChange {
  idempotencyKey: string;
  entityType: string;
  entityId: string;
  operation: "create" | "update" | "delete";
  data?: unknown;
  clientTimestamp: string;
}

export interface SyncResult {
  idempotencyKey: string;
  entityId: string;
  status:
    | "Success"
    | "Conflict"
    | "NotFound"
    | "ValidationError"
    | "AlreadyProcessed";
  errorMessage?: string;
  serverData?: unknown;
}

class SyncEngine {
  private isSyncing = false;
  private syncInterval: NodeJS.Timeout | null = null;

  async queueChange(
    entityType: string,
    entityId: string,
    operation: "create" | "update" | "delete",
    payload: unknown
  ): Promise<void> {
    const db = await getDatabase();
    const idempotencyKey = uuidv4();
    const now = Date.now();

    await db.runAsync(
      `INSERT INTO sync_queue (id, entity_type, entity_id, operation, payload, idempotency_key, created_at)
       VALUES (?, ?, ?, ?, ?, ?, ?)`,
      [
        uuidv4(),
        entityType,
        entityId,
        operation,
        JSON.stringify(payload),
        idempotencyKey,
        now,
      ]
    );

    // Trigger sync
    this.triggerSync();
  }

  async triggerSync(): Promise<void> {
    if (this.isSyncing) return;

    try {
      this.isSyncing = true;
      await this.pushChanges();
      await this.pullChanges();
    } catch (error) {
      console.error("Sync failed:", error);
    } finally {
      this.isSyncing = false;
    }
  }

  private async pushChanges(): Promise<void> {
    const db = await getDatabase();

    // Get pending changes
    const pending = await db.getAllAsync<{
      id: string;
      entity_type: string;
      entity_id: string;
      operation: string;
      payload: string;
      idempotency_key: string;
      created_at: number;
    }>(
      `SELECT * FROM sync_queue WHERE synced_at IS NULL ORDER BY created_at ASC LIMIT 50`
    );

    if (pending.length === 0) return;

    const changes: SyncChange[] = pending.map((p) => ({
      idempotencyKey: p.idempotency_key,
      entityType: p.entity_type,
      entityId: p.entity_id,
      operation: p.operation as "create" | "update" | "delete",
      data: JSON.parse(p.payload),
      clientTimestamp: new Date(p.created_at).toISOString(),
    }));

    try {
      const response = await apiClient.post<{ results: SyncResult[] }>(
        "/sync/push",
        {
          changes,
        }
      );

      const now = Date.now();
      for (const result of response.data.results) {
        if (
          result.status === "Success" ||
          result.status === "AlreadyProcessed"
        ) {
          await db.runAsync(
            `UPDATE sync_queue SET synced_at = ? WHERE idempotency_key = ?`,
            [now, result.idempotencyKey]
          );
        } else {
          await db.runAsync(
            `UPDATE sync_queue SET retry_count = retry_count + 1, last_error = ? WHERE idempotency_key = ?`,
            [result.errorMessage || "Unknown error", result.idempotencyKey]
          );
        }
      }
    } catch (error) {
      console.error("Failed to push changes:", error);
    }
  }

  private async pullChanges(): Promise<void> {
    const db = await getDatabase();

    // Get last sync timestamp
    const lastSync = await db.getFirstAsync<{ value: string }>(
      `SELECT value FROM sync_state WHERE key = 'last_pull_timestamp'`
    );

    try {
      const response = await apiClient.get<{
        serverTimestamp: string;
        changes: SyncChange[];
      }>("/sync/pull", {
        params: {
          since: lastSync?.value,
        },
      });

      // Apply changes to local database
      for (const change of response.data.changes) {
        await this.applyChange(change);
      }

      // Update last sync timestamp
      await db.runAsync(
        `INSERT OR REPLACE INTO sync_state (key, value) VALUES ('last_pull_timestamp', ?)`,
        [response.data.serverTimestamp]
      );
    } catch (error) {
      console.error("Failed to pull changes:", error);
    }
  }

  private async applyChange(change: SyncChange): Promise<void> {
    const db = await getDatabase();

    switch (change.entityType) {
      case "exercise_sets":
        await this.applyExerciseSetChange(db, change);
        break;
      case "workout_exercises":
        await this.applyWorkoutExerciseChange(db, change);
        break;
      case "workout_days":
        await this.applyWorkoutDayChange(db, change);
        break;
    }
  }

  private async applyExerciseSetChange(
    db: Awaited<ReturnType<typeof getDatabase>>,
    change: SyncChange
  ): Promise<void> {
    const data = change.data as Record<string, unknown>;

    if (change.operation === "delete") {
      await db.runAsync(`DELETE FROM exercise_sets WHERE id = ?`, [
        change.entityId,
      ]);
    } else {
      await db.runAsync(
        `INSERT OR REPLACE INTO exercise_sets 
         (id, workout_exercise_id, set_number, reps, weight, is_completed, completed_at, notes, synced_at)
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
          change.entityId,
          data.workoutExerciseId,
          data.setNumber,
          data.reps,
          data.weight,
          data.isCompleted ? 1 : 0,
          data.completedAt,
          data.notes,
          Date.now(),
        ]
      );
    }
  }

  private async applyWorkoutExerciseChange(
    db: Awaited<ReturnType<typeof getDatabase>>,
    change: SyncChange
  ): Promise<void> {
    // Simplified implementation
    console.log("Applying workout exercise change:", change);
  }

  private async applyWorkoutDayChange(
    db: Awaited<ReturnType<typeof getDatabase>>,
    change: SyncChange
  ): Promise<void> {
    // Simplified implementation
    console.log("Applying workout day change:", change);
  }

  startAutoSync(intervalMs: number = 30000): void {
    if (this.syncInterval) return;

    this.syncInterval = setInterval(() => {
      this.triggerSync();
    }, intervalMs);
  }

  stopAutoSync(): void {
    if (this.syncInterval) {
      clearInterval(this.syncInterval);
      this.syncInterval = null;
    }
  }

  async getPendingChangesCount(): Promise<number> {
    const db = await getDatabase();
    const result = await db.getFirstAsync<{ count: number }>(
      `SELECT COUNT(*) as count FROM sync_queue WHERE synced_at IS NULL`
    );
    return result?.count || 0;
  }
}

export const syncEngine = new SyncEngine();
