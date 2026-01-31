using trAInr.Domain.Entities;

namespace trAInr.Infrastructure.Data.SeedData;

public static class MuscleSeed
{
    public static List<Muscle> GetMuscleSeedData()
    {
        return new List<Muscle>
        {
            // Chest
            Muscle.Create("Pectoralis Major", "chest"),
            Muscle.Create("Pectoralis Minor", "chest"),
            
            // Back
            Muscle.Create("Latissimus Dorsi", "back"),
            Muscle.Create("Trapezius Upper", "back"),
            Muscle.Create("Trapezius Middle", "back"),
            Muscle.Create("Trapezius Lower", "back"),
            Muscle.Create("Rhomboids", "back"),
            Muscle.Create("Erector Spinae", "back"),
            Muscle.Create("Teres Major", "back"),
            Muscle.Create("Infraspinatus", "back"),
            
            // Shoulders
            Muscle.Create("Anterior Deltoid", "shoulders"),
            Muscle.Create("Lateral Deltoid", "shoulders"),
            Muscle.Create("Posterior Deltoid", "shoulders"),
            Muscle.Create("Rotator Cuff", "shoulders"),
            
            // Arms
            Muscle.Create("Biceps Brachii", "arms"),
            Muscle.Create("Brachialis", "arms"),
            Muscle.Create("Brachioradialis", "arms"),
            Muscle.Create("Triceps Brachii", "arms"),
            Muscle.Create("Forearm Flexors", "arms"),
            Muscle.Create("Forearm Extensors", "arms"),
            
            // Core
            Muscle.Create("Rectus Abdominis", "core"),
            Muscle.Create("External Obliques", "core"),
            Muscle.Create("Internal Obliques", "core"),
            Muscle.Create("Transverse Abdominis", "core"),
            Muscle.Create("Serratus Anterior", "core"),
            
            // Legs - Quads
            Muscle.Create("Rectus Femoris", "legs"),
            Muscle.Create("Vastus Lateralis", "legs"),
            Muscle.Create("Vastus Medialis", "legs"),
            Muscle.Create("Vastus Intermedius", "legs"),
            
            // Legs - Hamstrings
            Muscle.Create("Biceps Femoris", "legs"),
            Muscle.Create("Semitendinosus", "legs"),
            Muscle.Create("Semimembranosus", "legs"),
            
            // Legs - Glutes
            Muscle.Create("Gluteus Maximus", "legs"),
            Muscle.Create("Gluteus Medius", "legs"),
            Muscle.Create("Gluteus Minimus", "legs"),
            
            // Legs - Calves
            Muscle.Create("Gastrocnemius", "legs"),
            Muscle.Create("Soleus", "legs"),
            
            // Legs - Other
            Muscle.Create("Hip Adductors", "legs"),
            Muscle.Create("Hip Abductors", "legs"),
            Muscle.Create("Hip Flexors", "legs")
        };
    }
}
