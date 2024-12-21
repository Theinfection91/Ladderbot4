using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Models;
using Ladderbot4.Models.Achievements;
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
        private readonly AchievementManager _achievementManager;
        private readonly EmbedManager _embedManager;
        private readonly LeagueManager _leagueManager;
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


        public LadderManager(DiscordSocketClient client, AchievementManager achievementManager, EmbedManager embedManager, LeagueManager leagueManager, GitBackupManager gitBackupManager, HistoryManager historyManager, TeamManager teamManager, MemberManager memberManager, ChallengeManager challengeManager, SettingsManager settingsManager, StatesManager statesManager)
        {
            _client = client;
            _achievementManager = achievementManager;
            _embedManager = embedManager;
            _leagueManager = leagueManager;
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

            // Start Automated Backup Task
            StartAutomatedBackupTask();   
        }
        #endregion

        #region Automated Backup Logic

        public void StartAutomatedBackupTask()
        {
            Task.Run(() => RunAutomatedBackupTaskAsync());
        }

        private async Task RunAutomatedBackupTaskAsync()
        {
            while (true)
            {
                // Default time between automated backup is 60 minutes
                await Task.Delay(TimeSpan.FromMinutes(60));
                await SendBackupRepoToGitRepo();
            }
        }

        private async Task SendBackupRepoToGitRepo()
        {
            if (_settingsManager.IsGitPatTokenSet())
            {
                _backupManager.CopyJsonFilesToBackupRepo();
                _backupManager.ForceBackupFiles();
            }
        }

        #endregion

        #region Channel Tasks Logic

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

                // Get the embed for the challenges in the division
                Embed challengesEmbed = _challengeManager.GetChallengesEmbed(division);

                if (challengesEmbed == null)
                {
                    Console.WriteLine($"No challenges to display for the {division} division.");
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
                            // Edit the existing message with the new embed
                            await existingMessage.ModifyAsync(msg => msg.Embed = challengesEmbed);
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(embed: challengesEmbed);
                            _standingsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(embed: challengesEmbed);
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

                // Get the standings embed for the division
                Embed standingsEmbed = _teamManager.GetStandingsEmbed(division);

                if (standingsEmbed == null)
                {
                    Console.WriteLine($"No standings to display for the {division} division.");
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
                            // Edit the existing message with the new embed
                            await existingMessage.ModifyAsync(msg =>
                            {
                                msg.Embed = standingsEmbed;
                                msg.Content = string.Empty; // Clear any existing text content
                            });
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(embed: standingsEmbed);
                            _standingsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(embed: standingsEmbed);
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

                // Get the teams embed for the division
                Embed teamsEmbed = _teamManager.GetTeamsEmbed(division);

                if (teamsEmbed == null)
                {
                    Console.WriteLine($"No teams to display for the {division} division.");
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
                            // Edit the existing message with the new embed
                            await existingMessage.ModifyAsync(msg =>
                            {
                                msg.Embed = teamsEmbed;
                                msg.Content = string.Empty; // Clear any existing text content
                            });
                        }
                        else
                        {
                            // Message was deleted, send a new one
                            var newMessage = await channel.SendMessageAsync(embed: teamsEmbed);
                            _teamsMessageMap[channelId] = newMessage.Id;
                        }
                    }
                    else
                    {
                        // No existing message, send a new one
                        var newMessage = await channel.SendMessageAsync(embed: teamsEmbed);
                        _teamsMessageMap[channelId] = newMessage.Id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating teams in channel {channelId}: {ex.Message}");
                }
            }
        }
        #endregion

        #endregion

        #region Start/End Ladder Logic
        public Embed StartLadderByDivisionProcess(string division)
        {
            _statesManager.LoadStatesDatabase();

            // Check if ladder is currently running or not
            if (!_statesManager.IsLadderRunning(division))
            {
                // Set ladder running to true in division
                _statesManager.SetLadderRunning(division, true);

                // Save and reload the database
                _statesManager.SaveAndReloadStatesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.StartLadderSuccessEmbed(division);
            }
            else
            {
                return _embedManager.StartLadderAlreadyRunningEmbed(division);
            }  
        }

        public Embed EndLadderByDivisionProcess(string division)
        {
            _statesManager.LoadStatesDatabase();

            // Check if ladder is currently running or not
            if (_statesManager.IsLadderRunning(division))
            {
                // Set ladder running to false in division
                _statesManager.SetLadderRunning(division, false);

                // Save and reload the database
                _statesManager.SaveAndReloadStatesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.EndLadderSuccessEmbed(division);
            }
            else
            {
                return _embedManager.EndLadderNotRunningEmbed(division);
            }
        }
        #endregion

        #region Create/Delete League Logic
        public Embed CreateLeagueProcess(string leagueName, string divisionType)
        {
            // Check if desired League name is taken
            if (_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Check if given division type is correct
                if (_leagueManager.IsValidDivisionType(divisionType))
                {
                    // Create new League object
                    League newLeague = new(leagueName, divisionType);

                    // Add new league to database
                    _leagueManager.AddNewLeague(newLeague);

                    // Save and reload
                    _leagueManager.SaveAndReloadLeaguesDatabase();

                    // Return success embed
                    return _embedManager.CreateLeagueSuccessEmbed(newLeague);
                }
                return _embedManager.CreateLeagueErrorEmbed($"Invalid Division Type given: {divisionType}. Choose between 1v1, 2v2, or 3v3.");
            }
            return _embedManager.CreateLeagueErrorEmbed($"The given League Name ({leagueName}) is already taken. Choose another name for the new League.");
        }

        public Embed DeleteLeagueProcess(string leagueName)
        {
            // Load latest save
            _leagueManager.LoadLeaguesDatabase();

            // Check if League by given name exist
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league object
                League leagueToRemove = _leagueManager.GetLeagueByName(leagueName);
                if (leagueToRemove != null)
                {
                    // TODO - Remove all challenges associated with all teams in league

                    // Remove League from leagues.json
                    _leagueManager.RemoveLeague(leagueToRemove.LeagueName, leagueToRemove.Division);

                    // Save and reload
                    _leagueManager.SaveAndReloadLeaguesDatabase();

                    // Backup to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    // Return success embed
                    return _embedManager.DeleteLeagueSuccessEmbed(leagueToRemove);
                }
                return _embedManager.DeleteLeagueErrorEmbed($"The League object that was found was null. Contact the bot's admin.");
            }
            return _embedManager.DeleteLeagueErrorEmbed($"No League was found by the given League Name: {leagueName}");
        }

        #endregion

        #region Register/Remove Team Logic

        public Embed RegisterTeamToLeagueProcess(SocketInteractionContext context, string teamName, string leagueName, List<IUser> members)
        {
            // Load latest save for both Managers
            _leagueManager.LoadLeaguesDatabase();
            _teamManager.LoadLeaguesDatabase();

            // Check if League by given name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab reference of league
                League leagueReference = _leagueManager.GetLeagueByName(leagueName);

                if (_teamManager.IsTeamNameUnique(teamName))
                {
                    // Convert User Context Info into Member objects
                    List<Member> newMemberList = _memberManager.ConvertMembersListToObjects(members);

                    if (_memberManager.IsMemberCountCorrect(newMemberList, leagueReference.Division) || _settingsManager.IsUserSuperAdmin(context.User.Id))
                    {
                        // Grab all teams from League
                        List<Team>? leagueTeams = _teamManager.GetTeamsInLeague(leagueReference);

                        // Check if any member is already on a team in the given league
                        foreach (Member member in newMemberList)
                        {
                            if (_memberManager.IsMemberOnTeamInLeague(member, leagueReference.Teams) && !_settingsManager.IsUserSuperAdmin(context.User.Id))
                            {
                                return _embedManager.RegisterTeamErrorEmbed($"{member.DisplayName} is already on a team in the {leagueReference.LeagueName} League.");
                            }
                        }

                        // Create team object
                        Team newTeam = _teamManager.CreateTeamObject(teamName, leagueReference.Division, _teamManager.GetTeamCountInLeague(leagueReference) + 1, newMemberList);

                        // Add team to league
                        _teamManager.AddNewTeamToLeague(newTeam, leagueReference);

                        // Save and reload Leagues Database from the Team Manager
                        _teamManager.SaveAndReloadLeaguesDatabase();

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.RegisterTeamToLeagueSuccessEmbed(newTeam, leagueReference);
                    }
                    return _embedManager.RegisterTeamErrorEmbed($"Incorrect amount of members given for specified division type: Division - {leagueReference.Division} | Member Count - {newMemberList.Count}.");
                }
                return _embedManager.RegisterTeamErrorEmbed($"The given team name is already being used by another team: {teamName}.");
            }
            return _embedManager.RegisterTeamErrorEmbed($"No League was found by the given name of: {leagueName}");
        }

        public Embed RemoveTeamFromLeagueProcess(string teamName)
        {
            // Load latest save
            _teamManager.LoadLeaguesDatabase();
            _leagueManager.LoadLeaguesDatabase();

            // Check if Team exists in any League
            if (!_teamManager.IsTeamNameUnique(teamName))
            {
                // Grab Team Object
                Team teamToRemove = _teamManager.GetTeamByNameFromLeagues(teamName);
                // Grab League object
                League correctLeague = _leagueManager.GetLeagueFromTeamName(teamName);

                // TODO - Remove all Challenges from Database associated with team


                // TODO - Remove the team correctly and correct ranks
                _teamManager.RemoveTeamFromLeague(teamToRemove, correctLeague);
                ReassignRanksInLeague(correctLeague);

                // Save and reload
                _leagueManager.SaveAndReloadLeaguesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                // Return success embed
                return _embedManager.RemoveTeamSuccessEmbed(teamToRemove, correctLeague);
            }

            return _embedManager.RemoveTeamErrorEmbed($"The team '{teamName}' does not exist in the database. Please try again.");
        }

        /// <summary>
        /// Starts the process of trying to remove a team
        /// </summary>
        /// <param name="teamName">Name of team to try and remove from database</param>
        /// <returns>Embed from the bot that will cover error handling and confirmation of removal.</returns>
        public Embed RemoveTeamProcess(string teamName)
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

                // Return a success embed
                return _embedManager.RemoveTeamSuccessEmbed(teamReference, null);

            }
            // Return an error embed if the team does not exist
            return _embedManager.RemoveTeamErrorEmbed($"The team '{teamName}' does not exist in the database. Please try again.");
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
        public Embed ChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load the latest save of the Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if both teams exist in the database
            if (!_teamManager.IsTeamNameUnique(challengerTeam) && !_teamManager.IsTeamNameUnique(challengedTeam))
            {
                // Get team object references
                Team objectChallengerTeam = _teamManager.GetTeamByName(challengerTeam);
                Team objectChallengedTeam = _teamManager.GetTeamByName(challengedTeam);

                // Check if the ladder is running in the challenger's division
                if (!_statesManager.IsLadderRunning(objectChallengerTeam.Division))
                {
                    return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in the {objectChallengerTeam.Division} division, and challenges may not be initiated yet.");
                }

                // Grab Discord ID of the user who invoked the command
                ulong discordId = context.User.Id;

                // Check if the user is on the challenger team
                if (_memberManager.IsDiscordIdOnGivenTeam(discordId, objectChallengerTeam))
                {
                    // Check if the teams are in the same division
                    if (_teamManager.IsTeamsInSameDivision(objectChallengerTeam, objectChallengedTeam))
                    {
                        // Ensure the ranks are within range
                        if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                        {
                            // Check if the challenger has no pending challenges
                            if (!_challengeManager.IsTeamAwaitingChallengeMatch(objectChallengerTeam))
                            {
                                // Check if the challenged team has no pending challenges
                                if (!_challengeManager.IsTeamAwaitingChallengeMatch(objectChallengedTeam))
                                {
                                    // Create and save the new Challenge
                                    _challengeManager.AddNewChallenge(new Challenge(objectChallengerTeam.Division, objectChallengerTeam.TeamName, objectChallengerTeam.Rank, objectChallengedTeam.TeamName, objectChallengedTeam.Rank));

                                    // Change IsChallengeable of both teams to false
                                    _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                    _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                    _teamManager.SaveAndReloadTeamsDatabase();

                                    _challengeManager.SaveAndReloadChallenges();
                                    _backupManager.CopyAndBackupFilesToGit();

                                    // Get the newly created challenge
                                    Challenge newChallenge = _challengeManager.GetChallengeByTeamObject(objectChallengedTeam);

                                    // Notify the challenged team
                                    foreach (Member member in objectChallengedTeam.Members)
                                    {
                                        _challengeManager.SendChallengeNotification(member.DiscordId, newChallenge);
                                    }

                                    // Return a success embed
                                    return _embedManager.ChallengeSuccessEmbed(objectChallengerTeam, objectChallengedTeam, newChallenge);
                                }
                                else
                                {
                                    return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.TeamName} is already awaiting a challenge match. Please try again later.");
                                }
                            }
                            else
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName} is already awaiting a challenge match. Please try again later.");
                            }
                        }
                        else
                        {
                            if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank ({objectChallengerTeam.Rank}) is higher than {objectChallengedTeam.TeamName}'s rank ({objectChallengedTeam.Rank}). A challenge cannot be initiated. Please try again.");
                            }
                            else
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank ({objectChallengerTeam.Rank}) is not within the allowed range to challenge {objectChallengedTeam.TeamName}'s rank ({objectChallengedTeam.Rank}). Challenges can only be made for teams within two ranks above. Please try again.");
                            }
                        }
                    }
                    else
                    {
                        return _embedManager.ChallengeErrorEmbed($"The teams are not in the same division. Challenger Division: {objectChallengerTeam.Division}, Challenged Division: {objectChallengedTeam.Division}. Please try again.");
                    }
                }
                else
                {
                    return _embedManager.ChallengeErrorEmbed($"You are not a member of Team {objectChallengerTeam.TeamName}. Please try again.");
                }
            }

            return _embedManager.ChallengeErrorEmbed($"One or both team names were not found in the database. Please try again.");
        }

        public Embed CancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
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
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in the {challengerTeamObject.Division} division so there are no challenges to cancel yet.");
                }

                // Check if invoker is part of challengerTeam, as this is the user-level logic
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, challengerTeamObject))
                {
                    // Check if Team has a challenge sent out to actually cancel
                    if (_challengeManager.IsTeamChallenger(challengerTeamObject))
                    {
                        // Grab challenge object to use to change challenged team IsChallengeable status
                        Challenge? challenge = _challengeManager.GetChallengeByTeamObject(challengerTeamObject);
                        Team challengedTeamObject = _teamManager.GetTeamByName(challenge.Challenged);

                        // Set IsChallengeable for both teams back to true
                        _teamManager.ChangeChallengeStatus(challengerTeamObject, true);
                        _teamManager.ChangeChallengeStatus(challengedTeamObject, true);
                        _teamManager.SaveAndReloadTeamsDatabase();

                        // Cancel the challenge, save challenges database and reload it
                        _challengeManager.RemoveChallenge(challengerTeamObject.TeamName, challengerTeamObject.Division);

                        // Save and reload Challenges database
                        _challengeManager.SaveAndReloadChallenges();

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.CancelChallengeSuccessEmbed(challengerTeamObject);
                    }
                    else
                    {
                        return _embedManager.CancelChallengeErrorEmbed($"Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.");
                    }
                }
                else
                {
                    return _embedManager.CancelChallengeErrorEmbed($"You are not a member of Team {challengerTeamObject.TeamName}... That team's member(s) consists of: {challengerTeamObject.GetAllMemberNamesToStr()}");
                }
            }
            return _embedManager.CancelChallengeErrorEmbed($"No team found by the name of: {challengerTeam}");
        }

        public Embed AdminChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
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
                    return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in the {objectChallengerTeam.Division} division and challenges may not be initiated yet.");
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
                                _challengeManager.AddNewChallenge(new Challenge(objectChallengerTeam.Division, objectChallengerTeam.TeamName, objectChallengerTeam.Rank, objectChallengedTeam.TeamName, objectChallengedTeam.Rank));

                                // Change IsChallengeable of both teams to false
                                _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                _teamManager.SaveAndReloadTeamsDatabase();

                                // Save and reload Challenges database
                                _challengeManager.SaveAndReloadChallenges();

                                // Backup the database to Git
                                _backupManager.CopyAndBackupFilesToGit();

                                return _embedManager.AdminChallengeSuccessEmbed(objectChallengerTeam, objectChallengedTeam, context);
                            }
                            else
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated.");
                            }
                        }
                        else
                        {
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName} is currently waiting for a challenge match to be played, the challenge was not initiated.");
                        }
                    }
                    else
                    {
                        if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                        {
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is greater than Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank}, the challenge was not initiated.");
                        }
                        else
                        {
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank of {objectChallengerTeam.Rank} is not in range of Team {objectChallengedTeam.TeamName}'s rank of {objectChallengedTeam.Rank} to make a challenge, the challenge was not initiated. Teams may only challenge at most TWO ranks above them.");
                        }
                    }
                }
                else
                {
                    return _embedManager.ChallengeErrorEmbed($"Error - The given teams are not in the same division. Challenger Team Division: {objectChallengerTeam.Division} - Challenged Team Division: {objectChallengedTeam.Division}");
                }
            }
            return _embedManager.ChallengeErrorEmbed($"One or both team names not found in the database.");
        }

        public Embed AdminCancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
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
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in the {challengerTeamObject.Division} division so there are no challenges to cancel yet.");
                }

                // Check if Team has a challenge sent out to actually cancel
                if (_challengeManager.IsTeamChallenger(challengerTeamObject))
                {
                    // Grab challenge object to use to change challenged team IsChallengeable status
                    Challenge? challenge = _challengeManager.GetChallengeByTeamObject(challengerTeamObject);
                    Team challengedTeamObject = _teamManager.GetTeamByName(challenge.Challenged);

                    // Set IsChallengeable for both teams back to true
                    _teamManager.ChangeChallengeStatus(challengerTeamObject, true);
                    _teamManager.ChangeChallengeStatus(challengedTeamObject, true);
                    _teamManager.SaveAndReloadTeamsDatabase();

                    // Cancel the challenge, save challenges database and reload it
                    _challengeManager.RemoveChallenge(challengerTeamObject.TeamName, challengerTeamObject.Division);

                    // Save and reload Challenges database
                    _challengeManager.SaveAndReloadChallenges();

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.AdminCancelChallengeSuccessEmbed(challengerTeamObject, context);
                }
                else
                {
                    return _embedManager.CancelChallengeErrorEmbed($"Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.");
                }
            }
            return _embedManager.CancelChallengeErrorEmbed($"No team found by the name of: {challengerTeam} - Please try again.");
        }
        #endregion

        #region Reporting Logic

        public Embed ReportWinProcess(SocketInteractionContext context, string winningTeamName)
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
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in the {winningTeam.Division} division so there are no matches to report on yet.");
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

                            // Add to members wins and losses
                            foreach (Member member in winningTeam.Members)
                            {
                                _memberManager.AddToMemberWins(null, winningTeam.Division, 1);
                            }

                            foreach (Member member in losingTeam.Members)
                            {
                                _memberManager.AddToMemberLosses(member, losingTeam.Division, 1);
                            }

                            // TODO: Create Match object to add to History (Past Matches)
                            // Create correct match ID from division
                            int matchId = _historyManager.GetDivisionMatchCount(challenge.Division);

                            _historyManager.AddNewMatch(_historyManager.CreateMatchObject(matchId + 1, challenge.Division, challenge.Challenger, challenge.ChallengerRank, challenge.Challenged, challenge.ChallengedRank, winningTeam.TeamName, losingTeam.TeamName, challenge.CreatedOn));

                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _teamManager.SaveAndReloadTeamsDatabase();

                            // Remove the challenge
                            _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);

                            // Backup the database to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with true, showing rank change
                            return _embedManager.ReportWinSuccessEmbed(winningTeam, losingTeam, true, winningTeam.Division);

                        }
                        // If winningTeam is challenged, no rank change will occur
                        else
                        {
                            // Assign win and loss correctly
                            _teamManager.AddToWins(winningTeam, 1);
                            _teamManager.AddToLosses(losingTeam, 1);

                            // Add to members wins and losses
                            foreach (Member member in winningTeam.Members)
                            {
                                _memberManager.AddToMemberWins(null, winningTeam.Division, 1);
                            }

                            foreach (Member member in losingTeam.Members)
                            {
                                _memberManager.AddToMemberLosses(member, losingTeam.Division, 1);
                            }

                            // TODO: Create Match object to add to History (Past Matches)
                            // Create correct match ID from division
                            int matchId = _historyManager.GetDivisionMatchCount(challenge.Division);

                            _historyManager.AddNewMatch(_historyManager.CreateMatchObject(matchId + 1, challenge.Division, challenge.Challenger, challenge.ChallengerRank, challenge.Challenged, challenge.ChallengedRank, winningTeam.TeamName, losingTeam.TeamName, challenge.CreatedOn));

                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _teamManager.SaveAndReloadTeamsDatabase();

                            // If the challenged team wins, no rank change
                            _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);

                            // Backup the database to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with false, showing no rank change
                            return _embedManager.ReportWinSuccessEmbed(winningTeam, losingTeam, false, winningTeam.Division);

                        }
                    }
                    else
                    {
                        return _embedManager.ReportWinErrorEmbed($"Team {winningTeam.TeamName} is not currently waiting on a challenge match.");
                    }
                }
                else
                {
                    return _embedManager.ReportWinErrorEmbed($"You are not part of Team {winningTeam.TeamName}.");
                }
            }
            return _embedManager.ReportWinErrorEmbed($"The given team name was not found in the database: {winningTeamName}");
        }

        public Embed ReportWinAdminProcess(SocketInteractionContext context, string winningTeamName)
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
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in the {winningTeam.Division} division so there are no matches to report on yet.");
                }

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

                        // Add to members wins and losses
                        foreach (Member member in winningTeam.Members)
                        {
                            _memberManager.AddToMemberWins(null, winningTeam.Division, 1);
                        }

                        foreach (Member member in losingTeam.Members)
                        {
                            _memberManager.AddToMemberLosses(member, losingTeam.Division, 1);
                        }

                        // TODO: Create Match object to add to History (Past Matches)
                        // Create correct match ID from division
                        int matchId = _historyManager.GetDivisionMatchCount(challenge.Division);

                        _historyManager.AddNewMatch(_historyManager.CreateMatchObject(matchId + 1, challenge.Division, challenge.Challenger, challenge.ChallengerRank, challenge.Challenged, challenge.ChallengedRank, winningTeam.TeamName, losingTeam.TeamName, challenge.CreatedOn));

                        // Set IsChallengeable status of both teams back to true
                        _teamManager.ChangeChallengeStatus(winningTeam, true);
                        _teamManager.ChangeChallengeStatus(losingTeam, true);

                        // Save the updated teams database and reload
                        _teamManager.SaveAndReloadTeamsDatabase();

                        // Remove the challenge
                        _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        // Return Success Embed with true, showing rank change
                        return _embedManager.ReportWinAdminSuccessEmbed(context, winningTeam, losingTeam, true, winningTeam.Division);
                    }
                    // If winningTeam is challenged, no rank change will occur
                    else
                    {
                        // Assign win and loss correctly
                        _teamManager.AddToWins(winningTeam, 1);
                        _teamManager.AddToLosses(losingTeam, 1);

                        // Add to members wins and losses
                        //foreach (Member member in winningTeam.Members)
                        //{
                        //    _memberManager.AddToMemberWins(member, winningTeam.Division, 1);
                        //}

                        foreach (Member member in losingTeam.Members)
                        {
                            _memberManager.AddToMemberLosses(member, losingTeam.Division, 1);
                        }

                        // TODO: Create Match object to add to History (Past Matches)
                        // Create correct match ID from division
                        int matchId = _historyManager.GetDivisionMatchCount(challenge.Division);

                        _historyManager.AddNewMatch(_historyManager.CreateMatchObject(matchId + 1, challenge.Division, challenge.Challenger, challenge.ChallengerRank, challenge.Challenged, challenge.ChallengedRank, winningTeam.TeamName, losingTeam.TeamName, challenge.CreatedOn));

                        // Set IsChallengeable status of both teams back to true
                        _teamManager.ChangeChallengeStatus(winningTeam, true);
                        _teamManager.ChangeChallengeStatus(losingTeam, true);

                        // Save the updated teams data and reload
                        _teamManager.SaveAndReloadTeamsDatabase();

                        // If the challenged team wins, no rank change
                        _challengeManager.RemoveChallenge(challenge.Challenger, winningTeam.Division);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        // Return Success Embed with false, showing no rank change
                        return _embedManager.ReportWinAdminSuccessEmbed(context, winningTeam, losingTeam, false, winningTeam.Division);

                    }

                }
                else
                {
                    return _embedManager.ReportWinErrorEmbed($"Team {winningTeam.TeamName} is not currently waiting on a challenge match.");
                }

            }
            return _embedManager.ReportWinErrorEmbed($"The given team name was not found in the database: {winningTeamName}");
        }
        #endregion

        #region History Logic

        public Embed ShowAllHistoryByDivisionProcess(string division)
        {
            // Embed testing
            var embed = new EmbedBuilder()
                .WithTitle("Embed with Footer")
                .WithFooter("Generated by Ladderbot4", "https://example.com/icon.png")
                .WithTimestamp(DateTimeOffset.Now)
                .Build();
            return embed;
        }

        #endregion

        #region Post Standings/Challenges/Teams Logic

        public Embed PostStandingsProcess(SocketInteractionContext context, string division)
        {
            return _teamManager.GetStandingsEmbed(division);
        }

        public Embed PostTeamsProcess(SocketInteractionContext context, string division)
        {
            return _teamManager.GetTeamsEmbed(division);
        }

        public Embed PostChallengesProcess(SocketInteractionContext context, string division)
        {
            return _challengeManager.GetChallengesEmbed(division);
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

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return $"```Team {teamName} has been moved to rank {newRank} in the {teamToAdjust.Division} division.```";
            }
            return $"```The given team name was not found in the database: {teamName}.```";
        }
        #endregion

        #region Set Standings/Challenges/Teams Channel Logic

        public Embed SetChallengesChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                case "2v2":
                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetChallengesChannelId(division, channel.Id);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.SetChannelIdSuccessEmbed(division, channel, "Challenges");
                    }
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Challenges", $"{channel.Id} is incorrect for a channel Id.");

                default:
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Challenges", "Incorrect division type given.");
            }
        }

        public Embed SetStandingsChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                case "2v2":
                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetStandingsChannelId(division, channel.Id);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.SetChannelIdSuccessEmbed(division, channel, "Standings");
                    }
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Standings", $"{channel.Id} is incorrect for a channel Id.");

                default:
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Standings", "Incorrect division type given.");
            }
        }

        public Embed SetTeamsChannelIdProcess(string division, IMessageChannel channel)
        {
            switch (division)
            {
                case "1v1":
                case "2v2":
                case "3v3":
                    if (channel.Id != 0)
                    {
                        _statesManager.SetTeamsChannelId(division, channel.Id);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.SetChannelIdSuccessEmbed(division, channel, "Teams");
                    }
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Teams", $"{channel.Id} is incorrect for a channel Id.");

                default:
                    return _embedManager.SetChannelIdErrorEmbed(division, channel, "Teams", "Incorrect division type given.");
            }
        }

        #endregion

        #region Add/Subtract Win/Loss Logic

        // For Admin command use, ReportWin logic uses directly to TeamManager
        public Embed AddToWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
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

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToWinCountSuccessEmbed(team, numberOfWins, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
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

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.SubtractFromWinCountSuccessEmbed(team, numberOfWins, context);
                }
                else
                {
                    return _embedManager.NegativeCountErrorEmbed(team, numberOfWins, "Wins");
                }
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed AddToLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
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

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToLossCountSuccessEmbed(team, numberOfLosses, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
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

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.SubtractFromWinCountSuccessEmbed(team, numberOfLosses, context);
                }
                else
                {
                    return _embedManager.NegativeCountErrorEmbed(team, numberOfLosses, "Losses");
                }

            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
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

        private void ReassignRanksInLeague(League league)
        {
            List<Team> teamsInLeague = _teamManager.GetTeamsInLeague(league);

            // Sort teams by current rank
            teamsInLeague.Sort((a, b) => a.Rank.CompareTo(b.Rank));

            // Reassign ranks sequentially
            for (int i = 0; i <  teamsInLeague.Count; i++)
            {
                teamsInLeague[i].Rank = i + 1;
            }
        }
        #endregion

        #region Settings Logic

        public Embed SetGuildId(SocketCommandContext context)
        {
            // Grab Guild Id command was invoked from
            ulong guildId = context.Guild.Id;

            // Set in Settings using SettingsManager then save and reload Settings
            _settingsManager.Settings.GuildId = guildId;
            _settingsManager.SaveSettings();
            _settingsManager.LoadSettingsData();

            return _embedManager.SetGuildIdSuccessEmbed(guildId);
        }

        public Embed SetSuperAdminModeOnOffProcess(string onOrOff)
        {
            switch (onOrOff.Trim().ToLower())
            {
                case "on":
                    _settingsManager.SetSuperAdminModeOnOff(true);
                    _settingsManager.SaveAndReloadSettingsDatabase();
                    return _embedManager.SuperAdminModeOnEmbed();

                case "off":
                    _settingsManager.SetSuperAdminModeOnOff(false);
                    _settingsManager.SaveAndReloadSettingsDatabase();
                    return _embedManager.SuperAdminModeOffEmbed();

                default:
                    return _embedManager.SuperAdminInvalidInputEmbed();
            }
        }

        public Embed AddSuperAdminIdProcess(IUser user)
        {
            // Grab discord Id of given user to make Super Admin
            ulong newAdminId = user.Id;

            if (!_settingsManager.IsDiscordIdInSuperAdminList(newAdminId))
            { 
                _settingsManager.AddSuperAdminId(newAdminId);
                _settingsManager.SaveAndReloadSettingsDatabase();
                return _embedManager.AddSuperAdminIdSuccessEmbed(user);                
            }
            return _embedManager.AddSuperAdminIdAlreadyExistsEmbed(user);
        }

        public Embed RemoveSuperAdminIdProcess(IUser user)
        {
            // Grab discord Id of user to remove from Super Admin list
            ulong adminId = user.Id;

            if (_settingsManager.IsDiscordIdInSuperAdminList(adminId))
            {
                _settingsManager.RemoveSuperAdminId(adminId);
                _settingsManager.SaveAndReloadSettingsDatabase();
                return _embedManager.RemoveSuperAdminIdSuccessEmbed(user);
            }
            return _embedManager.RemoveSuperAdminIdNotFoundEmbed(user);
        }
        #endregion
    }
}
