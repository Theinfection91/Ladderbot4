using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class StatesByDivision
    {
        // 1v1 Specific
        public List<State> States1v1 { get; set; }

        // 2v2 Specific
        public List<State> States2v2 { get; set; }

        // 3v3 Specific
        public List<State> States3v3 { get; set; }

        public StatesByDivision()
        {
            States1v1 = [];
            States2v2 = [];
            States3v3 = [];
        }
    }
}
