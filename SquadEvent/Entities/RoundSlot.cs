﻿using SquadEvent.SquadGameInfos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadEvent.Entities
{
    public class RoundSlot
    {
        public int RoundSlotID { get; set; }

        public int SlotNumber { get; set; }

        public int RoundSquadID { get; set; }

        public RoundSquad Squad { get; set; }

        [Display(Name = "Libellé")]
        public string Label { get; set; }

        [Display(Name="Fire team")]
        public FireTeamRole? Role { get; set; }

        public int? MatchUserID { get; set; }

        public MatchUser AssignedUser { get; set; }

        [Display(Name = "Kit")]
        public Kit? AssignedKit { get; set; }

        public long? Timestamp { get; set; }

        public void SetTimestamp()
        {
            Timestamp = DateTime.UtcNow.Ticks;
        }
    }
}