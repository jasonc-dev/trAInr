/**
 * SQLite Database Manager
 * Handles database initialization and migrations
 */

import * as SQLite from "expo-sqlite";
import { createTablesSQL } from "./schema";

let db: SQLite.SQLiteDatabase | null = null;

export async function getDatabase(): Promise<SQLite.SQLiteDatabase> {
  if (!db) {
    db = await SQLite.openDatabaseAsync("trainr.db");
    await initializeDatabase();
  }
  return db;
}

async function initializeDatabase(): Promise<void> {
  if (!db) return;

  // Enable foreign keys
  await db.execAsync("PRAGMA foreign_keys = ON;");

  // Create tables
  await db.execAsync(createTablesSQL);

  console.log("Database initialized");
}

export async function closeDatabase(): Promise<void> {
  if (db) {
    await db.closeAsync();
    db = null;
  }
}

// Helper function for transactions
export async function runTransaction<T>(
  callback: (db: SQLite.SQLiteDatabase) => Promise<T>,
): Promise<T> {
  const database = await getDatabase();
  return await database.withTransactionAsync(async () => {
    return await callback(database);
  });
}
