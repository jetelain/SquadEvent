using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class RoundSquad
    {
        public int RoundSquadID { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public int RoundSideID { get; set; }
        public RoundSide Side { get; set; }

        [NotMapped]
        public MatchUser Leader 
        { 
            get { return Slots?.FirstOrDefault(s => s.Role == FireTeamRole.TeamLeader)?.AssignedUser; } 
        }

        public bool RestrictTeamComposition { get; set; }

        public List<RoundSlot> Slots { get; set; }
    }
}
