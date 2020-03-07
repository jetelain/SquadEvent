using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class RoundSide
    {
        public int RoundSideID { get; set; }

        public List<RoundSquad> Squads { get; set; }

        public GameSide GameSide { get; set; }

        public int RoundID { get; set; }
        public Round Round { get; set; }

        public int MatchSideID { get; set; }

        public MatchSide MatchSide { get; set; }

        public Faction? Faction { get; set; }
    }
}
