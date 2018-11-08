using ASG.Areas.BugTracker.Models;
using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker
{
        public class ProjectAssignHelper
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

        }
    }
