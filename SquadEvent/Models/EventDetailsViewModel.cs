using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;

namespace SquadEvent.Models
{
    public class EventDetailsViewModel
    {
        public Match Match { get; internal set; }
        public User User { get; internal set; }
        public MatchUser MatchUser { get; internal set; }
    }
}
