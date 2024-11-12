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
        public State States1v1 { get; set; } = new State();

        // 2v2 Specific
        public State States2v2 { get; set; } = new State();

        // 3v3 Specific
        public State States3v3 { get; set; } = new State();

        public StatesByDivision()
        {

        }
    }
}
