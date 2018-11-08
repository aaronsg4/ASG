using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ASG.Areas.BugTracker.Models
{
        public class UsersRolesViewModel
        {
            public ApplicationDbContext db { get; set; }
            public string UserId { get; set; }
            public ApplicationUser User { get; set; }
            public IList<string> Roles { get; set; }
            public IList<string> Projects { get; set; }
            public virtual ICollection<Ticket> Tickets { get; set; }
            public virtual ICollection<ApplicationUser> Users { get; set; }
            public virtual ICollection<Project> Projectsb { get; set; }
        }

        public class ProjectAssignViewModel
        {
        public MultiSelectList Users { get; set; }
        public int? projectId { get; set; }
        public string[] selectedvalue { get; set; }

        //public List<ApplicationUser> Users { get; set; }
        //public int? projectId { get; set; }
        //public string [] selectedvalue { get; set; }
    }
        public class TicketAssignViewModel
        {
            public MultiSelectList Users { get; set; }
            public int? TicketId { get; set; }
            public string[] selectedvalue { get; set; }
        }

    }
