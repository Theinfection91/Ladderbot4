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
        private readonly TeamManager _teamManager;
        private readonly MemberManager _memberManager;
        private readonly ChallengeManager _challengeManager;
        private readonly SettingsManager _settingsManager;

        // Super Admin Mode
        public bool IsSuperAdminModeOn { get; set; } = false;

        public LadderManager(TeamManager teamManager, MemberManager memberManager, ChallengeManager challengeManager, SettingsManager settingsManager)
        {
            _teamManager = teamManager;
            _memberManager = memberManager;
            _challengeManager = challengeManager;
            _settingsManager = settingsManager;
        }

        #region Super Admin/High Level Testing Logic
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
        #endregion

        #region Start/End Ladder
        public string StartLadderByDivision(string division)
        {
            return "";
        }

        public string EndLadderByDivision(string division)
        {
            return "";
        }

        #endregion

        #region Team/Member Logic
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
                    // Convert User Context Info into Member objects
                    List<Member> newMemberList = _memberManager.ConvertMembersListToObjects(members);

                    if (_memberManager.IsMemberCountCorrect(newMemberList, divisionType) || IsSuperAdminModeOn)
                    {
                        // Grab all teams from correct division to compare with
                        List<Team> divisionTeams = _teamManager.GetTeamsByDivision(divisionType);

                        foreach (Member member in newMemberList)
                        {
                            if (!IsSuperAdminModeOn && _memberManager.IsMemberOnTeamInDivision(member, divisionTeams))
                            {
                                return $"{member.DisplayName} is already on a team in the {divisionType} division.\nPlease try again.";
                            }
                        }

                        // All members are eligible, all conditions passed, add the new team to the database.
                        Team newTeam = _teamManager.CreateTeamObject(teamName, divisionType, _teamManager.GetTeamCount(divisionType) + 1, newMemberList);
                        _teamManager.AddNewTeam(newTeam);
                        return $"Team {newTeam.TeamName} has been created in the {divisionType} division with the following member(s): {newTeam.GetAllMemberNamesToStr()}";
                    }
                    else
                    {
                        return $"Incorrect amount of members given for specified division type: Division - {divisionType} | Member Count - {newMemberList.Count}\nPlease try again.";
                    }
                }
                else
                {
                    return $"Incorrect division type given: {divisionType}\nPlease try again.";
                }
            }

            return $"The given team name is already being used by another team: {teamName}\nPlease try again.";
        }
        #endregion

        #region Challenge Logic
        public string ChallengeProcess(SocketCommandContext context, string challengerTeam, string challengedTeam)
        {
            // Need to check if both teams actually exist in entire Teams database
            if (!_teamManager.IsTeamNameUnique(challengerTeam) && !_teamManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab Team object references
                Team objectChallengerTeam = _teamManager.GetTeamByName(challengerTeam);
                Team objectChallengedTeam = _teamManager.GetTeamByName(challengedTeam);
                
                // Grab Discord Id of user who invoked this command
                ulong discordId = context.User.Id;

                // Check if user who invoked command is actually on challenger team
                if (_memberManager.IsDiscordIdOnGivenTeam(discordId, objectChallengerTeam))
                {

                    // If both are real and user is on challengerTeam then compare the two teams division types
                    if (_teamManager.IsTeamsInSameDivision(objectChallengerTeam, objectChallengedTeam))
                    {

                        // If in the same division then check ranks to make sure challenger isnt above and also isnt more than 2 below challenged in rank
                        if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                        {

                            // If ranks are in correct range then make sure challenger hasnt issued another challenge to be resolved first
                            if (!_challengeManager.IsTeamAwaitingChallengeMatch(objectChallengerTeam))
                            {

                                // If challenger has no open challenges, check if challengedTeam is currently under a challenge
                                if (!_challengeManager.IsTeamAwaitingChallengeMatch(objectChallengedTeam))
                                {
                                    // If all checks are passed, create and save the new Challenge object, save Challenges database
                                    _challengeManager.AddNewChallenge(new Challenge(objectChallengerTeam.Division, objectChallengerTeam.TeamName, objectChallengedTeam.TeamName));
                                    return $"{objectChallengerTeam.TeamName}(#{objectChallengerTeam.Rank}) has challenged {objectChallengedTeam.TeamName}(#{objectChallengedTeam.Rank}) in the {objectChallengerTeam.Division} division! ";
                                }
                                else
                                {
                                    return $"Team {objectChallengedTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated. Please try again.";
                                }
                            }
                            else
                            {
                                return $"Team {objectChallengerTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated. Please try again.";
                            }
                        }
                        else
                        {
                            if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                            {
                                return $"{objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is greater than {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank}, the challenge was not initiated. Please try again.";
                            }
                            else
                            {
                                return $"{objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is not in range of {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank} to make a challenge, the challenge was not initiated. Teams may only challenge at most TWO ranks above them. Please try again.";
                            }
                        }
                    }
                    else
                    {
                        return $"Error - The given teams are not in the same division. Challenger Team Division: {objectChallengerTeam.Division} - Challenged Team Division: {objectChallengedTeam.Division} - Please try again.";
                    }
                }
                else
                {
                    return $"You are not part of Team {objectChallengerTeam.TeamName}. Please try again.";
                }
            }

            return "One or both team names not found in the database. Please try again.";
        }

        #endregion
    }
}
