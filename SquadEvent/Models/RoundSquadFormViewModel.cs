using Microsoft.AspNetCore.Mvc.Rendering;
using SquadEvent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Models
{
    public class RoundSquadFormViewModel
    {
        public RoundSquad Squad { get; set; }
        public List<SelectListItem> MatchUserDropdownList { get; internal set; }
    }
}
