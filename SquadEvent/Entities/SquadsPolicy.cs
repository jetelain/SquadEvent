using System.ComponentModel.DataAnnotations;

namespace SquadEvent.Entities
{
    public enum SquadsPolicy
    {
        [Display(Name = "Aucune restriction")]
        Unrestricted,

        [Display(Name = "Définition des squad par les organisateurs")]
        SquadsRestricted,

        [Display(Name = "Définition des squad et de leur composition par les organisateurs")]
        SquadsAndSlotsRestricted
    }
}