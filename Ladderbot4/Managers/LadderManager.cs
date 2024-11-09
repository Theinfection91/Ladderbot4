using Discord.Commands;
using Discord.WebSocket;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class LadderManager
    {
        // Sub-Managers
        private TeamManager _teamManager;
        private MemberManager _memberManager;

        // Super Admin Mode
        public bool IsSuperAdminModeOn { get; set; } = false;

        public LadderManager(TeamManager teamManager, MemberManager memberManager)
        {
            _teamManager = teamManager;
            _memberManager = memberManager;
        }

        public string SetSuperAdminMode(string trueOrFalse)
        {
            switch (trueOrFalse.Trim().ToLower())
            {
                case "true":
                    IsSuperAdminModeOn = true;
                    return $"Super Admin Mode set to {IsSuperAdminModeOn}";

                case "false":
                    IsSuperAdminModeOn = false;
                    return $"Super Admin Mode set to {IsSuperAdminModeOn}";

                default:
                    throw new ArgumentException("Incorrent variable given.");
            }
        }

        public string RegisterTeamProcess(string teamName, string divisionType, params SocketGuildUser[] members)
        {
            // Load latest save of Teams database
            _teamManager.LoadTeamsDatabase();

            // Compares given Team Name against database
            if (_teamManager.IsTeamNameUnique(teamName))
            {
                // Checks if a correct division type is given
                if (_teamManager.IsValidDivisionType(divisionType))
                {
                    // Convert socket user info into Member objects
                    List<Member> newMemberList = _memberManager.ConvertMembersListToObjects(members);

                    if (_memberManager.IsMemberCountCorrect(newMemberList, divisionType) || IsSuperAdminModeOn)
                    {
                        // Grab all teams from correct division to compare with
                        List<Team> divisionTeams = _teamManager.GetTeamsByDivision(divisionType);

                        foreach (Member member in newMemberList)
                        {
                            if (!IsSuperAdminModeOn && _memberManager.IsMemberOnTeamInDivision(member, divisionTeams))
                            {
                                return $"{member.DisplayName} is already on a team in the {divisionType}.";
                            }
                        }

                        // All members are eligible, all conditions passed, add the new team to the database.
                        Team newTeam = _teamManager.CreateTeamObject(teamName, divisionType, _teamManager.GetTeamCount(divisionType) + 1, newMemberList);
                        _teamManager.AddNewTeam(newTeam);
                        return $"Team {newTeam.TeamName} has been created in the {divisionType} division with the following member(s): {newTeam.GetAllMemberNamesToStr()}";
                    }
                    else
                    {
                        return $"Incorrect amount of members given for specified division type: Division - {divisionType} | Member Count - {newMemberList.Count}";
                    }
                }
                else
                {
                    return $"Incorrect division type given: {divisionType}";
                }
            }

            return $"The given team name is already being used by another team: {teamName}\nPlease try again.";
        }
    }
}
