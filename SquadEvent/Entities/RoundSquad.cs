using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class RoundSquad
    {
        public int RoundSquadID { get; set; }

        public int Number { get; set; }

        [Display(Name = "Libellé")]
        public string Name { get; set; }

        public int RoundSideID { get; set; }

        [Display(Name = "Armée")]
        public RoundSide Side { get; set; }

        [NotMapped]
        public MatchUser Leader 
        { 
            get { return Slots?.FirstOrDefault(s => s.Role == FireTeamRole.SquadLeader)?.AssignedUser; } 
        }

        [Display(Name = "Permettre uniquement les emplacements prédéfinits")]
        public bool RestrictTeamComposition { get; set; }

        [Display(Name = "Accès sur invitation uniquement")]
        public bool InviteOnly { get; set; }

        public List<RoundSlot> Slots { get; set; }

        public int SlotsCount { get; set; }
    }


}
