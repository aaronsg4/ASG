using ASG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class TicketComment
        {

            public int Id { get; set; }
            public int TicketId { get; set; }
            public string UserId { get; set; }
            public string Comment { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTimeOffset CreatedDate { get; set; }


            public virtual ApplicationUser User { get; set; }
            public virtual Ticket Ticket { get; set; }

        }
    }
