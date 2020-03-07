using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class GameLayout
    {
        public int GameLayoutID { get; set; }

        public string Name { get; set; }

        public Faction? Left { get; set; }

        public Faction? Right { get; set; }

        public string Image { get; set; }

        public string Thumbnail { get; set; }

        public string MapFull { get; set; }

        public int GameMapID { get; set; }

        public GameMap GameMap { get; set; }
    }
}
