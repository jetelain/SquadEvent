using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SquadEvent.SquadGameInfos
{
    public enum Kit
    {
        [Display(Name = "Automatic Rifleman")]
        AutomaticRifleman,
        [Display(Name = "Combat Engineer")]
        CombatEngineer,
        [Display(Name = "Crewman")]
        Crewman,
        [Display(Name = "Grenadier")]
        Grenadier,
        [Display(Name = "Heavy Anti Tank")]
        HeavyAntiTank,
        [Display(Name = "Light Anti Tank")]
        LightAntiTank,
        [Display(Name = "Machine Gunner")]
        MachineGunner,
        [Display(Name = "Marksman")]
        Marksman,
        [Display(Name = "Medic")]
        Medic,
        [Display(Name = "Raider")]
        Raider,
        [Display(Name = "Rifleman")]
        Rifleman,
        [Display(Name = "Sapper")]
        Sapper,
        //Scout,
        [Display(Name = "Squad Leader")]
        SquadLeader
    }
}
