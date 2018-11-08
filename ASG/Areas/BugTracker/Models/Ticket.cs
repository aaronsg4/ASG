using ASG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class Ticket
        {

            public Ticket()
            {
                TicketAttachments = new HashSet<TicketAttachment>();
                TicketComments = new HashSet<TicketComment>();
                TicketHistories = new HashSet<TicketHistory>();
                TicketNotifications = new HashSet<TicketNotification>();
            }


            public int Id { get; set; }
            public int ProjectId { get; set; }
            public int TicketTypeId { get; set; }
            public int TicketPriorityId { get; set; }
            public int TicketStatusId { get; set; }

            public string SubmitterUserId { get; set; }
            public string AssignedToUserId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTime CreatedDate { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTime? UpdatedDate { get; set; }

            public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
            public virtual ICollection<TicketComment> TicketComments { get; set; }
            public virtual ICollection<TicketHistory> TicketHistories { get; set; }
            public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
            public virtual ApplicationUser SubmitterUser { get; set; }
            public virtual ApplicationUser AssignedToUser { get; set; }
            public virtual Project Project { get; set; }
            public virtual TicketStatus TicketStatus { get; set; }
            public virtual TicketPriority TicketPriority { get; set; }
            public virtual TicketType TicketType { get; set; }


        }
    }
