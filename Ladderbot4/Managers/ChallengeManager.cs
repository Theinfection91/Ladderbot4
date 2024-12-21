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
        private readonly ChallengeData _challengeData;

        public ChallengeManager(ChallengeData challengeData, DiscordSocketClient client)
        {
            _client = client;
            _challengeData = challengeData;
        }

        public void LoadChallengesDatabase()
        {
            _challengeData.LoadAllChallenges();
        }

        public void SaveChallengesDatabase()
        {
            _challengeData.SaveChallenges(_challengeData.LoadAllChallenges());
        }

        public Challenge? GetChallengeForTeam(string division, string leagueName, Team team)
        {
            var challenges = _challengeData.GetChallenges(division, leagueName);

            return challenges.FirstOrDefault(challenge =>
                challenge.Challenger.Equals(team.TeamName, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(team.TeamName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsTeamInChallenge(string division, string leagueName, Team team)
        {
            var challenges = _challengeData.GetChallenges(division, leagueName);

            return challenges.Any(challenge =>
                challenge.Challenger.Equals(team.TeamName, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(team.TeamName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsTeamChallengeable(Team challengerTeam, Team challengedTeam)
        {
            return challengerTeam.Rank > challengedTeam.Rank &&
                   challengerTeam.Rank <= challengedTeam.Rank + 2;
        }

        public string GetChallengesData(string division, string leagueName)
        {
            var challenges = _challengeData.GetChallenges(division, leagueName);
            StringBuilder sb = new();

            sb.AppendLine($"```\n");
            foreach (var challenge in challenges)
            {
                sb.AppendLine($"Challenger Team: {challenge.Challenger} - Challenged Team: {challenge.Challenged} - Created: {challenge.CreatedOn}\n");
            }
            sb.AppendLine("\n```");

            return sb.ToString();
        }

        public Embed GetChallengesEmbed(string division, string leagueName)
        {
            var challenges = _challengeData.GetChallenges(division, leagueName);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"⚔️ Active Challenges for {leagueName} in {division} Division")
                .WithColor(Color.Orange)
                .WithDescription($"Current active challenges in **{leagueName} ({division} Division)**:");

            if (challenges.Count > 0)
            {
                foreach (var challenge in challenges)
                {
                    embedBuilder.AddField(
                        $"Challenger: {challenge.Challenger}",
                        $"*Challenged:* *{challenge.Challenged}*\n> Created On: {challenge.CreatedOn:MM/dd/yyyy HH:mm}",
                        inline: false
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"There are no active challenges in **{leagueName} ({division} Division)** at this time.");
            }

            embedBuilder.WithFooter("Last Updated").WithTimestamp(DateTimeOffset.Now);
            return embedBuilder.Build();
        }

        public async void SendChallengeNotification(ulong userId, Challenge challenge)
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
                    .WithDescription($"Your team, **{challenge.Challenged}(#{challenge.ChallengedRank})**, has been challenged by **{challenge.Challenger}(#{challenge.ChallengerRank})** in the **{challenge.Division} Division**.")
                    .AddField("Challenger Team", challenge.Challenger, inline: true)
                    .AddField("Your Team", challenge.Challenged, inline: true)
                    .WithFooter("Prepare for your match!")
                    .WithTimestamp(DateTimeOffset.Now);

                await dmChannel.SendMessageAsync(embed: embedBuilder.Build());
                Console.WriteLine($"Notification sent to user {user.Username} (ID: {userId}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to user {userId}: {ex.Message}");
            }
        }

        public void AddNewChallenge(string division, string leagueName, Challenge challenge)
        {
            _challengeData.AddChallenge(division, leagueName, challenge);
        }

        public void RemoveChallenge(string division, string leagueName, Predicate<Challenge> match)
        {
            _challengeData.RemoveChallenge(division, leagueName, match);
        }

        public void SudoRemoveChallenge(string division, string leagueName, string teamName)
        {
            _challengeData.SudoRemoveChallenge(division, leagueName, teamName);
        }
    }
}
