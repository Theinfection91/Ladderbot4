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
            return divisionType switch
            {
                "1v1" => membersList.Count == 1,
                "2v2" => membersList.Count == 2,
                "3v3" => membersList.Count == 3,
                _ => false,
            };
        }

        public bool IsMemberOnTeamInDivision(Member member, List<Team> divisionTeams)
        {
            foreach (Team team in divisionTeams)
            {
                foreach (Member teamMember in team.Members)
                {
                    if (teamMember.Equals(member))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsDiscordIdOnGivenTeam(ulong discordId, Team team)
        {
            foreach (Member member in team.Members)
            {
                if (member.DiscordId == discordId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
