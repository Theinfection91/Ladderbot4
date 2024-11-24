using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class LadderManager
    {
        #region Properties and Constructor
        // Sub-Managers
        private readonly DiscordSocketClient _client;
        private readonly GitBackupManager _backupManager;
        private readonly HistoryManager _historyManager;
        private readonly TeamManager _teamManager;
        private readonly MemberManager _memberManager;
        private readonly ChallengeManager _challengeManager;
        private readonly StatesManager _statesManager;
        private readonly SettingsManager _settingsManager;

        // Channel Tasks
        private readonly Dictionary<ulong, ulong> _challengesMessageMap = new();
        private readonly Dictionary<ulong, ulong> _standingsMessageMap = new();
        private readonly Dictionary<ulong, ulong> _teamsMessageMap = new();


        public LadderManager(DiscordSocketClient client, GitBackupManager gitBackupManager, HistoryManager historyManager, TeamManager teamManager, MemberManager memberManager, ChallengeManager challengeManager, SettingsManager settingsManager, StatesManager statesManager)
        {
            _client = client;
            _backupManager = gitBackupManager;
            _historyManager = historyManager;
            _teamManager = teamManager;
            _memberManager = memberManager;
            _challengeManager = challengeManager;
            _settingsManager = settingsManager;
            _statesManager = statesManager;

            // Begin Channel Update Tasks
            StartingChallengesTask();
            StartingStandingsTask();
            StartingTeamsTask();
        }
        #endregion

        #region TODO - Automated/Event Driven Backup Logic

        public string ManualBackupProcess()
        {
            _backupManager.CopyJsonFilesToBackupRepo();
            _backupManager.BackupFiles();
            return "Executed BackupFiles()";
        }

        #endregion

        #region TODO - Channel Tasks Logic

        #region --Challenges

        public void StartingChallengesTask()
        {
            Task.Run(() => RunChallengesUpdateTaskAsync());
        }

        private async Task RunChallengesUpdateTaskAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                await SendChallengesToChannelAsync();
            }
        }

        private async Task SendChallengesToChannelAsync()
        {
            // Map divisions to their respective channel IDs
            var divisionChannelMap = new Dictionary<string, ulong>
            {
                { "1v1", _statesManager.GetChallengesChannelId("1v1") },
                { "2v2", _statesManager.GetChallengesChannelId("2v2") },
                { "3v3", _statesManager.GetChallengesChannelId("3v3") }
            };

            foreach (var division in divisionChannelMap.Keys)
            {
                ulong channelId = divisionChannelMap[division];

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    continue;
                }

                // Get the standings for the division
                string standings = _challengeManager.GetChallengesData(division) + $"Updated: {DateTime.Now}";

                if (string.IsNullOrEmpty(standings))
                {
                    continue;
                }

                try
                {
                    // Check if a message already exists in the channel
                    if (_standingsMessageMap.TryGetValue(channelId, out ulong messageId))
                    {
                        // Try to fetch the existing message
                        var existingMessage = await channel.GetMessageAsync(messageId) as IUserMessage;

                        if (existingMessage != null)
                        {
                            // Edit the existing message
                            await existingMessage.ModifyAsync(msg => msg.Content = standings);
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(standings);
                            _standingsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(standings);
                        _standingsMessageMap[channelId] = newMessage.Id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating challenges in channel {channelId}: {ex.Message}");
                }
            }
        }
        #endregion

        #region --Standings
        public void StartingStandingsTask()
        {
            Task.Run(() => RunStandingsUpdateTaskAsync());
        }

        private async Task RunStandingsUpdateTaskAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                await SendStandingsToChannelAsync();
            }
        }

        private async Task SendStandingsToChannelAsync()
        {
            // Map divisions to their respective channel IDs
            var divisionChannelMap = new Dictionary<string, ulong>
            {
                { "1v1", _statesManager.GetStandingsChannelId("1v1") },
                { "2v2", _statesManager.GetStandingsChannelId("2v2") },
                { "3v3", _statesManager.GetStandingsChannelId("3v3") }
            };

            foreach (var division in divisionChannelMap.Keys)
            {
                ulong channelId = divisionChannelMap[division];

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    continue;
                }

                // Get the standings for the division
                string standings = _teamManager.GetStandingsData(division) + $"Updated: {DateTime.Now}";

                if (string.IsNullOrEmpty(standings))
                {
                    continue;
                }

                try
                {
                    // Check if a message already exists in the channel
                    if (_standingsMessageMap.TryGetValue(channelId, out ulong messageId))
                    {
                        // Try to fetch the existing message
                        var existingMessage = await channel.GetMessageAsync(messageId) as IUserMessage;

                        if (existingMessage != null)
                        {
                            // Edit the existing message
                            await existingMessage.ModifyAsync(msg => msg.Content = standings);
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(standings);
                            _standingsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(standings);
                        _standingsMessageMap[channelId] = newMessage.Id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating standings in channel {channelId}: {ex.Message}");
                }
            }
        }
        #endregion

        #region --Teams

        public void StartingTeamsTask()
        {
            Task.Run(() => RunTeamsUpdateTaskAsync());
        }

        private async Task RunTeamsUpdateTaskAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                await SendTeamsToChannelAsync();
            }
        }

        private async Task SendTeamsToChannelAsync()
        {
            // Map divisions to their respective channel IDs
            var divisionChannelMap = new Dictionary<string, ulong>
            {
                { "1v1", _statesManager.GetTeamsChannelId("1v1") },
                { "2v2", _statesManager.GetTeamsChannelId("2v2") },
                { "3v3", _statesManager.GetTeamsChannelId("3v3") }
            };

            foreach (var division in divisionChannelMap.Keys)
            {
                ulong channelId = divisionChannelMap[division];

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    continue;
                }

                // Get the standings for the division
                string standings = _teamManager.GetTeamsData(division) + $"Updated: {DateTime.Now}";

                if (string.IsNullOrEmpty(standings))
                {
                    continue;
                }

                try
                {
                    // Check if a message already exists in the channel
                    if (_teamsMessageMap.TryGetValue(channelId, out ulong messageId))
                    {
                        // Try to fetch the existing message
                        var existingMessage = await channel.GetMessageAsync(messageId) as IUserMessage;

                        if (existingMessage != null)
                        {
                            // Edit the existing message
                            await existingMessage.ModifyAsync(msg => msg.Content = standings);
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(standings);
                            _teamsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(standings);
                        _teamsMessageMap[channelId] = newMessage.Id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating standings in channel {channelId}: {ex.Message}");
                }
            }
        }

        #endregion

        #endregion

        #region Start/End Ladder Logic
        public string StartLadderByDivisionProcess(string division)
        {
            _statesManager.LoadStatesDatabase();

            // Check if ladder is currently running or not
            if (!_statesManager.IsLadderRunning(division))
            {
                // Set ladder running to true in division
                _statesManager.SetLadderRunning(division, true);

                // Save and reload the database
                _statesManager.SaveAndReloadStatesDatabase();

                return $"```The ladder in the {division} division has started!```";
            }
            else
            {
                return $"```The ladder in the {division} division is already running...```";
            }  
        }

        public string EndLadderByDivisionProcess(string division)
        {
            _statesManager.LoadStatesDatabase();

            // Check if ladder is currently running or not
            if (_statesManager.IsLadderRunning(division))
            {
                // Set ladder running to false in division
                _statesManager.SetLadderRunning(division, false);

                // Save and reload the database
                _statesManager.SaveAndReloadStatesDatabase();

                return $"```The ladder in the {division} division has ended!```";
            }
            else
            {
                return $"```The ladder in the {division} division hasn't started yet...```";
            }
        }
        #endregion

        #region Register/Remove Team Logic
        /// <summary>
        /// Goes through the process of trying to register a new team with given name, division type, and members.
        /// </summary>
        /// <param name="teamName">Desired team name</param>
        /// <param name="divisionType">Which division to place the team in</param>
        /// <param name="members">The members for the new team</param>
        /// <returns>String for the bot that will cover error handling and confirmation of registration.</returns>
        public string RegisterTeamProcess(SocketInteractionContext context, string teamName, string divisionType, List<IUser> members)
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

                    if (_memberManager.IsMemberCountCorrect(newMemberList, divisionType) || _settingsManager.IsUserSuperAdmin(context.User.Id))
                    {
                        // Grab all teams from correct division to compare with
                        List<Team> divisionTeams = _teamManager.GetTeamsByDivision(divisionType);

                        foreach (Member member in newMemberList)
                        {
                            if (_memberManager.IsMemberOnTeamInDivision(member, divisionTeams) && !_settingsManager.IsUserSuperAdmin(context.User.Id))
                            {
                                return $"```{member.DisplayName} is already on a team in the {divisionType} division. Please try again.```";
                            }
                        }
                        // All members are eligible, all conditions passed, add the new team to the database.
                        Team newTeam = _teamManager.CreateTeamObject(teamName, divisionType, _teamManager.GetTeamCount(divisionType) + 1, newMemberList);
                        _teamManager.AddNewTeam(newTeam);

                        // Save and reload database
                        _teamManager.SaveAndReloadTeamsDatabase();

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return $"```Team {newTeam.TeamName}(#{newTeam.Rank}) has been created in the {divisionType} division with the following member(s): {newTeam.GetAllMemberNamesToStr()}```";
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

        /// <summary>
        /// Starts the process of trying to remove a team
        /// </summary>
        /// <param name="teamName">Name of team to try and remove from database</param>
        /// <returns>String for the bot that will cover error handling and confirmation of removal.</returns>
        public string RemoveTeamProcess(string teamName)
        {
            // Load latest save of Teams database
            _teamManager.LoadTeamsDatabase();

            // Check if Team is in database
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab object reference to work with
                Team teamReference = _teamManager.GetTeamByName(teamName);

                // Remove any challenges related to team, if any
                if (_challengeManager.IsTeamAwaitingChallengeMatch(teamReference))
                {
                    _challengeManager.SudoRemoveChallenge(teamReference.TeamName, teamReference.Division);
                }

                // Remove the team correctly, with ranks falling into place programmatically
                _teamManager.RemoveTeam(teamReference.TeamName, teamReference.Division);
                ReassignRanks(teamReference.Division);

                // Save and reload database
                _teamManager.SaveAndReloadTeamsDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return $"```Team {teamReference.TeamName} removed from {teamReference.Division} division.```";

            }
            return $"```Given Team Name does not exist in the teams database: {teamName} - Please try again.```";
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
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if both teams actually exist in entire Teams database
            if (!_teamManager.IsTeamNameUnique(challengerTeam) && !_teamManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab Team object references
                Team objectChallengerTeam = _teamManager.GetTeamByName(challengerTeam);
                Team objectChallengedTeam = _teamManager.GetTeamByName(challengedTeam);

                // Check if ladder is running in Challenger's division
                if (!_statesManager.IsLadderRunning(objectChallengerTeam.Division))
                {
                    return $"The ladder is not currently running in the {objectChallengerTeam.Division} division and challenges may not be initiated yet.";
                }
                
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

                                    // Save and reload Challenges database
                                    _challengeManager.SaveAndReloadChallenges();

                                    // Grab new Challenge object reference
                                    Challenge newChallenge = _challengeManager.GetChallengeByTeamObject(objectChallengedTeam);

                                    // TODO - Send challenged notification
                                    foreach (Member member in objectChallengedTeam.Members)
                                    {
                                        _challengeManager.SendChallengeNotification(member.DiscordId, newChallenge);
                                    }

                                    return $"```Team {objectChallengerTeam.TeamName}(#{objectChallengerTeam.Rank}) has challenged Team {objectChallengedTeam.TeamName}(#{objectChallengedTeam.Rank}) in the {objectChallengerTeam.Division} division!```";
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
                                return $"```Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is greater than Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank}, the challenge was not initiated. Please try again.```";
                            }
                            else
                            {
                                return $"```Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is not in range of Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank} to make a challenge, the challenge was not initiated. Teams may only challenge at most TWO ranks above them. Please try again.```";
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
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if given team exists
            if (!_teamManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team reference object
                Team challengerTeamObject = _teamManager.GetTeamByName(challengerTeam);

                // Check if ladder is running in Challenger's division
                if (!_statesManager.IsLadderRunning(challengerTeamObject.Division))
                {
                    return $"The ladder is not currently running in the {challengerTeamObject.Division} division so there are no challenges to cancel yet.";
                }

                // Check if invoker is part of challengerTeam, as this is the user-level logic
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, challengerTeamObject))
                {
                    // Check if Team has a challenge sent out to actually cancel
                    if (_challengeManager.IsTeamChallenger(challengerTeamObject))
                    {
                        // Cancel the challenge, save challenges database and reload it
                        _challengeManager.RemoveChallenge(challengerTeamObject.TeamName, challengerTeamObject.Division);

                        // Save and reload Challenges database
                        _challengeManager.SaveAndReloadChallenges();

                        return $"```{challengerTeamObject.TeamName}(#{challengerTeamObject.Rank}) has canceled the challenge they sent out in the {challengerTeamObject.Division} division.```";
                    }
                    else
                    {
                        return $"```Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.```";
                    }
                }
                else
                {
                    return $"```You are not a member of Team {challengerTeamObject.TeamName}... That team's member(s) consists of: {challengerTeamObject.GetAllMemberNamesToStr()}```";
                }
            }
            return $"```No team found by the name of: {challengerTeam} - Please try again.```";
        }

        public string AdminChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if both teams actually exist in entire Teams database
            if (!_teamManager.IsTeamNameUnique(challengerTeam) && !_teamManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab Team object references
                Team objectChallengerTeam = _teamManager.GetTeamByName(challengerTeam);
                Team objectChallengedTeam = _teamManager.GetTeamByName(challengedTeam);

                // Check if ladder is running in Challenger's division
                if (!_statesManager.IsLadderRunning(objectChallengerTeam.Division))
                {
                    return $"The ladder is not currently running in the {objectChallengerTeam.Division} division and challenges may not be initiated yet.";
                }

                // Check if both teams are in the same division
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
                                // If all checks are passed, create and save the new Challenge object
                                _challengeManager.AddNewChallenge(new Challenge(objectChallengerTeam.Division, objectChallengerTeam.TeamName, objectChallengedTeam.TeamName));

                                // Save and reload Challenges database
                                _challengeManager.SaveAndReloadChallenges();

                                return $"```An Admin({context.User.GlobalName}) has initiated a challenge: Team {objectChallengerTeam.TeamName}(#{objectChallengerTeam.Rank}) has challenged Team {objectChallengedTeam.TeamName}(#{objectChallengedTeam.Rank}) in the {objectChallengerTeam.Division} division!```";
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
                            return $"```Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is greater than Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank}, the challenge was not initiated. Please try again.```";
                        }
                        else
                        {
                            return $"```Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is not in range of Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank} to make a challenge, the challenge was not initiated. Teams may only challenge at most TWO ranks above them. Please try again.```";
                        }
                    }
                }
                else
                {
                    return $"```Error - The given teams are not in the same division. Challenger Team Division: {objectChallengerTeam.Division} - Challenged Team Division: {objectChallengedTeam.Division} - Please try again.```";
                }
            }
            return $"```One or both team names not found in the database. Please try again.```";
        }

        public string AdminCancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if given team exists
            if (!_teamManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team reference object
                Team challengerTeamObject = _teamManager.GetTeamByName(challengerTeam);

                // Check if ladder is running in Challenger's division
                if (!_statesManager.IsLadderRunning(challengerTeamObject.Division))
                {
                    return $"The ladder is not currently running in the {challengerTeamObject.Division} division so there are no challenges to cancel yet.";
                }

                // Check if Team has a challenge sent out to actually cancel
                if (_challengeManager.IsTeamChallenger(challengerTeamObject))
                {
                    // Cancel the challenge, save challenges database and reload it
                    _challengeManager.RemoveChallenge(challengerTeamObject.TeamName, challengerTeamObject.Division);

                    // Save and reload Challenges database
                    _challengeManager.SaveAndReloadChallenges();

                    return $"```Team {challengerTeamObject.TeamName}(#{challengerTeamObject.Rank})'s challenge in the {challengerTeamObject.Division} division has been canceled by an Admin ({context.User.GlobalName})```";
                }
                else
                {
                    return $"```Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.```";
                }
            }
            return $"```No team found by the name of: {challengerTeam} - Please try again.```";
        }
        #endregion

        #region Reporting Logic

        public string ReportWinProcess(SocketInteractionContext context, string winningTeamName)
        {
            // Check if given team name exists
            if (!_teamManager.IsTeamNameUnique(winningTeamName))
            {
                // Grab winningTeam object, add placeholder for losingTeam object
                Team winningTeam = _teamManager.GetTeamByName(winningTeamName);
                Team losingTeam;

                // Check if ladder is running in Challenger's division
                if (!_statesManager.IsLadderRunning(winningTeam.Division))
                {
                    return $"```The ladder is not currently running in the {winningTeam.Division} division so there are no matches to report on yet.```";
                }

                // Is the invoker on the team, using context
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, winningTeam))
                {
                    // Is the team part of an active challenge (Challenger or Challenged)
                    if (_challengeManager.IsTeamAwaitingChallengeMatch(winningTeam))
                    {
                        // Grab challenge object for reference
                        Challenge? challenge = _challengeManager.GetChallengeByTeamObject(winningTeam);

                        // Determine if winningTeam is Challenger or Challenged
                        bool isWinningTeamChallenger;
                        if (challenge.Challenger.Equals(winningTeam.TeamName, StringComparison.OrdinalIgnoreCase))
                        {
                            isWinningTeamChallenger = true;
                            losingTeam = _teamManager.GetTeamByName(challenge.Challenged);
                        }
                        else
                        {
                            isWinningTeamChallenger = false;
                            losingTeam = _teamManager.GetTeamByName(challenge.Challenger);
                        }

                        // If winningTeam is challenger, rank change will occur
                        if (isWinningTeamChallenger)
                        {
                            // The winning team takes the rank of the losing team
                            winningTeam.Rank = losingTeam.Rank;
                            losingTeam.Rank++;

                            // Reassign ranks for the entire division
                            ReassignRanks(winningTeam.Division);

                            // Assign win and loss correctly
                            _teamManager.AddToWins(winningTeam, 1);
                            _teamManager.AddToLosses(losingTeam, 1);

                            // Save the updated teams database and reload
                            _teamManager.SaveAndReloadTeamsDatabase();

                            // Remove the challenge
                            _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);
                            return $"```Team {winningTeam.TeamName} has won the challenge they initiated against Team {losingTeam.TeamName} in the {winningTeam.Division} division and taken their rank of (#{winningTeam.Rank})! Team {losingTeam.TeamName} drops down one in the ranks to (#{losingTeam.Rank}). All other ranks are adjusted accordingly.```";
                        }
                        // If winningTeam is challenged, no rank change will occur
                        else
                        {
                            // Assign win and loss correctly
                            _teamManager.AddToWins(winningTeam, 1);
                            _teamManager.AddToLosses(losingTeam, 1);

                            // Save the updated teams data and reload
                            _teamManager.SaveAndReloadTeamsDatabase();

                            // If the challenged team wins, no rank change
                            _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);
                            return $"```Team {winningTeam.TeamName}(#{winningTeam.Rank}) has defeated Team {losingTeam.TeamName}(#{losingTeam.Rank}) and defended their rank in the {winningTeam.Division} division. No rank change has occured.```";
                        }
                    }
                    else
                    {
                        return $"```Team {winningTeam.TeamName} is not currently waiting on a challenge match.```";
                    }
                }
                else
                {
                    return $"```You are not part of Team {winningTeam.TeamName}.```";
                }
            }

            return $"```The given team name not found in the database: {winningTeamName}```";
        }
        #endregion

        #region Post Standings/Challenges/Teams Logic

        public string PostStandingsProcess(SocketInteractionContext context, string division)
        {
            // Return all the data as one string
            return _teamManager.GetStandingsData(division);
        }

        public string PostTeamsProcess(SocketInteractionContext context, string division)
        {
            return _teamManager.GetTeamsData(division);
        }

        public string PostChallengesProcess(SocketInteractionContext context, string division)
        {
            return _challengeManager.GetChallengesData(division);
        }

        #endregion

        #region Set Rank Logic

        public string SetRankProcess(string teamName, int newRank)
        {
            // Check if the team exists
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Get the team object
                Team teamToAdjust = _teamManager.GetTeamByName(teamName);

                // Get the current rank of the team
                int currentRank = teamToAdjust.Rank;

                // Get all teams in the same division
                List<Team> teamsInDivision = _teamManager.GetTeamsByDivision(teamToAdjust.Division);

                if (newRank == currentRank)
                {
                    return $"```Team {teamName} is already at rank {newRank}. No changes made.```";
                }

                // Moving the team up in rank (newRank < currentRank)
                if (newRank < currentRank)
                {
                    for (int i = 0; i < teamsInDivision.Count; i++)
                    {
                        if (teamsInDivision[i].Rank >= newRank && teamsInDivision[i].Rank < currentRank && teamsInDivision[i].TeamName != teamToAdjust.TeamName)
                        {
                            teamsInDivision[i].Rank++;
                        }
                    }
                }
                // Moving the team down in rank (newRank > currentRank)
                else if (newRank > currentRank)
                {
                    for (int i = 0; i < teamsInDivision.Count; i++)
                    {
                        if (teamsInDivision[i].Rank <= newRank && teamsInDivision[i].Rank > currentRank && teamsInDivision[i].TeamName != teamToAdjust.TeamName)
                        {
                            teamsInDivision[i].Rank--;
                        }
                    }
                }

                // Finally, set the new rank for the team
                teamToAdjust.Rank = newRank;

                // Reassign ranks to ensure consistency
                ReassignRanks(teamToAdjust.Division);

                // Save and reload the teams database
                _teamManager.SaveAndReloadTeamsDatabase();

                return $"```Team {teamName} has been moved to rank {newRank} in the {teamToAdjust.Division} division.```";
            }
            return $"```The given team name was not found in the database: {teamName}.```";
        }
        #endregion

        #region Set Standings/Challenges/Teams Channel Logic

        public string SetChallengesChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetChallengesChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Challenges.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "2v2":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetChallengesChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Challenges.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetChallengesChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Challenges.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                default:
                    return "Incorrect division type given.";
            }
        }

        public string SetStandingsChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetStandingsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Standings.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "2v2":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetStandingsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Standings.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetStandingsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Standings.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                default:
                    return "Incorrect division type given.";
            }
        }

        public string SetTeamsChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetTeamsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Teams.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "2v2":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetTeamsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Teams.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetTeamsChannelId(division, channel.Id);
                        return $"{channel.Id} was set for {division} Teams.";
                    }
                    return $"{channel.Id} is incorrect for a channel Id.";

                default:
                    return "Incorrect division type given.";
            }
        }

        #endregion

        #region Add/Subtract Win/Loss Logic

        // For Admin command use, ReportWin logic uses directly to TeamManager
        public string AddToWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Check if team name exists in database
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team team = _teamManager.GetTeamByName(teamName);

                // Add the numberOfWins to the team
                _teamManager.AddToWins(team, numberOfWins);

                // Save and reload teams database
                _teamManager.SaveAndReloadTeamsDatabase();

                return $"```Team {team.TeamName} has had {numberOfWins} win(s) added to their win count by an Admin({context.User.GlobalName})```";
            }
            return $"```Given team name not found in the database: {teamName} - Please try again.```";
        }

        public string SubtractFromWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Check if team name exists in database
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team team = _teamManager.GetTeamByName(teamName);

                // Make sure the result will not be negative
                int result = team.Wins - numberOfWins;
                if (result >= 0)
                {
                    // Safely subtract from wins
                    _teamManager.SubtractFromWins(team, numberOfWins);

                    // Save and reload teams database
                    _teamManager.SaveAndReloadTeamsDatabase();

                    return $"```Team {team.TeamName} has had {numberOfWins} win(s) subtracted from their win count by an Admin({context.User.GlobalName})```";
                }
                else
                {
                    return $"```Subtracting {numberOfWins} win(s) from Team {team.TeamName}'s {team.Wins} wins would result in a negative number. Please try again.```";
                }

            }
            return $"```Given team name not found in the database: {teamName} - Please try again.```";
        }

        public string AddToLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
        {
            // Check if team name exists in database
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team team = _teamManager.GetTeamByName(teamName);

                // Add the numberOfLosses to the team
                _teamManager.AddToLosses(team, numberOfLosses);

                // Save and reload teams database
                _teamManager.SaveAndReloadTeamsDatabase();

                return $"```Team {team.TeamName} has had {numberOfLosses} loss(es) added to their losses count by an Admin({context.User.GlobalName})```";
            }
            return $"```Given team name not found in the database: {teamName} - Please try again.```";
        }

        public string SubtractFromLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
        {
            // Check if team name exists in database
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team team = _teamManager.GetTeamByName(teamName);

                // Make sure the result will not be negative
                int result = team.Losses - numberOfLosses;
                if (result >= 0)
                {
                    // Safely subtract from losses
                    _teamManager.SubtractFromLosses(team, numberOfLosses);

                    // Save and reload teams database
                    _teamManager.SaveAndReloadTeamsDatabase();

                    return $"```Team {team.TeamName} has had {numberOfLosses} loss(es) subtracted from their losses count by an Admin({context.User.GlobalName})```";
                }
                else
                {
                    return $"```Subtracting {numberOfLosses} loss(es) from Team {team.TeamName}'s {team.Losses} losses would result in a negative number. Please try again.```";
                }

            }
            return $"```Given team name not found in the database: {teamName} - Please try again.```";
        }

        #endregion

        #region Helper Methods
        private void ReassignRanks(string division)
        {
            // Get the list of teams in the specified division
            List<Team> teamsInDivision = _teamManager.GetTeamsByDivision(division);

            // Sort teams by their current rank
            teamsInDivision.Sort((a, b) => a.Rank.CompareTo(b.Rank));

            // Reassign ranks sequentially
            for (int i = 0; i < teamsInDivision.Count; i++)
            {
                teamsInDivision[i].Rank = i + 1;
            }
        }
        #endregion

        #region Settings Logic

        public string SetGuildId(SocketCommandContext context)
        {
            // Grab Guild Id command was invoked from
            ulong guildId = context.Guild.Id;

            // Set in Settings using SettingsManager then save and reload Settings
            _settingsManager.Settings.GuildId = guildId;
            _settingsManager.SaveSettings();
            _settingsManager.LoadSettingsData();

            return $"```Set GuildId in config.json to {guildId} - If this is the first time setting the GuildId for Slash Commands, then please restart the bot now.```";
        }

        public string SetSuperAdminModeOnOffProcess(string onOrOff)
        {
            switch (onOrOff.Trim().ToLower())
            {
                case "on":
                    _settingsManager.SetSuperAdminModeOnOff(true);
                    _settingsManager.SaveAndReloadSettingsDatabase();
                    return $"```Super Admin Mode is on.```";

                case "off":
                    _settingsManager.SetSuperAdminModeOnOff(false);
                    _settingsManager.SaveAndReloadSettingsDatabase();
                    return $"```Super Admin Mode is off```";

                default:
                    return $"```Incorrent variable given.```";
            }
        }

        public string AddSuperAdminIdProcess(IUser user)
        {
            // Grab discord Id of given user to make Super Admin
            ulong newAdminId = user.Id;

            if (!_settingsManager.IsDiscordIdInSuperAdminList(newAdminId))
            { 
                _settingsManager.AddSuperAdminId(newAdminId);
                _settingsManager.SaveAndReloadSettingsDatabase();
                return $"```New Super Admin Id added to Settings config.json: {newAdminId}```";
                
            }
            return $"```{newAdminId} was already found among the Super Admin List in Settings config.json and can not be added again.```";
        }

        public string RemoveSuperAdminIdProcess(IUser user)
        {
            // Grab discord Id of user to remove from Super Admin list
            ulong adminId = user.Id;

            if (_settingsManager.IsDiscordIdInSuperAdminList(adminId))
            {
                _settingsManager.RemoveSuperAdminId(adminId);
                _settingsManager.SaveAndReloadSettingsDatabase();
                return $"```{adminId} was removed from the Super Admin Id from Settings config.json.```";
            }
            return $"```{adminId} was not found among the Super Admin List in Settings config.json.```";
        }
        #endregion
    }
}
