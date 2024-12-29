using Discord;
using Discord.WebSocket;
using Ladderbot4.Data;
using Ladderbot4.Models;
using Ladderbot4.Models.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class AchievementManager
    {
        private DiscordSocketClient _client;

        // Will work off of MemberData
        private readonly MemberData _memberData;

        private MembersList _membersList;

        public AchievementManager(MemberData memberData, DiscordSocketClient client)
        {
            _client = client;
            _memberData = memberData;
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveMembersAchievements()
        {
            _memberData.SaveAllMembers(_membersList);
        }

        public void LoadMembersAchievements()
        {
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveAndReloadMembersAchievements()
        {
            SaveMembersAchievements();
            LoadMembersAchievements();
        }

        public async void SendAchievementNotification(ulong userId, Achievement achievement)
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
                    .WithTitle("🏆 Achievement Unlocked!") // Title for the embed
                    .WithColor(Color.Gold) // A gold color for achievements
                    .WithDescription($"Congratulations! You've unlocked a new achievement.")
                    .AddField("Achievement", $"**{achievement.Name}**", inline: false)
                    .AddField("Description", achievement.Description, inline: false)
                    .AddField("Points", $"{achievement.AchievementPointsValue} 🏅", inline: true)
                    .AddField("Achieved On", achievement.AchievedOn.ToString("f"), inline: true)
                    .WithFooter("Keep it up!")
                    .WithTimestamp(DateTimeOffset.Now); // Add timestamp to the embed

                // Send the embed as a message
                await dmChannel.SendMessageAsync(embed: embedBuilder.Build());

                Console.WriteLine($"Achievement notification sent to user {user.Username} (ID: {userId}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send achievement notification to user {userId}: {ex.Message}");
            }
        }

    }
}
