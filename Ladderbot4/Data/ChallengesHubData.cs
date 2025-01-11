using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;

namespace Ladderbot4.Data
{
    public class ChallengesHubData : Data<ChallengesHub>
    {
        public ChallengesHubData() : base("challenges_hub.json", "Databases")
        {

        }

    }
}
