using Discord.WebSocket;
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

        public bool IsMemberAmountCorrect(List<Member> members, string divisionType)
        {
            return false;
        }

        public Member CreateMemberObject(ulong discordId, string displayName)
        {
            return new Member(discordId, displayName);
        }

        public List<Member> ConvertMembersListToObjects(params SocketGuildUser[] members)
        {
            List<Member> membersList = [];
            foreach (var member in members)
            {
                membersList.Add(new Member(member.Id, member.DisplayName));
            }
            return membersList;
        }

        public bool IsMemberCountCorrect(List<Member> membersList, string divisionType)
        {
            switch (divisionType)
            {
                case "1v1":
                    return membersList.Count == 1;

                case "2v2":
                    return membersList.Count == 2;

                case "3v3":
                    return membersList.Count == 3;

                default:
                    return false;
            }
        }
    }
}
