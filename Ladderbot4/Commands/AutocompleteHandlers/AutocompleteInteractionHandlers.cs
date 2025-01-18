using Discord;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Ladderbot4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ladderbot4.Commands.AutocompleteHandlers
{
    public class AutocompleteInteractionHandlers
    {
        private readonly IServiceProvider _services;
        private readonly LeagueManager _leagueManager;

        public AutocompleteInteractionHandlers(IServiceProvider services, LeagueManager leagueManager)
        {
            _services = services;
            _leagueManager = leagueManager;
        }

        // Initialize method to set up the autocomplete event listener
        public Task InitializeAsync()
        {
            var client = _services.GetRequiredService<DiscordSocketClient>();
            client.AutocompleteExecuted += HandleAutoCompleteAsync;
            return Task.CompletedTask;
        }

        // Handle autocomplete interactions when invoked
        private async Task HandleAutoCompleteAsync(SocketAutocompleteInteraction interaction)
        {
            // Check if the interaction is for the 'team register' command
            if (interaction.Data.CommandName == "team" && interaction.Data.Options.FirstOrDefault()?.Name == "register")
            {
                var leagueNameOption = interaction.Data.Options.FirstOrDefault(o => o.Name == "league_name");
                if (leagueNameOption != null)
                {
                    // Get the user input from the league_name parameter
                    string input = leagueNameOption.Value?.ToString()?.Trim() ?? string.Empty;

                    Console.WriteLine($"Autocomplete Input: '{input}'"); // Debugging line

                    // Handle empty or null input
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Input is empty or whitespace, returning all leagues.");
                        await interaction.RespondAsync(GetLeagueNamesMatchingInput("")); // Pass empty string to get all leagues
                        return;
                    }

                    // Get suggestions based on the input
                    var suggestions = GetLeagueNamesMatchingInput(input);

                    // Send suggestions back as autocomplete results
                    await interaction.RespondAsync(suggestions);
                }
            }
        }

        private List<AutocompleteResult> GetLeagueNamesMatchingInput(string input)
        {
            // Get all the leagues
            List<League> allLeagues = _leagueManager.GetAllLeagues();

            // If the input is empty or only whitespace, return all leagues sorted alphabetically
            if (string.IsNullOrWhiteSpace(input))
            {
                // Return all leagues, sorted alphabetically by name
                return allLeagues
                    .OrderBy(league => league.Name) // Sort alphabetically by league name
                    .Select(league => new AutocompleteResult(league.Name, league.Name))
                    .ToList();
            }

            // Filter leagues based on the input and sort them alphabetically
            var filteredLeagues = allLeagues
                .Where(league => league.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(league => league.Name) // Sort alphabetically by league name
                .ToList();

            // Return filtered and sorted leagues
            return filteredLeagues
                .Select(league => new AutocompleteResult(league.Name, league.Name))
                .ToList();
        }
    }
}
