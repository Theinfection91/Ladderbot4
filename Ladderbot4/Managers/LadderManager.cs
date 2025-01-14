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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.Name} ({league.Format}).");
                    continue;
                }

                // Get the challenges embed for the league
                List<Challenge> challenges = _challengeManager.GetChallengesForLeague(league);
                Embed challengesEmbed = _embedManager.PostChallengesEmbed(league, challenges);

                if (challengesEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No challenges to display for league: {league.Name} ({league.Format}).");
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
                    Console.WriteLine($"Error updating challenges in channel {channelId} for league {league.Name}: {ex.Message}");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.Name} ({league.Format} League).");
                    continue;
                }

                // Get the standings embed for the league
                Embed standingsEmbed = _embedManager.PostStandingsEmbed(league);

                if (standingsEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No standings to display for league: {league.Name} ({league.Format} League).");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Error updating standings in channel {channelId} for league {league.Name}: {ex.Message}");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Channel not found for ID {channelId} in league: {league.Name} ({league.Format} League).");
                    continue;
                }

                // Get the teams embed for the league
                Embed teamsEmbed = _embedManager.PostTeamsEmbed(league, league.Teams);

                if (teamsEmbed == null)
                {
                    Console.WriteLine($"{DateTime.Now} LadderManager - No teams to display for league: {league.Name} ({league.Format} League).");
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
                    Console.WriteLine($"{DateTime.Now} LadderManager - Error updating teams in channel {channelId} for {league.Name} ({league.Format} League): {ex.Message}");
                }
            }
        }
        #endregion

        #endregion

        #region Start/End Ladder Logic
        public Embed StartLeagueLadderProcess(string leagueName)
        {
            // Load states
            _statesManager.LoadStatesAtlas();

            // Check if League exists by name
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Grab state associated with league
                State state = _statesManager.GetStateByLeague(league);

                // Check ladder status
                if (!_statesManager.IsLadderRunning(league))
                {
                    // Ensure every team in league is at 0 wins/losses
                    league.ResetTeamsToZero();

                    // Save and reload leagues
                    _leagueManager.SaveAndReloadLeagueRegistry();

                    // Set ladder running to true
                    _statesManager.SetLadderRunning(league, true);

                    // Backup database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.StartLadderSuccessEmbed(league);
                }
                return _embedManager.StartLadderAlreadyRunningEmbed(league);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed EndLeagueLadderProcess(string leagueName)
        {
            // Load states
            _statesManager.LoadStatesAtlas();

            // Check if League exists by name
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Grab state associated with league
                State state = _statesManager.GetStateByLeague(league);

                // Check ladder status
                if (_statesManager.IsLadderRunning(league))
                {
                    // Set ladder running to false
                    _statesManager.SetLadderRunning(league, false);

                    // Remove league index from challenges entirely
                    _challengeManager.RemoveLeagueFromChallenges(league.Name);

                    // Backup database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    // Return embed with ladder results
                    return _embedManager.EndLadderSuccessEmbed(league);
                }
                return _embedManager.EndLadderNotRunningEmbed(league);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }
        #endregion

        #region Create/Delete League Logic
        public Embed CreateLeagueProcess(string leagueName, int teamSize)
        {
            // Check if desired XvX League name is taken
            if (_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Generate division tag based on team size
                string divisionTag = _leagueManager.ConvertTeamSizeToDivisionTag(teamSize);

                // Create new XvX League Object
                League newLeague = _leagueManager.CreateLeagueObject(leagueName, divisionTag, teamSize);

                // Add new league to LeagueRegistry
                _leagueManager.AddNewLeague(newLeague);

                // Create and add State for new XvX League
                _statesManager.AddNewState(_statesManager.CreateNewState(newLeague.Name, newLeague.Format));

                // Backup to Git
                _backupManager.CopyAndBackupFilesToGit();

                // Return embed
                return _embedManager.CreateLeagueSuccessEmbed(newLeague);
            }
            return _embedManager.CreateLeagueErrorEmbed($"'{leagueName.Trim()}' already exists as a League in the database.");
        }

        public Embed DeleteLeagueProcess(string leagueName)
        {
            // Load latest save
            _leagueManager.LoadLeagueRegistry();

            // Check if League by given name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league object as reference for correct league info for embed
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Remove all challenges associated with league
                _challengeManager.RemoveLeagueFromChallenges(league.Name);

                // Remove the state associated with the league
                _statesManager.RemoveState(_statesManager.GetStateByLeague(league));

                // Delete League from database
                _leagueManager.DeleteLeague(league.Name);

                // Backup to Git
                _backupManager.CopyAndBackupFilesToGit();

                // Return embed
                return _embedManager.DeleteLeagueSuccessEmbed(league);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }
        #endregion

        #region Register/Remove Team Logic
        public Embed RegisterTeamToLeagueProcess(SocketInteractionContext context, string teamName, string leagueName, List<IUser> members)
        {
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

            // Check if league by given name exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab reference of League
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Check if given team name is unique across every league
                if (_leagueManager.IsTeamNameUnique(teamName))
                {
                    // Convert IUser list to Members list
                    List<Member> membersList = _memberManager.ConvertMembersListToObjects(members);

                    // Check if member count matches team size
                    // !! Teams that require 21 or more members will need to add remaining members using soon to be implemented commands !!
                    if (_memberManager.IsMemberCountCorrect(membersList, league.TeamSize) || _settingsManager.IsUserSuperAdmin(context.User.Id))
                    {
                        // Check if any member is already on a team in the given league
                        foreach (Member member in membersList)
                        {
                            if (_memberManager.IsMemberOnTeamInLeague(member, league.Teams) && !_settingsManager.IsUserSuperAdmin(context.User.Id))
                            {
                                return _embedManager.RegisterTeamErrorEmbed($"{member.DisplayName} is already on a team in the {league.Name} League.");
                            }
                        }

                        // Create team object
                        Team team = _teamManager.CreateTeamObject(teamName, league.Name, league.TeamSize, league.Format, _teamManager.GetTeamCountInLeague(league) + 1, membersList);

                        // Add team to league
                        _leagueManager.AddTeamToLeague(team, league);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.RegisterTeamToLeagueSuccessEmbed(team, league);
                    }
                    return _embedManager.RegisterTeamErrorEmbed($"Incorrect amount of members given for league format: Format - {league.Format} | Member Count - {membersList.Count}.\n\nTeams that require 21 or more members will need to add remaining members using soon to be implemented commands in `/team add`.");
                }
                return _embedManager.RegisterTeamErrorEmbed($"The given team name is already being used by another team: {teamName}.");
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed RemoveTeamFromLeagueProcess(string teamName)
        {
            // Load latest save
            _leagueManager.LoadLeagueRegistry();

            // Check if Team exists in any League
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab Team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Grab League object
                League league = _leagueManager.GetLeagueByName(team.League);

                // Remove all Challenges associated with Team
                _challengeManager.SudoRemoveChallenge(league.Name, team.Name);

                // Remove team
                _leagueManager.RemoveTeamFromLeague(team, league);

                ReassignRanksInLeague(league);

                // Save and reload
                _leagueManager.SaveAndReloadLeagueRegistry();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                // Return success embed
                return _embedManager.RemoveTeamSuccessEmbed(team, league);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }
        #endregion

        #region Challenge Based Logic
        public Embed SendChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load Challenges and Leagues
            _leagueManager.LoadLeagueRegistry();
            _challengeManager.LoadChallengesHub();

            // Check if each team exists in the database
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                if (!_leagueManager.IsTeamNameUnique(challengedTeam))
                {
                    // Grab team objects
                    Team? objectChallengerTeam = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                    Team? objectChallengedTeam = _leagueManager.GetTeamByNameFromLeagues(challengedTeam);

                    // Grab league
                    League league = _leagueManager.GetLeagueByName(objectChallengerTeam.League);

                    // Check if ladder is running in League
                    if (!_statesManager.IsLadderRunning(league))
                    {
                        return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in **{league.Name}** ({league.Format} League). Challenges may not be initiated yet.");
                    }

                    // Grab Discord ID of user who invoked command
                    ulong discordId = context.User.Id;

                    // Check if user is on the challenger team
                    if (_memberManager.IsDiscordIdOnGivenTeam(discordId, objectChallengerTeam))
                    {
                        // Check if teams are in the same League
                        if (_leagueManager.IsTeamsInSameLeague(league, objectChallengerTeam, objectChallengedTeam))
                        {
                            // Ensure the ranks are within range
                            if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                            {
                                // Check if Challenger team has any pending challenges
                                if (!_challengeManager.IsTeamInChallenge(league.Name, objectChallengerTeam))
                                {
                                    // Check if the challenged has any pending challenges
                                    if (!_challengeManager.IsTeamInChallenge(league.Name, objectChallengedTeam))
                                    {
                                        // Create and save new Challenge
                                        Challenge challenge = new(objectChallengerTeam.Name, objectChallengerTeam.Rank, objectChallengedTeam.Name, objectChallengedTeam.Rank);
                                        _challengeManager.AddNewChallenge(league.Name, challenge);

                                        // Change IsChallengeable of both teams to false and save league
                                        _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                        _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                        _leagueManager.SaveAndReloadLeagueRegistry();

                                        // Backup to Git
                                        _backupManager.CopyAndBackupFilesToGit();

                                        // Send challenge message
                                        foreach (Member member in objectChallengedTeam.Members)
                                        {
                                            _challengeManager.SendChallengeNotification(member.DiscordId, challenge, league);
                                        }

                                        return _embedManager.ChallengeSuccessEmbed(objectChallengerTeam, objectChallengedTeam, challenge);
                                    }
                                    return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.Name} is already awaiting a challenge match.");
                                }
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name} is already awaiting a challenge match.");
                            }
                            else
                            {
                                if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                                {
                                    return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name}'s rank ({objectChallengerTeam.Rank}) is higher than {objectChallengedTeam.Name}'s rank ({objectChallengedTeam.Rank}). A challenge cannot be initiated.");
                                }
                                else
                                {
                                    return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name}'s rank ({objectChallengerTeam.Rank}) is not within the allowed range to challenge {objectChallengedTeam.Name}'s rank ({objectChallengedTeam.Rank}). Challenges can only be made for teams within two ranks above.");
                                }
                            }
                        }
                        return _embedManager.ChallengeErrorEmbed($"The teams are not in the same League. Challenger team's League: {objectChallengerTeam.League}, Challenged team's League: {objectChallengedTeam.League}. Please try again.");
                    }
                    return _embedManager.ChallengeErrorEmbed($"You are not a member of Team {objectChallengerTeam.Name}.");
                }
                return _embedManager.TeamNotFoundErrorEmbed(challengedTeam);
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }

        public Embed CancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Load latest Challenges database save
            _challengeManager.LoadChallengesHub();

            // Check if team exists
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team and league objects
                Team? team = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                League league = _leagueManager.GetLeagueByName(team.League);

                // Check if ladder is running in league
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in {league.Name} ({league.Format} League) so there are no challenges to cancel yet.");
                }

                // Check if invoker is part of challenging team
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, team))
                {
                    // Check if team has a challenge sent out to actually cancel
                    if (_challengeManager.IsTeamChallenger(team.League, team))
                    {
                        // Grab Challenge object
                        Challenge? challenge = _challengeManager.GetChallengeForTeam(team.League, team);

                        // Grab challenger Team object
                        Team? otherTeam = _leagueManager.GetTeamByNameFromLeagues(challenge.Challenged);

                        // Set IsChallengeable for both teams back to true
                        _teamManager.ChangeChallengeStatus(team, true);
                        _teamManager.ChangeChallengeStatus(otherTeam, true);

                        // Save leagues
                        _leagueManager.SaveAndReloadLeagueRegistry();

                        // Cancel the challenge
                        _challengeManager.SudoRemoveChallenge(team.League, team.Name);

                        // Backup the database to Git
                        _backupManager.CopyAndBackupFilesToGit();

                        return _embedManager.CancelChallengeSuccessEmbed(team);
                    }
                    return _embedManager.CancelChallengeErrorEmbed($"Team {team.Name} does not have any pending challenges sent out to cancel.");
                }
                return _embedManager.CancelChallengeErrorEmbed($"You are not a member of Team **{team.Name}**\nThat team's member(s) consists of: {team.GetAllMemberNamesToStr()}");
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }

        public Embed SendAdminChallengeProcess(SocketInteractionContext context, string challengerTeam, string challengedTeam)
        {
            // Load Challenges and Leagues
            _leagueManager.LoadLeagueRegistry();
            _challengeManager.LoadChallengesHub();

            // Check if each team exists in the database
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                if (!_leagueManager.IsTeamNameUnique(challengedTeam))
                {
                    // Grab team objects
                    Team? objectChallengerTeam = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                    Team? objectChallengedTeam = _leagueManager.GetTeamByNameFromLeagues(challengedTeam);

                    // Grab league
                    League league = _leagueManager.GetLeagueByName(objectChallengerTeam.League);

                    // Check if ladder is running in League
                    if (!_statesManager.IsLadderRunning(league))
                    {
                        return _embedManager.ChallengeErrorEmbed($"The ladder is not currently running in **{league.Name}** ({league.Format} League). Challenges may not be initiated yet.");
                    }
                    // Check if teams are in the same League
                    if (_leagueManager.IsTeamsInSameLeague(league, objectChallengerTeam, objectChallengedTeam))
                    {
                        // Ensure the ranks are within range
                        if (_challengeManager.IsTeamChallengeable(objectChallengerTeam, objectChallengedTeam))
                        {
                            // Check if Challenger team has any pending challenges
                            if (!_challengeManager.IsTeamInChallenge(league.Name, objectChallengerTeam))
                            {
                                // Check if the challenged has any pending challenges
                                if (!_challengeManager.IsTeamInChallenge(league.Name, objectChallengedTeam))
                                {
                                    // Create and save new Challenge
                                    Challenge challenge = new(objectChallengerTeam.Name, objectChallengerTeam.Rank, objectChallengedTeam.Name, objectChallengedTeam.Rank);
                                    _challengeManager.AddNewChallenge(league.Name, challenge);

                                    // Change IsChallengeable of both teams to false and save league
                                    _teamManager.ChangeChallengeStatus(objectChallengerTeam, false);
                                    _teamManager.ChangeChallengeStatus(objectChallengedTeam, false);
                                    _leagueManager.SaveAndReloadLeagueRegistry();

                                    // Backup to Git
                                    _backupManager.CopyAndBackupFilesToGit();

                                    // Send challenge message
                                    foreach (Member member in objectChallengedTeam.Members)
                                    {
                                        _challengeManager.SendChallengeNotification(member.DiscordId, challenge, league);
                                    }

                                    return _embedManager.AdminChallengeSuccessEmbed(context, objectChallengerTeam, objectChallengedTeam);
                                }
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengedTeam.Name} is already awaiting a challenge match.");
                            }
                            return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name} is already awaiting a challenge match.");
                        }
                        else
                        {
                            if (objectChallengerTeam.Rank < objectChallengedTeam.Rank)
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name}'s rank ({objectChallengerTeam.Rank}) is higher than {objectChallengedTeam.Name}'s rank ({objectChallengedTeam.Rank}). A challenge cannot be initiated.");
                            }
                            else
                            {
                                return _embedManager.ChallengeErrorEmbed($"Team {objectChallengerTeam.Name}'s rank ({objectChallengerTeam.Rank}) is not within the allowed range to challenge {objectChallengedTeam.Name}'s rank ({objectChallengedTeam.Rank}). Challenges can only be made for teams within two ranks above.");
                            }
                        }
                    }
                    return _embedManager.ChallengeErrorEmbed($"The teams are not in the same League. Challenger team's League: {objectChallengerTeam.League}, Challenged team's League: {objectChallengedTeam.League}. Please try again.");
                }
                return _embedManager.TeamNotFoundErrorEmbed(challengedTeam);
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }

        public Embed AdminCancelChallengeProcess(SocketInteractionContext context, string challengerTeam)
        {
            // Load latest Challenges database save
            _challengeManager.LoadChallengesHub();

            // Check if team exists
            if (!_leagueManager.IsTeamNameUnique(challengerTeam))
            {
                // Grab team and league objects
                Team? team = _leagueManager.GetTeamByNameFromLeagues(challengerTeam);
                League league = _leagueManager.GetLeagueByName(team.League);

                // Check if ladder is running in league
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.CancelChallengeErrorEmbed($"The ladder is not currently running in {league.Name} ({league.Format} League) so there are no challenges to cancel yet.");
                }

                // Check if team has a challenge sent out to actually cancel
                if (_challengeManager.IsTeamChallenger(team.League, team))
                {
                    // Grab Challenge object
                    Challenge? challenge = _challengeManager.GetChallengeForTeam(team.League, team);

                    // Grab challenger Team object
                    Team? otherTeam = _leagueManager.GetTeamByNameFromLeagues(challenge.Challenged);

                    // Set IsChallengeable for both teams back to true
                    _teamManager.ChangeChallengeStatus(team, true);
                    _teamManager.ChangeChallengeStatus(otherTeam, true);

                    // Save leagues
                    _leagueManager.SaveAndReloadLeagueRegistry();

                    // Cancel the challenge
                    _challengeManager.SudoRemoveChallenge(team.League, team.Name);

                    // Backup the database to Git
                    _backupManager.CopyAndBackupFilesToGit();

                    return _embedManager.AdminCancelChallengeSuccessEmbed(context, team, otherTeam);
                }
                return _embedManager.CancelChallengeErrorEmbed($"**{team.Name}** does not have any pending challenges sent out to cancel.");
            }
            return _embedManager.TeamNotFoundErrorEmbed(challengerTeam);
        }    
        #endregion

        #region Reporting Logic
        public Embed ReportWinProcess(SocketInteractionContext context, string winningTeamName)
        {
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

            // Check if team name exists
            if (!_leagueManager.IsTeamNameUnique(winningTeamName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueFromTeamName(winningTeamName);

                // Check if ladder is running
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in {league.Name} ({league.Format} League) so there are no matches to report on yet.");
                }

                // Grab winningTeam object, add placeholder for losingTeam object
                Team? winningTeam = _leagueManager.GetTeamByNameFromLeagues(winningTeamName);
                Team? losingTeam;

                // Is invoker on the winningTeam
                if (_memberManager.IsDiscordIdOnGivenTeam(context.User.Id, winningTeam))
                {
                    // Is the team part of an active challenge (Challenger or Challenged)
                    if (_challengeManager.IsTeamInChallenge(winningTeam.League, winningTeam))
                    {
                        // Grab challenge
                        Challenge? challenge = _challengeManager.GetChallengeForTeam(winningTeam.League, winningTeam);

                        // Determine if winningTeam is Challenger or Challenged
                        bool isWinningTeamChallenger;
                        if (challenge.Challenger.Equals(winningTeam.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            isWinningTeamChallenger = true;
                            losingTeam = league.Teams.FirstOrDefault(t => t.Name.Equals(challenge.Challenged, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            isWinningTeamChallenger = false;
                            losingTeam = league.Teams.FirstOrDefault(t => t.Name.Equals(challenge.Challenger, StringComparison.OrdinalIgnoreCase));
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
                            _leagueManager.SaveAndReloadLeagueRegistry();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Name, challenge.Challenger);

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

                            // TODO: Add wins and losses to Member Profiles


                            // TODO: Create Match object to add to History (Past Matches)


                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _leagueManager.SaveAndReloadLeagueRegistry();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Name, challenge.Challenger);

                            // Backup the database to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with false, showing no rank change
                            return _embedManager.ReportWinSuccessEmbed(winningTeam, losingTeam, false, league);
                        }
                    }
                    return _embedManager.ReportWinErrorEmbed($"**{winningTeam.Name}** is not currently waiting on a challenge match.");
                }
                return _embedManager.ReportWinErrorEmbed($"You are not part of **{winningTeam.Name}**\nThat team's member(s) consists of: {winningTeam.GetAllMemberNamesToStr()}.");
            }
            return _embedManager.TeamNotFoundErrorEmbed(winningTeamName);
        }

        public Embed ReportWinAdminProcess(SocketInteractionContext context, string winningTeamName)
        {
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

            // Check if team name exists
            if (!_leagueManager.IsTeamNameUnique(winningTeamName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueFromTeamName(winningTeamName);

                // Check if ladder is running
                if (!_statesManager.IsLadderRunning(league))
                {
                    return _embedManager.ReportWinErrorEmbed($"The ladder is not currently running in {league.Name} ({league.Format} League) so there are no matches to report on yet.");
                }

                // Grab winningTeam object, add placeholder for losingTeam object
                Team? winningTeam = _leagueManager.GetTeamByNameFromLeagues(winningTeamName);
                Team? losingTeam;
                    // Is the team part of an active challenge (Challenger or Challenged)
                    if (_challengeManager.IsTeamInChallenge(winningTeam.League, winningTeam))
                    {
                        // Grab challenge
                        Challenge? challenge = _challengeManager.GetChallengeForTeam(winningTeam.League, winningTeam);

                        // Determine if winningTeam is Challenger or Challenged
                        bool isWinningTeamChallenger;
                        if (challenge.Challenger.Equals(winningTeam.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            isWinningTeamChallenger = true;
                            losingTeam = league.Teams.FirstOrDefault(t => t.Name.Equals(challenge.Challenged, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            isWinningTeamChallenger = false;
                            losingTeam = league.Teams.FirstOrDefault(t => t.Name.Equals(challenge.Challenger, StringComparison.OrdinalIgnoreCase));
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
                            _leagueManager.SaveAndReloadLeagueRegistry();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Name, challenge.Challenger);

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

                            // TODO: Add wins and losses to Member Profiles


                            // TODO: Create Match object to add to History (Past Matches)


                            // Set IsChallengeable status of both teams back to true
                            _teamManager.ChangeChallengeStatus(winningTeam, true);
                            _teamManager.ChangeChallengeStatus(losingTeam, true);
                            _leagueManager.SaveAndReloadLeagueRegistry();

                            // Remove the challenge
                            _challengeManager.SudoRemoveChallenge(league.Name, challenge.Challenger);

                            // Backup the database to Git
                            _backupManager.CopyAndBackupFilesToGit();

                            // Return Success Embed with false, showing no rank change
                            return _embedManager.ReportWinAdminSuccessEmbed(context, winningTeam, losingTeam, false, league);
                    }
                    }
                    return _embedManager.ReportWinErrorEmbed($"**{winningTeam.Name}** is not currently waiting on a challenge match.");
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
            // Load challenges
            _challengeManager.LoadChallengesHub();

            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueByName(leagueName);

                // Grab list of challenges in league
                List<Challenge> challenges = _challengeManager.GetChallengesForLeague(league);

                return _embedManager.PostChallengesEmbed(league, challenges);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed PostLeaguesProcess(SocketInteractionContext context, string leagueFormat)
        {
            return _embedManager.CreateDebugEmbed("TODO");
        }

        public Embed PostStandingsProcess(SocketInteractionContext context, string leagueName)
        {
            // Load leagues
            _leagueManager.LoadLeagueRegistry();

            // Check if league exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueByName(leagueName);

                return _embedManager.PostStandingsEmbed(league);
            }
            return _embedManager.LeagueNotFoundErrorEmbed(leagueName);
        }

        public Embed PostTeamsProcess(SocketInteractionContext context, string leagueName)
        {
            // Load leagues
            _leagueManager.LoadLeagueRegistry();

            // Check if league exists
            if (!_leagueManager.IsLeagueNameUnique(leagueName))
            {
                // Grab league
                League league = _leagueManager.GetLeagueByName(leagueName);

                return _embedManager.PostTeamsEmbed(league, league.Teams);
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

                // Check if team has an open challenge
                if (_challengeManager.IsTeamInChallenge(league.Name, teamToAdjust))
                {
                    return _embedManager.SetRankErrorEmbed($"Team {teamToAdjust.Name} is currently apart of a challenge and can not have their rank adjusted at this time. Please resolve the challenge by completing the match or canceling the challenge first.");
                }

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
                        if (teamsInLeague[i].Rank >= newRank && teamsInLeague[i].Rank < currentRank && teamsInLeague[i].Name != teamToAdjust.Name)
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
                        if (teamsInLeague[i].Rank <= newRank && teamsInLeague[i].Rank > currentRank && teamsInLeague[i].Name != teamToAdjust.Name)
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
                _leagueManager.SaveAndReloadLeagueRegistry();

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
        public Embed AddToWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

            // Check if team name exists in database
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Add the numberOfWins to the team
                _teamManager.AddToWins(team, numberOfWins);

                // Save and reload teams database
                _leagueManager.SaveAndReloadLeagueRegistry();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToWinCountSuccessEmbed(team, numberOfWins, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromWinCountProcess(SocketInteractionContext context, string teamName, int numberOfWins)
        {
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

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
                    _leagueManager.SaveAndReloadLeagueRegistry();

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
            // Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

            // Check if team name exists in database
            if (!_leagueManager.IsTeamNameUnique(teamName))
            {
                // Grab team object
                Team? team = _leagueManager.GetTeamByNameFromLeagues(teamName);

                // Add the numberOfLosses to the team
                _teamManager.AddToLosses(team, numberOfLosses);

                // Save and reload teams database
                _leagueManager.SaveAndReloadLeagueRegistry();

                // Backup the database to Git
                _backupManager.CopyAndBackupFilesToGit();

                return _embedManager.AddToLossCountSuccessEmbed(team, numberOfLosses, context);
            }
            return _embedManager.TeamNotFoundErrorEmbed(teamName);
        }

        public Embed SubtractFromLossCountProcess(SocketInteractionContext context, string teamName, int numberOfLosses)
        {// Load latest LeagueRegistry save
            _leagueManager.LoadLeagueRegistry();

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
                    _leagueManager.SaveAndReloadLeagueRegistry();

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

        #region Git Commands Logic
        public string GitBranchBackupDataProcess(SocketInteractionContext context, string optionalName)
        {
            _backupManager.BackupDataToNewBranch(optionalName);
            return "Git branch backup ran. Check results on repo.";
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
            _settingsManager.SaveAndReloadSettingsVault();

            return _embedManager.SetGuildIdSuccessEmbed(guildId);
        }

        public Embed SetSuperAdminModeOnOffProcess(string onOrOff)
        {
            switch (onOrOff.Trim().ToLower())
            {
                case "on":
                    _settingsManager.SetSuperAdminModeOnOff(true);
                    _settingsManager.SaveAndReloadSettingsVault();
                    return _embedManager.SuperAdminModeOnEmbed();

                case "off":
                    _settingsManager.SetSuperAdminModeOnOff(false);
                    _settingsManager.SaveAndReloadSettingsVault();
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
                _settingsManager.SaveAndReloadSettingsVault();
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
                _settingsManager.SaveAndReloadSettingsVault();
                return _embedManager.RemoveSuperAdminIdSuccessEmbed(user);
            }
            return _embedManager.RemoveSuperAdminIdNotFoundEmbed(user);
        }
        #endregion

        #region Testing Methods

        #endregion
    }
}
