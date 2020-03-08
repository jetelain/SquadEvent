using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;

namespace SquadEvent.Models
{
    public class UserRoundSlotViewModel
    {
        public int? RoundSlotID { get; set; }

        public Round Round { get; set; }
    }
}
