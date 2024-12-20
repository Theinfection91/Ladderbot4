﻿using Discord;
using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class TeamManager
    {
        private readonly TeamData _teamData;

        private TeamsByDivision _teamsByDivision;

        public TeamManager(TeamData teamData)
        {
            _teamData = teamData;
            _teamsByDivision = _teamData.LoadAllTeams();
        }

        public void SaveTeams()
        {
            _teamData.SaveTeams(_teamsByDivision);
        }

        public void LoadTeamsDatabase()
        {
            _teamsByDivision = _teamData.LoadAllTeams();
        }

        public void SaveAndReloadTeamsDatabase()
        {
            SaveTeams();
            LoadTeamsDatabase();
        }

        // Check if two given teams are in the same division
        public bool IsTeamsInSameDivision(Team teamOne, Team teamTwo)
        {
            return teamOne.Division == teamTwo.Division;
        }

        // Check if the team name is unique across all divisions
        public bool IsTeamNameUnique(string teamName)
        {
            // Check each division for the team name
            foreach (var division in new[] { _teamsByDivision.Division1v1, _teamsByDivision.Division2v2, _teamsByDivision.Division3v3 })
            {
                // Iterate through the teams in this division
                foreach (Team team in division)
                {
                    // Compare team names (case-insensitive)
                    if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Name is not unique
                    }
                }
            }

            // If no match was found, the name is unique
            return true;
        }

        public bool IsValidDivisionType(string divisionType)
        {
            foreach (string division in new[] { "1v1", "2v2", "3v3" })
            {
                if (divisionType.ToLower().Equals(division))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetStandingsData(string division)
        {
            // Load database
            LoadTeamsDatabase();

            List<Team> divisionTeams = GetTeamsByDivision(division);

            StringBuilder sb = new();

            sb.AppendLine($"```\n");
            foreach(Team team in divisionTeams)
            {
                sb.AppendLine($"Team Name: {team.TeamName} - Rank: {team.Rank} - W: {team.Wins} - L: {team.Losses}\n");
            }
            sb.AppendLine("\n```");

            return sb.ToString();
        }

        public Embed GetStandingsEmbed(string division)
        {
            // Load database
            LoadTeamsDatabase();

            List<Team> divisionTeams = GetTeamsByDivision(division);

            // Create the embed
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🏆 Standings for {division} Division")
                .WithColor(Color.Gold)
                .WithDescription($"Current {division} standings:");

            // Format the standings
            foreach (Team team in divisionTeams)
            {
                string status = team.IsChallengeable ? "Free" : "Challenged";
                embedBuilder.AddField(
                    $"#{team.Rank} {team.TeamName}",
                    $"*Wins:* **{team.Wins}** | *Losses:* **{team.Losses}** | *Status:* **{status}**",
                    inline: false // Stacked vertically for better readability
                );
            }

            // Add a footer with timestamp
            embedBuilder.WithFooter("Updated")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public string GetTeamsData(string division)
        {
            List<Team> divisionTeams = GetTeamsByDivision(division);
            
            StringBuilder sb = new();

            sb.AppendLine($"```\n");
            foreach (Team team in divisionTeams)
            {
                sb.AppendLine($"Team Name: {team.TeamName} - Member(s): {team.GetAllMemberNamesToStr()}");
            }
            sb.AppendLine("\n```");

            return sb.ToString();
        }

        public Embed GetTeamsEmbed(string division)
        {
            // Load the database
            LoadTeamsDatabase();

            List<Team> divisionTeams = GetTeamsByDivision(division);

            // Create the embed
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🛡️ Teams for {division} Division")
                .WithColor(Color.Blue)
                .WithDescription($"Current teams in the **{division} Division**:");

            // Format the team data
            foreach (Team team in divisionTeams)
            {
                embedBuilder.AddField(
                    $"{team.TeamName}",
                    $"*Members:* {team.GetAllMemberNamesToStr()}",
                    inline: false // Stacked vertically for readability
                );
            }

            // Add a footer with timestamp
            embedBuilder.WithFooter("Updated")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }


        public int GetTeamCount(string division)
        {
            return division switch
            {
                "1v1" => _teamsByDivision.Division1v1.Count,
                "2v2" => _teamsByDivision.Division2v2.Count,
                "3v3" => _teamsByDivision.Division3v3.Count,
                _ => 0
            };
            ;
        }

        public List<Team> GetTeamsByDivision(string division)
        {
            return division switch
            {
                "1v1" => _teamsByDivision.Division1v1,
                "2v2" => _teamsByDivision.Division2v2,
                "3v3" => _teamsByDivision.Division3v3,
                _ => null
            };
            ;
        }

        public Team GetTeamByName(string teamName)
        {
            // Search in Division1v1
            foreach (var team in _teamsByDivision.Division1v1)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // Search in Division2v2
            foreach (var team in _teamsByDivision.Division2v2)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // Search in Division3v3
            foreach (var team in _teamsByDivision.Division3v3)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // If no team is found
            return null;
        }

        public void ChangeChallengeStatus(Team team, bool trueOrFalse)
        {
            switch (trueOrFalse)
            {
                case true:
                    Console.WriteLine("Set to false");
                    team.IsChallengeable = true;
                    break;

                case false:
                    Console.WriteLine("Set to false");
                    team.IsChallengeable = false;
                    break;
            }
        }

        public void AddToWins(Team team, int numberOfWins)
        {
            team.Wins += numberOfWins;
        }

        public void SubtractFromWins(Team team, int numberOfWins)
        {
            team.Wins -= numberOfWins;
        }

        public void AddToLosses(Team team, int numberOfLosses)
        {
            team.Losses += numberOfLosses;
        }

        public void SubtractFromLosses(Team team, int numberOfLosses)
        {
           team.Losses -= numberOfLosses;
        }

        public Team CreateTeamObject(string teamName, string division, int rank, List<Member> members, int wins = 0, int losses = 0)
        {
            return new Team(teamName, division, rank, wins, losses, members);
        }

        public void AddNewTeam(Team newTeam)
        {
            _teamData.AddTeam(newTeam);

            // Loads newest save of the database to backing field
            LoadTeamsDatabase();
        }

        public void RemoveTeam(string teamName, string division)
        {
            _teamData.RemoveTeam(teamName, division);

            // Loads newest save of the database to backing field
            LoadTeamsDatabase();
        }
    }
}
