//using System.Data.Entity;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System.Collections.Generic;

//namespace ASG.Areas.BugTracker.Models
//{    
//        // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
//        public class ApplicationUser : IdentityUser
//        {
//            public string FirstName { get; set; }
//            public string LastName { get; set; }
//            public string DisplayName { get; set; }

//            public ApplicationUser()
//            {
//                Projects = new HashSet<Project>();
//                TicketAttachments = new HashSet<TicketAttachment>();
//                TicketComments = new HashSet<TicketComment>();
//                TicketHistories = new HashSet<TicketHistory>();
//                TicketNotifications = new HashSet<TicketNotification>();


//            }

//            public virtual ICollection<Project> Projects { get; set; }
//            public virtual ICollection<SystemUpdates> SystemUpdates { get; set; }
//            public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
//            public virtual ICollection<TicketComment> TicketComments { get; set; }
//            public virtual ICollection<TicketHistory> TicketHistories { get; set; }
//            public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

//            public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
//            {
//                // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
//                var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
//                // Add custom user claims here
//                return userIdentity;
//            }
//        }

//        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//        {
//            public ApplicationDbContext()
//                : base("DefaultConnection", throwIfV1Schema: false)
//            {
//            }

//            public static ApplicationDbContext Create()
//            {
//                return new ApplicationDbContext();

//            }
//            public DbSet<ProjectStatus> ProjectStatuses { get; set; }
//            public DbSet<Project> Projects { get; set; }
//            public DbSet<Ticket> Tickets { get; set; }
//            public DbSet<SystemUpdates> SystemUpdates { get; set; }
//            public DbSet<TicketAttachment> TicketAttachments { get; set; }
//            public DbSet<TicketComment> TicketComments { get; set; }
//            public DbSet<TicketHistory> TicketHistories { get; set; }
//            public DbSet<TicketNotification> TicketNotifications { get; set; }
//            public DbSet<TicketPriority> TicketPriorities { get; set; }
//            public DbSet<TicketStatus> TicketStatuses { get; set; }
//            public DbSet<TicketType> TicketTypes { get; set; }

//            //public System.Data.Entity.DbSet<ASG.Areas.BugTracker.Models.ApplicationUser> ApplicationUsers { get; set; }
//        }
//    }
 