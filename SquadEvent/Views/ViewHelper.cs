using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;
using SquadEvent.SquadGameInfos;

namespace SquadEvent
{
    public static class ViewHelper
    {
        public static string Icon(Kit? kit)
        {
            if (kit != null)
            {
                return $"/img/kits/{kit}.png";
            }
            return "";
        }
        public static string Icon(Faction? faction)
        {
            if (faction != null)
            {
                return $"/img/factions/{faction}.png";
            }
            return "/img/factions/none.png";
        }
        public static string Icon(FireTeamRole? role)
        {
            if (role != null)
            {
                return $"/img/roles/{role}.png";
            }
            return "/img/roles/Alpha.png";
        }
        public static string Style(GameLayout layout)
        {
            if (layout != null)
            {
                if (!string.IsNullOrEmpty(layout.Image))
                {
                    return $"background-image: url({layout.Image});";
                }
                if (layout.GameMap != null && !string.IsNullOrEmpty(layout.GameMap.Image))
                {
                    return $"background-image: url({layout.GameMap.Image});";
                }
            }
            return "";
        }

    }
}
