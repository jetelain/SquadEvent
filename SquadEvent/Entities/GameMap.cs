using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class GameMap
    {
        public int GameMapID { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public GameMapRegion Region { get; set; }
    }
}
