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
        private readonly MembersListData _membersListData;

        private MembersList _membersList;

        public MemberManager(MembersListData membersListData)
        {
            _membersListData = membersListData;
            _membersList = _membersListData.Load();
        }

        public void SaveMembersList()
        {
            _membersListData.Save(_membersList);
        }

        public void LoadMembersList()
        {
            _membersList = _membersListData.Load();
        }

        public void SaveAndReloadMembersList()
        {
            SaveMembersList();
            LoadMembersList();
        }
        
        public void AddToMemberProfileWins(MemberProfile member, int amount)
        {
            member.Wins += amount;
        }

        public void AddToMemberProfileLosses(MemberProfile member, int amount)
        {
            member.Losses += amount;
        }

        public void AddToMemberProfileChampionships(MemberProfile member, int amount)
        {
            member.LeagueChampionships += amount;
        }

        public void AddToMemberProfileMatchCount(MemberProfile member, int amount)
        {
            member.TotalMatchCount += amount;
        }

        public void AddToMemberProfileTeamCount(MemberProfile member, int amount)
        {
            member.TotalTeamCount += amount;
        }

        public MemberProfile? GetMemberProfileFromDiscordId(ulong discordId)
        {
            foreach (var member in _membersList.Members)
            {
                if (member.DiscordId.Equals(discordId))
                {
                    return member;
                }
            }
            return null;
        }

        public List<MemberProfile> GetAllMemberProfiles()
        {
            return _membersList.Members;
        }

        public void RegisterNewMemberProfile(ulong discordId, string displayName)
        {
            MemberProfile memberProfile = CreateMemberProfile(discordId, displayName);
            AddNewMemberProfile(memberProfile);
            SaveAndReloadMembersList();
        }

        /// <summary>
        /// Handles the adding of 1 to TotalMatchCount, and either Wins or Losses for each MemberProfile's in a given team.
        /// </summary>
        /// <param name="team">The team to iterate through and increment each members stats</param>
        /// <param name="isWinner">Determines if wins or losses are added to stats</param>
        public void HandleMemberProfileWinLossMatchProcess(Team team, bool isWinner)
        {
            foreach (Member member in team.Members)
            {
                MemberProfile? memberProfile = GetMemberProfileFromDiscordId(member.DiscordId);
                if (memberProfile != null)
                {
                    if (isWinner)
                    {
                        AddToMemberProfileWins(memberProfile, 1);                       
                    }
                    else
                    {
                        AddToMemberProfileLosses(memberProfile, 1);
                    }
                    AddToMemberProfileMatchCount(memberProfile, 1);
                }
            }
            SaveAndReloadMembersList();
        }

        public void HandleMemberProfileRegisterProcess(Team team)
        {
            foreach (Member member in team.Members)
            {
                if (!IsMemberProfileRegistered(member.DiscordId))
                {
                    RegisterNewMemberProfile(member.DiscordId, member.DisplayName);
                }
            }
        }

        public void HandleMemberProfileTeamCountProcess(Team team)
        {
            foreach (Member member in team.Members)
            {
                MemberProfile? memberProfile = GetMemberProfileFromDiscordId(member.DiscordId);
                if (memberProfile != null)
                {
                    AddToMemberProfileTeamCount(memberProfile, 1);                    
                }
            }
            SaveAndReloadMembersList();
        }

        public void HandleMemberProfileLeagueChampionProcess(Team team)
        {
            foreach (Member member in team.Members)
            {
                MemberProfile? memberProfile = GetMemberProfileFromDiscordId(member.DiscordId);
                if (memberProfile != null)
                {
                    AddToMemberProfileChampionships(memberProfile, 1);
                }
            }
            SaveAndReloadMembersList();
        }

        public bool IsMemberProfileRegistered(ulong discordId)
        {
            foreach (MemberProfile memberProfile in _membersList.Members)
            {
                if (memberProfile.DiscordId.Equals(discordId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMemberCountCorrect(List<Member> members, int teamSize)
        {
            // Case 1: For team sizes of 20 or less, the member count must match the team size.
            if (teamSize <= 20 && members.Count == teamSize)
                return true;

            // Case 2: For team sizes 21 or greater, exactly 20 members must be provided.
            if (teamSize >= 21 && members.Count == 20)
                return true;
                
            // Any other case is invalid.
            return false;
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

        public void ValidateMembersListData(List<Member> members)
        {
            foreach (Member member in members)
            {
                if (!IsMemberProfileRegistered(member.DiscordId))
                {
                    AddNewMemberProfile(CreateMemberProfile(member.DiscordId, member.DisplayName));

                    Console.WriteLine($"{DateTime.Now} MemberManager - A Discord ID was found registered in a league but not registered to the MembersList. Creating new MemberProfile - Name: {member.DisplayName} - Discord ID: {member.DiscordId}");
                }
            }
        }

        public void AddNewMemberProfile(MemberProfile memberProfile)
        {
            _membersListData.AddMemberProfile(memberProfile);
            LoadMembersList();
        }

        public MemberProfile CreateMemberProfile(ulong discordId, string displayName)
        {
            return new MemberProfile(discordId, displayName);
        }

        public Member CreateMemberObject(ulong discordId, string displayName)
        {
            return new Member(discordId, displayName);
        }
    }
}
