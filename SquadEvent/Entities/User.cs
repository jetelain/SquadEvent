using System.ComponentModel.DataAnnotations;

namespace SquadEvent.Entities
{
    public class User
    {
        public int UserID { get; set; }

        [StringLength(100)]
        [Display(Name = "Pseudonyme")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Identifiant Steam")]
        public string SteamId { get; set; }

        [StringLength(100)]
        [Display(Name = "Nom dans steam")]
        public string SteamName { get; set; }

        [StringLength(10)]
        [Display(Name = "Préfixe en jeu")]
        public string NamePrefix { get; set; }
    }
}