using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Ladderbot4.Models;
using Microsoft.Extensions.Hosting;

namespace Ladderbot4.Services
{
    public class StandingsUpdaterService : IHostedService, IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly StatesManager _statesManager;
        private readonly TeamManager _teamManager;
        private Timer _timer;

        // Channel Id's for different divisions
        private ulong _standingsChannelId1v1;
        private ulong _standingsChannelId2v2;
        private ulong _standingsChannelId3v3;

        // Dictionary to store message IDs for each division
        private Dictionary<string, ulong> _standingsMessageIds = new();

        public StandingsUpdaterService(DiscordSocketClient client, StatesManager statesManager , TeamManager teamManager)
        {
            Console.WriteLine("Test");
            _client = client;
            _statesManager = statesManager;
            _teamManager = teamManager;
            GetAllChannelIds();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Fire: {_standingsChannelId1v1}");
            _timer = new Timer(UpdateStandings, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return Task.CompletedTask;
        }

        private void GetAllChannelIds()
        {
            _standingsChannelId1v1 =  _statesManager.GetStandingsChannelId("1v1");
            _standingsChannelId2v2 = _statesManager.GetStandingsChannelId("2v2");
            _standingsChannelId3v3 = _statesManager.GetStandingsChannelId("3v3");
        }

        private async void UpdateStandings(object state)
        {
            // Make sure the bot is connected to Discord
            if (_client.ConnectionState != ConnectionState.Connected) return;

            // Update standings for each division
            await UpdateDivisionStandings("1v1", _standingsChannelId1v1);
            await UpdateDivisionStandings("2v2", _standingsChannelId2v2);
            await UpdateDivisionStandings("3v3", _standingsChannelId3v3);
        }

        private async Task UpdateDivisionStandings(string division, ulong channelId)
        {
            if (channelId == 0) return;

            var channel = _client.GetChannel(channelId) as IMessageChannel;
            if (channel == null) return;

            // Fetch the current standings
            List<Team> teams = _teamManager.GetTeamsByDivision(division);
            if (teams == null || teams.Count == 0) return;

            string standingsMessage = BuildStandingsMessage(teams);

            // Check if we already have a message for this division
            if (_standingsMessageIds.TryGetValue(division, out ulong messageId))
            {
                var existingMessage = await channel.GetMessageAsync(messageId) as IUserMessage;
                if (existingMessage != null)
                {
                    // Modify the existing message
                    await existingMessage.ModifyAsync(msg => msg.Content = standingsMessage);
                    return;
                }
            }

            // Send a new message if no existing one is found
            var newMessage = await channel.SendMessageAsync(standingsMessage) as IUserMessage;
            if (newMessage != null)
            {
                _standingsMessageIds[division] = newMessage.Id;
            }
        }

        private string BuildStandingsMessage(List<Team> teams)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("**Current Standings**");
            foreach (var team in teams.OrderBy(t => t.Rank))
            {
                sb.AppendLine($"Rank {team.Rank}: {team.TeamName} - W: {team.Wins} | L: {team.Losses}");
            }
            return sb.ToString();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
