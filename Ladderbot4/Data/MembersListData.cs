using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;

namespace Ladderbot4.Data
{
    public class MembersListData : Data<MembersList>
    {
        public MembersListData() : base("members_list.json", "Databases")
        {

        }
    }
}
