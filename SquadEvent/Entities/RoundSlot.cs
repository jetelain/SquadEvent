using SquadEvent.SquadGameInfos;

namespace SquadEvent.Entities
{
    public class RoundSlot
    {
        public int RoundSlotID { get; set; }

        public int SlotNumber { get; set; }

        public string Label { get; set; }

        public FireTeamRole? Role { get; set; }

        public int? MatchUserID { get; set; }

        public MatchUser AssignedUser { get; set; }

        public Kit? AssignedKit { get; set; }
    }
}