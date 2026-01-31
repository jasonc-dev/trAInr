using trAInr.Domain.Entities;

namespace trAInr.Infrastructure.Data.SeedData;

public static class EquipmentSeed
{
    public static List<Equipment> GetEquipmentSeedData()
    {
        return new List<Equipment>
        {
            // Free weights
            Equipment.Create("Barbell", "free_weight", false),
            Equipment.Create("Dumbbell", "free_weight", true),
            Equipment.Create("Kettlebell", "free_weight", true),
            Equipment.Create("EZ Bar", "free_weight", false),
            Equipment.Create("Trap Bar", "free_weight", false),
            Equipment.Create("Weight Plates", "free_weight", true),
            
            // Machines
            Equipment.Create("Cable Machine", "cable", false),
            Equipment.Create("Leg Press", "machine", false),
            Equipment.Create("Leg Extension", "machine", false),
            Equipment.Create("Leg Curl", "machine", false),
            Equipment.Create("Smith Machine", "machine", false),
            Equipment.Create("Chest Press Machine", "machine", false),
            Equipment.Create("Shoulder Press Machine", "machine", false),
            Equipment.Create("Lat Pulldown", "machine", false),
            Equipment.Create("Seated Row Machine", "machine", false),
            Equipment.Create("Pec Deck", "machine", false),
            Equipment.Create("Hip Abductor", "machine", false),
            Equipment.Create("Hip Adductor", "machine", false),
            Equipment.Create("Calf Raise Machine", "machine", false),
            Equipment.Create("Back Extension", "machine", false),
            
            // Bodyweight/Minimal
            Equipment.Create("Bodyweight", "bodyweight", true),
            Equipment.Create("Pull-up Bar", "bodyweight", false),
            Equipment.Create("Dip Station", "bodyweight", false),
            Equipment.Create("Resistance Bands", "bands", true),
            Equipment.Create("Suspension Trainer", "bodyweight", true),
            
            // Benches & Racks
            Equipment.Create("Flat Bench", "bench", false),
            Equipment.Create("Incline Bench", "bench", false),
            Equipment.Create("Decline Bench", "bench", false),
            Equipment.Create("Adjustable Bench", "bench", false),
            Equipment.Create("Squat Rack", "rack", false),
            Equipment.Create("Power Rack", "rack", false),
            
            // Cardio
            Equipment.Create("Treadmill", "cardio", false),
            Equipment.Create("Stationary Bike", "cardio", false),
            Equipment.Create("Rowing Machine", "cardio", false),
            Equipment.Create("Elliptical", "cardio", false),
            Equipment.Create("Assault Bike", "cardio", false),
            Equipment.Create("Ski Erg", "cardio", false),
            
            // Accessories
            Equipment.Create("Yoga Mat", "accessory", true),
            Equipment.Create("Foam Roller", "accessory", true),
            Equipment.Create("Medicine Ball", "accessory", true),
            Equipment.Create("Bosu Ball", "accessory", true),
            Equipment.Create("Battle Ropes", "accessory", false),
            Equipment.Create("Plyo Box", "accessory", false),
            Equipment.Create("Sandbag", "accessory", true),
            Equipment.Create("Sled", "accessory", false),
            Equipment.Create("Ab Wheel", "accessory", true)
        };
    }
}
