using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ASG.Models;

namespace ASG.Areas.BugTracker.Models
{
        public class SystemUpdates
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTime? UpdateDate { get; set; }

            public virtual ApplicationUser User { get; set; }
        }
    }
