using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class Match
    {
        public int MatchID { get; set; }

        [Display(Name = "Titre")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [NotMapped]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Required]
        [Display(Name = "Heure de début")]
        [NotMapped]
        public DateTime StartTime { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Description résumée")]
        public string ShortDescription { get; set; }

        public DateTime StartDate
        {
            get { return new DateTime(Date.Year, Date.Month, Date.Day, StartTime.Hour, StartTime.Minute, StartTime.Second); }
            set
            {
                Date = value.Date;
                StartTime = value;
            }
        }

        [Display(Name = "Accès sur invitation uniquement")]
        public bool InviteOnly { get; set; }

        public List<Round> Rounds { get; set; }
        public List<MatchSide> Sides { get; set; }
        public List<MatchUser> Users { get; set; }
    }
}
