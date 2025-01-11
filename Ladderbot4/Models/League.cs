using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Ladderbot4.Models
{
    public class League
    {
        public string Name { get; set; }
        public int TeamSize { get; set; }
        public string Format { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Team> Teams { get; set; }

        public League(string leagueName, string leagueFormat)
        {
            Name = leagueName;
            Format = leagueFormat;
            CreatedOn = DateTime.Now;
            Teams = [];
        }

        public void AddTeam(Team team)
        {
            if (team != null)
            {
                Teams.Add(team);
            }
        }

        public void RemoveTeam(Team teamToRemove)
        {
            if (teamToRemove != null)
            {
                for (int i = 0; i < Teams.Count; i++)
                {
                    Team team = Teams[i];
                    if (Teams[i].Name == teamToRemove.Name)
                    {
                        Teams.Remove(team);
                    }
                }
            }
        }

        public void SortTeamsByRank()
        {
            Teams = Teams.OrderBy(t => t.Rank).ToList();
        }
    }
}
