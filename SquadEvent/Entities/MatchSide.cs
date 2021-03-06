﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.Entities
{
    public class MatchSide
    {
        public int MatchSideID { get; set; }

        public int MatchID { get; set; }

        public Match Match { get; set; }

        public string Name { get; set; }

        public List<RoundSide> Rounds { get; set; }

        public List<MatchUser> Users { get; set; }
        public int Number { get; set; }

        [Display(Name="Nombre maximum d'inscrits")]
        public int MaxUsersCount { get; set; }

        public SquadsPolicy SquadsPolicy { get; set; }
    }
}
