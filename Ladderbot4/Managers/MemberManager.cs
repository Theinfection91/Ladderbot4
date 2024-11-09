using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class MemberManager
    {
        public MemberManager()
        {

        }

        public Member CreateMemberObject(ulong discordId, string displayName)
        {
            return new Member(discordId, displayName);
        }
    }
}
