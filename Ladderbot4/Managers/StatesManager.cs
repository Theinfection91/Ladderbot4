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

        public State GetStateByLeague(League league)
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

        public bool IsLadderRunning(League league)
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

        public ulong GetChallengesChannelId(League leagueRef)
        {
            State state = GetStateByLeague(leagueRef);
            return state?.ChallengesChannelId ?? 0;
        }

        public void SetChallengesChannelId(League leagueRef, ulong channelId)
        {
            State state = GetStateByLeague(leagueRef);

            if (state != null)
            {
                state.ChallengesChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetStandingsChannelId(League leagueRef)
        {
            State state = GetStateByLeague(leagueRef);
            return state?.StandingsChannelId ?? 0;
        }

        public void SetStandingsChannelId(League leagueRef, ulong channelId)
        {
            State state = GetStateByLeague(leagueRef);

            if (state != null)
            {
                state.StandingsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetTeamsChannelId(League leagueRef)
        {
            State state = GetStateByLeague(leagueRef);
            return state?.TeamsChannelId ?? 0;
        }

        public void SetTeamsChannelId(League leagueRef, ulong channelId)
        {
            State state = GetStateByLeague(leagueRef);

            if (state != null)
            {
                state.TeamsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public void SetLadderRunning(League league, bool trueOrFalse)
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
                ChallengesMessageId = 0,
                StandingsChannelId = 0,
                StandingsMessageId = 0,
                TeamsChannelId = 0,
                TeamsMessageId = 0
            };
        }

        public void AddNewState(State state)
        {
            _statesAtlasData.AddState(state);

            LoadStatesAtlas();
        }

        public void RemoveState(State state)
        {
            _statesAtlasData.RemoveState(state);

            LoadStatesAtlas();
        }
    }
}
