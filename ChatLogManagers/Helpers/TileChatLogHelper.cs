using System;
using System.Collections.Generic;
using System.Text;
using BoardGameFrontend.Models;

// Assuming necessary namespaces and classes (Resource, Artifact) are defined.

namespace BoardGameFrontend.Helpers
{
    public class TileRewardDescriptionBuilder
    {

        public static string BuildDescription(TileReward tileReward)
        {
            StringBuilder description = new StringBuilder();

            if (tileReward.Resources != null && tileReward.Resources.Count > 0)
            {
                description.Append("has received the following resources:\n");
                foreach (var resource in tileReward.Resources)
                {
                    description.Append($"- {resource.Type}: {resource.Amount}\n"); 
                }
            }

            if (tileReward.ExperiencePoints.HasValue)
            {
                description.Append($"has {tileReward.ExperiencePoints.Value} experience points.\n");
            }

            if (tileReward.TeleportedTileId.HasValue)
            {
                description.Append($"has teleported pawn to tile ID: {tileReward.TeleportedTileId.Value}\n");
            }

            if (tileReward.RerollMercenaryAction.HasValue)
            {
                description.Append("has the option to reroll the mercenary action.\n");
            }

            if (tileReward.GetRandomArtifact.HasValue)
            {
                description.Append("has received a random artifact.\n");
            }

            if (tileReward.TempSignet == true)
            {
                description.Append("received temporary signet.\n");
            }


            if (tileReward.Artifact != null)
            {
                description.Append($"has received the artifact: {tileReward.Artifact.NameEng}.\n"); 
            }

            if (tileReward.GotArtifact)
            {
                description.Append("has obtained the artifact!\n");
            }

            if (tileReward.TokenReward != null)
            {
                description.Append("has received a token reward:\n");
                description.Append($"- {RewardDescriptionBuilder.BuildDescription(tileReward.TokenReward.Reward)}\n"); 
            }


            return description.ToString().TrimEnd();
        }
    }
}
