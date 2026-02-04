using trAInr.Domain.Entities;
using trAInr.Domain.Enums;

namespace trAInr.Infrastructure.Data.SeedData;

public static class TagSeed
{
    public static List<Tag> GetTagSeedData()
    {
        return new List<Tag>
        {
            // Goal tags
            Tag.Create(TagNamespace.Goal, "strength"),
            Tag.Create(TagNamespace.Goal, "hypertrophy"),
            Tag.Create(TagNamespace.Goal, "power"),
            Tag.Create(TagNamespace.Goal, "endurance"),
            Tag.Create(TagNamespace.Goal, "fat_loss"),
            Tag.Create(TagNamespace.Goal, "mobility"),
            Tag.Create(TagNamespace.Goal, "athletic_performance"),
            
            // Joint friendly tags
            Tag.Create(TagNamespace.JointFriendly, "knee_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "shoulder_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "low_back_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "wrist_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "elbow_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "hip_friendly"),
            Tag.Create(TagNamespace.JointFriendly, "ankle_friendly"),
            
            // Contraindication tags
            Tag.Create(TagNamespace.Contra, "wrist_stress"),
            Tag.Create(TagNamespace.Contra, "high_spine_load"),
            Tag.Create(TagNamespace.Contra, "overhead_required"),
            Tag.Create(TagNamespace.Contra, "knee_flexion_deep"),
            Tag.Create(TagNamespace.Contra, "shoulder_impingement_risk"),
            Tag.Create(TagNamespace.Contra, "high_impact"),
            Tag.Create(TagNamespace.Contra, "ballistic"),
            Tag.Create(TagNamespace.Contra, "requires_mobility"),
            
            // Context tags
            Tag.Create(TagNamespace.Context, "home_gym"),
            Tag.Create(TagNamespace.Context, "commercial_gym"),
            Tag.Create(TagNamespace.Context, "minimal_setup"),
            Tag.Create(TagNamespace.Context, "travel_friendly"),
            Tag.Create(TagNamespace.Context, "beginner_friendly"),
            Tag.Create(TagNamespace.Context, "advanced_only"),
            Tag.Create(TagNamespace.Context, "time_efficient"),
            Tag.Create(TagNamespace.Context, "requires_spotter"),
            
            // Sport tags
            Tag.Create(TagNamespace.Sport, "boxing"),
            Tag.Create(TagNamespace.Sport, "running"),
            Tag.Create(TagNamespace.Sport, "cycling"),
            Tag.Create(TagNamespace.Sport, "swimming"),
            Tag.Create(TagNamespace.Sport, "soccer"),
            Tag.Create(TagNamespace.Sport, "basketball"),
            Tag.Create(TagNamespace.Sport, "crossfit"),
            Tag.Create(TagNamespace.Sport, "powerlifting"),
            Tag.Create(TagNamespace.Sport, "bodybuilding"),
            Tag.Create(TagNamespace.Sport, "olympic_lifting")
        };
    }
}
