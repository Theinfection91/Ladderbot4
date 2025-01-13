﻿using Discord;
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

        public ChallengeManager(ChallengesHubData challengesHubData, DiscordSocketClient client)
        {
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

        public Challenge? GetChallengeForTeam(string leagueName, Team team)
        {
            var challenges = _challengesHub.GetChallenges(leagueName);

            return challenges.FirstOrDefault(challenge =>
                challenge.Challenger.Equals(team.Name, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(team.Name, StringComparison.OrdinalIgnoreCase));
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
                var user = await _client.GetUserAsync(userId);

                if (user == null)
                {
                    Console.WriteLine($"User with ID {userId} not found.");
                    return;
                }

                var dmChannel = await user.CreateDMChannelAsync();

                var embedBuilder = new EmbedBuilder()
                    .WithTitle("⚔️ You've Been Challenged!")
                    .WithColor(Color.Gold)
                    .WithDescription($"Your team, **{challenge.Challenged}(#{challenge.ChallengedRank})**, has been challenged by **{challenge.Challenger}(#{challenge.ChallengerRank})** in **{league.Name}** ({league.Format} League).")
                    .AddField("Challenger Team", challenge.Challenger, inline: true)
                    .AddField("Your Team", challenge.Challenged, inline: true)
                    .WithFooter("Prepare for your match!")
                    .WithTimestamp(DateTimeOffset.Now);

                await dmChannel.SendMessageAsync(embed: embedBuilder.Build());
                Console.WriteLine($"{DateTime.Now} ChallengeManager - Notification sent to user {user.Username} (ID: {userId}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to user {userId}: {ex.Message}");
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
