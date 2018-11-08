using ASG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class TicketHistory
        {
            public int Id { get; set; }
            public int TicketId { get; set; }
            public string UserId { get; set; }
            public string Property { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTimeOffset DateChanged { get; set; }

            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }
        }

    }
 