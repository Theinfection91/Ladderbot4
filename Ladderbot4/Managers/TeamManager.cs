using Discord;
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
        // Migrating to new LeagueData
        private readonly LeagueData _leagueData;

        private LeaguesByDivision _leaguesByDivision;

        public TeamManager(LeagueData leagueData)
        {
            _leagueData = leagueData;
            _leaguesByDivision = _leagueData.LoadAllLeagues();
        }

        public void SaveLeagues()
        {
            _leagueData.SaveLeagues(_leaguesByDivision);
        }

        public void LoadLeaguesDatabase()
        {
            _leaguesByDivision = _leagueData.LoadAllLeagues();
        }

        public void SaveAndReloadLeaguesDatabase()
        {
            SaveLeagues();
            LoadLeaguesDatabase();
        }

        // Check if two given teams are in the same division
        //public bool IsTeamsInSameDivision(Team teamOne, Team teamTwo)
        //{
        //    return teamOne.Division == teamTwo.Division;
        //}

        // Check if the team name is unique across all divisions
        //public bool IsTeamNameUnique(string teamName)
        //{
        //    // Check each division for the team name
        //    foreach (var division in new[] { _teamsByDivision.Division1v1, _teamsByDivision.Division2v2, _teamsByDivision.Division3v3 })
        //    {
        //        // Iterate through the teams in this division
        //        foreach (Team team in division)
        //        {
        //            // Compare team names (case-insensitive)
        //            if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
        //            {
        //                return false; // Name is not unique
        //            }
        //        }
        //    }

        //    // If no match was found, the name is unique
        //    return true;
        //}

        //public string GetStandingsData(string division)
        //{
        //    // Load database
        //    LoadTeamsDatabase();

        //    List<Team> divisionTeams = GetTeamsByDivision(division);

        //    StringBuilder sb = new();

        //    sb.AppendLine($"```\n");
        //    foreach(Team team in divisionTeams)
        //    {
        //        sb.AppendLine($"Team Name: {team.TeamName} - Rank: {team.Rank} - W: {team.Wins} - L: {team.Losses}\n");
        //    }
        //    sb.AppendLine("\n```");

        //    return sb.ToString();
        //}

        //public Embed GetStandingsEmbed(string division)
        //{
        //    // Load database
        //    LoadTeamsDatabase();

        //    List<Team> divisionTeams = GetTeamsByDivision(division);

        //    // Create the embed
        //    var embedBuilder = new EmbedBuilder()
        //        .WithTitle($"🏆 Standings for {division} Division")
        //        .WithColor(Color.Gold)
        //        .WithDescription($"Current {division} standings:");

        //    // Format the standings
        //    foreach (Team team in divisionTeams)
        //    {
        //        string status = team.IsChallengeable ? "Free" : "Challenged";
        //        embedBuilder.AddField(
        //            $"#{team.Rank} {team.TeamName}",
        //            $"*Wins:* **{team.Wins}** | *Losses:* **{team.Losses}** | *Status:* **{status}**",
        //            inline: false // Stacked vertically for better readability
        //        );
        //    }

        //    // Add a footer with timestamp
        //    embedBuilder.WithFooter("Updated")
        //                .WithTimestamp(DateTimeOffset.Now);

        //    return embedBuilder.Build();
        //}

        //public string GetTeamsData(string division)
        //{
        //    List<Team> divisionTeams = GetTeamsByDivision(division);
            
        //    StringBuilder sb = new();

        //    sb.AppendLine($"```\n");
        //    foreach (Team team in divisionTeams)
        //    {
        //        sb.AppendLine($"Team Name: {team.TeamName} - Member(s): {team.GetAllMemberNamesToStr()}");
        //    }
        //    sb.AppendLine("\n```");

        //    return sb.ToString();
        //}

        //public Embed GetTeamsEmbed(string division)
        //{
        //    // Load the database
        //    LoadTeamsDatabase();

        //    List<Team> divisionTeams = GetTeamsByDivision(division);

        //    // Create the embed
        //    var embedBuilder = new EmbedBuilder()
        //        .WithTitle($"🛡️ Teams for {division} Division")
        //        .WithColor(Color.Blue)
        //        .WithDescription($"Current teams in the **{division} Division**:");

        //    // Format the team data
        //    foreach (Team team in divisionTeams)
        //    {
        //        embedBuilder.AddField(
        //            $"{team.TeamName}",
        //            $"*Members:* {team.GetAllMemberNamesToStr()}",
        //            inline: false // Stacked vertically for readability
        //        );
        //    }

        //    // Add a footer with timestamp
        //    embedBuilder.WithFooter("Updated")
        //                .WithTimestamp(DateTimeOffset.Now);

        //    return embedBuilder.Build();
        //}

        public int GetTeamCountInLeague(League league)
        {
            return league.Teams.Count;
        }

        public List<Team>? GetTeamsInLeague(League leagueRef)
        {
            List<League>? leagues = leagueRef.Division switch
            {
                "1v1" => _leaguesByDivision.Leagues1v1,
                "2v2" => _leaguesByDivision.Leagues2v2,
                "3v3" => _leaguesByDivision.Leagues3v3,
                _ => null
            };

            if (leagues == null) return null;

            return leagues.FirstOrDefault(l => l.LeagueName == leagueRef.LeagueName)?.Teams;
        }     

        public void ChangeChallengeStatus(Team team, bool trueOrFalse)
        {
            switch (trueOrFalse)
            {
                case true:
                    team.IsChallengeable = true;
                    break;

                case false:
                    team.IsChallengeable = false;
                    break;
            }
        }

        public void AddToWins(Team team, int numberOfWins)
        {
            team.Wins += numberOfWins;
            team.WinStreak++;
            team.LoseStreak = 0;
        }

        public void AdminAddToWins(Team team, int numberOfWins)
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
            team.LoseStreak++;
            team.WinStreak = 0;
        }

        public void AdminAddToLosses(Team team, int numberOfLosses)
        {
            team.Losses += numberOfLosses;
        }

        public void SubtractFromLosses(Team team, int numberOfLosses)
        {
           team.Losses -= numberOfLosses;
        }

        public Team CreateTeamObject(string teamName, string leagueName, string division, int rank, List<Member> members, int wins = 0, int losses = 0)
        {
            return new Team(teamName, leagueName, division, rank, wins, losses, members);
        }
    }
}
