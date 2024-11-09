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

        public Member(ulong discordId, string displayName)
        {
            DiscordId = discordId;
            DisplayName = displayName;
        }
    }
}
