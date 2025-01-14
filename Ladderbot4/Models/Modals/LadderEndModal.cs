using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

namespace Ladderbot4.Models.Modals
{
    public class LadderEndModal : IModal
    {
        public string Title => "End Ladder Confirmation";

        [InputLabel("League Name (Case-Sensitive)")]
        [ModalTextInput("league_name_one", placeholder: "Enter the league name")]
        public string LeagueNameOne { get; set; }

        [InputLabel("League Name (Case-Sensitive)")]
        [ModalTextInput("league_name_two", placeholder: "Enter the league name again")]
        public string LeagueNameTwo { get; set; }        
    }
}
