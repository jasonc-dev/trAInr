-- SQL script to insert ExerciseDefinitions from CSV data
-- This script inserts all exercise definitions into the ExerciseDefinitions table

BEGIN;

-- Clear existing exercise definitions (optional - remove if you want to keep existing data)
-- DELETE FROM "ExerciseDefinitions";

-- Insert all exercise definitions
INSERT INTO "ExerciseDefinitions" (
    "Id",
    "Name",
    "Description",
    "Type",
    "MovementPattern",
    "PrimaryMuscleGroup",
    "SecondaryMuscleGroup",
    "Instructions",
    "VideoUrl",
    "IsSystemExercise",
    "CreatedByUserId",
    "CreatedAt",
    "EquipmentRequirements"
) VALUES
-- Row 1: Bench Press
('11111111-1111-1111-1111-111111111111', 'Bench Press', 'A compound exercise targeting the chest, shoulders, and triceps. Lie on a bench and press a barbell upward from your chest.', 1, 1, 1, 5, 'Lie flat on bench, grip bar slightly wider than shoulders. Lower bar to chest, then press up explosively.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 2: Push-ups
('11111111-1111-1111-1111-111111111112', 'Push-ups', 'A bodyweight exercise that strengthens the chest, shoulders, and triceps. No equipment needed.', 3, 1, 1, 5, 'Start in plank position, lower body until chest nearly touches floor, push back up.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 3: Overhead Press
('11111111-1111-1111-1111-111111111113', 'Overhead Press', 'A vertical pressing movement that primarily targets the shoulders and triceps.', 1, 1, 3, 5, 'Stand with feet shoulder-width apart, press barbell or dumbbells overhead from shoulder height.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 4: Dips
('11111111-1111-1111-1111-111111111114', 'Dips', 'A bodyweight exercise targeting triceps, chest, and shoulders. Performed on parallel bars or bench.', 3, 1, 5, 1, 'Support body on parallel bars, lower until elbows are at 90 degrees, push back up.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 5: Dumbbell Flyes
('11111111-1111-1111-1111-111111111115', 'Dumbbell Flyes', 'An isolation exercise that targets the chest muscles through a wide arc of motion.', 1, 1, 1, NULL, 'Lie on bench, hold dumbbells with arms slightly bent. Lower weights in wide arc, bring together above chest.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 6: Pull-ups
('22222222-2222-2222-2222-222222222221', 'Pull-ups', 'A bodyweight exercise that targets the back and biceps. One of the best upper body exercises.', 3, 2, 2, 4, 'Hang from bar with palms facing away, pull body up until chin clears bar, lower with control.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 7: Barbell Rows
('22222222-2222-2222-2222-222222222222', 'Barbell Rows', 'A compound pulling exercise that targets the back, biceps, and rear deltoids.', 1, 2, 2, 4, 'Bend at hips, keep back straight, pull barbell to lower chest/upper abdomen, squeeze back muscles.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 8: Lat Pulldown
('22222222-2222-2222-2222-222222222223', 'Lat Pulldown', 'A machine exercise that targets the latissimus dorsi and biceps.', 1, 2, 2, 4, 'Sit at lat pulldown machine, pull bar to upper chest, control the weight on the way up.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 9: Face Pulls
('22222222-2222-2222-2222-222222222224', 'Face Pulls', 'A rear deltoid and upper back exercise performed with a cable machine.', 1, 2, 3, 2, 'Set cable at face height, pull rope to face level, separate handles at end, squeeze rear delts.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 10: Bent-Over Rows
('22222222-2222-2222-2222-222222222225', 'Bent-Over Rows', 'A compound exercise targeting the middle and upper back, biceps, and rear deltoids.', 1, 2, 2, 4, 'Bend forward at hips, keep back straight, pull dumbbells or barbell to lower chest/upper abdomen.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 11: Back Squat
('33333333-3333-3333-3333-333333333331', 'Back Squat', 'The king of leg exercises. A compound movement targeting quadriceps, glutes, and core.', 1, 3, 8, 10, 'Bar on upper back, feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 12: Front Squat
('33333333-3333-3333-3333-333333333332', 'Front Squat', 'A squat variation with the bar in front, emphasizing quadriceps and core strength.', 1, 3, 8, 7, 'Bar across front deltoids, elbows up, squat down keeping torso upright, drive up through midfoot.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 13: Goblet Squat
('33333333-3333-3333-3333-333333333333', 'Goblet Squat', 'A beginner-friendly squat variation using a single dumbbell or kettlebell.', 1, 3, 8, 10, 'Hold weight at chest, squat down keeping torso upright, drive up through heels.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 14: Bodyweight Squat
('33333333-3333-3333-3333-333333333334', 'Bodyweight Squat', 'A fundamental bodyweight exercise for lower body strength. No equipment needed.', 3, 3, 8, 10, 'Feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 15: Jump Squat
('33333333-3333-3333-3333-333333333335', 'Jump Squat', 'A plyometric variation of the squat that adds explosive power and cardiovascular benefits.', 3, 3, 8, 10, 'Perform a squat, then explosively jump up, land softly and immediately go into next squat.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 16: Deadlift
('44444444-4444-4444-4444-444444444441', 'Deadlift', 'The ultimate posterior chain exercise. Targets hamstrings, glutes, and entire back.', 1, 4, 9, 10, 'Feet hip-width apart, bar over midfoot, hinge at hips, keep back straight, drive hips forward to stand.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 17: Romanian Deadlift
('44444444-4444-4444-4444-444444444442', 'Romanian Deadlift', 'A deadlift variation emphasizing hamstring and glute stretch and strength.', 1, 4, 9, 10, 'Start standing, hinge at hips keeping legs relatively straight, lower bar along legs, feel hamstring stretch, return.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 18: Good Mornings
('44444444-4444-4444-4444-444444444443', 'Good Mornings', 'A posterior chain exercise performed with a barbell on the upper back.', 1, 4, 9, 10, 'Bar on upper back, hinge at hips keeping back straight, lower torso until parallel to floor, return.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 19: Hip Thrust
('44444444-4444-4444-4444-444444444444', 'Hip Thrust', 'An exercise that directly targets the glutes through hip extension.', 1, 4, 10, 9, 'Upper back on bench, bar across hips, drive hips up squeezing glutes, lower with control.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 20: Kettlebell Swing
('44444444-4444-4444-4444-444444444445', 'Kettlebell Swing', 'A dynamic hip hinge movement that builds power and endurance in the posterior chain.', 1, 4, 10, 9, 'Hinge at hips, swing kettlebell from between legs to chest height using hip drive, not arms.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 21: Walking Lunges
('55555555-5555-5555-5555-555555555551', 'Walking Lunges', 'A dynamic unilateral leg exercise that improves balance and leg strength.', 1, 5, 8, 10, 'Step forward into lunge position, both knees at 90 degrees, push off front foot to step forward with other leg.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 22: Reverse Lunges
('55555555-5555-5555-5555-555555555552', 'Reverse Lunges', 'A lunge variation that reduces knee stress while targeting quadriceps and glutes.', 1, 5, 8, 10, 'Step backward into lunge, front knee at 90 degrees, push through front heel to return to start.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 23: Bulgarian Split Squat
('55555555-5555-5555-5555-555555555553', 'Bulgarian Split Squat', 'An intense unilateral leg exercise performed with rear foot elevated.', 1, 5, 8, 10, 'Rear foot on bench, front leg does the work, squat down until front thigh parallel, drive up.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 24: Lateral Lunges
('55555555-5555-5555-5555-555555555554', 'Lateral Lunges', 'A side-to-side lunge variation that targets the inner thighs and glutes.', 1, 5, 8, 10, 'Step to the side, shift weight to that leg, squat down keeping other leg straight, return to center.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 25: Forward Lunges
('55555555-5555-5555-5555-555555555555', 'Forward Lunges', 'A classic lunge variation stepping forward to target quadriceps and glutes.', 1, 5, 8, 10, 'Step forward into lunge, both knees at 90 degrees, push through front heel to return to start.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 26: Farmer's Walk
('66666666-6666-6666-6666-666666666661', 'Farmer''s Walk', 'A full-body strength and conditioning exercise. Carry heavy weights while walking.', 1, 6, 12, 7, 'Pick up heavy weights (dumbbells, kettlebells, or farmer''s walk handles), walk for distance or time, keep core tight.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 27: Suitcase Carry
('66666666-6666-6666-6666-666666666662', 'Suitcase Carry', 'A unilateral carry that challenges core stability and grip strength.', 1, 6, 7, 12, 'Carry a single heavy weight at your side, walk while resisting lateral flexion, switch sides.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 28: Overhead Carry
('66666666-6666-6666-6666-666666666663', 'Overhead Carry', 'A carry variation with weight held overhead, challenging shoulder stability and core.', 1, 6, 3, 7, 'Press weight overhead, walk while maintaining overhead position, keep core engaged.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 29: Rack Carry
('66666666-6666-6666-6666-666666666664', 'Rack Carry', 'Carrying weight in the front rack position, challenging core and upper body.', 1, 6, 7, 12, 'Hold weight(s) at shoulder height in front rack position, walk maintaining position.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 30: Plank
('77777777-7777-7777-7777-777777777771', 'Plank', 'A fundamental core strengthening exercise performed in a static position.', 3, 7, 7, NULL, 'Hold body in straight line on forearms and toes, keep core tight, don''t let hips sag or rise.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 31: Wall Sit
('77777777-7777-7777-7777-777777777772', 'Wall Sit', 'An isometric leg exercise that builds quadriceps and glute endurance.', 3, 7, 8, 10, 'Back against wall, slide down until thighs parallel to floor, hold position, keep back flat against wall.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 32: Hollow Body Hold
('77777777-7777-7777-7777-777777777773', 'Hollow Body Hold', 'An advanced core exercise that targets the entire anterior core chain.', 3, 7, 7, NULL, 'Lie on back, lift shoulders and legs off ground, maintain banana shape, keep lower back pressed to floor.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 33: L-Sit
('77777777-7777-7777-7777-777777777774', 'L-Sit', 'An advanced isometric exercise that challenges core, hip flexors, and triceps.', 3, 7, 7, 5, 'Support body on parallel bars or floor, lift legs to form L-shape, hold position.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 34: Dead Hang
('77777777-7777-7777-7777-777777777775', 'Dead Hang', 'An isometric exercise that improves grip strength and shoulder mobility.', 3, 7, 2, 6, 'Hang from pull-up bar with straight arms, hold position, focus on grip and shoulder engagement.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 35: Running
('88888888-8888-8888-8888-888888888881', 'Running', 'A fundamental cardiovascular exercise that improves endurance and cardiovascular health.', 2, 8, 13, 12, 'Maintain steady pace, land on midfoot, keep posture upright, breathe rhythmically.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 36: Cycling
('88888888-8888-8888-8888-888888888882', 'Cycling', 'A low-impact cardiovascular exercise that targets the legs and improves endurance.', 2, 8, 8, 13, 'Maintain consistent cadence, adjust resistance for intensity, keep core engaged.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 37: Rowing
('88888888-8888-8888-8888-888888888883', 'Rowing', 'A full-body cardiovascular exercise that targets legs, back, and core.', 2, 8, 12, 13, 'Drive with legs, lean back slightly, pull handle to chest, return with control.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 38: Burpees
('88888888-8888-8888-8888-888888888884', 'Burpees', 'A high-intensity full-body exercise combining squat, push-up, and jump.', 3, 8, 12, 13, 'Squat down, jump feet back to plank, do push-up, jump feet forward, jump up with arms overhead.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 39: Jump Rope
('88888888-8888-8888-8888-888888888885', 'Jump Rope', 'A high-intensity cardiovascular exercise that improves coordination and endurance.', 2, 8, 13, 11, 'Keep elbows close, jump on balls of feet, maintain rhythm, start slow and build speed.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 40: Hip Flexor Stretch
('99999999-9999-9999-9999-999999999991', 'Hip Flexor Stretch', 'A stretching exercise that improves hip flexibility and reduces lower back tension.', 4, 9, 7, NULL, 'Kneel on one knee, other foot forward, push hips forward until stretch felt in front of hip, hold 30-60 seconds.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 41: Hamstring Stretch
('99999999-9999-9999-9999-999999999992', 'Hamstring Stretch', 'A flexibility exercise targeting the hamstring muscles.', 4, 9, 9, NULL, 'Sit or stand, extend one leg, lean forward until stretch felt in back of thigh, hold 30-60 seconds.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 42: Shoulder Mobility
('99999999-9999-9999-9999-999999999993', 'Shoulder Mobility', 'A mobility exercise that improves shoulder range of motion and flexibility.', 4, 9, 3, NULL, 'Perform arm circles, cross-body stretches, and wall slides to improve shoulder mobility.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 43: Cat-Cow Stretch
('99999999-9999-9999-9999-999999999994', 'Cat-Cow Stretch', 'A spinal mobility exercise that improves flexibility in the back and core.', 4, 9, 7, 2, 'Start on hands and knees, arch back (cow), then round back (cat), move slowly between positions.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 44: Pigeon Pose
('99999999-9999-9999-9999-999999999995', 'Pigeon Pose', 'A yoga-inspired stretch that targets the hip flexors and glutes.', 4, 9, 10, 7, 'From plank, bring one knee forward to same-side wrist, extend other leg back, hold and breathe.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 45: Thruster
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Thruster', 'A full-body exercise combining front squat and overhead press in one movement.', 1, 3, 12, 3, 'Hold weight at shoulder height, squat down, drive up explosively and press weight overhead.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 46: Turkish Get-Up
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab', 'Turkish Get-Up', 'A complex full-body movement that improves stability, strength, and coordination.', 1, 6, 12, 7, 'Lie on back holding weight overhead, get up to standing position through series of movements, reverse to return.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 47: Renegade Row
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac', 'Renegade Row', 'A compound exercise combining plank and rowing motion for core and back.', 1, 2, 2, 7, 'In plank position with dumbbells, row one weight to side while maintaining plank, alternate sides.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 48: Mountain Climbers
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad', 'Mountain Climbers', 'A dynamic core and cardio exercise performed in plank position.', 3, 8, 7, 13, 'In plank position, alternate bringing knees to chest rapidly, keep core engaged throughout.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 49: Calf Raises
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaae', 'Calf Raises', 'An isolation exercise targeting the calf muscles.', 1, 1, 11, NULL, 'Stand on balls of feet, raise up onto toes, lower with control, can add weight for resistance.', NULL, true, NULL, '2024-01-01 00:00:00.000 +0000'::timestamp with time zone, '[]'::text),
-- Row 50: Push Jerk (rack)
('b6a1b1dd-b4ef-4e9e-ac2b-297b4b64c9d1', 'Push Jerk (rack)', 'An Olympic weightlifting movement combining a front squat with an overhead press.', 1, 1, 3, 5, 'Start with barbell in front rack position, dip slightly then drive up explosively while pressing the bar overhead, split feet if needed.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[]'::text),
-- Row 51: Incline Barbell Press
('fda07b99-e6ab-4920-ad3c-167896e7bb88', 'Incline Barbell Press', 'A chest exercise performed on an inclined bench targeting upper chest and shoulders.', 1, 1, 1, 3, 'Lie on inclined bench, grip bar slightly wider than shoulders, lower to upper chest, press up explosively.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 52: Plyo Push-Up
('728332bf-cdef-4dbf-970b-444c87593e3e', 'Plyo Push-Up', 'An explosive push-up variation that builds power in chest, shoulders, and triceps.', 3, 1, 1, 5, 'Start in push-up position, lower chest to ground then explode up with enough force to clap hands.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[]'::text),
-- Row 53: Half Kneeling Landmine Press
('e99e22c4-5131-49fa-8ed7-d81ebb6c1ce4', 'Half Kneeling Landmine Press', 'A unilateral shoulder press using a landmine setup for stability.', 1, 1, 3, 5, 'Kneel with one knee down, press landmine bar from shoulder height overhead.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Landmine", "IsRequired": true}]'::text),
-- Row 54: DB Bench
('c4898ce9-e069-4115-899b-cc634a753fe8', 'DB Bench', 'A dumbbell variation of the bench press targeting chest, shoulders, and triceps.', 1, 1, 1, 5, 'Lie on bench with dumbbells, press up from chest height, control the descent.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 55: DB Arnold Press
('184d0b26-6fc6-4e56-bb91-257beddb7bd4', 'DB Arnold Press', 'A shoulder press variation with rotation for full shoulder development.', 1, 1, 3, 5, 'Start with palms facing you, rotate outward as you press overhead, reverse on descent.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}]'::text),
-- Row 56: Cable Fly (low to high)
('429d87bb-b7a8-4780-9342-4c54b23d6587', 'Cable Fly (low to high)', 'A cable chest fly targeting upper chest fibers.', 1, 1, 1, NULL, 'Set cables low, pull handles up and together in front of chest, squeeze chest at top.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": true}]'::text),
-- Row 57: Overhead Rope Triceps Extension
('913e4491-851d-4db6-844f-742662654423', 'Overhead Rope Triceps Extension', 'A triceps isolation exercise using a rope attachment.', 1, 1, 5, NULL, 'Hold rope overhead, lower behind head by bending elbows, extend arms to return.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": true}, {"Name": "Rope Attachment", "IsRequired": true}]'::text),
-- Row 58: Chest Supported DB Row
('05c31822-535b-42bb-a484-5669755d3601', 'Chest Supported DB Row', 'A rowing variation performed on an incline bench for better isolation.', 1, 2, 2, 4, 'Lie chest-down on incline bench, row dumbbells to hips, squeeze back muscles.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 59: Cable Face Pull
('6441030a-4a30-4052-9bb9-13c93b8476ab', 'Cable Face Pull', 'A rear deltoid and upper back exercise using a rope attachment.', 1, 2, 3, 2, 'Set cable at face height, pull rope to face with elbows high, squeeze rear delts.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": true}, {"Name": "Rope Attachment", "IsRequired": true}]'::text),
-- Row 60: EZ-bar Reverse Curl
('c6a29a86-4f93-4fdf-b952-b54358091108', 'EZ-bar Reverse Curl', 'A bicep curl variation targeting the forearms and brachialis.', 1, 2, 4, 6, 'Hold EZ-bar with reverse grip, curl up to shoulders, control the descent.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "EZ-Bar", "IsRequired": true}]'::text),
-- Row 61: Hang Snatch Pull
('faccf760-957e-4976-8a07-b29e32367c1e', 'Hang Snatch Pull', 'A weightlifting movement focusing on the explosive hip drive.', 1, 2, 2, 3, 'Start in hang position, explode hips forward while pulling bar to hip level.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}]'::text),
-- Row 62: Strict Pull-up
('89bf355e-bcf5-409a-983e-96e340a54a86', 'Strict Pull-up', 'A bodyweight back exercise performed without momentum.', 3, 2, 2, 4, 'Hang from bar, pull up with straight body until chin clears bar, lower with control.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Pull-up Bar", "IsRequired": true}]'::text),
-- Row 63: Cable High Row
('1a930eb9-948f-459e-ba4d-2fb82d6fe99e', 'Cable High Row', 'A machine rowing exercise targeting mid-back muscles.', 1, 2, 2, 4, 'Sit at cable machine, pull handles to torso with elbows high, squeeze shoulder blades.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": true}]'::text),
-- Row 64: DB Lateral Raise
('b4235113-ed70-46df-90c7-55f59832bcdf', 'DB Lateral Raise', 'An isolation exercise targeting the medial deltoids.', 1, 2, 3, NULL, 'Hold dumbbells at sides, raise arms out to sides until parallel to floor, control descent.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}]'::text),
-- Row 65: Chest Supported Row
('199658c1-1a08-4539-8a2f-42629b7d653b', 'Chest Supported Row', 'A supported rowing variation for better back isolation.', 1, 2, 2, 4, 'Lie chest-down on bench, row weight to hips, focus on back contraction.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": false}, {"Name": "Barbell", "IsRequired": false}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 66: Deficit RDL
('9394bb36-937b-48c5-b265-e0292fd1616f', 'Deficit RDL', 'A Romanian deadlift variation performed from a deficit for increased range of motion.', 1, 4, 9, 10, 'Stand on small platform, hinge at hips with slight knee bend, lower bar along legs, feel hamstring stretch.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}, {"Name": "Platform", "IsRequired": true}]'::text),
-- Row 67: Barbell Hip Thrust
('ad4bb414-b47d-4e94-88ef-ed767590a0c3', 'Barbell Hip Thrust', 'A glute-focused hip extension exercise using a barbell.', 1, 4, 10, 9, 'Upper back on bench, bar across hips, drive hips up squeezing glutes, hold at top.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 68: Power Clean from Blocks
('1e37acb0-5ead-4aff-a536-e09125e1d63d', 'Power Clean from Blocks', 'An Olympic lift variation starting from elevated blocks.', 1, 4, 12, 3, 'Start with bar on blocks at knee height, explode up pulling bar to shoulders in one motion.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}, {"Name": "Blocks", "IsRequired": true}]'::text),
-- Row 69: Rear-Foot Elevated Split Squat (DB)
('19656685-64a4-445a-9667-9ea585ef0b51', 'Rear-Foot Elevated Split Squat (DB)', 'A single-leg exercise with rear foot elevated for increased range of motion.', 1, 5, 8, 10, 'Rear foot on bench, hold dumbbells, lower until front thigh parallel to floor, drive up.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 70: Pavlov Press Walkout
('89790898-ac99-448e-a534-e1fc840c45bd', 'Pavlov Press Walkout', 'A walking overhead press variation combining mobility and strength.', 1, 6, 3, 7, 'Walk out to plank position with hands on dumbbells, then walk back while pressing dumbbells overhead.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}]'::text),
-- Row 71: Zercher Squat
('9e2354be-17df-4807-844d-6e49ddb23fe4', 'Zercher Squat', 'A front-loaded squat variation with bar held in crooks of elbows.', 1, 3, 8, 7, 'Hold bar in elbows, squat down keeping torso upright, drive up through heels.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Barbell", "IsRequired": true}]'::text),
-- Row 72: Smith Machine Hack Squat
('022e91ee-c07d-47d8-8fe8-90903e709173', 'Smith Machine Hack Squat', 'A squat variation using the Smith machine for stability.', 1, 3, 8, 10, 'Position feet forward, lower by bending knees, drive up keeping back against pads.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Smith Machine", "IsRequired": true}]'::text),
-- Row 73: Nordic
('3d33035f-d866-4fe9-ad0c-1ff4c6c7b22e', 'Nordic', 'An eccentric hamstring exercise performed from kneeling position.', 3, 7, 9, NULL, 'Kneel with partner holding ankles, slowly lower torso to ground using hamstrings, push back up.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[]'::text),
-- Row 74: Box Jump
('c450403b-5248-4781-80a2-5c29904d9a83', 'Box Jump', 'A plyometric exercise jumping onto and off a box.', 3, 8, 8, 10, 'Stand facing box, squat then jump onto box, step down or jump down carefully.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Box", "IsRequired": true}]'::text),
-- Row 75: Broad Jump
('3f48081c-7c31-4b15-9a8b-8c9f6c868d42', 'Broad Jump', 'A horizontal jumping exercise for power development.', 3, 8, 8, 10, 'Squat down, swing arms back, jump forward as far as possible, land softly.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[]'::text),
-- Row 76: Incline DB Curl
('5d756b74-02fe-40a6-96c2-279522775c8f', 'Incline DB Curl', 'A bicep curl performed on an incline bench for better isolation.', 1, 9, 4, NULL, 'Lie on incline bench, curl dumbbells to shoulders, control the descent.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}, {"Name": "Bench", "IsRequired": true}]'::text),
-- Row 77: Wood Choppers
('d54a8b7c-147b-4f9d-8c7f-99431ddffbee', 'Wood Choppers', 'A rotational core exercise mimicking a chopping motion.', 1, 9, 7, 3, 'Hold weight overhead, rotate and chop down across body to opposite hip.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": false}, {"Name": "Medicine Ball", "IsRequired": false}]'::text),
-- Row 78: Ab Rollout
('bd3b9493-e581-4349-b468-e175a4bbd04d', 'Ab Rollout', 'An advanced core exercise using an ab wheel or barbell.', 3, 7, 7, NULL, 'Kneel with wheel, roll forward extending body, keep core tight, roll back.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Ab Wheel", "IsRequired": false}, {"Name": "Barbell", "IsRequired": false}]'::text),
-- Row 79: Weighted Leg Raise
('d9cefaee-c114-472e-9026-07491a9d1d79', 'Weighted Leg Raise', 'A hanging leg raise variation with added weight.', 1, 7, 7, NULL, 'Hang from bar, raise legs to parallel or higher while holding weight between feet.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Pull-up Bar", "IsRequired": true}, {"Name": "Weight", "IsRequired": false}]'::text),
-- Row 80: Cable Crunch
('09c7f511-6d17-4e84-8a14-f1e253fdb6ef', 'Cable Crunch', 'A machine-assisted abdominal crunch exercise.', 1, 7, 7, NULL, 'Kneel facing cable machine, crunch down pulling cable to knees, squeeze abs.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Cable Machine", "IsRequired": true}]'::text),
-- Row 81: Standing Single-Leg Calf Raise (DB)
('ac64e6a6-8902-4b6f-af5e-5e254cc884f1', 'Standing Single-Leg Calf Raise (DB)', 'A unilateral calf exercise performed standing with dumbbells.', 1, 1, 11, NULL, 'Hold dumbbells, stand on one leg, raise up onto toes, lower with control, switch legs.', NULL, true, NULL, '2026-01-03 22:00:46.614 +0000'::timestamp with time zone, '[{"Name": "Dumbbells", "IsRequired": true}]'::text)
ON CONFLICT ("Id") DO UPDATE SET
    "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Type" = EXCLUDED."Type",
    "MovementPattern" = EXCLUDED."MovementPattern",
    "PrimaryMuscleGroup" = EXCLUDED."PrimaryMuscleGroup",
    "SecondaryMuscleGroup" = EXCLUDED."SecondaryMuscleGroup",
    "Instructions" = EXCLUDED."Instructions",
    "VideoUrl" = EXCLUDED."VideoUrl",
    "IsSystemExercise" = EXCLUDED."IsSystemExercise",
    "CreatedByUserId" = EXCLUDED."CreatedByUserId",
    "CreatedAt" = EXCLUDED."CreatedAt",
    "EquipmentRequirements" = EXCLUDED."EquipmentRequirements";

COMMIT;
