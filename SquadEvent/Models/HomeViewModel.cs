using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;

namespace SquadEvent.Models
{
    public class HomeViewModel
    {
        public List<Match> Matchs { get; internal set; }
        public User User { get; internal set; }
    }
}
