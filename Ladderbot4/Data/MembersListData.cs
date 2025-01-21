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

        public void AddMemberProfile(MemberProfile member)
        {
            MembersList membersList = Load();

            if (membersList != null)
            {
                membersList.Members.Add(member);

                Save(membersList);
            }
        }

        public void RemoveMemberProfile(MemberProfile member)
        {
            // TODO - Don't really need for now as MemberProfile's aren't meant to be removed from the list.
            // Could probably use later on to remove old profiles on list after so much time, but that's advanced and not needed for now.
        }
    }
}
