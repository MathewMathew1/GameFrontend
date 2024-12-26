using System.Collections.Generic;

namespace BoardGameFrontend.Models
{
    public class Reward
    {
        public List<Resource> Resources { get; set; }
        public List<HeroResource> HeroResources { get; set; }
        public List<AuraTypeWithLongevity> AurasTypes { get; set; }
        public List<EndGameAura> EndGameAura { get; set; }
        public int? Morale {get; set;}

        public Reward()
        {
            Resources = new List<Resource>();
            AurasTypes = new List<AuraTypeWithLongevity>();
            HeroResources = new List<HeroResource>();
            EndGameAura = new List<EndGameAura>();
        }
    }
}