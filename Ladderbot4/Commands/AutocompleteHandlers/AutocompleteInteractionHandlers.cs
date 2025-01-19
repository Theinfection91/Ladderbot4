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
            // Determine the command and handle autocomplete accordingly
            if (interaction.Data.CommandName == "team" ||
                interaction.Data.CommandName == "challenge" ||
                interaction.Data.CommandName == "post" ||
                interaction.Data.CommandName == "report" ||
                interaction.Data.CommandName == "set")
            {
                // Get the currently focused option
                var focusedOption = interaction.Data.Current;

                if (focusedOption != null)
                {
                    string input = focusedOption.Value?.ToString()?.Trim() ?? string.Empty;

                    // Handle team-related autocomplete options
                    List<AutocompleteResult> suggestions = focusedOption.Name switch
                    {
                        "challenger_team" => string.IsNullOrWhiteSpace(input)
                            ? GetTeamNamesMatchingInput("")
                            : GetTeamNamesMatchingInput(input),
                        "challenged_team" => string.IsNullOrWhiteSpace(input)
                            ? GetTeamNamesMatchingInput("")
                            : GetTeamNamesMatchingInput(input),
                        "league_name" => string.IsNullOrWhiteSpace(input)
                            ? GetLeagueNamesMatchingInput("")
                            : GetLeagueNamesMatchingInput(input),
                        "team_name" => string.IsNullOrWhiteSpace(input)
                            ? GetTeamNamesMatchingInput("")
                            : GetTeamNamesMatchingInput(input),
                        "winning_team_name" => string.IsNullOrWhiteSpace(input)
                            ? GetTeamNamesMatchingInput("")
                            : GetTeamNamesMatchingInput(input),
                        _ => new List<AutocompleteResult>()
                    };

                    // Respond with the suggestions
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
                    .OrderBy(league => league.Name)
                    .Select(league => new AutocompleteResult($"{league.Name} - ({league.Format} League)", league.Name))
                    .ToList();
            }

            // Filter leagues based on the input and sort them alphabetically
            var filteredLeagues = allLeagues
                .Where(league => league.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(league => league.Name)
                .ToList();

            // Return filtered and sorted leagues
            return filteredLeagues
                .Select(league => new AutocompleteResult($"{league.Name} - ({league.Format} League)", league.Name))
                .ToList();
        }

        private List<AutocompleteResult> GetTeamNamesMatchingInput(string input)
        {
            // Get all Teams
            List<Team> teams = _leagueManager.GetAllTeams();

            // If the input is empty or only whitespace, return all teams sorted alphabetically by League
            if (string.IsNullOrWhiteSpace(input))
            {
                // Return all leagues, sorted alphabetically by name
                return teams
                    .OrderBy(team => team.League)
                    .Select(team => new AutocompleteResult($"{team.Name} - ({team.League} | {team.LeagueFormat} League)", team.Name))
                    .ToList();
            }

            // Filter leagues based on the input and sort them alphabetically by League
            var filteredTeams = teams
                .Where(team => team.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(team => team.League)
                .ToList();

            // Return filtered and sorted leagues
            return filteredTeams
                .Select(team => new AutocompleteResult($"{team.Name} - ({team.League} | {team.LeagueFormat} League)", team.Name))
                .ToList();
        }
    }
}
