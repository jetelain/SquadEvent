using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class MatchUser
    {
        public int MatchUserID { get; set; }

        [Display(Name = "Evénement")]
        public int MatchID { get; set; }

        [Display(Name = "Evénement")]
        public Match Match { get; set; }

        [Display(Name = "Coté")]
        public int? MatchSideID { get; set; }

        [Display(Name="Coté")]
        public MatchSide Side { get; set; }

        public List<RoundSlot> Slots { get; set; }

        [Display(Name = "Inscrit")]
        public int UserID { get; set; }

        [Display(Name = "Inscrit")]
        public User User { get; set; }
    }
}
