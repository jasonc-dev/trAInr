using trAInr.Domain.Entities;

namespace trAInr.Infrastructure.Data.SeedData;

public static class EquipmentSeed
{
    public static List<Equipment> GetEquipmentSeedData()
    {
        return new List<Equipment>
        {
            // Free weights
            Equipment.Create("Barbell", "free_weight"),
            Equipment.Create("Dumbbell", "free_weight", true),
            Equipment.Create("Kettlebell", "free_weight", true),
            Equipment.Create("EZ Bar", "free_weight"),
            Equipment.Create("Trap Bar", "free_weight"),
            Equipment.Create("Weight Plates", "free_weight", true),
            
            // Machines
            Equipment.Create("Cable Machine", "cable"),
            Equipment.Create("Leg Press", "machine"),
            Equipment.Create("Leg Extension", "machine"),
            Equipment.Create("Leg Curl", "machine"),
            Equipment.Create("Smith Machine", "machine"),
            Equipment.Create("Chest Press Machine", "machine"),
            Equipment.Create("Shoulder Press Machine", "machine"),
            Equipment.Create("Lat Pulldown", "machine"),
            Equipment.Create("Seated Row Machine", "machine"),
            Equipment.Create("Pec Deck", "machine"),
            Equipment.Create("Hip Abductor", "machine"),
            Equipment.Create("Hip Adductor", "machine"),
            Equipment.Create("Calf Raise Machine", "machine"),
            Equipment.Create("Back Extension", "machine"),
            
            // Bodyweight/Minimal
            Equipment.Create("Bodyweight", "bodyweight", true),
            Equipment.Create("Pull-up Bar", "bodyweight"),
            Equipment.Create("Dip Station", "bodyweight"),
            Equipment.Create("Resistance Bands", "bands", true),
            Equipment.Create("Suspension Trainer", "bodyweight", true),
            
            // Benches & Racks
            Equipment.Create("Flat Bench", "bench"),
            Equipment.Create("Incline Bench", "bench"),
            Equipment.Create("Decline Bench", "bench"),
            Equipment.Create("Adjustable Bench", "bench"),
            Equipment.Create("Squat Rack", "rack"),
            Equipment.Create("Power Rack", "rack"),
            
            // Cardio
            Equipment.Create("Treadmill", "cardio"),
            Equipment.Create("Stationary Bike", "cardio"),
            Equipment.Create("Rowing Machine", "cardio"),
            Equipment.Create("Elliptical", "cardio"),
            Equipment.Create("Assault Bike", "cardio"),
            Equipment.Create("Ski Erg", "cardio"),
            
            // Accessories
            Equipment.Create("Yoga Mat", "accessory", true),
            Equipment.Create("Foam Roller", "accessory", true),
            Equipment.Create("Medicine Ball", "accessory", true),
            Equipment.Create("Bosu Ball", "accessory", true),
            Equipment.Create("Battle Ropes", "accessory"),
            Equipment.Create("Plyo Box", "accessory"),
            Equipment.Create("Sandbag", "accessory", true),
            Equipment.Create("Sled", "accessory"),
            Equipment.Create("Ab Wheel", "accessory", true)
        };
    }
}
