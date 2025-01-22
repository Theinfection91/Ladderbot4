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
    public class ChallengeManager
    {
        private readonly DiscordSocketClient _client;

        private readonly ChallengesHubData _challengesHubData;

        private ChallengesHub _challengesHub;

        private LeagueManager _leagueManager;

        public ChallengeManager(LeagueManager leagueManager, ChallengesHubData challengesHubData, DiscordSocketClient client)
        {
            _leagueManager = leagueManager;
            _client = client;
            _challengesHubData = challengesHubData;
            _challengesHub = _challengesHubData.Load();
        }

        public void SaveChallengesHub()
        {
            _challengesHubData.Save(_challengesHub);
        }

        public void LoadChallengesHub()
        {
            _challengesHub = _challengesHubData.Load();
        }

        public void SaveAndReloadChallengesHub()
        {
            SaveChallengesHub();
            LoadChallengesHub();
        }

        public void ChallengeRankComparisonProcess(List<Team> teams)
        {
            foreach (Team team in teams)
            {
                if (!IsChallengeRankCorrect(team))
                {
                    Challenge? challengeToEdit = GetChallengeForTeam(team.League, team);
                    if (team.Name.Equals(challengeToEdit.Challenger, StringComparison.OrdinalIgnoreCase))
                    {
                        challengeToEdit.ChallengerRank = team.Rank;
                    }
                    else if (team.Name.Equals(challengeToEdit.Challenged, StringComparison.OrdinalIgnoreCase))
                    {
                        challengeToEdit.ChallengedRank = team.Rank;
                    }
                }
            }
            SaveAndReloadChallengesHub();
        }

        public Challenge? GetChallengeForTeam(string leagueName, Team team)
        {
            var challenges = _challengesHub.GetChallenges(leagueName);

            return challenges.FirstOrDefault(challenge =>
                challenge.Challenger.Equals(team.Name, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(team.Name, StringComparison.OrdinalIgnoreCase));
        }

        public List<Team> GetTeamsInLeagueChallenges(string leagueName)
        {
            List<Team> teams = [];
            foreach (Challenge challenge in _challengesHub.Challenges[leagueName])
            {
                teams.Add(_leagueManager.GetTeamByNameFromLeagues(challenge.Challenger));
                teams.Add(_leagueManager.GetTeamByNameFromLeagues(challenge.Challenged));
            }
            return teams;
        }

        public bool IsChallengeRankCorrect(Team team)
        {
            foreach (Challenge challenge in _challengesHub.Challenges[team.League])
            {
                if (challenge.Challenged.Equals(team.Name, StringComparison.OrdinalIgnoreCase) || challenge.Challenger.Equals(team.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (challenge.ChallengedRank == team.Rank || challenge.ChallengerRank == team.Rank)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsTeamInChallenge(string leagueName, Team team)
        {
            var challenges = _challengesHub.GetChallenges(leagueName);

            return challenges.Any(challenge =>
                challenge.Challenger.Equals(team.Name, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(team.Name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsTeamChallenger(string leagueName, Team team)
        {
            var challenges = _challengesHub.GetChallenges(leagueName);

            return challenges.Any(challenge =>
                challenge.Challenger.Equals(team.Name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsTeamChallengeable(Team challengerTeam, Team challengedTeam)
        {
            return challengerTeam.Rank > challengedTeam.Rank &&
                   challengerTeam.Rank <= challengedTeam.Rank + 2;
        }

        public List<Challenge> GetChallengesForLeague(League league)
        {
            return _challengesHub.GetChallenges(league.Name);
        }

        public async void SendChallengeNotification(ulong userId, Challenge challenge, League league)
        {
            try
            {
                // Fetch the user by ID
                var user = await _client.GetUserAsync(userId);

                if (user == null)
                {
                    Console.WriteLine($"[Error] User with ID {userId} not found.");
                    return;
                }

                // Check if the user is a bot
                if (user.IsBot)
                {
                    return;
                }

                try
                {
                    // Try to create a DM channel
                    var dmChannel = await user.CreateDMChannelAsync();

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("⚔️ You've Been Challenged!")
                        .WithColor(Color.Gold)
                        .WithDescription($"Your team, **{challenge.Challenged}(#{challenge.ChallengedRank})**, has been challenged by **{challenge.Challenger}(#{challenge.ChallengerRank})** in **{league.Name}** ({league.Format} League).")
                        .AddField("Challenger Team", challenge.Challenger, inline: true)
                        .AddField("Your Team", challenge.Challenged, inline: true)
                        .WithFooter("Prepare for your match!")
                        .WithTimestamp(DateTimeOffset.Now);

                    // Send the embed message
                    await dmChannel.SendMessageAsync(embed: embedBuilder.Build());
                    Console.WriteLine($"[Info] Notification sent to {user.Username} (ID: {userId}).");
                }
                catch (Discord.Net.HttpException httpEx) when (httpEx.HttpCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Cannot send DMs to the user
                    Console.WriteLine($"[Warning] Cannot send messages to user {userId}: DMs disabled or bot blocked.");
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                Console.WriteLine($"[Error] An error occurred while sending a notification to user {userId}: {ex.Message}");
            }
        }


        public void AddNewChallenge(string leagueName, Challenge challenge)
        {
            _challengesHubData.AddChallenge(leagueName, challenge);
            LoadChallengesHub();
        }

        public void RemoveChallenge(string leagueName, Predicate<Challenge> challenge)
        {
            _challengesHubData.RemoveChallenge(leagueName, challenge);
            LoadChallengesHub();
        }

        public void SudoRemoveChallenge(string leagueName, string teamName)
        {
            _challengesHubData.SudoRemoveChallenge(leagueName, teamName);
        }

        public void RemoveLeagueFromChallenges(string leagueName)
        {
            _challengesHubData.RemoveLeagueFromChallenges(leagueName);

            LoadChallengesHub();
        }
    }
}
