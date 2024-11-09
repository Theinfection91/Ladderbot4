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
        private TeamManager _teamManager;
        private MemberManager _memberManager;

        public LadderManager(TeamManager teamManager, MemberManager memberManager)
        {
            _teamManager = teamManager;
            _memberManager = memberManager;
        }

        public string RegisterTeamProcess(string teamName, string divisionType, params SocketGuildUser[] members)
        {
            // Compares given Team Name against database
            if (_teamManager.IsTeamNameUnique(teamName))
            {
                // Checks if a correct division type is given
                if (_teamManager.IsValidDivisionType(divisionType))
                {
                    // Convert socket user info into Member objects
                    List<Member> newMemberList = _memberManager.ConvertMembersListToObjects(members);

                    if (_memberManager.IsMemberCountCorrect(newMemberList, divisionType))
                    {
                        Team newTeam = _teamManager.CreateTeamObject(teamName, divisionType, _teamManager.GetTeamCount(divisionType) + 1, newMemberList);
                        _teamManager.AddNewTeam(newTeam);
                        return $"Team {newTeam.TeamName} has been created in the {divisionType} division with the following member(s): {newTeam.GetAllMemberNames()}";
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
