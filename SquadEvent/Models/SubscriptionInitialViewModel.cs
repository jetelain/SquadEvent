using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;

namespace SquadEvent.Models
{
    public class SubscriptionInitialViewModel
    {
        public int? MatchSideID { get; set; }
        public int? RoundSquadID { get; set; }
        public User User { get; set; }
        public Match Match { get; set; }

        [Display(Name = "J'accepte le traitement des données nécessaires à mon inscription")]
        [Required]
        public bool AcceptSubscription { get; set; }

        [Display(Name ="J'ai lu et j'accepte le règlement de l'événement")]
        [Required]
        public bool AcceptMatchRules { get; set; }
    }
}
