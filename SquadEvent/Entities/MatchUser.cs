using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class MatchUser
    {
        public int MatchUserID { get; set; }

        public int MatchID { get; set; }

        public Match Match { get; set; }

        public int? MatchSideID { get; set; }

        public MatchSide Side { get; set; }

        public List<RoundSlot> Slots { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }
    }
}
