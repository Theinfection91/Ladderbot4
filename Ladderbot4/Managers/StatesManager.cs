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

            switch (leagueRef.Division)
            {
                case "1v1":
                    states = _statesByDivision.States1v1;
                    break;

                case "2v2":
                    states = _statesByDivision.States2v2; // Assuming States2v2 exists
                    break;

                case "3v3":
                    states = _statesByDivision.States3v3; // Assuming States3v3 exists
                    break;

                default:
                    return null;
            }

            return states?.FirstOrDefault(state =>
                state.LeagueName.Equals(leagueRef.LeagueName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsLadderRunning(League leagueRef)
        {
            switch (leagueRef.Division)
            {
                case "1v1":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                case "2v2":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                case "3v3":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (state.LeagueName.Equals(leagueRef.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            return state.IsLadderRunning;
                        }
                    }
                    return false;

                default:
                    return false;
            }
        }

        //public bool IsLadderRunning(string division)
        //{
        //    LoadStatesDatabase();

        //    switch (division)
        //    {
        //        case "1v1":
        //            return _statesByDivision.States1v1.IsLadderRunning;

        //        case "2v2":
        //            return _statesByDivision.States2v2.IsLadderRunning;

        //        case "3v3":
        //            return _statesByDivision.States3v3.IsLadderRunning;

        //        default:
        //            throw new ArgumentException("Invalid division type given.");
        //    }
        //}

        //public ulong GetChallengesChannelId(string division)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            return _statesByDivision.States1v1.ChallengesChannelId;

        //        case "2v2":
        //            return _statesByDivision.States2v2.ChallengesChannelId;

        //        case "3v3":
        //            return _statesByDivision.States3v3.ChallengesChannelId;

        //        default:
        //            return 0;
        //    }
        //}

        //public void SetChallengesChannelId(string division, ulong channelId)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            _statesByDivision.States1v1.ChallengesChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "2v2":
        //            _statesByDivision.States2v2.ChallengesChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "3v3":
        //            _statesByDivision.States3v3.ChallengesChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;
        //    }
        //}

        //public ulong GetStandingsChannelId(string division)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            return _statesByDivision.States1v1.StandingsChannelId;

        //        case "2v2":
        //            return _statesByDivision.States2v2.StandingsChannelId;

        //        case "3v3":
        //            return _statesByDivision.States3v3.StandingsChannelId;

        //        default:
        //            return 0;
        //    }
        //}

        //public void SetStandingsChannelId(string division, ulong channelId)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            _statesByDivision.States1v1.StandingsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "2v2":
        //            _statesByDivision.States2v2.StandingsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "3v3":
        //            _statesByDivision.States3v3.StandingsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;
        //    }
        //}

        //public ulong GetTeamsChannelId(string division)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            return _statesByDivision.States1v1.TeamsChannelId;

        //        case "2v2":
        //            return _statesByDivision.States2v2.TeamsChannelId;

        //        case "3v3":
        //            return _statesByDivision.States3v3.TeamsChannelId;

        //        default:
        //            return 0;
        //    }
        //}

        //public void SetTeamsChannelId(string division, ulong channelId)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            _statesByDivision.States1v1.TeamsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "2v2":
        //            _statesByDivision.States2v2.TeamsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;

        //        case "3v3":
        //            _statesByDivision.States3v3.TeamsChannelId = channelId;
        //            SaveAndReloadStatesDatabase();
        //            break;
        //    }
        //}

        public void SetLadderRunning(League leagueRef, bool trueOrFalse)
        {
            switch (leagueRef.Division)
            {
                case "1v1":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.LeagueName.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;

                case "2v2":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.LeagueName.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;

                case "3v3":
                    foreach (State state in _statesByDivision.States1v1)
                    {
                        if (leagueRef.LeagueName.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase))
                        {
                            state.IsLadderRunning = trueOrFalse;
                        }
                    }
                    break;
            }
        }

        public State CreateStateObject(string leagueName)
        {
            return new State()
            {
                LeagueName = leagueName,
                IsLadderRunning = false,
                ChallengesChannelId = 0,
                StandingsChannelId = 0,
                TeamsChannelId = 0
            };
        }

        //public void SetLadderRunning(string division, bool trueOrFalse)
        //{
        //    switch (division)
        //    {
        //        case "1v1":
        //            _statesByDivision.States1v1.IsLadderRunning = trueOrFalse;
        //            break;

        //        case "2v2":
        //            _statesByDivision.States2v2.IsLadderRunning = trueOrFalse;
        //            break;

        //        case "3v3":
        //            _statesByDivision.States3v3.IsLadderRunning = trueOrFalse;
        //            break;
        //    }
        //}
    }
}
