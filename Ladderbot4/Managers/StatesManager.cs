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
        private readonly LadderData _ladderData;

        private StatesByDivision _statesByDivision;

        public StatesManager(LadderData ladderData)
        {
            _ladderData = ladderData;
            _statesByDivision = _ladderData.LoadAllStates();
        }

        public void LoadStatesDatabase()
        {
            _statesByDivision = _ladderData.LoadAllStates();
        }

        public void SaveStatesDatabase()
        {
            _ladderData.SaveAllStates(_statesByDivision);
        }

        public void SaveAndReloadStatesDatabase()
        {
            SaveStatesDatabase();
            LoadStatesDatabase();
        }

        public State GetStateByLeague(League leagueRef)
        {
            IEnumerable<State> states;

            switch (leagueRef.Format)
            {
                case "1v1":
                    states = _statesByDivision.States1v1;
                    break;

                case "2v2":
                    states = _statesByDivision.States2v2;
                    break;

                case "3v3":
                    states = _statesByDivision.States3v3;
                    break;

                default:
                    return null;
            }

            return states?.FirstOrDefault(state =>
                state.LeagueName.Equals(leagueRef.Name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsLadderRunning(League leagueRef)
        {
            switch (leagueRef.Format)
            {
                case "1v1":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                case "2v2":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                case "3v3":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                default:
                    return false;
            }
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
                SaveAndReloadStatesDatabase();
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
                SaveAndReloadStatesDatabase();
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
                SaveAndReloadStatesDatabase();
            }
        }

        public void SetLadderRunning(League leagueRef, bool trueOrFalse)
        {
            switch (leagueRef.Format)
            {
                case "1v1":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.Name.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;

                case "2v2":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.Name.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;

                case "3v3":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.Name.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;
            }
        }

        public State CreateNewState(string leagueName, string leagueDivision)
        {
            return new State(leagueName, leagueDivision)
            {
                IsLadderRunning = false,
                ChallengesChannelId = 0,
                StandingsChannelId = 0,
                TeamsChannelId = 0
            };
        }

        public void AddNewState(State state)
        {
            _ladderData.AddState(state);

            LoadStatesDatabase();
        }

        public void RemoveLeagueState(string leagueName, string division)
        {
            _ladderData.RemoveState(leagueName, division);

            LoadStatesDatabase();
        }
    }
}
