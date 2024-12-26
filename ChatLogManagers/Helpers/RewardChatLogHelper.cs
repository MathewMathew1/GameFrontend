using System;
using System.Collections.Generic;
using System.Text;
using BoardGameFrontend.Models;

// Assuming the necessary namespaces and classes are defined, including Reward, Resource, HeroResource, AuraTypeWithLongevity, etc.

namespace BoardGameFrontend.Helpers
{
    public static class RewardDescriptionBuilder
    {
    
        public static string BuildDescription(Reward reward)
        {
            StringBuilder description = new StringBuilder();

            if (reward.Resources != null && reward.Resources.Count > 0)
            {
                description.Append("has received the following resources:\n");
                foreach (var resource in reward.Resources)
                {
                    description.Append($"- {resource.Type}: {resource.Amount}\n"); 
                }
            }

            if (reward.HeroResources != null && reward.HeroResources.Count > 0)
            {
                description.Append("has received hero resources:\n");
                foreach (var heroResource in reward.HeroResources)
                {
                    description.Append($"- {heroResource.Type}: {heroResource.Amount}\n"); 
                }
            }

            if (reward.AurasTypes != null && reward.AurasTypes.Count > 0)
            {
                description.Append("has received the following auras:\n");
                foreach (var aura in reward.AurasTypes)
                {
                    string permanence = aura.Permanent ? " (Permanent)" : " (Temporary)";
                    description.Append($"- {aura.Aura}{permanence}\n"); 
                }
            }

            if (reward.EndGameAura != null && reward.EndGameAura.Count > 0)
            {
                description.Append("has received end game auras:\n");
                foreach (var endGameAura in reward.EndGameAura)
                {
                    description.Append($"- {endGameAura}\n"); 
                }
            }

            if (reward.Morale.HasValue)
            {
                description.Append($"has been granted {reward.Morale.Value} morales.\n");
            }

            return description.ToString().TrimEnd();
        }
    }
}