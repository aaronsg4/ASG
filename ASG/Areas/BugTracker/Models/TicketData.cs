using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class TicketData
        {
            public DateTime Day { get; set; }
            public string DayofWeek { get; set; }
            public string Status { get; set; }
            public int StatusA { get; set; }
            public int StatusB { get; set; }
            public int StatusC { get; set; }
            public int TicketCount { get; set; }
        }
    }
