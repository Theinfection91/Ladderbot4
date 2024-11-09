using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Member
    {
        public ulong DiscordId { get; set; }
        public string DisplayName { get; set; }
        public List<Team> CurrentTeams { get; set; }

        public Member(ulong discordId, string displayName)
        {
            DiscordId = discordId;
            DisplayName = displayName;
            CurrentTeams = []; // TODO: For keeping track of all teams a member is on
        }
    }
}
