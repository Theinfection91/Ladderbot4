﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class MembersList
    {
        public List<Member> AllMembers { get; set; }

        public MembersList()
        {
            AllMembers = [];
        }
    }
}