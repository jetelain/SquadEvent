using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.SquadGameInfos
{
    public enum FireTeamRole
    {
        [Display(Name = "SL")]
        SquadLeader,
        [Display(Name = "A")]
        Alpha,
        [Display(Name = "B - FTL")]
        BravoLeader,
        [Display(Name = "B")]
        Bravo,
        [Display(Name = "C - FTL")]
        CharlieLeader,
        [Display(Name = "C")]
        Charlie
    }
}
