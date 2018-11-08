using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class UserData
        {
            public int Developers { get; set; }
            public int ProjectManagers { get; set; }
            public int Administrators { get; set; }
            public int Submitters { get; set; }
            public int TotalUsers { get; set; }
        }
    }
