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
        public TeamManager() { }

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

        public void SubtractFromLosses(Team team, int numberOfLosses)
        {
           team.Losses -= numberOfLosses;
        }

        public Team CreateTeamObject(string teamName, string leagueName, int teamSize, string leagueFormat, int rank, List<Member> members, int wins = 0, int losses = 0)
        {
            return new Team(teamName, leagueName, leagueFormat, rank, wins, losses, members)
            {
                Size = teamSize
            };
        }
    }
}
