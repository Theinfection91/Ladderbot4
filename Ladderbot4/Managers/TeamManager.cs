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
            _leaguesByDivision = _leagueData.Load();
        }

        public void SaveLeagues()
        {
            _leagueData.Save(_leaguesByDivision);
        }

        public void LoadLeaguesDatabase()
        {
            _leaguesByDivision = _leagueData.Load();
        }

        public void SaveAndReloadLeaguesDatabase()
        {
            SaveLeagues();
            LoadLeaguesDatabase();
        }

        public int GetTeamCountInLeague(League league)
        {
            return league.Teams.Count;
        }

        public List<Team>? GetTeamsInLeague(League leagueRef)
        {
            return leagueRef.Teams;
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
