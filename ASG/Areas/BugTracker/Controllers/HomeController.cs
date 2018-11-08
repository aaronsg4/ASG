using ASG.Areas.BugTracker.Models;
using ASG.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASG.Areas.BugTracker.Models.Controllers
{  
        public class HomeController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();
            public ActionResult Index()
            {
            // here if no tickets over the last week, create a bunch of tickets
            CreateDemoTickets();
            //delete old demo tickets so the system does accumulate too many
            RemoveOldDemoTickets(); 

            return View();
            }

        public void CreateDemoTickets()
       {
            var sixAgo = DateTime.Now.Date.AddDays(-6);
            var sixDaysAgo = db.Tickets.Where(t => t.CreatedDate == sixAgo);
            if (sixDaysAgo.Count() == 0 && sixAgo.DayOfWeek != DayOfWeek.Saturday && sixAgo.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-6)); };

            var fiveAgo = DateTime.Now.Date.AddDays(-5);
            var fiveDaysAgo = db.Tickets.Where(t => t.CreatedDate == fiveAgo);
            if (fiveDaysAgo.Count() == 0 && fiveAgo.DayOfWeek != DayOfWeek.Saturday && fiveAgo.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-5)); };

            var fourAgo = DateTime.Now.Date.AddDays(-4);
            var fourDaysAgo = db.Tickets.Where(t => t.CreatedDate == fourAgo);
            if (fourDaysAgo.Count() == 0 && fourAgo.DayOfWeek != DayOfWeek.Saturday && fourAgo.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-4)); };

            var threeAgo = DateTime.Now.Date.AddDays(-3);
            var threeDaysAgo = db.Tickets.Where(t => t.CreatedDate == threeAgo);
            if (threeDaysAgo.Count() == 0 && threeAgo.DayOfWeek != DayOfWeek.Saturday && threeAgo.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-3)); };

            var twoAgo = DateTime.Now.Date.AddDays(-2);
            var twoDaysAgo = db.Tickets.Where(t => t.CreatedDate == twoAgo);
            if (twoDaysAgo.Count() == 0 && twoAgo.DayOfWeek != DayOfWeek.Saturday && twoAgo.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-2)); };

            var yesterday = DateTime.Now.Date.AddDays(-1);
            var oneDayAgo = db.Tickets.Where(t => t.CreatedDate == yesterday);
            if (oneDayAgo.Count() == 0 && yesterday.DayOfWeek != DayOfWeek.Saturday && yesterday.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-1)); };

            var today = DateTime.Now.Date.AddDays(-0);
            var zeroDaysAgo = db.Tickets.Where(t => t.CreatedDate == today);
            if (zeroDaysAgo.Count() == 0 && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) { TicketCreator(DateTime.Now.Date.AddDays(-0)); };
        }

        public void RemoveOldDemoTickets ()
        {
            DateTime tenDaysAgo = DateTime.Now.AddDays(-10);
            //rather than archiving, will remove, because these are not true intentionally archived tickets, as you would do if a ticket was closed
            var Tickets = db.Tickets.Where(t => t.CreatedDate < tenDaysAgo && t.Title.Contains("Generator")).ToList();
            foreach (var ticket in Tickets)
            {
                db.Tickets.Remove(ticket);          
            }
            db.SaveChanges();
        }

        public void TicketCreator(DateTime date)

        {
            var ticketList = new List<Ticket>();
            Random r = new Random();
            int rInt = r.Next(0, 10);

            for (var i = 0; i < rInt; i++)
            {
                var ticket = new Ticket();

                ticket.CreatedDate = date;
                ticket.ProjectId = r.Next(1, 3);

                //Random x = new Random();
                int ticketPriorityId = r.Next(1, 4);
                ticket.TicketPriorityId = ticketPriorityId;

                //Random y = new Random();
                int ticketTypeId = r.Next(1, 3);
                ticket.TicketTypeId = ticketTypeId;

                //Random rand = new Random();
                int toSkip = r.Next(0, db.Users.Count());
                var AssignedToUser = db.Users.OrderBy(u=>u.FirstName).Skip(toSkip).Take(1).FirstOrDefault(); 
                ticket.AssignedToUser = AssignedToUser;

                //Random rand2 = new Random();
                int toSkip2 = r.Next(0, db.Users.Count());         
                var SubmitterUser = db.Users.OrderBy(u => u.FirstName).Skip(toSkip2).Take(1).FirstOrDefault();
                ticket.SubmitterUser = SubmitterUser;

                ticket.TicketStatusId = 1;
                string ticketTitleAndDescription = r.Next(0, 100).ToString();
                ticket.Title = "Generator" + ticketTitleAndDescription;
                ticket.Description = "Generator" + ticketTitleAndDescription;

                //ticketList.Add(ticket);
                db.Tickets.Add(ticket);
                //saving inside the loop to create id's so that histories can be updated
                db.SaveChanges();

                var AdminUserId = db.Users.Where(u => u.Email == "Administrator@Mailinator.com").Select(u => u.Id).FirstOrDefault();
                TicketHistory th = new TicketHistory  //filling in all the properties of ticket history
                {
                    TicketId = ticket.Id,
                    UserId = AdminUserId,
                    Property = "Title",
                    OldValue = "N/A",
                    NewValue = ticket.Title,
                    DateChanged = ticket.CreatedDate,

                };
                db.TicketHistories.Add(th);

                TicketHistory th2 = new TicketHistory  //filling in all the properties of ticket history
                {
                    TicketId = ticket.Id,
                    UserId = ticket.SubmitterUserId,
                    Property = "Ticket Submitted by",
                    OldValue = "N/A",
                    NewValue = ticket.SubmitterUser.FullName,
                    DateChanged = ticket.CreatedDate,

                };

                db.TicketHistories.Add(th2);

                TicketHistory th3 = new TicketHistory  //filling in all the properties of ticket history
                {
                    TicketId = ticket.Id,
                    UserId = ticket.SubmitterUserId,
                    Property = "Created Date",
                    OldValue = "N/A",
                    NewValue = ticket.CreatedDate.ToString(),
                    DateChanged = ticket.CreatedDate,

                };

                db.TicketHistories.Add(th3);

                db.SaveChanges();
            }



    
        }

        public PartialViewResult RegistrationConfirmation()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_RegistrationConfirmation.cshtml");
        }

        public PartialViewResult BugTrackerAccess()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_LoginAddRole.cshtml");
        }

        public PartialViewResult BugTrackerForgotPassword()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_ForgotPassword.cshtml");
        }

        public PartialViewResult BugTrackerForgotPasswordConfirmation()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_ForgotPasswordConfirmation.cshtml");
        }

        public PartialViewResult BugTrackerResetPassword()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_ResetPassword.cshtml");
        }

        public PartialViewResult BugTrackerResetPasswordConfirmation()
        {
            return PartialView("~/Areas/BugTracker/Views/Home/_ResetPasswordConfirmation.cshtml");
        }

        public PartialViewResult SystemUpdatesPartial()
            {
                return PartialView("~/Areas/BugTracker/Views/Home/_SystemUpdatesPartialView.cshtml", db.SystemUpdates.ToList());
            }
            public ActionResult Landing()
            {
                List<TicketData> TicketChartList = new List<TicketData>();



                TicketData TicketData1 = new TicketData();

                var dateago = DateTime.Now.Date.AddDays(-6).ToString("d");
                var dateago2 = DateTime.Now.Date.AddDays(-5).ToString("d");
                var dateago3 = DateTime.Now.Date.AddDays(-4).ToString("d");
                var dateago4 = DateTime.Now.Date.AddDays(-3).ToString("d");
                var dateago5 = DateTime.Now.Date.AddDays(-2).ToString("d");
                var dateago6 = DateTime.Now.Date.AddDays(-1).ToString("d");
                var dateago7 = DateTime.Now.Date.AddDays(0).ToString("d");
                
            var tickets = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago)
                    {
                        tickets.Add(ticket);
                    }
                }

                TicketData1.DayofWeek = DateTime.Now.Date.AddDays(-6).DayOfWeek.ToString();
                if (tickets.Count() != 0)
                {
                    TicketData1.StatusA = tickets.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData1.StatusB = tickets.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData1.StatusC = tickets.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData1);
                }
                else
                {
                    TicketData1.StatusA = 0;
                    TicketData1.StatusB = 0; //makes sense go ahead and apply this to every case ok
                    TicketData1.StatusC = 0;
                    TicketChartList.Add(TicketData1);
                }

                var tickets2 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago2)
                    {
                        tickets2.Add(ticket);
                    }
                }

                TicketData TicketData3 = new TicketData();
                TicketData3.DayofWeek = DateTime.Now.Date.AddDays(-5).DayOfWeek.ToString();
                if (tickets2.Count() != 0)
                {
                    TicketData3.StatusA = tickets2.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData3.StatusB = tickets2.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData3.StatusC = tickets2.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData3);
                }
                else
                {
                    TicketData3.StatusA = 0;
                    TicketData3.StatusB = 0;
                    TicketData3.StatusC = 0;
                    TicketChartList.Add(TicketData3);
                }

                var tickets3 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago3)
                    {
                        tickets3.Add(ticket);
                    }
                }

                TicketData TicketData4 = new TicketData();
                TicketData4.DayofWeek = DateTime.Now.Date.AddDays(-4).DayOfWeek.ToString();

                if (tickets3.Count() != 0)
                {
                    TicketData4.StatusA = tickets3.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData4.StatusB = tickets3.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData4.StatusC = tickets3.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData4);
                }
                else
                {
                    TicketData4.StatusA = 0;
                    TicketData4.StatusB = 0;
                    TicketData4.StatusC = 0;
                    TicketChartList.Add(TicketData4);
                }


                var tickets4 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago4)
                    {
                        tickets4.Add(ticket);
                    }
                }


                TicketData TicketData5 = new TicketData();
                TicketData5.DayofWeek = DateTime.Now.Date.AddDays(-3).DayOfWeek.ToString();
                if (tickets4.Count() != 0)
                {
                    TicketData5.StatusA = tickets4.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData5.StatusB = tickets4.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData5.StatusC = tickets4.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData5);
                }
                else
                {
                    TicketData5.StatusA = 0;
                    TicketData5.StatusB = 0;
                    TicketData5.StatusC = 0;
                    TicketChartList.Add(TicketData5);
                }


                var tickets5 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago5)
                    {
                        tickets5.Add(ticket);
                    }
                }

                TicketData TicketData6 = new TicketData();
                TicketData6.DayofWeek = DateTime.Now.Date.AddDays(-2).DayOfWeek.ToString();
                if (tickets5.Count() != 0)
                {


                    TicketData6.StatusA = tickets5.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData6.StatusB = tickets5.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData6.StatusC = tickets5.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData6);
                }
                else
                {
                    TicketData6.StatusA = 0;
                    TicketData6.StatusB = 0;
                    TicketData6.StatusC = 0;
                    TicketChartList.Add(TicketData6);
                }

                var tickets6 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago6)
                    {
                        tickets6.Add(ticket);
                    }
                }

                TicketData TicketData7 = new TicketData();
                TicketData7.DayofWeek = DateTime.Now.Date.AddDays(-1).DayOfWeek.ToString();
                if (tickets6.Count() != 0)
                {



                    TicketData7.StatusA = tickets6.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData7.StatusB = tickets6.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData7.StatusC = tickets6.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData7);
                }
                else
                {

                    TicketData7.StatusA = 0;
                    TicketData7.StatusB = 0;
                    TicketData7.StatusC = 0;
                    TicketChartList.Add(TicketData7);
                }

                var tickets7 = new List<Ticket>();
                foreach (var ticket in db.Tickets)
                {
                    if (ticket.CreatedDate.ToString("d") == dateago7)
                    {
                        tickets7.Add(ticket);
                    }
                }

                TicketData TicketData8 = new TicketData();
                TicketData8.DayofWeek = DateTime.Now.Date.AddDays(0).DayOfWeek.ToString();
                if (tickets7.Count() != 0)
                {


                    TicketData8.StatusA = tickets7.Where(t => t.TicketPriority.Name == "High").Count();
                    TicketData8.StatusB = tickets7.Where(t => t.TicketPriority.Name == "Normal").Count();
                    TicketData8.StatusC = tickets7.Where(t => t.TicketPriority.Name == "Low").Count();
                    TicketChartList.Add(TicketData8);
                }
                else
                {
                    TicketData8.StatusA = 0;
                    TicketData8.StatusB = 0;
                    TicketData8.StatusC = 0;
                    TicketChartList.Add(TicketData8);
                }

       
            List<UserData2> UserChartList = new List<UserData2>();
                UserRolesHelper UH = new UserRolesHelper(db);

                UserData2 User1 = new UserData2();
                var AdminCount = UH.UsersInRole("Admin").Count();
                User1.label = "Administrator";
                User1.value = AdminCount;
                UserChartList.Add(User1);

                UserData2 User2 = new UserData2();
                var DevCount1 = UH.UsersInRole("Developer1").Count();
                var DevCount2 = UH.UsersInRole("Developer2").Count();
                var DevCount3 = UH.UsersInRole("Developer3").Count();
                var DevCount4 = UH.UsersInRole("Developer4").Count();

                User2.label = "Developers";
                User2.value = DevCount1 + DevCount2 + DevCount3 + DevCount4;
                UserChartList.Add(User2);

                UserData2 User3 = new UserData2();
                var PMCount1 = UH.UsersInRole("Project Manager1").Count();
                var PMCount2 = UH.UsersInRole("Project Manager2").Count();
                var PMCount3 = UH.UsersInRole("Project Manager3").Count();


                User3.label = "Project Managers";
                User3.value = PMCount1 + PMCount2 + PMCount3;
                UserChartList.Add(User3);


                UserData2 User4 = new UserData2();
                var SubmitterCount = UH.UsersInRole("Submitter").Count();
                User4.label = "Submitter";
                User4.value = SubmitterCount;
                UserChartList.Add(User4);
            

                ViewBag.ArrData2 = UserChartList.ToArray();
                ViewBag.ArrData = TicketChartList.ToArray();

                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper ph = new ProjectAssignHelper();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var userroleAdmin = helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator");
                var userroleDeveloper = helper.IsUserInRole(userId, "Developer1") || helper.IsUserInRole(userId, "Developer2") || helper.IsUserInRole(userId, "Developer3") || helper.IsUserInRole(userId, "Developer4");
                var userroleProjectManager = helper.IsUserInRole(userId, "Project Manager1") || helper.IsUserInRole(userId, "Project Manager2") || helper.IsUserInRole(userId, "Project Manager3");
                var userroleSubmitter = helper.IsUserInRole(userId, "Submitter");
                List<Project> DeveloperProjects = new List<Project>();
                List<ApplicationUser> Developers = helper.UsersInRole("Developer1").Concat(helper.UsersInRole("Developer2")).Concat(helper.UsersInRole("Developer3")).Concat(helper.UsersInRole("Developer4")).ToList();
                        

                List<Ticket> usertickets = new List<Ticket>();
                List<Project> userprojects = new List<Project>();
                if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator"))
                {
                    foreach (var project in db.Projects)
                    {
                        if (ph.IsUserOnAProject(userId, project.Id))
                        {
                            userprojects.Add(project);
                        }
                    }


                    foreach (var ticket in db.Tickets)
                    {

                        usertickets.Add(ticket);
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.message = "in the Bug Tracker system currently.";
                    ViewBag.ticketmessage = "in the Bug Tracker system currently.";
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(userprojects);
                }

                else if (userroleProjectManager)
                {


                    foreach (var x in db.Projects)
                    {

                        if (ph.IsUserOnAProject(userId, x.Id))
                        {
                            userprojects.Add(x);
                        }

                    }

                    foreach (var project in userprojects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }

                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.ticketmessage = "for projects that you're working on.";
                    ViewBag.message = "that you're working on.";
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }

                    return View(userprojects);
                }


                else if (userroleDeveloper)
                {


                    foreach (var x in db.Projects)
                    {

                        if (ph.IsUserOnAProject(userId, x.Id))
                        {
                            userprojects.Add(x);
                        }

                    }

                    foreach (var project in userprojects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }

                    }

                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");

                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    var urgentticketscount = urgenttickets.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.ticketmessage = "for projects that you're working on.";
                    ViewBag.message = "that you're working on.";
                    if (urgentticketscount == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(userprojects);

                }

                else if (userroleSubmitter)
                {
                    var submittertickets = db.Tickets.Where(t => t.SubmitterUserId == userId).ToList();

                    var nonarchivedtickets = submittertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.ticketmessage = "that you've submitted.";
                    ViewBag.projectmessage = "N/A";
                    ViewBag.urgentmessage = "N/A";
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";

                    var projects = db.Projects.ToList();
                    return View(projects);
                }
                return RedirectToAction("Index", "Home");
            }


            public ActionResult LayoutPartial()
            {

                var currentuser = db.Users.Find(User.Identity.GetUserId());
                if (currentuser != null)
                {


                    UserRolesHelper uh = new UserRolesHelper(db);
                    UsersRolesViewModel vm = new UsersRolesViewModel();
                    vm.User = db.Users.Find(User.Identity.GetUserId());
                    vm.Roles = uh.ListUserRoles(vm.User.Id);

                    UserRolesHelper helper = new UserRolesHelper(db);
                    ProjectAssignHelper ph = new ProjectAssignHelper();
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    var userroleAdmin = helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator");
                    var userroleDeveloper = helper.IsUserInRole(userId, "Developer1") || helper.IsUserInRole(userId, "Developer2") || helper.IsUserInRole(userId, "Developer3") || helper.IsUserInRole(userId, "Developer4");
                    var userroleProjectManager = helper.IsUserInRole(userId, "Project Manager1") || helper.IsUserInRole(userId, "Project Manager2") || helper.IsUserInRole(userId, "Project Manager3");
                    var userroleSubmitter = helper.IsUserInRole(userId, "Submitter");
                    List<Project> DeveloperProjects = new List<Project>();
                    List<ApplicationUser> Developers = helper.UsersInRole("Developer1").Concat(helper.UsersInRole("Developer2")).Concat(helper.UsersInRole("Developer3")).Concat(helper.UsersInRole("Developer4")).ToList();



                    List<Project> userprojects = new List<Project>();
                    foreach (var project in db.Projects)
                    {
                        if (ph.IsUserOnAProject(userId, project.Id))
                        {
                            userprojects.Add(project);
                        }
                    }

                    ViewBag.projectcount = userprojects.Count();


                    List<Ticket> usertickets = new List<Ticket>();

                    if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator"))
                    {
                        foreach (var ticket in db.Tickets)
                        {

                            usertickets.Add(ticket);
                        }
                        ViewBag.ticketcount = usertickets.Count();

                    }

                    else if (userroleProjectManager)
                    {

                        List<Project> UserProjects = new List<Project>();
                        foreach (var x in db.Projects)
                        {

                            if (ph.IsUserOnAProject(userId, x.Id))
                            {
                                UserProjects.Add(x);
                            }

                        }

                        foreach (var project in UserProjects)
                        {
                            foreach (var ticket in project.Tickets)
                            {
                                usertickets.Add(ticket);
                            }

                        }
                        var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                        var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                        ViewBag.ticketcount = nonarchivedtickets.Count();
                        ViewBag.urgentticketcount = urgenttickets.Count();

                    }


                    else if (userroleDeveloper)
                    {

                        List<Project> UserProjects = new List<Project>();
                        foreach (var x in db.Projects)
                        {

                            if (ph.IsUserOnAProject(userId, x.Id))
                            {
                                UserProjects.Add(x);
                            }

                        }

                        foreach (var project in UserProjects)
                        {
                            foreach (var ticket in project.Tickets)
                            {
                                usertickets.Add(ticket);
                            }

                        }
                        var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");

                        ViewBag.ticketcount = nonarchivedtickets.Count();

                    }

                    else if (userroleSubmitter)
                    {
                        var tickets = db.Tickets.Where(t => t.SubmitterUserId == userId).ToList();

                        var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");
                        ViewBag.ticketcount = nonarchivedtickets.Count();

                    }

                    return PartialView("~/Areas/BugTracker/Views/Shared/_LayoutPartial.cshtml", vm);
                }

                return RedirectToAction("Index", "Home", new { area = "BugTracker"});
            }


            public ActionResult SidebarPartial2()
            {

                var currentuser = db.Users.Find(User.Identity.GetUserId());
                if (currentuser != null)
                {

                    ProjectAssignHelper ph = new ProjectAssignHelper();
                    UserRolesHelper uh = new UserRolesHelper(db);
                    UsersRolesViewModel vm = new UsersRolesViewModel();
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    vm.User = db.Users.Find(User.Identity.GetUserId());
                    vm.Roles = uh.ListUserRoles(vm.User.Id);
                    if (uh.IsUserInRole(userId, "Admin") || uh.IsUserInRole(userId, "Administrator") || uh.IsUserInRole(userId, "Project Manager1") || uh.IsUserInRole(userId, "Project Manager2") || uh.IsUserInRole(userId, "Project Manager3"))
                    {
                        vm.Projectsb = db.Projects.ToList();
                    }
                    else
                    {
                        vm.Projectsb = ph.ListProjectsForAUser(userId);
                    }

                    return PartialView("~/Areas/BugTracker/Views/Projects/_SidebarPartial2.cshtml", vm);
                }
                return RedirectToAction("Index", "Home");
            }



            public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
        }
    }
