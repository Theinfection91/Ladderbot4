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

        public bool IsLadderRunning(string division)
        {
            LoadStatesDatabase();

            switch (division)
            {
                case "1v1":
                    return _statesByDivision.States1v1.IsLadderRunning;

                case "2v2":
                    return _statesByDivision.States2v2.IsLadderRunning;

                case "3v3":
                    return _statesByDivision.States3v3.IsLadderRunning;

                default:
                    throw new ArgumentException("Invalid division type given.");
            }
        }

        public void SetLadderRunning(string division, bool trueOrFalse)
        {
            switch (division)
            {
                case "1v1":
                    _statesByDivision.States1v1.IsLadderRunning = trueOrFalse;
                    break;

                case "2v2":
                    _statesByDivision.States2v2.IsLadderRunning = trueOrFalse;
                    break;

                case "3v3":
                    _statesByDivision.States3v3.IsLadderRunning = trueOrFalse;
                    break;
            }
        }
    }
}
