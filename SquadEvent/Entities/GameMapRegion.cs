using System.ComponentModel.DataAnnotations;

namespace SquadEvent.Entities
{
    public enum GameMapRegion
    {
        [Display(Name = "Europe du Nord")]
        NorthernEurope,
        [Display(Name = "Europe de l'Est")]
        EasternEurope,
        [Display(Name = "Asie du Sud")]
        SouthernAsia,
        [Display(Name = "Moyen orient")]
        MiddleEast,
        [Display(Name = "Amérique du Nord")]
        NorthAmerica,
        [Display(Name = "Entrainement")]
        Training
    }
}