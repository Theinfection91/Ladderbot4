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
        private DiscordSocketClient _client;

        private readonly ChallengeData _challengeData;

        private ChallengesByDivision _challengesByDivision;

        public ChallengeManager(ChallengeData challengeData, DiscordSocketClient client)
        {
            _client = client;
            _challengeData = challengeData;
            _challengesByDivision = _challengeData.LoadAllChallenges();
        }

        public void LoadChallengesDatabase()
        {
            _challengesByDivision = _challengeData.LoadAllChallenges();
        }

        public void SaveChallengesDatabase()
        {
            _challengeData.SaveChallenges(_challengesByDivision);
        }

        public void SaveAndReloadChallenges()
        {
            SaveChallengesDatabase();
            LoadChallengesDatabase();
        }

        public Challenge? GetChallengeByTeamObject(Team team)
        {
            // Load the challenges database
            LoadChallengesDatabase();

            string teamName = team.TeamName;
            List<Challenge> challenges = team.Division switch
            {
                "1v1" => _challengesByDivision.Challenges1v1,
                "2v2" => _challengesByDivision.Challenges2v2,
                "3v3" => _challengesByDivision.Challenges3v3,
                _ => null
            };

            // Iterate over the challenges in the specified division
            foreach (var challenge in challenges)
            {
                // Check if the team is either the Challenger or Challenged
                if ((challenge.Challenger != null && challenge.Challenger.Equals(teamName, StringComparison.OrdinalIgnoreCase)) ||
                    (challenge.Challenged != null && challenge.Challenged.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
                {
                    return challenge;
                }
            }

            // Return null if no challenge is found
            return null;
        }

        public bool IsTeamAwaitingChallengeMatch(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

            }
            return false;
        }

        public bool IsTeamChallenger(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool IsTeamChallenged(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool IsTeamChallengeable(Team challengerTeam, Team challengedTeam)
        {
            LoadChallengesDatabase();

            return challengerTeam.Rank > challengedTeam.Rank && challengerTeam.Rank <= challengedTeam.Rank + 2;
        }

        public string GetChallengesData(string division)
        {
            // Load database
            LoadChallengesDatabase();

            List<Challenge> challenges = GetChallengesByDivision(division);
            StringBuilder sb = new();

            sb.AppendLine($"```\n");
            foreach (Challenge challenge in challenges)
            {
                sb.AppendLine($"Challenger Team: {challenge.Challenger} - Challenged Team: {challenge.Challenged} - Created: {challenge.CreatedOn}\n");
            }
            sb.AppendLine("\n```");

            return sb.ToString();
        }

        public Embed GetChallengesEmbed(string division)
        {
            // Load the database
            LoadChallengesDatabase();

            List<Challenge> challenges = GetChallengesByDivision(division);

            // Create the embed
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"⚔️ Active Challenges for {division} Division")
                .WithColor(Color.Orange)
                .WithDescription($"Current active challenges in the **{division} Division**:");

            // Format the challenge data
            if (challenges.Count > 0)
            {
                foreach (Challenge challenge in challenges)
                {
                    embedBuilder.AddField(
                        $"Challenger: {challenge.Challenger}",
                        $"*Challenged:* *{challenge.Challenged}*\n> Created On: {challenge.CreatedOn:MM/dd/yyyy HH:mm}",
                        inline: false // Stacked vertically for readability
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"There are no active challenges in the **{division} Division** at this time.");
            }

            // Add a footer with timestamp
            embedBuilder.WithFooter("Last Updated")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }


        public List<Challenge> GetChallengesByDivision(string division)
        {
            return division switch
            {
                "1v1" => _challengesByDivision.Challenges1v1,
                "2v2" => _challengesByDivision.Challenges2v2,
                "3v3" => _challengesByDivision.Challenges3v3,
                _ => throw new ArgumentException($"Invalid division type given: {division}"),
            };
            ;
        }

        public async void SendChallengeNotification(ulong userId, Challenge challenge)
        {
            try
            {
                // Retrieve the user by ID
                var user = await _client.GetUserAsync(userId);

                if (user == null)
                {
                    Console.WriteLine($"User with ID {userId} not found.");
                    return;
                }

                // Open a DM channel with the user
                var dmChannel = await user.CreateDMChannelAsync();

                // Create the embed
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("⚔️ You've Been Challenged!")
                    .WithColor(Color.Gold)
                    .WithDescription($"Your team, **Team {challenge.Challenged}(#{challenge.ChallengedRank})**, has been challenged by **Team {challenge.Challenger}(#{challenge.ChallengerRank})** in the **{challenge.Division} Division**.")
                    .AddField("Challenger Team", challenge.Challenger, inline: true)
                    .AddField("Your Team", challenge.Challenged, inline: true)
                    .WithFooter("Prepare for your match!")
                    .WithTimestamp(DateTimeOffset.Now);

                // Send the embed message
                await dmChannel.SendMessageAsync(embed: embedBuilder.Build());

                Console.WriteLine($"Embed message sent to user {user.Username} (ID: {userId}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to user {userId}: {ex.Message}");
            }
        }

        public Challenge CreateChallengeObject(string division, string challenger, int challengerRank, string challenged, int challengedRank)
        {
            LoadChallengesDatabase();

            return new Challenge(division, challenger, challengerRank, challenged, challengedRank);
        }

        public void AddNewChallenge(Challenge challenge)
        {
            _challengeData.AddChallenge(challenge);

            // Load the newly saved Challenges database
            LoadChallengesDatabase();
        }

        public void RemoveChallenge(string challengerTeam, string division)
        {
            _challengeData.RemoveChallenge(challengerTeam, division);

            // Load the newly saved challenges database
            LoadChallengesDatabase();
        }

        public void SudoRemoveChallenge(string teamName, string division)
        {
            _challengeData.SudoRemoveChallenge(teamName, division);

            // Load the newly saved challenges database
            LoadChallengesDatabase();
        }
    }
}
