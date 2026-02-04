-- RAG Seed Data for Testing
-- Ensures Equipment, Muscles, Tags, ExerciseDefinitions and join tables are populated
-- with all new fields set so the RAG program generation feature can be tested.
--
-- Usage: Run after migrations (e.g. psql -f rag_seed_data.sql). For a clean test run,
-- use an empty or dedicated database; if Equipment/Muscles/Tags already exist from
-- C# seed, either truncate those tables first or run this on a fresh DB.
-- Uses fixed UUIDs and exercise IDs 10001+ for idempotency (ON CONFLICT DO NOTHING).
--
-- Enum reference: Type 1=WeightTraining, PrimaryMuscleGroup 1=Chest..13=Cardio,
-- SpinalLoad 0=None..3=High, SetupComplexity 0=Low..2=High, TrackingMode 0=Reps..4=TimeDistance,
-- LoadType 0=ExternalLoad..4=TimeDistance, MovementPattern 1=Push..9=Flexibility,
-- LevelOfDifficulty 1=Beginner..3=Advanced, MuscleRole 0=Primary 1=Secondary 2=Stabilizer.

BEGIN;

-- =============================================================================
-- 1. EQUIPMENT (fixed UUIDs for join references)
-- =============================================================================
INSERT INTO "Equipment" ("Id", "Name", "Category", "IsPortable")
VALUES
  ('a1000001-0000-0000-0000-000000000001', 'Barbell', 'free_weight', false),
  ('a1000001-0000-0000-0000-000000000002', 'Dumbbell', 'free_weight', true),
  ('a1000001-0000-0000-0000-000000000003', 'Flat Bench', 'bench', false),
  ('a1000001-0000-0000-0000-000000000004', 'Squat Rack', 'rack', false),
  ('a1000001-0000-0000-0000-000000000005', 'Cable Machine', 'cable', false),
  ('a1000001-0000-0000-0000-000000000006', 'Kettlebell', 'free_weight', true),
  ('a1000001-0000-0000-0000-000000000007', 'Bodyweight', 'bodyweight', true),
  ('a1000001-0000-0000-0000-000000000008', 'Pull-up Bar', 'bodyweight', false),
  ('a1000001-0000-0000-0000-000000000009', 'Leg Press', 'machine', false),
  ('a1000001-0000-0000-0000-00000000000a', 'Resistance Bands', 'bands', true),
  ('a1000001-0000-0000-0000-00000000000b', 'Treadmill', 'cardio', false),
  ('a1000001-0000-0000-0000-00000000000c', 'EZ Bar', 'free_weight', false),
  ('a1000001-0000-0000-0000-00000000000d', 'Lat Pulldown', 'machine', false),
  ('a1000001-0000-0000-0000-00000000000e', 'Smith Machine', 'machine', false),
  ('a1000001-0000-0000-0000-00000000000f', 'Incline Bench', 'bench', false),
  ('a1000001-0000-0000-0000-000000000010', 'Leg Extension', 'machine', false),
  ('a1000001-0000-0000-0000-000000000011', 'Leg Curl', 'machine', false)
ON CONFLICT ("Id") DO NOTHING;

-- =============================================================================
-- 2. MUSCLES (fixed UUIDs; one representative per muscle group for joins)
-- =============================================================================
INSERT INTO "Muscles" ("Id", "Name", "Group")
VALUES
  ('b1000001-0000-0000-0000-000000000001', 'Pectoralis Major', 'chest'),
  ('b1000001-0000-0000-0000-000000000002', 'Latissimus Dorsi', 'back'),
  ('b1000001-0000-0000-0000-000000000003', 'Anterior Deltoid', 'shoulders'),
  ('b1000001-0000-0000-0000-000000000004', 'Biceps Brachii', 'arms'),
  ('b1000001-0000-0000-0000-000000000005', 'Triceps Brachii', 'arms'),
  ('b1000001-0000-0000-0000-000000000006', 'Forearm Flexors', 'arms'),
  ('b1000001-0000-0000-0000-000000000007', 'Rectus Abdominis', 'core'),
  ('b1000001-0000-0000-0000-000000000008', 'Rectus Femoris', 'legs'),
  ('b1000001-0000-0000-0000-000000000009', 'Biceps Femoris', 'legs'),
  ('b1000001-0000-0000-0000-00000000000a', 'Gluteus Maximus', 'legs'),
  ('b1000001-0000-0000-0000-00000000000b', 'Gastrocnemius', 'legs')
ON CONFLICT ("Id") DO NOTHING;

-- =============================================================================
-- 3. TAGS (fixed UUIDs; subset for testing Goal, Contra, Context)
-- =============================================================================
INSERT INTO "Tags" ("Id", "Namespace", "Name")
VALUES
  ('c1000001-0000-0000-0000-000000000001', 0, 'strength'),
  ('c1000001-0000-0000-0000-000000000002', 0, 'hypertrophy'),
  ('c1000001-0000-0000-0000-000000000003', 0, 'endurance'),
  ('c1000001-0000-0000-0000-000000000004', 1, 'knee_friendly'),
  ('c1000001-0000-0000-0000-000000000005', 1, 'low_back_friendly'),
  ('c1000001-0000-0000-0000-000000000006', 2, 'high_spine_load'),
  ('c1000001-0000-0000-0000-000000000007', 2, 'overhead_required'),
  ('c1000001-0000-0000-0000-000000000008', 3, 'beginner_friendly'),
  ('c1000001-0000-0000-0000-000000000009', 3, 'commercial_gym'),
  ('c1000001-0000-0000-0000-00000000000a', 4, 'powerlifting')
ON CONFLICT ("Id") DO NOTHING;

-- =============================================================================
-- 4. EXERCISE DEFINITIONS (~5 per muscle group, all new fields populated)
-- Type 1=WeightTraining, 2=Cardio, 3=Bodyweight, 4=Flexibility
-- PrimaryMuscleGroup 1=Chest..11=Calves, 12=FullBody, 13=Cardio
-- SpinalLoad 0=None, 1=Low, 2=Moderate, 3=High
-- SetupComplexity 0=Low, 1=Medium, 2=High
-- TrackingMode 0=Reps, 1=RepsWeight, 2=Time, 3=Distance, 4=TimeDistance
-- LoadType 0=ExternalLoad, 1=Bodyweight, 2=Timed, 3=Distance, 4=TimeDistance
-- MovementPattern 1=Push..9=Flexibility | LevelOfDifficulty 1=Beginner..3=Advanced
-- =============================================================================
INSERT INTO "ExerciseDefinitions" (
  "Id", "Name", "Description", "Type", "MovementPattern", "PrimaryMuscleGroup", "SecondaryMuscleGroup",
  "LevelOfDifficulty", "Instructions", "VideoUrl", "IsSystemExercise", "CreatedAt", "EquipmentRequirements",
  "Aliases", "ShortCue", "SpinalLoad", "SetupComplexity", "RequiresOverheadPosition", "IsUnilateral", "IsBodyweight",
  "LoadType", "TrackingMode", "DefaultRepMin", "DefaultRepMax", "DefaultRestMinSec", "DefaultRestMaxSec", "TimePerSetEstimateSec"
)
VALUES
-- Chest (5) - PrimaryMuscleGroup=1
(10001, 'Barbell Bench Press', 'Compound chest press with barbell on flat bench.', 1, 1, 1, 5, 2, 'Set up with feet flat, retract scapula, lower to chest, press up.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Flat Bench","IsRequired":true}]', 'bench press,bb bench', 'Retract, press', 2, 1, false, false, false, 0, 1, 6, 12, 90, 180, 45),
(10002, 'Dumbbell Flye', 'Chest isolation with dumbbells in arc motion.', 1, 1, 1, 5, 1, 'Arms slightly bent, lower weights out to sides, squeeze chest to bring back.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true},{"Name":"Flat Bench","IsRequired":true}]', 'db flye,pec fly', 'Arc and squeeze', 1, 0, false, false, false, 0, 1, 10, 15, 60, 90, 40),
(10003, 'Incline Dumbbell Press', 'Upper chest emphasis with incline bench.', 1, 1, 1, 3, 2, 'Set incline 30-45°, press dumbbells up and slightly together.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true},{"Name":"Incline Bench","IsRequired":false}]', 'incline db press', 'Drive up', 1, 1, false, false, false, 0, 1, 8, 12, 90, 120, 50),
(10004, 'Push-Up', 'Bodyweight chest and triceps push.', 3, 1, 1, 7, 1, 'Hands shoulder-width, core tight, lower chest to floor, push back up.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'press up,pushup', 'Core tight, full range', 0, 0, false, false, true, 1, 1, 10, 20, 45, 90, 30),
(10005, 'Cable Chest Fly', 'Cable crossover for constant tension on chest.', 1, 1, 1, 5, 2, 'Stand between cables, bring handles together in front of chest.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'cable fly,crossover', 'Squeeze at center', 0, 1, false, false, false, 0, 1, 12, 15, 60, 90, 35),

-- Back (5) - PrimaryMuscleGroup=2
(10006, 'Barbell Row', 'Bent-over row for back thickness.', 1, 2, 2, 6, 2, 'Hinge at hips, pull bar to lower chest, squeeze shoulder blades.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true}]', 'bent over row,bor', 'Hinge, pull to hip', 2, 0, false, false, false, 0, 1, 6, 10, 90, 180, 50),
(10007, 'Lat Pulldown', 'Vertical pull for lats using cable or machine.', 1, 2, 2, 4, 1, 'Sit down, pull bar to upper chest, control the negative.', NULL, true, NOW(), '[{"Name":"Lat Pulldown","IsRequired":true}]', 'pulldown,lats', 'Pull to chest', 1, 0, false, false, false, 0, 1, 8, 12, 60, 120, 45),
(10008, 'Pull-Up', 'Bodyweight vertical pull for lats and biceps.', 3, 2, 2, 4, 2, 'Hang from bar, pull until chin over bar, lower with control.', NULL, true, NOW(), '[{"Name":"Pull-up Bar","IsRequired":true}]', 'chin up,pullup', 'Full hang to chin over', 1, 0, false, false, true, 1, 1, 6, 12, 90, 120, 40),
(10009, 'Dumbbell Single-Arm Row', 'Unilateral row for back and core stability.', 1, 2, 2, 6, 2, 'Support on bench, row dumbbell to hip, keep torso stable.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true},{"Name":"Flat Bench","IsRequired":true}]', 'single arm row,kroc row', 'Row to hip', 2, 1, false, true, false, 0, 1, 8, 12, 60, 90, 45),
(10010, 'Cable Seated Row', 'Horizontal pull for mid-back.', 1, 2, 2, 6, 1, 'Sit at cable, pull handle to stomach, squeeze shoulder blades.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'seated row,row', 'Squeeze at belly', 1, 0, false, false, false, 0, 1, 10, 15, 60, 90, 40),

-- Shoulders (5) - PrimaryMuscleGroup=3
(10011, 'Overhead Press', 'Standing barbell press for shoulders.', 1, 1, 3, 7, 2, 'Bar at front rack, press overhead to lockout.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Squat Rack","IsRequired":true}]', 'OHP,military press', 'Press to lockout', 2, 1, true, false, false, 0, 1, 5, 8, 120, 180, 50),
(10012, 'Dumbbell Lateral Raise', 'Isolation for lateral deltoids.', 1, 1, 3, NULL, 1, 'Raise dumbbells to sides to shoulder height, control descent.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'lateral raise,side raise', 'Raise to shoulder height', 0, 0, false, false, false, 0, 1, 12, 20, 45, 60, 30),
(10013, 'Face Pull', 'Rear delt and upper back with cable.', 1, 2, 3, 2, 1, 'Pull rope to face, externally rotate at end, squeeze rear delts.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'face pull', 'Pull to face', 0, 0, false, false, false, 0, 1, 15, 20, 45, 60, 35),
(10014, 'Arnold Press', 'Dumbbell press with rotation for all delt heads.', 1, 1, 3, 4, 2, 'Start palms in, press and rotate to palms out at top.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'arnold press', 'Rotate as you press', 1, 0, true, false, false, 0, 1, 8, 12, 60, 90, 45),
(10015, 'Push Press', 'Leg-driven overhead press for power.', 1, 1, 3, 8, 3, 'Dip and drive with legs, press bar overhead.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Squat Rack","IsRequired":true}]', 'push press', 'Dip, drive, press', 2, 1, true, false, false, 0, 1, 5, 8, 120, 180, 40),

-- Biceps (5) - PrimaryMuscleGroup=4
(10016, 'Barbell Curl', 'Standing barbell curl for biceps.', 1, 2, 4, NULL, 1, 'Elbows at sides, curl bar to shoulders, lower with control.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true}]', 'bb curl,curl', 'Curl to shoulder', 0, 0, false, false, false, 0, 1, 8, 12, 60, 90, 35),
(10017, 'Dumbbell Hammer Curl', 'Neutral grip curl for biceps and brachialis.', 1, 2, 4, 6, 1, 'Palms neutral, curl to shoulder, alternate or together.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'hammer curl', 'Neutral grip curl', 0, 0, false, true, false, 0, 1, 10, 15, 45, 60, 30),
(10018, 'Cable Curl', 'Constant tension biceps curl with cable.', 1, 2, 4, NULL, 1, 'Stand at cable, curl bar or handle to shoulders.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'cable curl', 'Squeeze at top', 0, 0, false, false, false, 0, 1, 10, 15, 45, 60, 35),
(10019, 'Preacher Curl', 'Strict biceps curl on preacher bench.', 1, 2, 4, NULL, 2, 'Arm supported on pad, curl weight up, full stretch at bottom.', NULL, true, NOW(), '[{"Name":"EZ Bar","IsRequired":true},{"Name":"Flat Bench","IsRequired":false}]', 'preacher curl', 'Strict curl', 0, 1, false, false, false, 0, 1, 8, 12, 60, 90, 40),
(10020, 'Concentration Curl', 'Isolation curl with elbow braced.', 1, 2, 4, NULL, 1, 'Elbow inside thigh, curl dumbbell to shoulder.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'concentration curl', 'Squeeze at top', 0, 0, false, true, false, 0, 1, 10, 15, 45, 60, 30),

-- Triceps (5) - PrimaryMuscleGroup=5
(10021, 'Triceps Pushdown', 'Cable pushdown for triceps.', 1, 1, 5, NULL, 1, 'Elbows at sides, push bar or rope down, squeeze at bottom.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'pushdown,tricep pushdown', 'Push to lockout', 0, 0, false, false, false, 0, 1, 10, 15, 45, 60, 30),
(10022, 'Close-Grip Bench Press', 'Compound triceps and chest with narrow grip.', 1, 1, 5, 1, 2, 'Hands shoulder-width or closer, lower to chest, press up.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Flat Bench","IsRequired":true}]', 'close grip bench,cgbp', 'Elbows in, press', 2, 1, false, false, false, 0, 1, 6, 10, 90, 120, 45),
(10023, 'Overhead Triceps Extension', 'Stretch and press for long head of triceps.', 1, 1, 5, NULL, 1, 'Dumbbell or EZ bar overhead, lower behind head, extend up.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'skull crusher,lying ext', 'Lower behind head', 1, 0, false, false, false, 0, 1, 10, 15, 60, 90, 40),
(10024, 'Dips', 'Bodyweight dip for triceps and chest.', 3, 1, 5, 1, 2, 'Support on bars, lower until upper arms parallel, push up.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'dip,chest dip', 'Full depth', 1, 0, false, false, true, 1, 1, 8, 15, 60, 90, 35),
(10025, 'Triceps Kickback', 'Isolation kickback with dumbbell.', 1, 1, 5, NULL, 1, 'Hinge at hip, extend arm back, squeeze triceps.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'kickback', 'Extend and squeeze', 0, 0, false, true, false, 0, 1, 12, 15, 45, 60, 25),

-- Core (5) - PrimaryMuscleGroup=7
(10026, 'Plank', 'Isometric core hold.', 3, 7, 7, NULL, 1, 'Forearms on floor, body straight, hold position.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'front plank', 'Stay straight', 0, 0, false, false, true, 1, 2, NULL, NULL, 30, 90, 60),
(10027, 'Dead Bug', 'Anti-extension core exercise.', 3, 7, 7, NULL, 1, 'On back, extend opposite arm and leg, keep lower back pressed.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'dead bug', 'Slow and controlled', 0, 0, false, false, true, 1, 2, NULL, NULL, 20, 45, 45),
(10028, 'Cable Wood Chop', 'Rotational core with cable.', 1, 7, 7, 2, 2, 'Pull cable across body with rotation, control return.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'wood chop', 'Rotate through', 1, 1, false, false, false, 0, 1, 10, 15, 45, 60, 35),
(10029, 'Hanging Leg Raise', 'Hanging knee or leg raise for lower abs.', 3, 7, 7, 2, 2, 'Hang from bar, raise knees or legs toward chest.', NULL, true, NOW(), '[{"Name":"Pull-up Bar","IsRequired":true}]', 'leg raise,hanging raise', 'Control the swing', 2, 0, false, false, true, 1, 1, 10, 20, 45, 90, 40),
(10030, 'Pallof Press', 'Anti-rotation hold and press.', 1, 7, 7, NULL, 1, 'Hold cable at chest, press out and return, resist rotation.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'pallof press', 'Resist rotation', 0, 0, false, false, false, 0, 1, 10, 15, 45, 60, 35),

-- Quadriceps (5) - PrimaryMuscleGroup=8
(10031, 'Barbell Back Squat', 'Full squat with barbell on back.', 1, 3, 8, 10, 3, 'Bar on upper back, squat to depth, drive up.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Squat Rack","IsRequired":true}]', 'squat,back squat', 'Break at hip and knee', 3, 2, false, false, false, 0, 1, 5, 10, 120, 300, 60),
(10032, 'Leg Press', 'Machine leg press for quads.', 1, 3, 8, 10, 1, 'Feet on platform, lower until 90°, press back up.', NULL, true, NOW(), '[{"Name":"Leg Press","IsRequired":true}]', 'leg press', 'Full range', 2, 0, false, false, false, 0, 1, 8, 15, 90, 120, 50),
(10033, 'Walking Lunge', 'Unilateral lunge with dumbbells or bodyweight.', 1, 5, 8, 10, 1, 'Step forward, lower back knee toward floor, stand and repeat.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":false}]', 'lunges,lunge', 'Long stride', 2, 0, false, true, false, 0, 1, 8, 12, 60, 90, 45),
(10034, 'Leg Extension', 'Isolation knee extension for quads.', 1, 3, 8, NULL, 1, 'Extend legs against pad, squeeze at top, control descent.', NULL, true, NOW(), '[{"Name":"Leg Press","IsRequired":false}]', 'leg ext,extension', 'Squeeze at top', 0, 0, false, false, false, 0, 1, 10, 15, 45, 60, 35),
(10035, 'Goblet Squat', 'Front-loaded squat with kettlebell or dumbbell.', 1, 3, 8, 7, 1, 'Hold weight at chest, squat to depth, drive up.', NULL, true, NOW(), '[{"Name":"Kettlebell","IsRequired":true}]', 'goblet squat', 'Elbows inside knees', 2, 0, false, false, false, 0, 1, 8, 15, 60, 120, 45),

-- Hamstrings (5) - PrimaryMuscleGroup=9
(10036, 'Romanian Deadlift', 'Hinge for hamstrings and glutes.', 1, 4, 9, 10, 2, 'Slight knee bend, hinge at hips, lower bar along legs, drive up.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true}]', 'RDL,romanian deadlift', 'Push hips back', 2, 0, false, false, false, 0, 1, 6, 12, 90, 180, 50),
(10037, 'Leg Curl', 'Lying or seated leg curl for hamstrings.', 1, 4, 9, NULL, 1, 'Curl heels toward glutes, squeeze, control return.', NULL, true, NOW(), '[{"Name":"Leg Press","IsRequired":false}]', 'leg curl,hamstring curl', 'Squeeze at top', 0, 0, false, false, false, 0, 1, 10, 15, 45, 60, 40),
(10038, 'Good Morning', 'Hip hinge with bar on back.', 1, 4, 9, 10, 3, 'Bar on back, hinge at hips until torso near horizontal.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true}]', 'good morning', 'Hinge only', 3, 1, false, false, false, 0, 1, 8, 12, 90, 120, 45),
(10039, 'Single-Leg RDL', 'Unilateral RDL for hamstring and balance.', 1, 4, 9, 10, 2, 'Balance on one leg, hinge and reach weight toward floor.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'single leg rdl,sl rdl', 'Hinge and reach', 2, 0, false, true, false, 0, 1, 8, 12, 60, 90, 45),
(10040, 'Nordic Curl', 'Eccentric hamstring curl with bodyweight.', 3, 4, 9, NULL, 3, 'Knees anchored, lower body forward with control, push back up.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'nordic hamstring curl', 'Control the negative', 1, 2, false, false, true, 1, 1, 5, 10, 90, 120, 50),

-- Glutes (5) - PrimaryMuscleGroup=10
(10041, 'Hip Thrust', 'Hip extension for glutes with barbell.', 1, 4, 10, 9, 2, 'Upper back on bench, drive hips up to lockout, squeeze glutes.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Flat Bench","IsRequired":true}]', 'hip thrust,barbell hip thrust', 'Squeeze at top', 1, 1, false, false, false, 0, 1, 8, 15, 60, 120, 50),
(10042, 'Glute Bridge', 'Floor hip bridge for glutes.', 3, 4, 10, 9, 1, 'Feet flat, drive hips up, squeeze glutes at top.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'bridge,hip bridge', 'Squeeze at top', 0, 0, false, false, true, 1, 1, 12, 20, 45, 60, 35),
(10043, 'Cable Kickback', 'Cable hip extension for glutes.', 1, 4, 10, NULL, 1, 'Kick leg back against cable, squeeze glute at end range.', NULL, true, NOW(), '[{"Name":"Cable Machine","IsRequired":true}]', 'glute kickback', 'Kick back', 0, 0, false, true, false, 0, 1, 12, 20, 45, 60, 30),
(10044, 'Bulgarian Split Squat', 'Rear-foot elevated split squat.', 1, 5, 10, 8, 2, 'Rear foot on bench, squat down on front leg.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":false},{"Name":"Flat Bench","IsRequired":true}]', 'BSS,bulgarian split', 'Front knee over toe', 2, 1, false, true, false, 0, 1, 6, 12, 90, 120, 50),
(10045, 'Sumo Deadlift', 'Wide-stance deadlift for glutes and inner leg.', 1, 4, 10, 9, 3, 'Wide stance, grip inside legs, drive through heels.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true}]', 'sumo dl,sumo deadlift', 'Push floor apart', 3, 1, false, false, false, 0, 1, 5, 10, 120, 180, 55),

-- Calves (5) - PrimaryMuscleGroup=11
(10046, 'Standing Calf Raise', 'Standing calf raise on machine or step.', 1, 3, 11, NULL, 1, 'Raise onto toes, full stretch at bottom, squeeze at top.', NULL, true, NOW(), '[{"Name":"Smith Machine","IsRequired":true}]', 'calf raise,standing calf', 'Full range', 0, 0, false, false, false, 0, 1, 12, 20, 30, 45, 25),
(10047, 'Seated Calf Raise', 'Seated calf raise for soleus.', 1, 3, 11, NULL, 1, 'Knees bent 90°, raise heels, lower for stretch.', NULL, true, NOW(), '[{"Name":"Leg Press","IsRequired":false}]', 'seated calf', 'Knees bent', 0, 0, false, false, false, 0, 1, 15, 25, 30, 45, 30),
(10048, 'Donkey Calf Raise', 'Bent-over calf raise with partner or machine.', 1, 3, 11, NULL, 2, 'Bend at waist, raise onto toes against resistance.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'donkey calf', 'Hips high', 1, 1, false, false, true, 1, 1, 15, 25, 30, 45, 35),
(10049, 'Jump Rope', 'Cardio and calf conditioning.', 2, 8, 11, 7, 1, 'Bounce on balls of feet, rotate rope with wrists.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'skipping,rope', 'Light on feet', 0, 0, false, false, true, 1, 2, NULL, NULL, 15, 30, 60),
(10050, 'Single-Leg Calf Raise', 'Unilateral calf raise for balance.', 1, 3, 11, NULL, 1, 'Stand on one leg, raise onto toe, lower with control.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'single leg calf', 'Full stretch', 0, 0, false, true, true, 1, 1, 12, 20, 30, 45, 25),

-- Forearms (3) - PrimaryMuscleGroup=6
(10051, 'Wrist Curl', 'Flexor curl for forearms.', 1, 2, 6, NULL, 1, 'Forearms on bench, curl weight with wrists.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'wrist curl', 'Curl at wrist', 0, 0, false, false, false, 0, 1, 15, 20, 30, 45, 25),
(10052, 'Reverse Wrist Curl', 'Extensor curl for forearms.', 1, 2, 6, NULL, 1, 'Palms down, curl weight up with wrists.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'reverse wrist curl', 'Extend at wrist', 0, 0, false, false, false, 0, 1, 15, 20, 30, 45, 25),
(10053, 'Farmer''s Walk', 'Carry for grip and forearms.', 1, 6, 6, 7, 2, 'Hold heavy weights, walk for distance or time.', NULL, true, NOW(), '[{"Name":"Dumbbell","IsRequired":true}]', 'farmer carry,carry', 'Walk tall', 1, 0, false, true, false, 0, 3, NULL, NULL, 60, 120, 60),

-- FullBody (3) - PrimaryMuscleGroup=12
(10054, 'Burpee', 'Full-body conditioning exercise.', 3, 1, 12, 7, 2, 'Drop to floor, push up, jump feet in, jump up.', NULL, true, NOW(), '[{"Name":"Bodyweight","IsRequired":true}]', 'burpee', 'Smooth flow', 1, 0, false, false, true, 1, 2, NULL, NULL, 30, 60, 15),
(10055, 'Kettlebell Swing', 'Hip hinge swing for posterior chain.', 1, 4, 12, 10, 2, 'Hinge and swing kettlebell to chest height, snap hips.', NULL, true, NOW(), '[{"Name":"Kettlebell","IsRequired":true}]', 'kb swing,swing', 'Hinge and snap', 2, 0, false, false, false, 0, 1, 10, 20, 45, 90, 25),
(10056, 'Thruster', 'Squat to press compound movement.', 1, 1, 12, 8, 3, 'Front squat then press overhead in one motion.', NULL, true, NOW(), '[{"Name":"Barbell","IsRequired":true},{"Name":"Squat Rack","IsRequired":true}]', 'thruster', 'Squat and press', 2, 2, true, false, false, 0, 1, 6, 10, 90, 120, 45),

-- Cardio (2) - PrimaryMuscleGroup=13, Type=2, TrackingMode 2=Time or 3=Distance
(10057, 'Treadmill Run', 'Running on treadmill for cardio.', 2, 8, 13, NULL, 1, 'Set speed and incline, run for time or distance.', NULL, true, NOW(), '[{"Name":"Treadmill","IsRequired":true}]', 'treadmill,running', 'Steady pace', 0, 0, false, false, true, 2, 4, NULL, NULL, 60, 300, 600),
(10058, 'Rowing Machine', 'Rowing for full-body cardio.', 2, 8, 13, 2, 1, 'Drive with legs, pull handle to chest, return with control.', NULL, true, NOW(), '[{"Name":"Treadmill","IsRequired":false}]', 'rower,erg', 'Legs then arms', 0, 0, false, false, true, 4, 4, NULL, NULL, 60, 180, 480)
ON CONFLICT ("Id") DO NOTHING;

-- =============================================================================
-- 5. EXERCISE-EQUIPMENT LINKS (fixed UUIDs from Equipment seed above)
-- =============================================================================
INSERT INTO "ExerciseEquipments" ("ExerciseDefinitionId", "EquipmentId")
VALUES
  (10001, 'a1000001-0000-0000-0000-000000000001'), (10001, 'a1000001-0000-0000-0000-000000000003'),
  (10002, 'a1000001-0000-0000-0000-000000000002'), (10002, 'a1000001-0000-0000-0000-000000000003'),
  (10003, 'a1000001-0000-0000-0000-000000000002'), (10003, 'a1000001-0000-0000-0000-00000000000f'),
  (10004, 'a1000001-0000-0000-0000-000000000007'),
  (10005, 'a1000001-0000-0000-0000-000000000005'),
  (10006, 'a1000001-0000-0000-0000-000000000001'),
  (10007, 'a1000001-0000-0000-0000-00000000000d'),
  (10008, 'a1000001-0000-0000-0000-000000000008'),
  (10009, 'a1000001-0000-0000-0000-000000000002'), (10009, 'a1000001-0000-0000-0000-000000000003'),
  (10010, 'a1000001-0000-0000-0000-000000000005'),
  (10011, 'a1000001-0000-0000-0000-000000000001'), (10011, 'a1000001-0000-0000-0000-000000000004'),
  (10012, 'a1000001-0000-0000-0000-000000000002'),
  (10013, 'a1000001-0000-0000-0000-000000000005'),
  (10014, 'a1000001-0000-0000-0000-000000000002'),
  (10015, 'a1000001-0000-0000-0000-000000000001'), (10015, 'a1000001-0000-0000-0000-000000000004'),
  (10016, 'a1000001-0000-0000-0000-000000000001'),
  (10017, 'a1000001-0000-0000-0000-000000000002'),
  (10018, 'a1000001-0000-0000-0000-000000000005'),
  (10019, 'a1000001-0000-0000-0000-00000000000c'),
  (10020, 'a1000001-0000-0000-0000-000000000002'),
  (10021, 'a1000001-0000-0000-0000-000000000005'),
  (10022, 'a1000001-0000-0000-0000-000000000001'), (10022, 'a1000001-0000-0000-0000-000000000003'),
  (10023, 'a1000001-0000-0000-0000-000000000002'),
  (10024, 'a1000001-0000-0000-0000-000000000007'),
  (10025, 'a1000001-0000-0000-0000-000000000002'),
  (10026, 'a1000001-0000-0000-0000-000000000007'),
  (10027, 'a1000001-0000-0000-0000-000000000007'),
  (10028, 'a1000001-0000-0000-0000-000000000005'),
  (10029, 'a1000001-0000-0000-0000-000000000008'),
  (10030, 'a1000001-0000-0000-0000-000000000005'),
  (10031, 'a1000001-0000-0000-0000-000000000001'), (10031, 'a1000001-0000-0000-0000-000000000004'),
  (10032, 'a1000001-0000-0000-0000-000000000009'),
  (10033, 'a1000001-0000-0000-0000-000000000002'),
  (10034, 'a1000001-0000-0000-0000-000000000010'),
  (10035, 'a1000001-0000-0000-0000-000000000006'),
  (10037, 'a1000001-0000-0000-0000-000000000011'),
  (10036, 'a1000001-0000-0000-0000-000000000001'),
  (10039, 'a1000001-0000-0000-0000-000000000002'),
  (10040, 'a1000001-0000-0000-0000-000000000007'),
  (10041, 'a1000001-0000-0000-0000-000000000001'), (10041, 'a1000001-0000-0000-0000-000000000003'),
  (10042, 'a1000001-0000-0000-0000-000000000007'),
  (10043, 'a1000001-0000-0000-0000-000000000005'),
  (10044, 'a1000001-0000-0000-0000-000000000002'), (10044, 'a1000001-0000-0000-0000-000000000003'),
  (10045, 'a1000001-0000-0000-0000-000000000001'),
  (10046, 'a1000001-0000-0000-0000-00000000000e'),
  (10049, 'a1000001-0000-0000-0000-000000000007'),
  (10050, 'a1000001-0000-0000-0000-000000000007'),
  (10051, 'a1000001-0000-0000-0000-000000000002'),
  (10052, 'a1000001-0000-0000-0000-000000000002'),
  (10053, 'a1000001-0000-0000-0000-000000000002'),
  (10054, 'a1000001-0000-0000-0000-000000000007'),
  (10055, 'a1000001-0000-0000-0000-000000000006'),
  (10056, 'a1000001-0000-0000-0000-000000000001'), (10056, 'a1000001-0000-0000-0000-000000000004'),
  (10057, 'a1000001-0000-0000-0000-00000000000b'),
  (10058, 'a1000001-0000-0000-0000-00000000000b')
ON CONFLICT ("ExerciseDefinitionId", "EquipmentId") DO NOTHING;

-- =============================================================================
-- 6. EXERCISE-MUSCLE LINKS (Primary=0, Secondary=1, Stabilizer=2; Contribution 1-100 optional)
-- Muscle UUIDs: chest b100..001, back b100..002, shoulders b100..003, biceps b100..004,
-- triceps b100..005, forearms b100..006, core b100..007, quads b100..008, hams b100..009, glutes b100..00a, calves b100..00b
-- =============================================================================
INSERT INTO "ExerciseMuscles" ("ExerciseDefinitionId", "MuscleId", "Role", "Contribution")
VALUES
  (10001, 'b1000001-0000-0000-0000-000000000001', 0, 80), (10001, 'b1000001-0000-0000-0000-000000000005', 1, 20),
  (10002, 'b1000001-0000-0000-0000-000000000001', 0, 90), (10002, 'b1000001-0000-0000-0000-000000000005', 1, 10),
  (10003, 'b1000001-0000-0000-0000-000000000001', 0, 75), (10003, 'b1000001-0000-0000-0000-000000000003', 1, 25),
  (10004, 'b1000001-0000-0000-0000-000000000001', 0, 70), (10004, 'b1000001-0000-0000-0000-000000000005', 1, 30),
  (10005, 'b1000001-0000-0000-0000-000000000001', 0, 85),
  (10006, 'b1000001-0000-0000-0000-000000000002', 0, 70), (10006, 'b1000001-0000-0000-0000-000000000006', 1, 30),
  (10007, 'b1000001-0000-0000-0000-000000000002', 0, 75), (10007, 'b1000001-0000-0000-0000-000000000004', 1, 25),
  (10008, 'b1000001-0000-0000-0000-000000000002', 0, 65), (10008, 'b1000001-0000-0000-0000-000000000004', 1, 35),
  (10009, 'b1000001-0000-0000-0000-000000000002', 0, 80), (10009, 'b1000001-0000-0000-0000-000000000006', 1, 20),
  (10010, 'b1000001-0000-0000-0000-000000000002', 0, 75), (10010, 'b1000001-0000-0000-0000-000000000004', 1, 25),
  (10011, 'b1000001-0000-0000-0000-000000000003', 0, 70), (10011, 'b1000001-0000-0000-0000-000000000007', 1, 30),
  (10012, 'b1000001-0000-0000-0000-000000000003', 0, 95),
  (10013, 'b1000001-0000-0000-0000-000000000003', 0, 75), (10013, 'b1000001-0000-0000-0000-000000000002', 1, 25),
  (10014, 'b1000001-0000-0000-0000-000000000003', 0, 70), (10014, 'b1000001-0000-0000-0000-000000000004', 1, 30),
  (10015, 'b1000001-0000-0000-0000-000000000003', 0, 65), (10015, 'b1000001-0000-0000-0000-000000000008', 1, 35),
  (10016, 'b1000001-0000-0000-0000-000000000004', 0, 90),
  (10017, 'b1000001-0000-0000-0000-000000000004', 0, 70), (10017, 'b1000001-0000-0000-0000-000000000006', 1, 30),
  (10018, 'b1000001-0000-0000-0000-000000000004', 0, 90),
  (10019, 'b1000001-0000-0000-0000-000000000004', 0, 95),
  (10020, 'b1000001-0000-0000-0000-000000000004', 0, 95),
  (10021, 'b1000001-0000-0000-0000-000000000005', 0, 95),
  (10022, 'b1000001-0000-0000-0000-000000000005', 0, 70), (10022, 'b1000001-0000-0000-0000-000000000001', 1, 30),
  (10023, 'b1000001-0000-0000-0000-000000000005', 0, 90),
  (10024, 'b1000001-0000-0000-0000-000000000005', 0, 65), (10024, 'b1000001-0000-0000-0000-000000000001', 1, 35),
  (10025, 'b1000001-0000-0000-0000-000000000005', 0, 95),
  (10026, 'b1000001-0000-0000-0000-000000000007', 0, 100),
  (10027, 'b1000001-0000-0000-0000-000000000007', 0, 100),
  (10028, 'b1000001-0000-0000-0000-000000000007', 0, 70), (10028, 'b1000001-0000-0000-0000-000000000002', 1, 30),
  (10029, 'b1000001-0000-0000-0000-000000000007', 0, 75), (10029, 'b1000001-0000-0000-0000-000000000002', 1, 25),
  (10030, 'b1000001-0000-0000-0000-000000000007', 0, 100),
  (10031, 'b1000001-0000-0000-0000-000000000008', 0, 70), (10031, 'b1000001-0000-0000-0000-00000000000a', 1, 30),
  (10032, 'b1000001-0000-0000-0000-000000000008', 0, 80), (10032, 'b1000001-0000-0000-0000-00000000000a', 1, 20),
  (10033, 'b1000001-0000-0000-0000-000000000008', 0, 65), (10033, 'b1000001-0000-0000-0000-00000000000a', 1, 35),
  (10034, 'b1000001-0000-0000-0000-000000000008', 0, 95),
  (10035, 'b1000001-0000-0000-0000-000000000008', 0, 70), (10035, 'b1000001-0000-0000-0000-000000000007', 1, 30),
  (10036, 'b1000001-0000-0000-0000-000000000009', 0, 70), (10036, 'b1000001-0000-0000-0000-00000000000a', 1, 30),
  (10037, 'b1000001-0000-0000-0000-000000000009', 0, 95),
  (10038, 'b1000001-0000-0000-0000-000000000009', 0, 60), (10038, 'b1000001-0000-0000-0000-00000000000a', 1, 40),
  (10039, 'b1000001-0000-0000-0000-000000000009', 0, 75), (10039, 'b1000001-0000-0000-0000-00000000000a', 1, 25),
  (10040, 'b1000001-0000-0000-0000-000000000009', 0, 95),
  (10041, 'b1000001-0000-0000-0000-00000000000a', 0, 80), (10041, 'b1000001-0000-0000-0000-000000000009', 1, 20),
  (10042, 'b1000001-0000-0000-0000-00000000000a', 0, 85), (10042, 'b1000001-0000-0000-0000-000000000009', 1, 15),
  (10043, 'b1000001-0000-0000-0000-00000000000a', 0, 95),
  (10044, 'b1000001-0000-0000-0000-00000000000a', 0, 65), (10044, 'b1000001-0000-0000-0000-000000000008', 1, 35),
  (10045, 'b1000001-0000-0000-0000-00000000000a', 0, 60), (10045, 'b1000001-0000-0000-0000-000000000009', 1, 40),
  (10046, 'b1000001-0000-0000-0000-00000000000b', 0, 95),
  (10047, 'b1000001-0000-0000-0000-00000000000b', 0, 95),
  (10048, 'b1000001-0000-0000-0000-00000000000b', 0, 90),
  (10049, 'b1000001-0000-0000-0000-00000000000b', 0, 70), (10049, 'b1000001-0000-0000-0000-000000000007', 1, 30),
  (10050, 'b1000001-0000-0000-0000-00000000000b', 0, 95),
  (10051, 'b1000001-0000-0000-0000-000000000006', 0, 95),
  (10052, 'b1000001-0000-0000-0000-000000000006', 0, 95),
  (10053, 'b1000001-0000-0000-0000-000000000006', 0, 60), (10053, 'b1000001-0000-0000-0000-000000000007', 1, 40),
  (10054, 'b1000001-0000-0000-0000-000000000008', 0, 40), (10054, 'b1000001-0000-0000-0000-000000000007', 1, 60),
  (10055, 'b1000001-0000-0000-0000-00000000000a', 0, 50), (10055, 'b1000001-0000-0000-0000-000000000009', 1, 50),
  (10056, 'b1000001-0000-0000-0000-000000000008', 0, 45), (10056, 'b1000001-0000-0000-0000-000000000003', 1, 55),
  (10057, 'b1000001-0000-0000-0000-00000000000b', 0, 50), (10057, 'b1000001-0000-0000-0000-000000000007', 1, 50),
  (10058, 'b1000001-0000-0000-0000-000000000002', 0, 50), (10058, 'b1000001-0000-0000-0000-000000000008', 1, 50)
ON CONFLICT ("ExerciseDefinitionId", "MuscleId", "Role") DO NOTHING;

-- =============================================================================
-- 7. EXERCISE-TAG LINKS (sample tags: strength, hypertrophy, knee_friendly, etc.)
-- =============================================================================
INSERT INTO "ExerciseTags" ("ExerciseDefinitionId", "TagId")
VALUES
  (10001, 'c1000001-0000-0000-0000-000000000001'), (10001, 'c1000001-0000-0000-0000-000000000009'),
  (10002, 'c1000001-0000-0000-0000-000000000002'), (10004, 'c1000001-0000-0000-0000-000000000008'),
  (10006, 'c1000001-0000-0000-0000-000000000001'), (10006, 'c1000001-0000-0000-0000-00000000000a'),
  (10007, 'c1000001-0000-0000-0000-000000000008'), (10008, 'c1000001-0000-0000-0000-000000000008'),
  (10011, 'c1000001-0000-0000-0000-000000000001'), (10011, 'c1000001-0000-0000-0000-000000000007'),
  (10012, 'c1000001-0000-0000-0000-000000000002'), (10016, 'c1000001-0000-0000-0000-000000000002'),
  (10021, 'c1000001-0000-0000-0000-000000000002'), (10026, 'c1000001-0000-0000-0000-000000000008'),
  (10031, 'c1000001-0000-0000-0000-000000000001'), (10031, 'c1000001-0000-0000-0000-00000000000a'),
  (10031, 'c1000001-0000-0000-0000-000000000006'), (10032, 'c1000001-0000-0000-0000-000000000004'),
  (10035, 'c1000001-0000-0000-0000-000000000008'), (10036, 'c1000001-0000-0000-0000-000000000005'),
  (10041, 'c1000001-0000-0000-0000-000000000002'), (10054, 'c1000001-0000-0000-0000-000000000003'),
  (10055, 'c1000001-0000-0000-0000-000000000001'), (10056, 'c1000001-0000-0000-0000-00000000000a'),
  (10057, 'c1000001-0000-0000-0000-000000000003'), (10058, 'c1000001-0000-0000-0000-000000000003')
ON CONFLICT ("ExerciseDefinitionId", "TagId") DO NOTHING;

-- =============================================================================
-- 8. RESET IDENTITY SEQUENCE (so next app-created exercise gets Id > 10058)
-- =============================================================================
SELECT setval(
  pg_get_serial_sequence('"ExerciseDefinitions"', 'Id'),
  (SELECT COALESCE(MAX("Id"), 1) FROM "ExerciseDefinitions")
);

COMMIT;
