namespace BoardGameFrontend.Models
{
    public enum TurnTypes
    {
        PHASE_BY_PHASE,
        FULL_TURN
    }

    public class EndOfTurnEventData{
        public required int TurnCount {get; set;}
    }
}