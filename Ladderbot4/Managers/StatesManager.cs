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

        public ulong GetChallengesChannelId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.ChallengesChannelId ?? 0;
        }

        public void SetChallengesChannelId(League league, ulong channelId)
        {
            State state = GetStateByLeague(league);

            if (state != null)
            {
                state.ChallengesChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetChallengesMessageId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.ChallengesMessageId ?? 0;
        }

        public void SetChallengesMessageId(League league, ulong messageId)
        {
            State state = GetStateByLeague(league);
            if (state != null)
            {
                state.ChallengesMessageId = messageId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetLeaguesChannelId()
        {
            return _statesAtlas.LeaguesChannelId;
        }

        public void SetLeaguesChannelId(ulong channelId)
        {
            if (channelId != 0)
                _statesAtlas.LeaguesChannelId = channelId;
                SaveAndReloadStatesAtlas();
        }

        public ulong GetLeaguesMessageId()
        {
            return _statesAtlas.LeaguesMessageId;
        }

        public void SetLeaguesMessageId(ulong messageId)
        {
            if (messageId != 0)
                _statesAtlas.LeaguesMessageId = messageId;
                SaveAndReloadStatesAtlas();
        }

        public ulong GetStandingsChannelId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.StandingsChannelId ?? 0;
        }        

        public void SetStandingsChannelId(League league, ulong channelId)
        {
            State state = GetStateByLeague(league);

            if (state != null)
            {
                state.StandingsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }        

        public ulong GetStandingsMessageId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.StandingsMessageId ?? 0;
        }

        public void SetStandingsMessageId(League league, ulong messageId)
        {
            State state = GetStateByLeague(league);
            if (state != null)
            {
                state.StandingsMessageId = messageId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetTeamsChannelId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.TeamsChannelId ?? 0;
        }

        public void SetTeamsChannelId(League league, ulong channelId)
        {
            State state = GetStateByLeague(league);

            if (state != null)
            {
                state.TeamsChannelId = channelId;
                SaveAndReloadStatesAtlas();
            }
        }

        public ulong GetTeamsMessageId(League league)
        {
            State state = GetStateByLeague(league);
            return state?.TeamsMessageId ?? 0;
        }

        public void SetTeamsMessageId(League league, ulong messageId)
        {
            State state = GetStateByLeague(league);
            if (state != null)
            {
                state.TeamsMessageId = messageId;
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
