using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SquadEvent.Entities;

namespace SquadEvent.Models
{
    public class MatchUserEditViewModel
    {
        public MatchUser MatchUser { get; set; }
        public SelectList MatchSideDropdownList { get; internal set; }
        public List<UserRoundSlotViewModel> SlotPerRound { get; set; }
    }
}
