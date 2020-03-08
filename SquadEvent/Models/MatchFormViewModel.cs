using Microsoft.AspNetCore.Mvc.Rendering;
using SquadEvent.Entities;
using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Models
{
    public class MatchFormViewModel
    {
        public Match Match { get; set; }
        public List<SelectListItem> MapsDropdownList { get; internal set; }
        public List<GameLayout> MapsData { get; internal set; }
    }
}
