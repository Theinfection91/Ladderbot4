using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class MembersList
    {
        public List<MemberProfile> Members { get; set; }

        public MembersList()
        {
            Members = [];
        }
    }
}
