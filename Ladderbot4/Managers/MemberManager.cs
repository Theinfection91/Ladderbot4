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
        private readonly MemberData _memberData;

        private MembersList _membersList;

        public MemberManager(MemberData memberData)
        {
            _memberData = memberData;
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveMembersList()
        {
            _memberData.SaveAllMembers(_membersList);
        }

        public void LoadMembersList()
        {
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveAndReloadMembersList()
        {
            SaveMembersList();
            LoadMembersList();
        }

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

        public bool IsMemberRegisteredToDatabase(Member memberToCheck)
        {
            foreach (Member member in _membersList.AllMembers)
            {
                if (member.Equals(memberToCheck))
                {
                    return true;
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

        public void AddToMemberWins(Member member, string division, int numberOfWins)
        {
            // Find the corresponding member in the list
            var targetMember = _membersList.AllMembers.FirstOrDefault(m => m.DiscordId == member.DiscordId);

            if (targetMember != null)
            {
                switch (division)
                {
                    case "1v1":
                        targetMember.Wins1v1 += numberOfWins;
                        break;

                    case "2v2":
                        targetMember.Wins2v2 += numberOfWins;
                        break;

                    case "3v3":
                        targetMember.Wins3v3 += numberOfWins;
                        break;
                }
                SaveAndReloadMembersList();
            }
        }

        public void AddToMemberLosses(Member member, string division, int numberOfLosses)
        {
            // Find the corresponding member in the list
            var targetMember = _membersList.AllMembers.FirstOrDefault(m => m.DiscordId == member.DiscordId);

            if (targetMember != null)
            {
                switch (division)
                {
                    case "1v1":
                        targetMember.Losses1v1 += numberOfLosses;
                        break;

                    case "2v2":
                        targetMember.Losses2v2 += numberOfLosses;
                        break;

                    case "3v3":
                        targetMember.Losses3v3 += numberOfLosses;
                        break;
                }
                SaveAndReloadMembersList();
            }
        }

        public void AddToDivisionTeamCount(Member member, string division)
        {
            // Find the corresponding member in the list
            var targetMember = _membersList.AllMembers.FirstOrDefault(m => m.DiscordId == member.DiscordId);

            if (targetMember != null)
            {
                switch (division)
                {
                    case "1v1":
                        targetMember.TeamCount1v1 += 1;
                        break;

                    case "2v2":
                        targetMember.TeamCount2v2 += 1;
                        break;

                    case "3v3":
                        targetMember.TeamCount3v3 += 1;
                        break;
                }
                targetMember.UpdateTotalTeamCount();
                SaveAndReloadMembersList();
            }
        }

        public void AddNewMember(Member member)
        {
            _memberData.AddMember(member);
            LoadMembersList();
        }
    }
}
