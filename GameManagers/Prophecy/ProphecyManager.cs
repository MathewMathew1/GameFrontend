
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BoardGameFrontend.Models
{
    public class ReqProphecies
    {
        public required int Id { get; set; }
        public required int IntValue1 { get; set; }
        public required int IntValue2 { get; set; }
    }

    public interface IProphecyPoints
    {
        bool GetCompletedProphecy(PlayerInGameViewModel player, MercenaryDisplay mercenary);
    }

    public abstract class BaseProphecyPoints : IProphecyPoints
    {
        protected int Value1 { get; }
        protected int Value2 { get; }

        protected BaseProphecyPoints(int value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }


        public bool GetCompletedProphecy(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            if (mercenary.IsAlwaysFulfilled) return true;


            return CalculatePoints(player, mercenary);
        }


        protected abstract bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary);
    }

    public class ProphecyTwenty : BaseProphecyPoints
    {
        public ProphecyTwenty(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualMagicThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            var moreOrEqualArmyThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            if (moreOrEqualArmyThanSiege && moreOrEqualMagicThanArmy) return true;

            return false;
        }
    }

    public class ProphecyTwentyOne : BaseProphecyPoints
    {
        public ProphecyTwentyOne(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualArmyThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            var moreOrEqualMagicThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            if (moreOrEqualArmyThanMagic && moreOrEqualMagicThanSiege) return true;

            return false;
        }
    }

    public class ProphecyTwentyTwo : BaseProphecyPoints
    {
        public ProphecyTwentyTwo(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualArmyThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            var moreOrEqualSiegeThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            if (moreOrEqualArmyThanSiege && moreOrEqualSiegeThanMagic) return true;

            return false;
        }
    }

    public class ProphecyTwentyThree : BaseProphecyPoints
    {
        public ProphecyTwentyThree(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualMagicThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            var moreOrEqualSiegeThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            if (moreOrEqualMagicThanSiege && moreOrEqualSiegeThanArmy) return true;

            return false;
        }
    }

    public class ProphecyTwentyFour : BaseProphecyPoints
    {
        public ProphecyTwentyFour(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualSiegeThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            var moreOrEqualMagicThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            if (moreOrEqualSiegeThanMagic && moreOrEqualMagicThanArmy) return true;

            return false;
        }
    }

    public class ProphecyTwentyFive : BaseProphecyPoints
    {
        public ProphecyTwentyFive(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var moreOrEqualSiegeThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            var moreOrEqualArmyThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            if (moreOrEqualSiegeThanArmy && moreOrEqualArmyThanMagic) return true;

            return false;
        }
    }

    public class ProphecyTwentySix : BaseProphecyPoints
    {
        public ProphecyTwentySix(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var amountOfConstructions = player.PlayerMercenariesManager.Mercenaries.Count(mercenaryDisplay => mercenaryDisplay.TypeCard == 2);
            if (amountOfConstructions >= Value2) return true;

            return false;
        }
    }

    public class ProphecyTwentySeven : BaseProphecyPoints
    {
        public ProphecyTwentySeven(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var amountOfMercenaries = player.PlayerMercenariesManager.Mercenaries.Count(m => m.TypeCard == 1);
            if (amountOfMercenaries >= Value2) return true;

            return false;
        }
    }

    public class ProphecyTwentyEight : BaseProphecyPoints
    {
        public ProphecyTwentyEight(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var allFactionsHaveAtLeastTwoHeroes = true;
            for (var i = 1; i < 5; i++)
            {
                var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == i);
                var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == i);

                if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight < Value2)
                {
                    allFactionsHaveAtLeastTwoHeroes = false;
                    break;
                }
            }

            if (allFactionsHaveAtLeastTwoHeroes) return true;

            return false;
        }
    }

    public class ProphecyTwentyNine : BaseProphecyPoints
    {
        public ProphecyTwentyNine(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight == 0) return true;

            return false;
        }
    }

    public class ProphecyThirty : BaseProphecyPoints
    {
        public ProphecyThirty(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight == 0) return true;

            return false;
        }
    }

    public class ProphecyThirtyOne : BaseProphecyPoints
    {
        public ProphecyThirtyOne(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight == 0) return true;

            return false;
        }
    }

    public class ProphecyThirtyTwo : BaseProphecyPoints
    {
        public ProphecyThirtyTwo(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight == 0) return true;

            return false;
        }
    }

    public class ProphecyThirtyThree : BaseProphecyPoints
    {
        public ProphecyThirtyThree(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight >= 5) return true;

            return false;
        }
    }

    public class ProphecyThirtyFour : BaseProphecyPoints
    {
        public ProphecyThirtyFour(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight >= 5) return true;

            return false;
        }
    }


    public class ProphecyThirtyFive : BaseProphecyPoints
    {
        public ProphecyThirtyFive(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight >= 5) return true;

            return false;
        }
    }

    public class ProphecyThirtySix : BaseProphecyPoints
    {
        public ProphecyThirtySix(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count(hero => hero.Faction.Id == Value2);
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count(hero => hero.Faction.Id == Value2);

            if (amountOfHerosWithFactionOnLeft + amountOfHerosWithFactionOnRight >= 5) return true;

            return false;
        }
    }

    public class ProphecyThirtySeven : BaseProphecyPoints
    {
        public ProphecyThirtySeven(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count();
            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count();

            if (amountOfHerosWithFactionOnLeft >= Value2 && amountOfHerosWithFactionOnRight >= Value2) return true;

            return false;
        }
    }

    public class ProphecyThirtyEight : BaseProphecyPoints
    {
        public ProphecyThirtyEight(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnRight = player.PlayerHeroCardManager.HeroCardsRight.Count();

            if (amountOfHerosWithFactionOnRight >= Value2) return true;

            return false;
        }
    }

    public class ProphecyThirtyNine : BaseProphecyPoints
    {
        public ProphecyThirtyNine(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfHerosWithFactionOnLeft = player.PlayerHeroCardManager.HeroCardsLeft.Count();

            if (amountOfHerosWithFactionOnLeft >= Value2) return true;

            return false;
        }
    }

    public class ProphecyNinetySeven : BaseProphecyPoints
    {
        public ProphecyNinetySeven(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var moreOrEqualSiegeThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            var moreOrEqualSiegeThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            if (moreOrEqualSiegeThanMagic && moreOrEqualSiegeThanArmy) return true;

            return false;
        }
    }

    public class ProphecyNinetyNine : BaseProphecyPoints
    {
        public ProphecyNinetyNine(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var moreOrEqualArmyThanMagic = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic);
            var moreOrEqualArmyThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            if (moreOrEqualArmyThanMagic && moreOrEqualArmyThanSiege) return true;

            return false;
        }
    }

    public class ProphecyNinetyEight : BaseProphecyPoints
    {
        public ProphecyNinetyEight(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var moreOrEqualMagicThanArmy = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Army);
            var moreOrEqualMagicThanSiege = player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Magic) >= player.ResourceHeroManager.GetResourceAmount(ResourceHeroType.Siege);
            if (moreOrEqualMagicThanArmy && moreOrEqualMagicThanSiege) return true;

            return false;
        }
    }

    public class ProphecyOneHundred : BaseProphecyPoints
    {
        public ProphecyOneHundred(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {

            var amountOfRoyalCards = player.PlayerRoyalCardManager.RolayCards.Count();
            if (amountOfRoyalCards >= Value2) return true;

            return false;
        }
    }

    public class ProphecyOneHundredOne : BaseProphecyPoints
    {
        public ProphecyOneHundredOne(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            if (player.Morale >= Value2) return true;

            return false;
        }
    }

    public class ProphecyOneHundredTwo : BaseProphecyPoints
    {
        public ProphecyOneHundredTwo(int value1, int value2) : base(value1, value2) { }

        protected override bool CalculatePoints(PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            if (player.PlayerArtifactManager.YourArtifacts.Count() >= Value2) return true;

            return false;
        }
    }


    public static class ProphecyRequirementStore
    {
        private static readonly Dictionary<int, IProphecyPoints> _requirements;

        static ProphecyRequirementStore()
        {
            string filePath = "Data/SpecialEffect.json";
            List<ReqProphecies> Reqs = new List<ReqProphecies>();

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                Reqs = JsonSerializer.Deserialize<List<ReqProphecies>>(jsonData) ?? new List<ReqProphecies>();
            }

            _requirements = new Dictionary<int, IProphecyPoints>();

            foreach (var requirementData in Reqs)
            {
                switch (requirementData.Id)
                {
                    case 20:
                        _requirements.Add(requirementData.Id, new ProphecyTwenty(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 21:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyOne(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 22:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyTwo(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 23:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyThree(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 24:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyFour(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 25:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyFive(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 26:
                        _requirements.Add(requirementData.Id, new ProphecyTwentySix(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 27:
                        _requirements.Add(requirementData.Id, new ProphecyTwentySeven(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 28:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyEight(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 29:
                        _requirements.Add(requirementData.Id, new ProphecyTwentyNine(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 30:
                        _requirements.Add(requirementData.Id, new ProphecyThirty(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 31:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyOne(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 32:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyTwo(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 33:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyThree(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 34:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyFour(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 35:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyFive(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 36:
                        _requirements.Add(requirementData.Id, new ProphecyThirtySix(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 37:
                        _requirements.Add(requirementData.Id, new ProphecyThirtySeven(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 38:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyEight(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 39:
                        _requirements.Add(requirementData.Id, new ProphecyThirtyNine(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 97:
                        _requirements.Add(requirementData.Id, new ProphecyNinetySeven(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 98:
                        _requirements.Add(requirementData.Id, new ProphecyNinetyEight(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 99:
                        _requirements.Add(requirementData.Id, new ProphecyNinetyNine(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 100:
                        _requirements.Add(requirementData.Id, new ProphecyOneHundred(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 101:
                        _requirements.Add(requirementData.Id, new ProphecyOneHundredOne(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    case 102:
                        _requirements.Add(requirementData.Id, new ProphecyOneHundredTwo(requirementData.IntValue1, requirementData.IntValue2));
                        break;
                    default:
                        break;
                }

            }
        }

        public static IProphecyPoints? GetRequirementById(int id)
        {
            if (_requirements.TryGetValue(id, out var requirement))
            {
                return requirement;
            }

            return null;
        }

        public static IEnumerable<IProphecyPoints> GetAllRequirements()
        {
            return _requirements.Values;
        }
    }



}