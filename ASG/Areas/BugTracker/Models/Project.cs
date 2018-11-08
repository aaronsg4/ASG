using ASG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class Project
        {

        private ApplicationDbContext db = new ApplicationDbContext();
        public bool IsUserOnAProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return flag;
        }

        public void AddUserToAProject(string userId, int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.Users.Add(user);
            db.SaveChanges();
        }
        public void RemoveUserFromAProject(string userId, int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.SaveChanges();
        }
        public List<Project> ListProjectsForAUser(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            return user.Projects.ToList();
        }
        public List<ApplicationUser> ListUsersOnAProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }
        public List<ApplicationUser> ListUsersNotOnAProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            var usersOnProject = project.Users;
            return project.Users.Where(u => !usersOnProject.Contains(u)).ToList();
        }

        public Project()
            {
                Users = new HashSet<ApplicationUser>();
                Tickets = new HashSet<Ticket>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string ProjectManagerName { get; set; }
            public string ProjectManagerId { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTimeOffset? ProjectStartDate { get; set; }
            public string ProjectStatus { get; set; }
            public int ProjectStatusId { get; set; }

            public virtual ProjectStatus ProjectStatuses { get; set; }
            public virtual ICollection<Ticket> Tickets { get; set; }

            public virtual ICollection<ApplicationUser> Users { get; set; }
        }
    }
