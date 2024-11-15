using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Microsoft.Extensions.Hosting;

namespace Ladderbot4.Services
{
    public class StandingsUpdaterService : IHostedService, IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly LadderManager _ladderManager;
        private Timer _timer;

        private ulong _standingsChannelId;
        
        public StandingsUpdaterService(DiscordSocketClient client, LadderManager ladderManager)
        {
            _client = client;
            _ladderManager = ladderManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateStandings, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return Task.CompletedTask;
        }

        private async void UpdateStandings(object state)
        {
            // Make sure the bot is connected to Discord
            if (_client.ConnectionState != ConnectionState.Connected) return;

            var channel = _client.GetChannel(_standingsChannelId) as IMessageChannel;
            if (channel == null) return;
            
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose() => _timer?.Dispose();
    }
}
