using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.SquadGameInfos
{
    public enum Faction
    {
        [Display(Name = "US Army")]
        US,
        [Display(Name = "British Forces")]
        UK,
        [Display(Name = "Russian Ground Forces")]
        RU,
        [Display(Name = "Insurgents")]
        Ins,
        [Display(Name = "Irregular Militia")]
        Irreg, 
        [Display(Name = "Canadian Armed Forces")]
        CA
    }
}
