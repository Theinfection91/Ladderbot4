using Discord;
using Discord.WebSocket;
using Ladderbot4.Data;
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
        public MemberManager() { }

        public Member CreateMemberObject(ulong discordId, string displayName)
        {
            return new Member(discordId, displayName);
        }

        public List<Member> ConvertMembersListToObjects(List<IUser> members)
        {
            List<Member> membersList = new List<Member>();

            foreach (IUser member in members)
            {
                string displayName;

                // Check if the member can be cast to SocketGuildUser
                if (member is SocketGuildUser guildUser)
                {
                    // If the user has a nickname (DisplayName), use it
                    displayName = !string.IsNullOrEmpty(guildUser.DisplayName) ? guildUser.DisplayName : guildUser.Username;
                }
                else
                {
                    // If it's not a guild user, use the global Username
                    displayName = member.Username;
                }

                // Create a new Member object with the Discord ID and display name
                membersList.Add(new Member(member.Id, displayName));
            }

            return membersList;
        }

        public bool IsMemberCountCorrect(List<Member> members, int teamSize)
        {
            if (members.Count.Equals(teamSize)) return true;

            else if (teamSize >= 21 && members.Count >= 20) return true;

            else return false;
        }

        public bool IsMemberOnTeamInLeague(Member member, List<Team> leagueTeams)
        {
            foreach (Team team in leagueTeams)
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
