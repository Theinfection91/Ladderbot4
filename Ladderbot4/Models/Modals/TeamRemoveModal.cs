using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

namespace Ladderbot4.Models.Modals
{
    public class TeamRemoveModal : IModal
    {
        public string Title => "Remove Team Confirmation";

        [InputLabel("Team Name (Case-Sensitive)")]
        [ModalTextInput("team_name_one", placeholder: "Enter the team name...")]
        public string TeamNameOne { get; set; }

        [InputLabel("Team Name (Case-Sensitive)")]
        [ModalTextInput("team_name_two", placeholder: "Enter the team name again...")]
        public string TeamNameTwo { get; set; }
    }
}
