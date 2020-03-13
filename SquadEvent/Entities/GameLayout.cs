using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class GameLayout
    {
        public int GameLayoutID { get; set; }

        [Display(Name = "Nom (tel qu'il l'apparait en jeu)")]
        public string Name { get; set; }

        [Display(Name = "Coté gauche")]
        public Faction? Left { get; set; }

        [Display(Name = "Coté droit")]
        public Faction? Right { get; set; }

        [Display(Name = "Image de fond")]
        public string Image { get; set; }

        [Display(Name = "Image de la carte - Petit format")]
        public string Thumbnail { get; set; }

        [Display(Name = "Image de la carte - Taille complète")]
        public string MapFull { get; set; }

        [Display(Name = "Map")]
        public int GameMapID { get; set; }

        [Display(Name = "Map")]
        public GameMap GameMap { get; set; }
    }
}
