using Discord;
using Discord.Commands;
using Discord.Interactions;
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

        #region Start/End Ladder Logic
        public string StartLadderByDivision(string division)
        {
            return "";
        }

        public string EndLadderByDivision(string division)
        {
            return "";
        }

        #endregion

        #region Team/Member Management Logic
        public string RegisterTeamProcess(string teamName, string divisionType, List<IUser> members)
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
                                return $"```{member.DisplayName} is already on a team in the {divisionType} division. Please try again.```";
                            }
                        }

                        // All members are eligible, all conditions passed, add the new team to the database.
                        Team newTeam = _teamManager.CreateTeamObject(teamName, divisionType, _teamManager.GetTeamCount(divisionType) + 1, newMemberList);
                        _teamManager.AddNewTeam(newTeam);
                        return $"```Team {newTeam.TeamName} has been created in the {divisionType} division with the following member(s): {newTeam.GetAllMemberNamesToStr()}```";
                    }
                    else
                    {
                        return $"```Incorrect amount of members given for specified division type: Division - {divisionType} | Member Count - {newMemberList.Count} - Please try again.```";
                    }
                }
                else
                {
                    return $"```Incorrect division type given: {divisionType} - Please try again.```";
                }
            }

            return $"```The given team name is already being used by another team: {teamName} - Please try again.```";
        }
        #endregion

        #region Challenge Based Logic
        /// <summary>
        /// User-level logic used to handle the process of one team challenging another to a match. This logic compares the invoker's Discord Id to the challenger team to make sure they are apart of it. To bypass this, an admin can use the admin_challenge command or enable Super Admin Mode (Check the documentation).
        /// </summary>
        /// <param name="context">Used to grab the invoker's Discord Id</param>
        /// <param name="challengerTeam">The name of the team initiating the challenge.</param>
        /// <param name="challengedTeam">The name of the team who is receiving the challenge.</param>
        /// <returns>String used by bot for error handling or to confirm the challenge was created.</returns>
        public string ChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Need to check if both teams actually exist in entire Teams database
            if (!_teamManager.IsTeamNameUnique(challengerTeam) && !_teamManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab Team object references
                Team objectChallengerTeam = _teamManager.GetTeamByName(challengerTeam);
                Team objectChallengedTeam = _teamManager.GetTeamByName(challengedTeam);
                
                // Grab Discord Id of user who invoked this command
                ulong discordId = context.User.Id;

                Console.WriteLine(discordId);

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
                                    return $"```{objectChallengerTeam.TeamName}(#{objectChallengerTeam.Rank}) has challenged {objectChallengedTeam.TeamName}(#{objectChallengedTeam.Rank}) in the {objectChallengerTeam.Division} division!```";
                                }
                                else
                                {
                                    return $"```Team {objectChallengedTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated. Please try again.```";
                                }
                            }
                            else
                            {
                                return $"```Team {objectChallengerTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated. Please try again.```";
                            }
                        }
                        else
                        {
                            if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                            {
                                return $"```{objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is greater than {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank}, the challenge was not initiated. Please try again.```";
                            }
                            else
                            {
                                return $"```{objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is not in range of {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank} to make a challenge, the challenge was not initiated. Teams may only challenge at most TWO ranks above them. Please try again.```";
                            }
                        }
                    }
                    else
                    {
                        return $"```Error - The given teams are not in the same division. Challenger Team Division: {objectChallengerTeam.Division} - Challenged Team Division: {objectChallengedTeam.Division} - Please try again.```";
                    }
                }
                else
                {
                    return $"```You are not part of Team {objectChallengerTeam.TeamName}. Please try again.```";
                }
            }

            return "```One or both team names not found in the database. Please try again.```";
        }

        public string CancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Check if given team exists
            if (!_teamManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team reference object
                Team challengerTeamObject = _teamManager.GetTeamByName(challengerTeam);

                // Check if invoker is part of challengerTeam, as this is the user-level logic
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, challengerTeamObject))
                {
                    // Check if Team has a challenge sent out to actually cancel
                    if (_challengeManager.IsTeamChallenger(challengerTeamObject))
                    {
                        // Cancel the challenge, save challenges database and reload it
                        _challengeManager.RemoveChallenge(challengerTeamObject.TeamName, challengerTeamObject.Division);
                        return $"```{challengerTeamObject.TeamName} has canceled the challenge they have sent out in the {challengerTeamObject.Division}```";
                    }
                    else
                    {
                        return $"```Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.```";
                    }
                }
                else
                {
                    return $"```You are not a member of that team... Team {challengerTeamObject.TeamName}'s members are currently: {challengerTeamObject.GetAllMemberNamesToStr()}```";
                }
            }
            return $"```No team found by the name of: {challengerTeam} - Please try again.```";
        }

        public string AdminChallengeProcess(string challengerTeam, string challengedTeam)
        {
            return "admin challenge";
        }

        public string AdminCancelChallengeProcess(string challengerTeam)
        {
            return "admin cancel challenge";
        }

        #endregion

        #region Settings Logic

        public string SetGuildId(SocketCommandContext context)
        {
            // Grab Guild Id command was invoked from
            ulong guildId = context.Guild.Id;

            // Set in Settings using SettingsManager then save and reload Settings
            _settingsManager.Settings.GuildId = guildId;
            _settingsManager.SaveSettings(_settingsManager.Settings);
            _settingsManager.LoadSettingsData();

            return $"```Set GuildId in config.json to {guildId} - If this is the first time setting the GuildId for Slash Commands, then please restart the bot now.```";
        }

        public string SetSuperAdminMode(string trueOrFalse)
        {
            switch (trueOrFalse.Trim().ToLower())
            {
                case "true":
                    IsSuperAdminModeOn = true;
                    return $"```Super Admin Mode set to {IsSuperAdminModeOn}```";

                case "false":
                    IsSuperAdminModeOn = false;
                    return $"```Super Admin Mode set to {IsSuperAdminModeOn}```";

                default:
                    throw new ArgumentException("Incorrent variable given.");
            }
        }

        public string AddSuperAdminId(IUser user)
        {
            return "";
        }

        public string RemoveSuperAdminId(IUser user)
        {
            return "";
        }
        #endregion
    }
}
