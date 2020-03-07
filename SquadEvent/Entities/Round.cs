using System.Collections.Generic;

namespace SquadEvent.Entities
{
    public class Round
    {
        public int RoundID { get; set; }

        public int Number { get; set; }

        public int MatchID { get; set; }

        public Match Match { get; set; }

        public List<RoundSide> Sides { get; set; }

        public int? GameLayoutID { get; set; }
        public GameLayout GameMap { get; set; }
    }
}