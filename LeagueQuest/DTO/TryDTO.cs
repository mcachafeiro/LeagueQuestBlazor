using LeagueQuest.Models;

namespace LeagueQuest.DTO
{
    public class TryDTO
    {
        public Player Player { get; set; }
        public ClueDTO Team { get; set; }
        public ClueDTO Country { get; set; }
        public ClueDTO Number { get; set; }
        public ClueDTO Age { get; set; }
        public ClueDTO Position { get; set; }
        public bool IsPlayer { get; set; }
    }

    public class ClueDTO
    {
        public bool Guessed { get; set; }
        public NumberClue? NumberClue { get; set; }
        public PositionClue? PositionClue { get; set; }
    }

    public enum NumberClue
    {
        Lower,
        Higher
    }

    public enum PositionClue
    {
        Wrong,
        Medium
    }
}
