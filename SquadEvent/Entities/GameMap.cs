using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class GameMap
    {
        public int GameMapID { get; set; }

        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Image de fond")]
        public string Image { get; set; }

        [Display(Name = "Région")]
        public GameMapRegion Region { get; set; }
    }
}
