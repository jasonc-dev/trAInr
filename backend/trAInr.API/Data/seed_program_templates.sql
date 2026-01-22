-- SQL script to seed pre-made program templates
-- This script creates 3 complete pre-made programmes that users can clone
-- It safely replaces any existing data without creating duplicates

BEGIN;

-- Clear existing data to prevent duplicates
DELETE FROM "ProgramTemplateWorkoutExercises";
DELETE FROM "ProgramTemplateWorkoutDays";
DELETE FROM "ProgramTemplateWeeks";
DELETE FROM "ProgramTemplates";

-- Insert Beginner Program Template (<1 year experience)
-- 4 weeks, 3 days (push/pull/lower), LevelOfDifficulty=1 exercises

-- Beginner Program Template
INSERT INTO "ProgramTemplates" (
    "Id",
    "Name",
    "Description",
    "DurationWeeks",
    "ExperienceLevel",
    "IsActive",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    '11111111-1111-1111-1111-111111111111',
    'Beginner Strength Program',
    'A 4-week beginner strength program designed for individuals with less than 1 year of training experience. Focuses on building fundamental strength with compound movements and proper form. 3 training days per week with progressive overload.',
    4,
    1, -- Beginner
    true,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone
);

-- Intermediate Program Template
INSERT INTO "ProgramTemplates" (
    "Id",
    "Name",
    "Description",
    "DurationWeeks",
    "ExperienceLevel",
    "IsActive",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    '22222222-2222-2222-2222-222222222222',
    'Intermediate Strength Program',
    'A 6-week intermediate strength program designed for individuals with 1-3 years of training experience. Builds on fundamental movements with increased intensity and volume. 4 training days per week.',
    6,
    2, -- Intermediate
    true,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone
);

-- Advanced Program Template
INSERT INTO "ProgramTemplates" (
    "Id",
    "Name",
    "Description",
    "DurationWeeks",
    "ExperienceLevel",
    "IsActive",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    '33333333-3333-3333-3333-333333333333',
    'Advanced Strength Program',
    'An 8-week advanced strength program designed for individuals with 3+ years of training experience. High-intensity training for experienced lifters with complex movements and higher volume. 4 training days per week.',
    8,
    3, -- Advanced
    true,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone,
    '2026-01-18 00:00:00.000 +0000'::timestamp with time zone
);

-- ========================================
-- BEGINNER PROGRAMME WEEKS (4 weeks)
-- ========================================

-- Beginner Programme Weeks
INSERT INTO "ProgramTemplateWeeks" (
    "Id",
    "ProgramTemplateId",
    "WeekNumber",
    "Notes",
    "CreatedAt"
) VALUES
    ('11111111-1111-1111-1111-111111111112', '11111111-1111-1111-1111-111111111111', 1, 'Focus on learning proper form and building confidence', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111113', '11111111-1111-1111-1111-111111111111', 2, 'Increase weights slightly while maintaining form', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111114', '11111111-1111-1111-1111-111111111111', 3, 'Build momentum and consistency', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111115', '11111111-1111-1111-1111-111111111111', 4, 'Focus on progressive overload', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- Beginner Programme Workout Days (Same structure each week)
INSERT INTO "ProgramTemplateWorkoutDays" (
    "Id",
    "ProgramTemplateWeekId",
    "Name",
    "Description",
    "IsRestDay",
    "CreatedAt"
) VALUES
    -- Week 1
    ('11111111-1111-1111-1111-111111111116', '11111111-1111-1111-1111-111111111112', 'Push Day', 'Chest, shoulders, and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111117', '11111111-1111-1111-1111-111111111112', 'Pull Day', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111118', '11111111-1111-1111-1111-111111111112', 'Lower Body Day', 'Legs and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2
    ('11111111-1111-1111-1111-111111111119', '11111111-1111-1111-1111-111111111113', 'Push Day', 'Chest, shoulders, and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111120', '11111111-1111-1111-1111-111111111113', 'Pull Day', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111121', '11111111-1111-1111-1111-111111111113', 'Lower Body Day', 'Legs and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3
    ('11111111-1111-1111-1111-111111111122', '11111111-1111-1111-1111-111111111114', 'Push Day', 'Chest, shoulders, and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111123', '11111111-1111-1111-1111-111111111114', 'Pull Day', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111124', '11111111-1111-1111-1111-111111111114', 'Lower Body Day', 'Legs and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4
    ('11111111-1111-1111-1111-111111111125', '11111111-1111-1111-1111-111111111115', 'Push Day', 'Chest, shoulders, and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111126', '11111111-1111-1111-1111-111111111115', 'Pull Day', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111127', '11111111-1111-1111-1111-111111111115', 'Lower Body Day', 'Legs and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- Beginner Programme Exercises
-- PUSH DAY EXERCISES (Chest, Shoulders, Triceps - LevelOfDifficulty=1)
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Week 1 Push Day
    ('11111111-1111-1111-1111-111111111128', '11111111-1111-1111-1111-111111111116', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - focus on chest activation', 3, 10, 40, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111129', '11111111-1111-1111-1111-111111111116', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - build forearm strength', 3, 12, 15, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111130', '11111111-1111-1111-1111-111111111116', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 8, 20, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2 Push Day
    ('11111111-1111-1111-1111-111111111131', '11111111-1111-1111-1111-111111111119', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - increase weight slightly', 3, 10, 45, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111132', '11111111-1111-1111-1111-111111111119', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - build forearm strength', 3, 12, 17.5, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111133', '11111111-1111-1111-1111-111111111119', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 8, 22.5, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3 Push Day
    ('11111111-1111-1111-1111-111111111134', '11111111-1111-1111-1111-111111111122', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - focus on form', 3, 10, 50, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111135', '11111111-1111-1111-1111-111111111122', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - build forearm strength', 3, 12, 20, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111136', '11111111-1111-1111-1111-111111111122', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 8, 25, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4 Push Day
    ('11111111-1111-1111-1111-111111111137', '11111111-1111-1111-1111-111111111125', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - progressive overload', 3, 10, 55, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111138', '11111111-1111-1111-1111-111111111125', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - build forearm strength', 3, 12, 22.5, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111139', '11111111-1111-1111-1111-111111111125', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 8, 27.5, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- PULL DAY EXERCISES (Back, Biceps - LevelOfDifficulty=1)
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Week 1 Pull Day
    ('11111111-1111-1111-1111-111111111140', '11111111-1111-1111-1111-111111111117', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 1, 'Wide-grip curl - focus on biceps peak', 3, 12, 12.5, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111141', '11111111-1111-1111-1111-111111111117', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 2, 'Machine high row - build back strength', 3, 10, 35, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111142', '11111111-1111-1111-1111-111111111117', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - isolate biceps', 3, 10, 10, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2 Pull Day
    ('11111111-1111-1111-1111-111111111143', '11111111-1111-1111-1111-111111111120', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 1, 'Wide-grip curl - increase weight slightly', 3, 12, 15, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111144', '11111111-1111-1111-1111-111111111120', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 2, 'Machine high row - build back strength', 3, 10, 40, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111145', '11111111-1111-1111-1111-111111111120', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - isolate biceps', 3, 10, 12.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3 Pull Day
    ('11111111-1111-1111-1111-111111111146', '11111111-1111-1111-1111-111111111123', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 1, 'Wide-grip curl - focus on form', 3, 12, 17.5, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111147', '11111111-1111-1111-1111-111111111123', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 2, 'Machine high row - build back strength', 3, 10, 45, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111148', '11111111-1111-1111-1111-111111111123', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - isolate biceps', 3, 10, 15, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4 Pull Day
    ('11111111-1111-1111-1111-111111111149', '11111111-1111-1111-1111-111111111126', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 1, 'Wide-grip curl - progressive overload', 3, 12, 20, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111150', '11111111-1111-1111-1111-111111111126', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 2, 'Machine high row - build back strength', 3, 10, 50, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111151', '11111111-1111-1111-1111-111111111126', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - isolate biceps', 3, 10, 17.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- LOWER BODY DAY EXERCISES (Legs, Core - LevelOfDifficulty=1)
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Week 1 Lower Body Day
    ('11111111-1111-1111-1111-111111111152', '11111111-1111-1111-1111-111111111118', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - build calf strength', 3, 15, 50, 60, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111153', '11111111-1111-1111-1111-111111111118', '87521926-cbeb-40e8-9eaf-731c6d85f2d1', 2, 'Decline reverse crunch - target lower abs', 3, 12, 0, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111154', '11111111-1111-1111-1111-111111111118', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 3, 'Pallof press - core stability', 3, 10, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2 Lower Body Day
    ('11111111-1111-1111-1111-111111111155', '11111111-1111-1111-1111-111111111121', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - increase weight slightly', 3, 15, 60, 60, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111156', '11111111-1111-1111-1111-111111111121', '87521926-cbeb-40e8-9eaf-731c6d85f2d1', 2, 'Decline reverse crunch - target lower abs', 3, 12, 0, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111157', '11111111-1111-1111-1111-111111111121', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 3, 'Pallof press - core stability', 3, 10, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3 Lower Body Day
    ('11111111-1111-1111-1111-111111111158', '11111111-1111-1111-1111-111111111124', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - focus on form', 3, 15, 70, 60, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111159', '11111111-1111-1111-1111-111111111124', '87521926-cbeb-40e8-9eaf-731c6d85f2d1', 2, 'Decline reverse crunch - target lower abs', 3, 12, 0, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111160', '11111111-1111-1111-1111-111111111124', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 3, 'Pallof press - core stability', 3, 10, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4 Lower Body Day
    ('11111111-1111-1111-1111-111111111161', '11111111-1111-1111-1111-111111111127', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - progressive overload', 3, 15, 80, 60, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111162', '11111111-1111-1111-1111-111111111127', '87521926-cbeb-40e8-9eaf-731c6d85f2d1', 2, 'Decline reverse crunch - target lower abs', 3, 12, 0, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('11111111-1111-1111-1111-111111111163', '11111111-1111-1111-1111-111111111127', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 3, 'Pallof press - core stability', 3, 10, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ========================================
-- INTERMEDIATE PROGRAMME WEEKS (6 weeks)
-- ========================================

-- Intermediate Programme Weeks
INSERT INTO "ProgramTemplateWeeks" (
    "Id",
    "ProgramTemplateId",
    "WeekNumber",
    "Notes",
    "CreatedAt"
) VALUES
    ('22222222-2222-2222-2222-222222222201', '22222222-2222-2222-2222-222222222222', 1, 'Build strength foundation with moderate intensity', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222202', '22222222-2222-2222-2222-222222222222', 2, 'Increase volume and intensity', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222203', '22222222-2222-2222-2222-222222222222', 3, 'Focus on technique and recovery', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222204', '22222222-2222-2222-2222-222222222222', 4, 'Progressive overload phase', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222205', '22222222-2222-2222-2222-222222222222', 5, 'Strength consolidation', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222206', '22222222-2222-2222-2222-222222222222', 6, 'Peak strength and recovery', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- Intermediate Programme Workout Days (4 days per week)
INSERT INTO "ProgramTemplateWorkoutDays" (
    "Id",
    "ProgramTemplateWeekId",
    "Name",
    "Description",
    "IsRestDay",
    "CreatedAt"
) VALUES
    -- Week 1
    ('22222222-2222-2222-2222-222222222207', '22222222-2222-2222-2222-222222222201', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222208', '22222222-2222-2222-2222-222222222201', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222209', '22222222-2222-2222-2222-222222222201', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222210', '22222222-2222-2222-2222-222222222201', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2
    ('22222222-2222-2222-2222-222222222211', '22222222-2222-2222-2222-222222222202', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222212', '22222222-2222-2222-2222-222222222202', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222213', '22222222-2222-2222-2222-222222222202', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222214', '22222222-2222-2222-2222-222222222202', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3
    ('22222222-2222-2222-2222-222222222215', '22222222-2222-2222-2222-222222222203', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222216', '22222222-2222-2222-2222-222222222203', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222217', '22222222-2222-2222-2222-222222222203', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222218', '22222222-2222-2222-2222-222222222203', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4
    ('22222222-2222-2222-2222-222222222219', '22222222-2222-2222-2222-222222222204', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222220', '22222222-2222-2222-2222-222222222204', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222221', '22222222-2222-2222-2222-222222222204', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222204', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 5
    ('22222222-2222-2222-2222-222222222223', '22222222-2222-2222-2222-222222222205', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222224', '22222222-2222-2222-2222-222222222205', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222225', '22222222-2222-2222-2222-222222222205', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222226', '22222222-2222-2222-2222-222222222205', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 6
    ('22222222-2222-2222-2222-222222222227', '22222222-2222-2222-2222-222222222206', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222228', '22222222-2222-2222-2222-222222222206', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222229', '22222222-2222-2222-2222-222222222206', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-2222-2222-2222-222222222230', '22222222-2222-2222-2222-222222222206', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ========================================
-- INTERMEDIATE PROGRAMME EXERCISES (6 weeks, 4 days)
-- ========================================

-- INTERMEDIATE Chest & Triceps Day (Week 1-6)
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Chest & Triceps - Week 1
    ('22222222-3333-4444-5555-666666666601', '22222222-2222-2222-2222-222222222207', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - chest focus', 4, 10, 150, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666602', '22222222-2222-2222-2222-222222222207', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 12, 120, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666603', '22222222-2222-2222-2222-222222222207', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Week 2
    ('22222222-3333-4444-5555-666666666604', '22222222-2222-2222-2222-222222222211', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - progressive', 4, 9, 160, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666605', '22222222-2222-2222-2222-222222222211', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 11, 130, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666606', '22222222-2222-2222-2222-222222222211', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Week 3
    ('22222222-3333-4444-5555-666666666607', '22222222-2222-2222-2222-222222222215', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - strength focus', 4, 8, 180, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666608', '22222222-2222-2222-2222-222222222215', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 10, 140, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666609', '22222222-2222-2222-2222-222222222215', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Week 4
    ('22222222-3333-4444-5555-666666666610', '22222222-2222-2222-2222-222222222219', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - overload', 4, 7, 200, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666611', '22222222-2222-2222-2222-222222222219', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 9, 150, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666612', '22222222-2222-2222-2222-222222222219', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Week 5
    ('22222222-3333-4444-5555-666666666613', '22222222-2222-2222-2222-222222222223', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - peak strength', 4, 6, 220, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666614', '22222222-2222-2222-2222-222222222223', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 8, 160, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666615', '22222222-2222-2222-2222-222222222223', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Week 6
    ('22222222-3333-4444-5555-666666666616', '22222222-2222-2222-2222-222222222227', '6083a676-2af7-4dfc-822c-9f54a925c0c9', 1, 'Wide-grip bench press - final peak', 4, 5, 240, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666617', '22222222-2222-2222-2222-222222222227', '5c1b239e-1060-46fc-8e3a-de01c4a54a10', 2, 'Standing reverse curl - triceps', 3, 7, 180, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666618', '22222222-2222-2222-2222-222222222227', '1466efe3-f400-417e-8358-068a14f35619', 3, 'Plank with biceps curl - core stability', 3, 10, 90, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- INTERMEDIATE Back & Biceps Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Back & Biceps - Week 1
    ('22222222-3333-4444-5555-666666666619', '22222222-2222-2222-2222-222222222208', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 10, 60, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666620', '22222222-2222-2222-2222-222222222208', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 12, 20, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666621', '22222222-2222-2222-2222-222222222208', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 12, 15, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Back & Biceps - Week 2
    ('22222222-3333-4444-5555-666666666622', '22222222-2222-2222-2222-222222222212', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 10, 65, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666623', '22222222-2222-2222-2222-222222222212', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 12, 22.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666624', '22222222-2222-2222-2222-222222222212', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 12, 17.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Back & Biceps - Week 3
    ('22222222-3333-4444-5555-666666666625', '22222222-2222-2222-2222-222222222216', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 10, 70, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666626', '22222222-2222-2222-2222-222222222216', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 12, 25, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666627', '22222222-2222-2222-2222-222222222216', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 12, 20, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Back & Biceps - Week 4
    ('22222222-3333-4444-5555-666666666628', '22222222-2222-2222-2222-222222222220', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 10, 75, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666629', '22222222-2222-2222-2222-222222222220', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 12, 27.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666630', '22222222-2222-2222-2222-222222222220', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 12, 22.5, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Back & Biceps - Week 5
    ('22222222-3333-4444-5555-666666666631', '22222222-2222-2222-2222-222222222224', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 8, 80, 130, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666632', '22222222-2222-2222-2222-222222222224', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 10, 30, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666633', '22222222-2222-2222-2222-222222222224', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 10, 25, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Back & Biceps - Week 6
    ('22222222-3333-4444-5555-666666666634', '22222222-2222-2222-2222-222222222228', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back development', 4, 8, 85, 130, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666635', '22222222-2222-2222-2222-222222222228', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 3, 10, 32.5, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666636', '22222222-2222-2222-2222-222222222228', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 10, 27.5, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- INTERMEDIATE Legs Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Legs - Week 1-6 (same exercises each week with progression)
    ('22222222-3333-4444-5555-666666666637', '22222222-2222-2222-2222-222222222209', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 15, 80, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666638', '22222222-2222-2222-2222-222222222209', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 15, 80, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666639', '22222222-2222-2222-2222-222222222209', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 15, 80, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666640', '22222222-2222-2222-2222-222222222213', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 15, 85, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666641', '22222222-2222-2222-2222-222222222213', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 15, 85, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666642', '22222222-2222-2222-2222-222222222213', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 15, 85, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666643', '22222222-2222-2222-2222-222222222217', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 15, 90, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666644', '22222222-2222-2222-2222-222222222217', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 15, 90, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666645', '22222222-2222-2222-2222-222222222217', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 15, 90, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666646', '22222222-2222-2222-2222-222222222221', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 15, 95, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666647', '22222222-2222-2222-2222-222222222221', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 15, 95, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666648', '22222222-2222-2222-2222-222222222221', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 15, 95, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666649', '22222222-2222-2222-2222-222222222225', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 12, 100, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666650', '22222222-2222-2222-2222-222222222225', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 12, 100, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666651', '22222222-2222-2222-2222-222222222225', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 12, 100, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666652', '22222222-2222-2222-2222-222222222229', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - calf development', 4, 12, 105, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666653', '22222222-2222-2222-2222-222222222229', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 2, 'Machine calf press - calf development', 4, 12, 105, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666654', '22222222-2222-2222-2222-222222222229', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 3, 'Machine calf press - calf development', 4, 12, 105, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- INTERMEDIATE Shoulders & Core Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Shoulders & Core - Week 1-6 (same exercises each week with progression)
    ('22222222-3333-4444-5555-666666666655', '22222222-2222-2222-2222-222222222210', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 12, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666656', '22222222-2222-2222-2222-222222222210', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 30, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666657', '22222222-2222-2222-2222-222222222210', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 15, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666658', '22222222-2222-2222-2222-222222222214', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 12, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666659', '22222222-2222-2222-2222-222222222214', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 30, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666660', '22222222-2222-2222-2222-222222222214', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 15, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666661', '22222222-2222-2222-2222-222222222218', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 12, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666662', '22222222-2222-2222-2222-222222222218', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 35, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666663', '22222222-2222-2222-2222-222222222218', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 16, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666664', '22222222-2222-2222-2222-222222222222', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 12, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666665', '22222222-2222-2222-2222-222222222222', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 35, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666666', '22222222-2222-2222-2222-222222222222', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 16, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666667', '22222222-2222-2222-2222-222222222226', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 10, 0, 70, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666668', '22222222-2222-2222-2222-222222222226', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 40, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666669', '22222222-2222-2222-2222-222222222226', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 18, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666670', '22222222-2222-2222-2222-222222222230', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - anti-rotation core stability', 3, 10, 0, 70, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666671', '22222222-2222-2222-2222-222222222230', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - core endurance', 3, 40, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('22222222-3333-4444-5555-666666666672', '22222222-2222-2222-2222-222222222230', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - oblique work', 3, 18, 0, 60, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ========================================
-- ADVANCED PROGRAMME WEEKS (8 weeks)
-- ========================================

-- Advanced Programme Weeks
INSERT INTO "ProgramTemplateWeeks" (
    "Id",
    "ProgramTemplateId",
    "WeekNumber",
    "Notes",
    "CreatedAt"
) VALUES
    ('33333333-3333-3333-3333-333333333301', '33333333-3333-3333-3333-333333333333', 1, 'High-intensity strength foundation', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333302', '33333333-3333-3333-3333-333333333333', 2, 'Power and strength development', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333303', '33333333-3333-3333-3333-333333333333', 3, 'Strength consolidation', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333304', '33333333-3333-3333-3333-333333333333', 4, 'Peak strength phase', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333305', '33333333-3333-3333-3333-333333333333', 5, 'Advanced techniques introduction', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333306', '33333333-3333-3333-3333-333333333333', 6, 'Strength maintenance', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333307', '33333333-3333-3333-3333-333333333333', 7, 'Recovery and adaptation', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333308', '33333333-3333-3333-3333-333333333333', 8, 'Peak performance and recovery', '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- Advanced Programme Workout Days (4 days per week)
INSERT INTO "ProgramTemplateWorkoutDays" (
    "Id",
    "ProgramTemplateWeekId",
    "Name",
    "Description",
    "IsRestDay",
    "CreatedAt"
) VALUES
    -- Week 1
    ('33333333-3333-3333-3333-333333333309', '33333333-3333-3333-3333-333333333301', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333310', '33333333-3333-3333-3333-333333333301', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333311', '33333333-3333-3333-3333-333333333301', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333312', '33333333-3333-3333-3333-333333333301', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 2
    ('33333333-3333-3333-3333-333333333313', '33333333-3333-3333-3333-333333333302', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333314', '33333333-3333-3333-3333-333333333302', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333315', '33333333-3333-3333-3333-333333333302', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333316', '33333333-3333-3333-3333-333333333302', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 3
    ('33333333-3333-3333-3333-333333333317', '33333333-3333-3333-3333-333333333303', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333318', '33333333-3333-3333-3333-333333333303', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333319', '33333333-3333-3333-3333-333333333303', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333320', '33333333-3333-3333-3333-333333333303', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 4
    ('33333333-3333-3333-3333-333333333321', '33333333-3333-3333-3333-333333333304', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333322', '33333333-3333-3333-3333-333333333304', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333323', '33333333-3333-3333-3333-333333333304', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333324', '33333333-3333-3333-3333-333333333304', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 5
    ('33333333-3333-3333-3333-333333333325', '33333333-3333-3333-3333-333333333305', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333326', '33333333-3333-3333-3333-333333333305', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333327', '33333333-3333-3333-3333-333333333305', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333328', '33333333-3333-3333-3333-333333333305', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 6
    ('33333333-3333-3333-3333-333333333329', '33333333-3333-3333-3333-333333333306', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333330', '33333333-3333-3333-3333-333333333306', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333331', '33333333-3333-3333-3333-333333333306', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333332', '33333333-3333-3333-3333-333333333306', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 7
    ('33333333-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333307', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333334', '33333333-3333-3333-3333-333333333307', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333335', '33333333-3333-3333-3333-333333333307', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333336', '33333333-3333-3333-3333-333333333307', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Week 8
    ('33333333-3333-3333-3333-333333333337', '33333333-3333-3333-3333-333333333308', 'Chest & Triceps', 'Chest and triceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333338', '33333333-3333-3333-3333-333333333308', 'Back & Biceps', 'Back and biceps focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333339', '33333333-3333-3333-3333-333333333308', 'Legs', 'Lower body focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-3333-3333-3333-333333333340', '33333333-3333-3333-3333-333333333308', 'Shoulders & Core', 'Shoulders and core focus', false, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ========================================
-- ADVANCED PROGRAMME EXERCISES (8 weeks)
-- ========================================

-- ADVANCED Chest & Triceps Day (Higher intensity, lower reps)
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Chest & Triceps - Weeks 1-4 (Strength focus)
    ('33333333-4444-5555-6666-777777777701', '33333333-3333-3333-3333-333333333309', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - heavy strength work', 5, 5, 150, 240, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777702', '33333333-3333-3333-3333-333333333309', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps strength', 4, 6, 120, 210, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777703', '33333333-3333-3333-3333-333333333309', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest strength', 4, 6, 130, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777704', '33333333-3333-3333-3333-333333333313', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - progressive overload', 5, 5, 160, 240, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777705', '33333333-3333-3333-3333-333333333313', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps strength', 4, 6, 125, 210, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777706', '33333333-3333-3333-3333-333333333313', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest strength', 4, 6, 140, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777707', '33333333-3333-3333-3333-333333333317', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - peak strength', 5, 4, 170, 270, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777708', '33333333-3333-3333-3333-333333333317', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps strength', 4, 5, 130, 240, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777709', '33333333-3333-3333-3333-333333333317', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest strength', 4, 5, 150, 240, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777710', '33333333-3333-3333-3333-333333333321', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - max strength', 5, 3, 180, 300, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777711', '33333333-3333-3333-3333-333333333321', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps strength', 4, 4, 140, 270, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777712', '33333333-3333-3333-3333-333333333321', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest strength', 4, 4, 160, 270, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    -- Chest & Triceps - Weeks 5-8 (Recovery and maintenance)
    ('33333333-4444-5555-6666-777777777713', '33333333-3333-3333-3333-333333333325', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - strength maintenance', 4, 6, 160, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777714', '33333333-3333-3333-3333-333333333325', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps maintenance', 3, 8, 120, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777715', '33333333-3333-3333-3333-333333333325', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest maintenance', 3, 8, 130, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777716', '33333333-3333-3333-3333-333333333329', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - recovery focus', 4, 6, 155, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777717', '33333333-3333-3333-3333-333333333329', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps recovery', 3, 8, 115, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777718', '33333333-3333-3333-3333-333333333329', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest recovery', 3, 8, 125, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777719', '33333333-3333-3333-3333-333333333333', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - deload week', 3, 8, 135, 180, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777720', '33333333-3333-3333-3333-333333333333', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps deload', 3, 10, 100, 150, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777721', '33333333-3333-3333-3333-333333333333', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest deload', 3, 10, 110, 150, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777722', '33333333-3333-3333-3333-333333333337', 'c3635fe4-953c-4504-827a-d912d5ccf532', 1, 'Bench press - final peak', 4, 5, 170, 240, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777723', '33333333-3333-3333-3333-333333333337', '5f599fdf-4a0f-4b1c-b22d-6e8106311d30', 2, 'Close-grip bench press - triceps peak', 3, 6, 130, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777724', '33333333-3333-3333-3333-333333333337', 'f59e522d-496d-47af-832b-51c78291d705', 3, 'Incline bench press - upper chest peak', 3, 6, 140, 210, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ADVANCED Back & Biceps Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Back & Biceps - Weeks 1-8 (Progressive overload throughout)
    ('33333333-4444-5555-6666-777777777725', '33333333-3333-3333-3333-333333333310', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back strength', 4, 8, 120, 150, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777726', '33333333-3333-3333-3333-333333333310', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps strength', 4, 8, 35, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777727', '33333333-3333-3333-3333-333333333310', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 10, 25, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777728', '33333333-3333-3333-3333-333333333314', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back strength', 4, 7, 130, 165, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777729', '33333333-3333-3333-3333-333333333314', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps strength', 4, 7, 37.5, 135, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777730', '33333333-3333-3333-3333-333333333314', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 9, 27.5, 100, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777731', '33333333-3333-3333-3333-333333333318', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back strength', 4, 6, 140, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777732', '33333333-3333-3333-3333-333333333318', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps strength', 4, 6, 40, 150, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777733', '33333333-3333-3333-3333-333333333318', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 8, 30, 110, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777734', '33333333-3333-3333-3333-333333333322', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back strength', 4, 5, 150, 210, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777735', '33333333-3333-3333-3333-333333333322', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps strength', 4, 5, 45, 165, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777736', '33333333-3333-3333-3333-333333333322', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps isolation', 3, 7, 32.5, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777737', '33333333-3333-3333-3333-333333333326', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - back maintenance', 4, 8, 120, 150, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777738', '33333333-3333-3333-3333-333333333326', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps maintenance', 4, 8, 35, 120, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777739', '33333333-3333-3333-3333-333333333326', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps maintenance', 3, 10, 25, 90, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777740', '33333333-3333-3333-3333-333333333330', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - recovery', 3, 10, 100, 120, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777741', '33333333-3333-3333-3333-333333333330', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps recovery', 3, 10, 27.5, 90, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777742', '33333333-3333-3333-3333-333333333330', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps recovery', 3, 12, 20, 75, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777743', '33333333-3333-3333-3333-333333333334', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - deload', 3, 12, 80, 90, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777744', '33333333-3333-3333-3333-333333333334', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps deload', 3, 12, 22.5, 75, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777745', '33333333-3333-3333-3333-333333333334', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps deload', 3, 15, 15, 60, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777746', '33333333-3333-3333-3333-333333333338', 'e8a35915-675c-4c43-b6bf-ddfd23c12924', 1, 'Machine high row - final peak', 4, 6, 140, 180, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777747', '33333333-3333-3333-3333-333333333338', 'da94575c-9d1a-412b-978e-7132cf7fb5ed', 2, 'Wide-grip curl - biceps peak', 4, 6, 40, 150, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777748', '33333333-3333-3333-3333-333333333338', '5b3a3274-c5dd-4151-b599-68805e50e05f', 3, 'Concentration curl - biceps peak', 3, 8, 30, 110, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ADVANCED Legs Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Legs - Weeks 1-8 (High intensity calf work)
    ('33333333-4444-5555-6666-777777777749', '33333333-3333-3333-3333-333333333311', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - heavy calf work', 5, 10, 150, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777750', '33333333-3333-3333-3333-333333333315', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - progressive calf work', 5, 9, 165, 135, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777751', '33333333-3333-3333-3333-333333333319', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - peak calf strength', 5, 8, 180, 150, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777752', '33333333-3333-3333-3333-333333333323', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - max calf strength', 5, 6, 210, 180, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777753', '33333333-3333-3333-3333-333333333327', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - maintenance', 4, 8, 165, 135, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777754', '33333333-3333-3333-3333-333333333331', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - recovery', 4, 10, 135, 105, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777755', '33333333-3333-3333-3333-333333333335', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - deload', 3, 12, 120, 90, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777756', '33333333-3333-3333-3333-333333333339', '7bb62eef-8185-4e65-9a6b-2a90f9bdc22b', 1, 'Machine calf press - final peak', 4, 8, 180, 150, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

-- ADVANCED Shoulders & Core Day
INSERT INTO "ProgramTemplateWorkoutExercises" (
    "Id",
    "ProgramTemplateWorkoutDayId",
    "ExerciseDefinitionId",
    "OrderIndex",
    "Notes",
    "TargetSets",
    "TargetReps",
    "TargetWeight",
    "RestSeconds",
    "TargetRpe",
    "CreatedAt"
) VALUES
    -- Shoulders & Core - Weeks 1-8 (Advanced core work with progression)
    ('33333333-4444-5555-6666-777777777757', '33333333-3333-3333-3333-333333333312', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - advanced anti-rotation', 4, 12, 0, 75, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777758', '33333333-3333-3333-3333-333333333312', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - maximum endurance', 3, 45, 0, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777759', '33333333-3333-3333-3333-333333333312', 'bb39f064-ae5c-4534-bce6-1430ca4e83cb', 3, 'Hanging leg raise - advanced core', 3, 10, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777760', '33333333-3333-3333-3333-333333333316', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - advanced anti-rotation', 4, 12, 0, 75, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777761', '33333333-3333-3333-3333-333333333316', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - maximum endurance', 3, 50, 0, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777762', '33333333-3333-3333-3333-333333333316', 'bb39f064-ae5c-4534-bce6-1430ca4e83cb', 3, 'Hanging leg raise - advanced core', 3, 10, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777763', '33333333-3333-3333-3333-333333333320', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - advanced anti-rotation', 4, 12, 0, 75, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777764', '33333333-3333-3333-3333-333333333320', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - maximum endurance', 3, 55, 0, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777765', '33333333-3333-3333-3333-333333333320', 'bb39f064-ae5c-4534-bce6-1430ca4e83cb', 3, 'Hanging leg raise - advanced core', 3, 10, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777766', '33333333-3333-3333-3333-333333333324', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - advanced anti-rotation', 4, 10, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777767', '33333333-3333-3333-3333-333333333324', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - maximum endurance', 3, 60, 0, 120, 9, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777768', '33333333-3333-3333-3333-333333333324', 'bb39f064-ae5c-4534-bce6-1430ca4e83cb', 3, 'Hanging leg raise - advanced core', 3, 8, 0, 105, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777769', '33333333-3333-3333-3333-333333333328', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - maintenance', 3, 12, 0, 75, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777770', '33333333-3333-3333-3333-333333333328', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - maintenance', 3, 40, 0, 105, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777771', '33333333-3333-3333-3333-333333333328', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - maintenance', 3, 20, 0, 75, 7, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777772', '33333333-3333-3333-3333-333333333332', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - recovery', 3, 15, 0, 60, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777773', '33333333-3333-3333-3333-333333333332', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - recovery', 3, 35, 0, 90, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777774', '33333333-3333-3333-3333-333333333332', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - recovery', 3, 18, 0, 60, 6, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777775', '33333333-3333-3333-3333-333333333336', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - deload', 3, 15, 0, 60, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777776', '33333333-3333-3333-3333-333333333336', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - deload', 3, 30, 0, 75, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777777', '33333333-3333-3333-3333-333333333336', '2eaa2187-a6fa-4134-8fbb-f62b1ce6849b', 3, 'Ab bicycle - deload', 3, 15, 0, 45, 5, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777778', '33333333-3333-3333-3333-333333333340', '887b9a3d-2b1d-4f22-991e-0bd6b119aced', 1, 'Pallof press - final peak', 4, 12, 0, 75, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777779', '33333333-3333-3333-3333-333333333340', '9fb1c3ec-7c93-4a6c-9964-69c39f3f6a00', 2, 'Elbow plank - final peak', 3, 50, 0, 120, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone),
    ('33333333-4444-5555-6666-777777777780', '33333333-3333-3333-3333-333333333340', 'bb39f064-ae5c-4534-bce6-1430ca4e83cb', 3, 'Hanging leg raise - final peak', 3, 12, 0, 90, 8, '2026-01-18 00:00:00.000 +0000'::timestamp with time zone);

COMMIT;