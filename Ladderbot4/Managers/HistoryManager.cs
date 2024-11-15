using Ladderbot4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class HistoryManager
    {
        private HistoryData _historyData;

        public HistoryManager(HistoryData historyData)
        {
            _historyData = historyData;
        }
    }
}
