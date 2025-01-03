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
            StartChallengesTask();
            StartStandingsTask();
            StartingTeamsTask();

            // Start Automated Backup Task
            StartAutomatedBackupTask();
        }
        #endregion

        #region Try-Catch Error Logic
        public Embed ExceptionErrorHandlingProcess(Exception ex, string commandName)
        {
            return _embedManager.CreateErrorEmbed(ex, commandName);
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
        public void StartChallengesTask()
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
            // Get all leagues from LeagueManager
            foreach (var league in _leagueManager.GetAllLeagues())
            {
                ulong channelId = _statesManager.GetChallengesChannelId(league);

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.LeagueName} ({league.Division}).");
                    continue;
                }

                // Get the challenges embed for the league
                List<Challenge> challenges = _challengeManager.GetChallengesForLeague(league);
                Embed challengesEmbed = _embedManager.PostChallengesEmbed(league, challenges);

                if (challengesEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No challenges to display for league: {league.LeagueName} ({league.Division}).");
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
                    Console.WriteLine($"Error updating challenges in channel {channelId} for league {league.LeagueName}: {ex.Message}");
                }
            }
        }
        #endregion

        #region --Standings
        public void StartStandingsTask()
        {
            Task.Run(() => RunStandingsUpdateTaskAsync());
        }

        private async Task RunStandingsUpdateTaskAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                await SendStandingsToChannelsAsync();
            }
        }

        private async Task SendStandingsToChannelsAsync()
        {
            // Get all leagues from LeagueManager
            foreach (var league in _leagueManager.GetAllLeagues())
            {
                ulong channelId = _statesManager.GetStandingsChannelId(league);

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.LeagueName} ({league.Division} League).");
                    continue;
                }

                // Get the standings embed for the league
                Embed standingsEmbed = _embedManager.PostStandingsEmbed(league);

                if (standingsEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No standings to display for league: {league.LeagueName} ({league.Division} League).");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Error updating standings in channel {channelId} for league {league.LeagueName}: {ex.Message}");
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
            // Get all leagues from LeagueManager
            foreach (var league in _leagueManager.GetAllLeagues())
            {
                ulong channelId = _statesManager.GetTeamsChannelId(league);

                if (channelId == 0)
                {
                    continue;
                }

                // Get the channel from the client
                IMessageChannel? channel = _client.GetChannel(channelId) as IMessageChannel;

                if (channel == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.LeagueName} ({league.Division} League).");
                    continue;
                }

                // Get the teams embed for the league
                Embed teamsEmbed = _embedManager.PostTeamsEmbed(league, league.Teams);

                if (teamsEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No teams to display for league: {league.LeagueName} ({league.Division} League).");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Error updating teams in channel {channelId} for {league.LeagueName} ({league.Division} League): {ex.Message}");
                }
            }
        }
        #endregion

        #endregion

        #region Start/End Ladder Logic
        public Embed StartLeagueLadderProcess(string leagueName)
        {
            _statesManager.LoadStatesDatabase();

            // Check if League by name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league reference
                League leagueRef = _leagueManager.GetLeagueByName(leagueName);

                // Grab associated State for League
                State state = _statesManager.GetStateByLeague(leagueRef);

                // Check ladder running status of league
                if (!_statesManager.IsLadderRunning(leagueRef))
                {
                    // Set ladder running to true
                    _statesManager.SetLadderRunning(leagueRef, true);

                    // Save and reload States database
                    _statesManager.SaveAndReloadStatesDatabase();

                    // Backup database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.StartLadderSuccessEmbed(leagueRef);
                }
                return _embedManager.StartLadderAlreadyRunningEmbed(leagueRef);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed EndLeagueLadderProcess(string leagueName)
        {
            _statesManager.LoadStatesDatabase();

            // Check if League by name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league reference
                League leagueRef = _leagueManager.GetLeagueByName(leagueName);

                // Grab associated State for League
                State state = _statesManager.GetStateByLeague(leagueRef);

                // Check ladder running status of league
                if (_statesManager.IsLadderRunning(leagueRef))
                {
                    // Set ladder running to false in League
                    _statesManager.SetLadderRunning(leagueRef, false);

                    // Save and reload the database
                    _statesManager.SaveAndReloadStatesDatabase();

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    // Return success embed
                    return _embedManager.EndLadderSuccessEmbed(leagueRef);
                }
                return _embedManager.EndLadderNotRunningEmbed(leagueRef);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
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
                    League newLeague = _leagueManager.CreateLeagueObject(leagueName, divisionType);

                    // Add new league to database
                    _leagueManager.AddNewLeague(newLeague);

                    // Save and reload Leagues Database
                    _leagueManager.SaveAndReloadLeaguesDatabase();

                    // Create new State object for League
                    State newState = _statesManager.CreateNewState(newLeague.LeagueName, newLeague.Division);

                    // Add new state
                    _statesManager.AddNewState(newState);

                    // Save and reload
                    _statesManager.SaveAndReloadStatesDatabase();

                    // Backup to Git
                    _backupManager.CopyAndBackupFilesToGit();

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
                    // Remove all challenges associated with all teams in challenges.json
                    _challengeManager.RemoveLeagueFromChallenges(leagueToRemove.Division, leagueToRemove.LeagueName);

                    // Remove League from leagues.json
                    _leagueManager.RemoveLeague(leagueToRemove.LeagueName, leagueToRemove.Division);

                    // Save and reload
                    _leagueManager.SaveAndReloadLeaguesDatabase();

                    // Remove the State associated with league
                    _statesManager.RemoveLeagueState(leagueToRemove.LeagueName, leagueToRemove.Division);

                    // Save and reload
                    _statesManager.SaveAndReloadStatesDatabase();

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
            // Load latest save
            _leagueManager.LoadLeaguesDatabase();

            // Check if League by given name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab reference of league
                League leagueReference = _leagueManager.GetLeagueByName(leagueName);

                if (_leagueManager.IsTeamNameUnique(teamName))
                {
                    // Convert User Context Info into Member objects
                    List<Member> newMemberList = _memberManager.ConvertMembersListToObjects(members);

                    if (_memberManager.IsMemberCountCorrect(newMemberList, leagueReference.Division) || _settingsManager.IsUserSuperAdmin(context.User.Id))
                    {
                        // Check if any member is already on a team in the given league
                        foreach (Member member in newMemberList)
                        {
                            if (_memberManager.IsMemberOnTeamInLeague(member, leagueReference.Teams) && !_settingsManager.IsUserSuperAdmin(context.User.Id))
                            {
                                return _embedManager.RegisterTeamErrorEmbed($"{member.DisplayName} is already on a team in the {leagueReference.LeagueName} League.");
                            }
                        }

                        // Create team object
                        Team newTeam = _teamManager.CreateTeamObject(teamName, leagueReference.LeagueName, leagueReference.Division, _teamManager.GetTeamCountInLeague(leagueReference) + 1, newMemberList);

                        // Add team to league
                        _leagueManager.AddNewTeamToLeague(newTeam, leagueReference);

                        // Save and reload Leagues Database
                        _leagueManager.SaveAndReloadLeaguesDatabase();

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.RegisterTeamToLeagueSuccessEmbed(newTeam, leagueReference);
                    }
                    return _embedManager.RegisterTeamErrorEmbed($"Incorrect amount of members given for specified division type: Division - {leagueReference.Division} | Member Count - {newMemberList.Count}.");
                }
                return _embedManager.RegisterTeamErrorEmbed($"The given team name is already being used by another team: {teamName}.");
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed RemoveTeamFromLeagueProcess(string teamName)
        {
            // Load latest save
            _leagueManager.LoadLeaguesDatabase();

            // Check if Team exists in any League
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab Team Object
                Team? teamToRemove = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Grab League object
                League correctLeague = _leagueManager.GetLeagueFromTeamName(teamToRemove.TeamName);

                // Remove all Challenges from Database associated with team
                _challengeManager.SudoRemoveChallenge(correctLeague.Division, correctLeague.LeagueName, teamName);

                //Remove the team correctly and correct ranks
                _leagueManager.RemoveTeamFromLeague(teamToRemove, correctLeague);

                ReassignRanksInLeague(correctLeague);

                // Save and reload
                _leagueManager.SaveAndReloadLeaguesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                // Return success embed
                return _embedManager.RemoveTeamSuccessEmbed(teamToRemove, correctLeague);
            }

            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }
        #endregion

        #region Challenge Based Logic
        public Embed ChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load the latest save of the Challenges and Leagues database
            _leagueManager.LoadLeaguesDatabase();
            _challengeManager.LoadChallengesDatabase();

            // Check if both teams exist in the database
            if (!_leagueManager.IsTeamNameUnique(challengerTeam) && !_leagueManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab correct league
                League correctLeague = _leagueManager.GetLeagueFromTeamName(challengerTeam);

                // Check if ladder is started in League
                if (!_statesManager.IsLadderRunning(correctLeague))
                {
                    return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in **{correctLeague.LeagueName}** ({correctLeague.Division} League). Challenges may not be initiated yet.");
                }

                // Grab team objects
                Team? objectChallengerTeam = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                Team? objectChallengedTeam = _leagueManager.GetTeamByNameFromLeagues(challengedTeam);

                // Grab Discord ID of the user who invoked the command
                ulong discordId = context.User.Id;

                // Check if the user is on the challenger team
                if (_memberManager.IsDiscordIdOnGivenTeam(discordId, objectChallengerTeam))
                {
                    // Check if teams are in the same League
                    if (_leagueManager.IsTeamsInSameLeague(correctLeague, objectChallengerTeam, objectChallengedTeam))
                    {
                        // Ensure the ranks are within range
                        if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                        {
                            // Check if the challenger has no pending challenges
                            if (!_challengeManager.IsTeamInChallenge(correctLeague.Division, correctLeague.LeagueName, objectChallengerTeam))
                            {
                                // Check if the challenged has no pending challenges
                                if (!_challengeManager.IsTeamInChallenge(correctLeague.Division, correctLeague.LeagueName, objectChallengedTeam))
                                {
                                    // Create and save new Challenge
                                    _challengeManager.AddNewChallenge(correctLeague.Division, correctLeague.LeagueName, new Challenge(correctLeague.Division, objectChallengerTeam.TeamName, objectChallengerTeam.Rank, objectChallengedTeam.TeamName, objectChallengedTeam.Rank));

                                    // Change IsChallengeable of both teams to false
                                    _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                    _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                    _leagueManager.SaveAndReloadLeaguesDatabase();

                                    // Save and reload database
                                    _challengeManager.SaveChallengesDatabase();
                                    _challengeManager.LoadChallengesDatabase();

                                    // Backup to git
                                    _backupManager.CopyAndBackupFilesToGit();

                                    // Grab newly created Challenge object
                                    Challenge? newChallenge = _challengeManager.GetChallengeForTeam(correctLeague.Division, correctLeague.LeagueName, objectChallengerTeam);

                                    // Notify the challenged team
                                    foreach (Member member in objectChallengedTeam.Members)
                                    {
                                        _challengeManager.SendChallengeNotification(member.DiscordId, newChallenge, correctLeague);
                                    }

                                    return _embedManager.ChallengeSuccessEmbed(objectChallengerTeam, objectChallengedTeam, newChallenge);
                                }
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.TeamName} is already awaiting a challenge match. Please try again later.");
                            }
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName} is already awaiting a challenge match. Please try again later.");
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
                    return _embedManager.ChallengeErrorEmbed($"The teams are not in the same League. Challenger team's League: {objectChallengerTeam.League}, Challenged team's League: {objectChallengedTeam.League}. Please try again.");
                }
                return _embedManager.ChallengeErrorEmbed($"You are not a member of Team {objectChallengerTeam.TeamName}. Please try again.");
            }
            return _embedManager.ChallengeErrorEmbed($"One or both team names were not found in the database. Please try again.");
        }

        public Embed CancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if team exists
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team reference
                Team challengerTeamObject = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);

                // Grab league reference
                League correctLeague = _leagueManager.GetLeagueFromTeamName(challengerTeamObject.TeamName);

                // Check if ladder is running in given league
                if (!_statesManager.IsLadderRunning(correctLeague))
                {
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in the {correctLeague.LeagueName} League so there are no challenges to cancel yet.");
                }

                // Check if invoker is part of challenger team
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, challengerTeamObject))
                {
                    // Check if Team has a challenge actually sent out
                    if (_challengeManager.IsTeamChallenger(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject))
                    {
                        // Grab challenge object
                        Challenge? challenge = _challengeManager.GetChallengeForTeam(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject);

                        // Grab challenged team object
                        Team challengedTeamObject = _leagueManager.GetTeamByNameFromLeagues(challenge.Challenged);

                        // Set IsChallengeable for both teams back to true
                        _teamManager.ChangeChallengeStatus(challengerTeamObject, true);
                        _teamManager.ChangeChallengeStatus(challengedTeamObject, true);

                        // Save and reload leagues and its teams
                        _leagueManager.SaveAndReloadLeaguesDatabase();

                        // Cancel the challenge
                        _challengeManager.SudoRemoveChallenge(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject.TeamName);

                        // Save and reload Challenges
                        _challengeManager.SaveChallengesDatabase();
                        _challengeManager.LoadChallengesDatabase();

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.CancelChallengeSuccessEmbed(challengerTeamObject);
                    }
                    return _embedManager.CancelChallengeErrorEmbed($"Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.");
                }
                return _embedManager.CancelChallengeErrorEmbed($"You are not a member of Team **{challengerTeamObject.TeamName}**\nThat team's member(s) consists of: {challengerTeamObject.GetAllMemberNamesToStr()}");
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }

        public Embed AdminChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load the latest save of the Challenges and Leagues database
            _leagueManager.LoadLeaguesDatabase();
            _challengeManager.LoadChallengesDatabase();

            // Check if both teams exist in the database
            if (!_leagueManager.IsTeamNameUnique(challengerTeam) && !_leagueManager.IsTeamNameUnique(challengedTeam))
            {
                // Grab correct league
                League correctLeague = _leagueManager.GetLeagueFromTeamName(challengerTeam);

                // Check if ladder is started in League
                if (!_statesManager.IsLadderRunning(correctLeague))
                {
                    return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in **{correctLeague.LeagueName}** ({correctLeague.Division} League). Challenges may not be initiated yet.");
                }

                // Grab team objects
                Team? objectChallengerTeam = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                Team? objectChallengedTeam = _leagueManager.GetTeamByNameFromLeagues(challengedTeam);

                // Check if teams are in the same League
                if (_leagueManager.IsTeamsInSameLeague(correctLeague, objectChallengerTeam, objectChallengedTeam))
                {
                    // Ensure the ranks are within range
                    if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                    {
                        // Check if the challenger has no pending challenges
                        if (!_challengeManager.IsTeamInChallenge(correctLeague.Division, correctLeague.LeagueName, objectChallengerTeam))
                        {
                            // Check if the challenged has no pending challenges
                            if (!_challengeManager.IsTeamInChallenge(correctLeague.Division, correctLeague.LeagueName, objectChallengedTeam))
                            {
                                // Create and save new Challenge
                                _challengeManager.AddNewChallenge(correctLeague.Division, correctLeague.LeagueName, new Challenge(correctLeague.Division, objectChallengerTeam.TeamName, objectChallengerTeam.Rank, objectChallengedTeam.TeamName, objectChallengedTeam.Rank));

                                // Change IsChallengeable of both teams to false
                                _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                _leagueManager.SaveAndReloadLeaguesDatabase();

                                // Save and reload database
                                _challengeManager.SaveChallengesDatabase();
                                _challengeManager.LoadChallengesDatabase();

                                // Backup to git
                                _backupManager.CopyAndBackupFilesToGit();

                                // Grab newly created Challenge object
                                Challenge? newChallenge = _challengeManager.GetChallengeForTeam(correctLeague.Division, correctLeague.LeagueName, objectChallengerTeam);

                                // Notify the challenged team
                                foreach (Member member in objectChallengedTeam.Members)
                                {
                                    _challengeManager.SendChallengeNotification(member.DiscordId, newChallenge, correctLeague);
                                }

                                return _embedManager.AdminChallengeSuccessEmbed(context, objectChallengerTeam, objectChallengedTeam);
                            }
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.TeamName} is already awaiting a challenge match. Please try again later.");
                        }
                        return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName} is already awaiting a challenge match. Please try again later.");
                    }
                    else
                    {
                        if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                        {
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank ({objectChallengerTeam.Rank}) is higher than {objectChallengedTeam.TeamName}'s rank ({objectChallengedTeam.Rank}). A challenge cannot be initiated.");
                        }
                        else
                        {
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.TeamName}'s rank ({objectChallengerTeam.Rank}) is not within the allowed range to challenge {objectChallengedTeam.TeamName}'s rank ({objectChallengedTeam.Rank}). Challenges can only be made for teams within two ranks above.");
                        }
                    }
                }
                return _embedManager.ChallengeErrorEmbed($"The teams are not in the same League. Challenger team's League: {objectChallengerTeam.League}, Challenged team's League: {objectChallengedTeam.League}. Please try again.");
            }
            return _embedManager.ChallengeErrorEmbed($"One or both team names were not found in the database. Please try again.");
        }

        public Embed AdminCancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Load latest save of Challenges database
            _challengeManager.LoadChallengesDatabase();

            // Check if team exists
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team reference
                Team challengerTeamObject = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);

                // Grab league reference
                League correctLeague = _leagueManager.GetLeagueFromTeamName(challengerTeamObject.TeamName);

                // Check if ladder is running in given league
                if (!_statesManager.IsLadderRunning(correctLeague))
                {
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in the {correctLeague.LeagueName} League so there are no challenges to cancel yet.");
                }

                // Check if Team has a challenge actually sent out
                if (_challengeManager.IsTeamChallenger(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject))
                {
                    // Grab challenge object
                    Challenge? challenge = _challengeManager.GetChallengeForTeam(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject);

                    // Grab challenged team object
                    Team challengedTeamObject = _leagueManager.GetTeamByNameFromLeagues(challenge.Challenged);

                    // Set IsChallengeable for both teams back to true
                    _teamManager.ChangeChallengeStatus(challengerTeamObject, true);
                    _teamManager.ChangeChallengeStatus(challengedTeamObject, true);

                    // Save and reload leagues and its teams
                    _leagueManager.SaveAndReloadLeaguesDatabase();

                    // Cancel the challenge
                    _challengeManager.SudoRemoveChallenge(correctLeague.Division, correctLeague.LeagueName, challengerTeamObject.TeamName);

                    // Save and reload Challenges
                    _challengeManager.SaveChallengesDatabase();
                    _challengeManager.LoadChallengesDatabase();

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.AdminCancelChallengeSuccessEmbed(context, challengerTeamObject);
                }
                return _embedManager.CancelChallengeErrorEmbed($"Team {challengerTeamObject.TeamName} does not have any pending challenges sent out to cancel.");
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }
        #endregion

        #region Reporting Logic
        public Embed ReportWinProcess(SocketInteractionContext context, string winningTeamName)
        {
            // Check if given team name exists
            if (!_leagueManager.IsTeamNameUnique(winningTeamName))
            {
                // Grab league object
                League league = _leagueManager.GetLeagueFromTeamName(winningTeamName);

                // Check if ladder is running
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in the {league.LeagueName} League so there are no matches to report on yet.");
                }

                // Grab winningTeam object, add placeholder for losingTeam object
                Team? winningTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(winningTeamName, StringComparison.OrdinalIgnoreCase));
                Team? losingTeam;

                // Is invoker on the winningTeam
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, winningTeam))
                {
                    // Is the team part of an active challenge (Challenger or Challenged)
                    if (_challengeManager.IsTeamInChallenge(league.Division, league.LeagueName, winningTeam))
                    {
                        // Grab challenge object for reference
                        Challenge? challenge = _challengeManager.GetChallengeForTeam(league.Division, league.LeagueName, winningTeam);

                        // Determine if winningTeam is Challenger or Challenged
                        bool isWinningTeamChallenger;
                        if (challenge.Challenger.Equals(winningTeam.TeamName, StringComparison.OrdinalIgnoreCase))
                        {
                            isWinningTeamChallenger = true;
                            losingTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(challenge.Challenged, StringComparison.OrdinalIgnoreCase)); ;
                        }
                        else
                        {
                            isWinningTeamChallenger = false;
                            losingTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(challenge.Challenger, StringComparison.OrdinalIgnoreCase));
                        }

                        // If winningTeam is challenger, rank change will occur
                        if (isWinningTeamChallenger)
                        {
                            // The winning team takes the rank of the losing team
                            winningTeam.Rank = losingTeam.Rank;
                            losingTeam.Rank++;

                            // Reassign ranks for the entire League
                            ReassignRanksInLeague(league);

                            // Add wins and losses correctly
                            _teamManager.AddToWins(winningTeam, 1);
                            _teamManager.AddToLosses(losingTeam, 1);

                            // TODO - Add wins and losses to Member Profiles


                            // TODO - Create Match object to add to History (Past Matches)


                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _leagueManager.SaveAndReloadLeaguesDatabase();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Division, league.LeagueName, challenge.Challenger);

                            // Save Challenges database
                            _challengeManager.SaveChallengesDatabase();
                            _challengeManager.LoadChallengesDatabase();

                            // Backup to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with true, showing rank change
                            return _embedManager.ReportWinSuccessEmbed(winningTeam, losingTeam, true, league);
                        }

                        // If winningTeam is challenged team, no rank change will occur
                        else
                        {
                            // Assign win and loss correctly
                            _teamManager.AddToWins(winningTeam, 1);
                            _teamManager.AddToLosses(losingTeam, 1);

                            // TODO Add wins and losses to Member Profiles


                            // TODO: Create Match object to add to History (Past Matches)


                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _leagueManager.SaveAndReloadLeaguesDatabase();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Division, league.LeagueName, challenge.Challenger);

                            // Backup the database to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with false, showing no rank change
                            return _embedManager.ReportWinSuccessEmbed(winningTeam, losingTeam, false, league);
                        }
                    }
                    return _embedManager.ReportWinErrorEmbed($"Team {winningTeam.TeamName} is not currently waiting on a challenge match.");
                }
                return _embedManager.ReportWinErrorEmbed($"You are not part of Team **{winningTeam.TeamName}**\nThat team's member(s) consists of: {winningTeam.GetAllMemberNamesToStr()}.");
            }
            return _embedManager.TeamNotFoundErrorEmbed(winningTeamName);
        }

        public Embed ReportWinAdminProcess(SocketInteractionContext context, string winningTeamName)
        {
            // Check if given team name exists
            if (!_leagueManager.IsTeamNameUnique(winningTeamName))
            {
                // Grab league object
                League league = _leagueManager.GetLeagueFromTeamName(winningTeamName);

                // Check if ladder is running
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in the {league.LeagueName} League so there are no matches to report on yet.");
                }

                // Grab winningTeam object, add placeholder for losingTeam object
                Team? winningTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(winningTeamName, StringComparison.OrdinalIgnoreCase));
                Team? losingTeam;


                // Is the team part of an active challenge (Challenger or Challenged)
                if (_challengeManager.IsTeamInChallenge(league.Division, league.LeagueName, winningTeam))
                {
                    // Grab challenge object for reference
                    Challenge? challenge = _challengeManager.GetChallengeForTeam(league.Division, league.LeagueName, winningTeam);

                    // Determine if winningTeam is Challenger or Challenged
                    bool isWinningTeamChallenger;
                    if (challenge.Challenger.Equals(winningTeam.TeamName, StringComparison.OrdinalIgnoreCase))
                    {
                        isWinningTeamChallenger = true;
                        losingTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(challenge.Challenged, StringComparison.OrdinalIgnoreCase)); ;
                    }
                    else
                    {
                        isWinningTeamChallenger = false;
                        losingTeam = league.Teams.FirstOrDefault(t => t.TeamName.Equals(challenge.Challenger, StringComparison.OrdinalIgnoreCase));
                    }

                    // If winningTeam is challenger, rank change will occur
                    if (isWinningTeamChallenger)
                    {
                        // The winning team takes the rank of the losing team
                        winningTeam.Rank = losingTeam.Rank;
                        losingTeam.Rank++;

                        // Reassign ranks for the entire League
                        ReassignRanksInLeague(league);

                        // Add wins and losses correctly
                        _teamManager.AddToWins(winningTeam, 1);
                        _teamManager.AddToLosses(losingTeam, 1);

                        // TODO - Add wins and losses to Member Profiles


                        // TODO - Create Match object to add to History (Past Matches)


                        // Set IsChallengeable status of both teams back to true
                        _teamManager.ChangeChallengeStatus(winningTeam, true);
                        _teamManager.ChangeChallengeStatus(losingTeam, true);
                        _leagueManager.SaveAndReloadLeaguesDatabase();

                        // Remove the challenge
                        _challengeManager.SudoRemoveChallenge(league.Division, league.LeagueName, challenge.Challenger);

                        // Save Challenges database
                        _challengeManager.SaveChallengesDatabase();
                        _challengeManager.LoadChallengesDatabase();

                        // Backup to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        // Return Success Embed with true, showing rank change
                        return _embedManager.ReportWinAdminSuccessEmbed(context, winningTeam, losingTeam, true, league);
                    }

                    // If winningTeam is challenged team, no rank change will occur
                    else
                    {
                        // Assign win and loss correctly
                        _teamManager.AddToWins(winningTeam, 1);
                        _teamManager.AddToLosses(losingTeam, 1);

                        // TODO Add wins and losses to Member Profiles


                        // TODO: Create Match object to add to History (Past Matches)


                        // Set IsChallengeable status of both teams back to true
                        _teamManager.ChangeChallengeStatus(winningTeam, true);
                        _teamManager.ChangeChallengeStatus(losingTeam, true);
                        _leagueManager.SaveAndReloadLeaguesDatabase();

                        // Remove the challenge
                        _challengeManager.SudoRemoveChallenge(league.Division, league.LeagueName, challenge.Challenger);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        // Return Success Embed with false, showing no rank change
                        return _embedManager.ReportWinAdminSuccessEmbed(context, winningTeam, losingTeam, false, league);
                    }
                }
                return _embedManager.ReportWinErrorEmbed($"Team {winningTeam.TeamName} is not currently waiting on a challenge match.");
            }
            return _embedManager.TeamNotFoundErrorEmbed(winningTeamName);
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
        public Embed PostChallengesProcess(SocketInteractionContext context, string leagueName)
        {
            _challengeManager.LoadChallengesDatabase();

            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league reference
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Grab List of challenges for given League
                List<Challenge> challenges = _challengeManager.GetChallengesForLeague(league);

                return _embedManager.PostChallengesEmbed(league, challenges);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed PostLeaguesProcess(SocketInteractionContext context, string divisionType)
        {
            return null;
        }

        public Embed PostStandingsProcess(SocketInteractionContext context, string leagueName)
        {
            _leagueManager.LoadLeaguesDatabase();

            // Check if League exists by given name
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league reference
                League league = _leagueManager.GetLeagueByName(leagueName);

                return _embedManager.PostStandingsEmbed(league);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed PostTeamsProcess(SocketInteractionContext context, string leagueName)
        {
            if (_leagueManager.IsTeamNameUnique(leagueName))
            {
                // Grab league reference
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Grab list of teams from league
                List<Team>? teams = _teamManager.GetTeamsInLeague(league);

                return _embedManager.PostTeamsEmbed(league, teams);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        #endregion

        #region Set Rank Logic        
        public Embed SetRankProcess(SocketInteractionContext context, string teamName, int newRank)
        {
            // Check if team exists
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Get league object
                League league = _leagueManager.GetLeagueFromTeamName(teamName);

                // Get team object
                Team? teamToAdjust = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Get current rank of team
                int currentRank = teamToAdjust.Rank;

                // Get all teams in league
                List<Team>? teamsInLeague = _teamManager.GetTeamsInLeague(league);

                if (newRank == currentRank)
                {
                    return _embedManager.SetRankErrorEmbed($"Team {teamName} is already at rank {newRank}. No changes were made.");
                }

                if (newRank > teamsInLeague.Count)
                {
                    return _embedManager.SetRankErrorEmbed($"You can not enter a rank greater than the number of teams in a league.\nTeam Count: {teamsInLeague.Count}");
                }

                // Moving the team up in rank
                if (newRank < currentRank)
                {
                    for (int i = 0; i < teamsInLeague.Count; i++)
                    {
                        if (teamsInLeague[i].Rank >= newRank && teamsInLeague[i].Rank < currentRank && teamsInLeague[i].TeamName != teamToAdjust.TeamName)
                        {
                            teamsInLeague[i].Rank++;
                        }
                    }
                }
                // Moving the team down in rank
                else if (newRank > currentRank)
                {
                    for (int i = 0; i < teamsInLeague.Count; i++)
                    {
                        if (teamsInLeague[i].Rank <= newRank && teamsInLeague[i].Rank > currentRank && teamsInLeague[i].TeamName != teamToAdjust.TeamName)
                        {
                            teamsInLeague[i].Rank--;
                        }
                    }
                }
                // Set the new rank for the team
                teamToAdjust.Rank = newRank;

                // Reassign ranks to ensure consistency
                ReassignRanksInLeague(league);

                // Save and reload database
                _leagueManager.SaveAndReloadLeaguesDatabase();

                // Backup to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.SetRankSuccessEmbed(teamToAdjust, league);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }
        #endregion

        #region Set Standings/Challenges/Teams Channel Logic
        public Embed SetChallengesChannelIdProcess(string leagueName, IMessageChannel channel)
        {
            // Check if league exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league object
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Set channel Id
                _statesManager.SetChallengesChannelId(league, channel.Id);

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.SetChannelIdSuccessEmbed(league, channel, "Challenges");
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed SetStandingsChannelIdProcess(string leagueName, IMessageChannel channel)
        {
            // Check if league exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league object
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Set channel Id
                _statesManager.SetStandingsChannelId(league, channel.Id);

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.SetChannelIdSuccessEmbed(league, channel, "Standings");
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed SetTeamsChannelIdProcess(string leagueName, IMessageChannel channel)
        {
            // Check if league exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league object
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Set channel Id
                _statesManager.SetTeamsChannelId(league, channel.Id);

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.SetChannelIdSuccessEmbed(league, channel, "Teams");
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }
        #endregion

        #region Add/Subtract Win/Loss Logic

        // For Admin command use, ReportWin logic uses directly to TeamManager
        public Embed AddToWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Check if team name exists in database
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Add the numberOfWins to the team
                _teamManager.AddToWins(team, numberOfWins);

                // Save and reload teams database
                _leagueManager.SaveAndReloadLeaguesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToWinCountSuccessEmbed(team, numberOfWins, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Check if team name exists in database
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Make sure the result will not be negative
                int result = team.Wins - numberOfWins;
                if (result >= 0)
                {
                    // Safely subtract from wins
                    _teamManager.SubtractFromWins(team, numberOfWins);

                    // Save and reload teams database
                    _leagueManager.SaveAndReloadLeaguesDatabase();

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
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Add the numberOfLosses to the team
                _teamManager.AddToLosses(team, numberOfLosses);

                // Save and reload teams database
                _leagueManager.SaveAndReloadLeaguesDatabase();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToLossCountSuccessEmbed(team, numberOfLosses, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
        {
            // Check if team name exists in database
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Make sure the result will not be negative
                int result = team.Losses - numberOfLosses;
                if (result >= 0)
                {
                    // Safely subtract from losses
                    _teamManager.SubtractFromLosses(team, numberOfLosses);

                    // Save and reload teams database
                    _leagueManager.SaveAndReloadLeaguesDatabase();

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
        private void ReassignRanksInLeague(League league)
        {
            // Ensure the league is valid
            if (league == null || league.Teams == null || !league.Teams.Any())
            {
                Console.WriteLine("No teams to reassign ranks.");
                return;
            }

            // Sort teams in the league by their current rank
            league.Teams.Sort((a, b) => a.Rank.CompareTo(b.Rank));

            // Reassign ranks sequentially
            for (int i = 0; i < league.Teams.Count; i++)
            {
                league.Teams[i].Rank = i + 1;
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

        #region Testing Methods

        #endregion
    }
}
