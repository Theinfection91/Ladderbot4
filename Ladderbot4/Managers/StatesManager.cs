using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class StatesManager
    {
        private readonly StatesAtlasData _statesAtlasData;

        private StatesAtlas _statesAtlas;

        public StatesManager(StatesAtlasData statesAtlasData)
        {
            _statesAtlasData = statesAtlasData;
            _statesAtlas = _statesAtlasData.Load();
        }

        public void SaveStatesAtlas()
        {
            _statesAtlasData.Save(_statesAtlas);
        }

        public void LoadStatesAtlas()
        {
            _statesAtlas = _statesAtlasData.Load();
        }

        public void SaveAndReloadStatesAtlas()
        {
            SaveStatesAtlas();
            LoadStatesAtlas();
        }

        public State GetXvXStateByLeague(League league)
        {
            foreach (State state in _statesAtlas.States)
            {
                if (state.LeagueName.Equals(league.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return state;
                }
            }
            return null;
        }

        public bool IsXvXLadderRunning(League league)
        {
            foreach (State state in _statesAtlas.States)
            {
                if (state.LeagueName.Equals(league.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return state.IsLadderRunning;
                }
            }
            return false;
        }     

        public ulong GetXvXChallengesChannelId(League leagueRef)
        {
            State state = GetXvXStateByLeague(leagueRef);
            return state?.ChallengesChannelId ?? 0;
        }

        public void SetXvXChallengesChannelId(League leagueRef, ulong channelId)
        {
            State state = GetXvXStateByLeague(leagueRef);

            if (state != null)
            {
                state.ChallengesChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetXvXStandingsChannelId(League leagueRef)
        {
            State state = GetXvXStateByLeague(leagueRef);
            return state?.StandingsChannelId ?? 0;
        }

        public void SetXvXStandingsChannelId(League leagueRef, ulong channelId)
        {
            State state = GetXvXStateByLeague(leagueRef);

            if (state != null)
            {
                state.StandingsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetXvXTeamsChannelId(League leagueRef)
        {
            State state = GetXvXStateByLeague(leagueRef);
            return state?.TeamsChannelId ?? 0;
        }

        public void SetXvXTeamsChannelId(League leagueRef, ulong channelId)
        {
            State state = GetXvXStateByLeague(leagueRef);

            if (state != null)
            {
                state.TeamsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public void SetXvXLadderRunning(League league, bool trueOrFalse)
        {
            foreach (State state in _statesAtlas.States)
            {
                if (league.Name.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                {
                    state.IsLadderRunning = trueOrFalse;
                    SaveAndReloadStatesAtlas();
                }
            }
        }

        public State CreateNewState(string leagueName, string leagueFormat)
        {
            return new State(leagueName, leagueFormat)
            {
                IsLadderRunning = false,
                ChallengesChannelId = 0,
                StandingsChannelId = 0,
                TeamsChannelId = 0
            };
        }

        public void AddNewXvXState(State state)
        {
            _statesAtlasData.AddState(state);

            LoadStatesAtlas();
        }

        public void RemoveXvXState(State state)
        {
            _statesAtlasData.RemoveState(state);

            LoadStatesAtlas();
        }
    }
}
