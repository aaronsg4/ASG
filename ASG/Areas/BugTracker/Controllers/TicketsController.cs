using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASG.Areas.BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;
using ASG.Models;

namespace ASG.Areas.BugTracker.Models.Controllers
{    
        [System.Runtime.InteropServices.Guid("B3F8489D-9EEE-4AD9-959D-AF4636B55EFA")]
        public class TicketsController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();
            ProjectAssignHelper pHelper = new ProjectAssignHelper();
                
            //GET: Tickets
            public ActionResult Index()
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper pHelper = new ProjectAssignHelper();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var userroleAdmin = helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator");
                var userroleDeveloper1 = helper.IsUserInRole(userId, "Developer1");
                var userroleDeveloper2 = helper.IsUserInRole(userId, "Developer2");
                var userroleDeveloper3 = helper.IsUserInRole(userId, "Developer3");
                var userroleDeveloper4 = helper.IsUserInRole(userId, "Developer4");
                var userroleProjectManager1 = helper.IsUserInRole(userId, "Project Manager1");
                var userroleProjectManager2 = helper.IsUserInRole(userId, "Project Manager2");
                var userroleProjectManager3 = helper.IsUserInRole(userId, "Project Manager3");
                var userroleSubmitter = helper.IsUserInRole(userId, "Submitter");
                List<Project> userprojects = new List<Project>();
                List<Ticket> usertickets = new List<Ticket>();

                if (userroleAdmin)
                {
                    var tickets = db.Tickets;
                    var projects = db.Projects;
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");


                    var userprojects2 = projects.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.projectcount = projects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.message = "in the BugTracker currently";
                    ViewBag.projectmessage = "in the BugTracker currently";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(nonarchivedtickets.ToList());



                }
                else if (userroleProjectManager1 || userroleProjectManager2 || userroleProjectManager3)
                {
                    var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
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
                    var nonarchivedtickets2 = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets2.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets2.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets2.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.message = "for projects that you're working on.";
                    ViewBag.projectmessage = "that you're working on";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }

                    return View(nonarchivedtickets2.ToList());

                }

                else if (userroleDeveloper1)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
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
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }


                    return View(nonarchivedtickets);

                }
                else if (userroleDeveloper2)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    var urgentticketscount = urgenttickets.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgentticketscount == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(nonarchivedtickets);


                }

                else if (userroleDeveloper3)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(nonarchivedtickets);

                }

                else if (userroleDeveloper4)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're woking on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                    return View(nonarchivedtickets);
                }


                else if (userroleSubmitter)
                {
                    var tickets = db.Tickets.Where(t => t.SubmitterUserId == userId).ToList();
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    return View(nonarchivedtickets);
                }
                List<Ticket> Tickets = new List<Ticket>();
                return View(Tickets);
            }


            public ActionResult ArchivedTickets()
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper pHelper = new ProjectAssignHelper();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var userroleAdmin = helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator");
                var userroleDeveloper1 = helper.IsUserInRole(userId, "Developer1");
                var userroleDeveloper2 = helper.IsUserInRole(userId, "Developer2");
                var userroleDeveloper3 = helper.IsUserInRole(userId, "Developer3");
                var userroleDeveloper4 = helper.IsUserInRole(userId, "Developer4");
                var userroleProjectManager1 = helper.IsUserInRole(userId, "Project Manager1");
                var userroleProjectManager2 = helper.IsUserInRole(userId, "Project Manager2");
                var userroleProjectManager3 = helper.IsUserInRole(userId, "Project Manager3");
                var userroleSubmitter = helper.IsUserInRole(userId, "Submitter");


                if (userroleAdmin)
                {
                    var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
                    var archivedtickets = tickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets.ToList());
                }
                else if (userroleProjectManager1 || userroleProjectManager2 || userroleProjectManager3)
                {
                    var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
                    var archivedtickets = tickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets.ToList());
                }

                else if (userroleDeveloper1)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    List<Ticket> usertickets = new List<Ticket>();
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var archivedtickets = usertickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets);
                }
                else if (userroleDeveloper2)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    List<Ticket> usertickets = new List<Ticket>();
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }

                    }
                    var archivedtickets = usertickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets);
                }

                else if (userroleDeveloper3)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }


                    List<Ticket> usertickets = new List<Ticket>();
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var archivedtickets = usertickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets);
                }

                else if (userroleDeveloper4)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }


                    List<Ticket> usertickets = new List<Ticket>();
                    foreach (var project in UserProjects)
                    {
                        foreach (var ticket in project.Tickets)
                        {
                            usertickets.Add(ticket);
                        }
                    }
                    var archivedtickets = usertickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets);
                }


                else if (userroleSubmitter)
                {
                    var tickets = db.Tickets.Where(t => t.SubmitterUserId == userId).ToList();
                    var archivedtickets = tickets.Where(t => t.TicketStatus.Name == "Archived");
                    return View(archivedtickets);
                }
                List<Ticket> Tickets = new List<Ticket>();
                return View(Tickets);
            }

            [Authorize(Roles = "Admin,Administrator,Project Manager1,Project Manager2,Project Manager3, Developer1, Developer2, Developer3, Developer4")]
            public ActionResult MyTickets()
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var nonarchivedtickets = db.Tickets.Where(t => t.TicketStatus.Name == "Assigned");

                return View(nonarchivedtickets.Where(t => t.AssignedToUserId == userId).ToList());
            }

            //[Authorize(Roles = "Admin,Administrator,Project Manager1,Project Manager2,Project Manager3")]
            public ActionResult MyArchivedTickets()
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var archivedtickets = db.Tickets.Where(t => t.TicketStatus.Name == "Archived");
                return View(archivedtickets.Where(t => t.AssignedToUserId == userId).ToList());
            }



            // GET: Tickets/Details/5
            public ActionResult Details(int? id)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                UserRolesHelper UH = new UserRolesHelper(db);
                ProjectAssignHelper PA = new ProjectAssignHelper();
                Ticket ticket = db.Tickets.Find(id);
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ticket == null)
                {
                    return HttpNotFound();
                }


                var userroleAdmin = UH.IsUserInRole(userId, "Admin") || UH.IsUserInRole(userId, "Administrator");
                var userroleDeveloper1 = UH.IsUserInRole(userId, "Developer1");
                var userroleDeveloper2 = UH.IsUserInRole(userId, "Developer2");
                var userroleDeveloper3 = UH.IsUserInRole(userId, "Developer3");
                var userroleDeveloper4 = UH.IsUserInRole(userId, "Developer4");
                var userroleProjectManager1 = UH.IsUserInRole(userId, "Project Manager1");
                var userroleProjectManager2 = UH.IsUserInRole(userId, "Project Manager2");
                var userroleProjectManager3 = UH.IsUserInRole(userId, "Project Manager3");
                var userroleSubmitter = UH.IsUserInRole(userId, "Submitter");
                List<Project> userprojects = new List<Project>();
                List<Ticket> usertickets = new List<Ticket>();

                if (userroleAdmin)
                {
                    var tickets = db.Tickets;
                    var projects = db.Projects;
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");


                    var userprojects2 = projects.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.projectcount = projects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.message = "in the BugTracker currently";
                    ViewBag.projectmessage = "in the BugTracker currently";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }                   

                }
                else if (userroleProjectManager1 || userroleProjectManager2 || userroleProjectManager3)
                {
                    var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            userprojects.Add(x);
                        }
                    }

                    foreach (var project in userprojects)
                    {
                        foreach (var tckt in project.Tickets)
                        {
                            usertickets.Add(tckt);
                        }
                    }
                    var nonarchivedtickets2 = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    var urgenttickets = nonarchivedtickets2.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets2.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.ticketcount = nonarchivedtickets2.Count();
                    ViewBag.projectcount = userprojects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.message = "for projects that you're working on.";
                    ViewBag.projectmessage = "that you're working on";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                }

                else if (userroleDeveloper1)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }
                    
                    foreach (var project in UserProjects)
                    {
                        foreach (var tk in project.Tickets)
                        {
                            usertickets.Add(tk);
                        }
                    }

                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }                    
                }
                else if (userroleDeveloper2)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }
           
                    foreach (var project in UserProjects)
                    {
                        foreach (var tk in project.Tickets)
                        {
                            usertickets.Add(tk);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    var urgentticketscount = urgenttickets.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgentticketscount == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                }

                else if (userroleDeveloper3)
                {

                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }

                    //return View(UserProjects);
                    foreach (var project in UserProjects)
                    {
                        foreach (var tk in project.Tickets)
                        {
                            usertickets.Add(tk);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're working on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }                    
                }

                else if (userroleDeveloper4)
                {
                    List<Project> UserProjects = new List<Project>();
                    foreach (var x in db.Projects)
                    {
                        if (pHelper.IsUserOnAProject(userId, x.Id))
                        {
                            UserProjects.Add(x);
                        }
                    }
                
                    foreach (var project in UserProjects)
                    {
                        foreach (var tk in project.Tickets)
                        {
                            usertickets.Add(tk);
                        }
                    }
                    var nonarchivedtickets = usertickets.Where(t => t.TicketStatus.Name != "Archived");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    var urgenttickets = nonarchivedtickets.Where(t => t.TicketPriority.Name == "High");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    ViewBag.projectcount = UserProjects.Count();
                    ViewBag.unassignedmessage = "Unassigned tickets in the Bug Tracker system";
                    ViewBag.urgentticketcount = urgenttickets.Count();
                    ViewBag.projectmessage = "that you're working on";
                    ViewBag.message = "for projects that you're woking on.";
                    if (urgenttickets.Count() == 0)
                    {
                        ViewBag.urgentmessage = "There are no urgent tickets.";
                    }
                    else
                    {
                        ViewBag.urgentmessage = "Please attend to these tickets if you are assigned to them.";
                    }
                }


                else if (userroleSubmitter)
                {
                    var tickets = db.Tickets.Where(t => t.SubmitterUserId == userId).ToList();
                    var nonarchivedtickets = tickets.Where(t => t.TicketStatus.Name != "Archived");
                    var unassignedtickets = nonarchivedtickets.Where(t => t.TicketStatus.Name == "Unassigned");
                    ViewBag.ticketcount = nonarchivedtickets.Count();
                    ViewBag.unassignedticketcount = unassignedtickets.Count();
                    return View(nonarchivedtickets);
                }
                List<Ticket> Tickets = new List<Ticket>();


                if (!(UH.IsUserInRole(userId, "Admin") || UH.IsUserInRole(userId, "Administrator")))
                {
                    if (!(UH.IsUserInRole(userId, "Project Manager1") && PA.IsUserOnAProject(userId, ticket.ProjectId)) && (!(UH.IsUserInRole(userId, "Project Manager2") && PA.IsUserOnAProject(userId, ticket.ProjectId)) && (!(UH.IsUserInRole(userId, "Project Manager3") && PA.IsUserOnAProject(userId, ticket.ProjectId)))))
                    {
                        if (!(ticket.AssignedToUserId == userId))
                        {
                            if (!(ticket.SubmitterUserId == userId))
                            {
                                var Temporary = "You cannot view Details of this ticket.  Please revisit your role assignment.  Tickets details can only be  viewed by their creator, a developer assigned to the ticket, a project manager, or an administrator.";
                                TempData["message"] = Temporary;
                                return RedirectToAction("Index", "Tickets", new { area = "BugTracker" });
                            }
                        }
                    }
                }
                else
                {
                    return View(ticket);
                }

                return View(ticket);
            }

            //[Authorize(Roles = "Submitter")]
            // GET: Tickets/Create
            public ActionResult Create()
            {


                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper ph = new ProjectAssignHelper();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var userroleSubmitter = helper.IsUserInRole(userId, "Submiter");


            var userRole = helper.ListUserRoles(userId);

            var userprojects = user.Projects.ToList();

            //if user is in any of these roles let them see all projects
            if (userRole.Contains("Admin") || userRole.Contains("Administrator") || (userRole.Contains("Project Manager1") || (userRole.Contains("Project Manager2") || (userRole.Contains("Project Manager3") || (userRole.Contains("Admin2"))))))
            {
                userprojects = db.Projects.Where(p => p.ProjectStatusId != 3).ToList();
            }   
            
                if (userprojects.Count > 0)
                {

                    ViewBag.ProjectId = new SelectList(userprojects, "Id", "Name");
                    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                    return View();
                }


                var Temporary = "You can only create tickets for projects that you are assigned to.";
                TempData["createticketmessage"] = Temporary;
                return RedirectToAction("Index", "Projects");

            }
            // TicketAssign GET
            public ActionResult TicketAssign(int? Id)


            {
                UserRolesHelper helper = new UserRolesHelper(db);
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                if (Id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ticket ticket = db.Tickets.Find(Id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }

                List<ApplicationUser> Developers = new List<ApplicationUser>();
                var devs = helper.UsersInRole("Developer1");
                var devs2 = helper.UsersInRole("Developer2");
                var devs3 = helper.UsersInRole("Developer3");
                var devs4 = helper.UsersInRole("Developer4");
                var project = ticket.Project;
                foreach (var dev in devs)
                {
                    if (dev.Projects.Contains(project))
                    {
                        Developers.Add(dev);
                    }
                }

                foreach (var dev in devs2)
                {
                    if (dev.Projects.Contains(project))
                    {
                        Developers.Add(dev);
                    }
                }

                foreach (var dev in devs3)
                {
                    if (dev.Projects.Contains(project))
                    {
                        Developers.Add(dev);
                    }
                }
                foreach (var dev in devs4)
                {
                    if (dev.Projects.Contains(project))
                    {
                        Developers.Add(dev);
                    }
                }                

                TempData["Devs"] = Developers;
                ViewBag.AssignedToUserId = new SelectList(Developers, "Id", "FullName");
                return View(ticket);
            }


            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> TicketAssign([Bind(Include = "Id,AssignedToUserId")] Ticket ticket)
            {
                if (ModelState.IsValid)
                {
                    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id.Equals(ticket.Id));
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    ticket.UpdatedDate = DateTime.Now;

                    int assignedId = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Assigned").Id;


                    ticket.TicketStatusId = assignedId;            

                    db.Tickets.Attach(ticket);
                    db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                    db.Entry(ticket).Property("AssignedToUserId").IsModified = true;
                    db.SaveChanges();


                    TicketNotification TN = new TicketNotification();
                    TN.TicketId = ticket.Id;
                    TN.UserId = ticket.AssignedToUserId;
                    TN.User = ticket.AssignedToUser;
                    db.TicketNotifications.Add(TN);

                    // Creating an instance of Ticket Assigned To User object to be able to access name from Id in code below
                    ticket.AssignedToUser = db.Users.Find(ticket.AssignedToUserId);


                    if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)  //Checking to see if New AssignedToUserId is Equal to old AssignedtoUserId 
                    {
                        if (oldTicket.AssignedToUserId != null)
                        {
                            TicketHistory th = new TicketHistory

                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Assigned to User",
                                OldValue = oldTicket.AssignedToUser.FirstName + " " + oldTicket.AssignedToUser.LastName,
                                NewValue = ticket.AssignedToUser.FirstName + " " + ticket.AssignedToUser.LastName,
                                DateChanged = DateTime.Now,

                            };
                            db.TicketHistories.Add(th);

                            TicketHistory th2 = new TicketHistory

                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Ticket Reassigned By",
                                OldValue = "N/A",
                                NewValue = user.FirstName + " " + user.LastName,
                                DateChanged = DateTime.Now,

                            };
                            db.TicketHistories.Add(th2);

                        }
                        else
                        {
                            TicketHistory th = new TicketHistory

                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Assigned to User",
                                OldValue = "Unassigned",

                                NewValue = ticket.AssignedToUser.FirstName + " " + ticket.AssignedToUser.LastName,
                                DateChanged = DateTime.Now,

                            };
                            db.TicketHistories.Add(th);

                            TicketHistory th2 = new TicketHistory

                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Ticket Assigned By",
                                OldValue = "N/A",

                                NewValue = user.FirstName + " " + user.LastName,
                                DateChanged = DateTime.Now,

                            };
                            db.TicketHistories.Add(th2);
                        }


                        db.SaveChanges();


                        if (TN != null)
                        {
                            EmailService email = new EmailService();

                            var message = new IdentityMessage
                            {
                                Body = "The ticket assignment" + " " + oldTicket.Title + " " + "from project" + " " + oldTicket.Project.Name + " " + "has been changed by" + " " + user.FirstName + user.LastName + "." + "  " +
                            "Please see your tickets to view the change.",
                                Subject = "Your ticket assignment has been changed.",
                                Destination = TN.User.Email
                            };
                            await email.SendAsync(message);

                        }
                    }

                    return RedirectToAction("Index", "Projects");
                }

                var Developers = (List<ApplicationUser>)TempData["Devs"];

                ViewBag.AssignedToUserId = new SelectList(Developers, "Id", "FirstName");
                return View();
            }


            public ActionResult TicketOwner()
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                return View(db.Tickets.Where(t => t.SubmitterUserId == userId).ToList());

            }


            // POST: Tickets/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "Id,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,Title,Description")] Ticket ticket, HttpPostedFileBase File)
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    ticket.CreatedDate = DateTime.Now;
                    ticket.SubmitterUser = user;
                    ticket.SubmitterUserId = userId;
                    var TicketOwner = ticket.SubmitterUser;
                    int unassignedId = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Unassigned").Id;
                    ticket.TicketStatusId = unassignedId;


                    TicketAttachment TA = new TicketAttachment();                  
                    if (FileUploadValidator.IsWebFriendlyFile(File))

                    {
                        var fileName = Path.GetFileName(File.FileName);
                        var customName = string.Format(Guid.NewGuid() + fileName);
                        File.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), customName));
                        TA.FilePath = "~/Uploads/" + customName;
                        TA.TicketId = ticket.Id;
                        TA.Description = "";
                        TA.CreatedDate = DateTimeOffset.Now;
                        TA.UserId = userId;
                        db.TicketAttachments.Add(TA);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Please select an image between 1KB-2MB and in an approved format (.jpg, .bmp, .png, .gif)";
                        TA.FilePath = "~/images/radio-img.png";
                    }
                                     

                    db.Tickets.Add(ticket);
                    db.SaveChanges();


                    TicketHistory th = new TicketHistory  //filling in all the properties of ticket history
                    {
                        TicketId = ticket.Id,
                        UserId = userId,
                        Property = "Title",
                        OldValue = "N/A",
                        NewValue = ticket.Title,
                        DateChanged = ticket.CreatedDate,

                    };
                    db.TicketHistories.Add(th);

                    TicketHistory th2 = new TicketHistory  //filling in all the properties of ticket history
                    {
                        TicketId = ticket.Id,
                        UserId = userId,
                        Property = "Ticket Submitted by",
                        OldValue = "N/A",
                        NewValue = ticket.SubmitterUser.FirstName,
                        DateChanged = ticket.CreatedDate,

                    };

                    db.TicketHistories.Add(th2);

                    TicketHistory th3 = new TicketHistory  //filling in all the properties of ticket history
                    {
                        TicketId = ticket.Id,
                        UserId = userId,
                        Property = "Created Date",
                        OldValue = "N/A",
                        NewValue = ticket.CreatedDate.ToString(),
                        DateChanged = ticket.CreatedDate,

                    };

                    db.TicketHistories.Add(th3);

                    db.SaveChanges();
                    return RedirectToAction("Index", "Projects");
                }

                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.SubmitterUserId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterUserId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
            }
        

            // GET: Tickets/Edit/5
            public ActionResult Edit(int? id, int? UserId)
            {

                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper ph = new ProjectAssignHelper();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ticket ticket = db.Tickets.Find(id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);


                if (helper.IsUserInRole(userId, "Administrator") || helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Project Manager1") && ph.IsUserOnAProject(userId, ticket.Project.Id) ||
                    helper.IsUserInRole(userId, "Project Manager2") && ph.IsUserOnAProject(userId, ticket.Project.Id) || helper.IsUserInRole(userId, "Project Manager3") && ph.IsUserOnAProject(userId, ticket.Project.Id) ||
                    helper.IsUserInRole(userId, "Developer1") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Developer2") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Developer3") && ticket.AssignedToUserId == userId ||
                    helper.IsUserInRole(userId, "Developer4") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Submitter") && ticket.SubmitterUserId == userId)
                {
                
                    if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator") || helper.IsUserInRole(userId, "Project Manager1") || helper.IsUserInRole(userId, "Project Manager2") || helper.IsUserInRole(userId, "Project Manager3"))
                    {
                        ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
                    }
                    ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                    if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator"))
                    {
                        ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                    }
                    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                    ViewBag.TicketAttachmentId = new SelectList(db.TicketAttachments, "Id", "Description", ticket.TicketAttachments.Where(t => t.Ticket.TicketAttachments.Any()));


                    if (helper.IsUserInRole(userId, "Developer1"))
                    {
                        if (userId == ticket.AssignedToUserId || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Developer2"))
                    {
                        if (userId == ticket.AssignedToUserId || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Developer3"))
                    {
                        if (userId == ticket.AssignedToUserId || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Developer4"))
                    {
                        if (userId == ticket.AssignedToUserId || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Project Manager1"))
                    {
                        if (ph.IsUserOnAProject(userId, ticket.ProjectId) || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Project Manager2"))
                    {
                        if (ph.IsUserOnAProject(userId, ticket.ProjectId) || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }

                    if (helper.IsUserInRole(userId, "Project Manager3"))
                    {
                        if (ph.IsUserOnAProject(userId, ticket.ProjectId) || userId == ticket.SubmitterUserId)
                        {
                            return View(ticket);
                        }
                    }

                    if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator"))
                    {
                        return View(ticket);
                    }

                    else
                    {
                        var Temporary = "You cannot edit this ticket.  Please revisit your role assignment.  Tickets can only be edited by their creator, a developer assigned to the ticket, a project manager, or an administrator.";
                        TempData["message"] = Temporary;
                    }

                    return RedirectToAction("Index", "Tickets");
                }
                else
                {
                    var Temporary = "You cannot edit this ticket.  Please revisit your role assignment.  Tickets can only be edited by their creator, a developer assigned to the ticket, a project manager, or an administrator.";
                    TempData["message"] = Temporary;
                }
                return RedirectToAction("Index");
            }

            // POST: Tickets/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId, CreatedDate, TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId, AssignedToUser, Title,Description")] Ticket ticket, HttpPostedFileBase File, string TicketAttachmentId)

            {
                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper ph = new ProjectAssignHelper();
                if (ModelState.IsValid)
                {
                    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id.Equals(ticket.Id));
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    ticket.UpdatedDate = DateTime.Now;
                    ticket.SubmitterUserId = oldTicket.SubmitterUserId;
                    if (ticket.AssignedToUserId == null)
                    {
                        ticket.AssignedToUserId = oldTicket.AssignedToUserId;
                    }
                    if (ticket.Title == null)
                    {
                        ticket.Title = oldTicket.Title;
                    }
                                        
                    TicketNotification TN = new TicketNotification();
                    TN.TicketId = ticket.Id;
                    TN.UserId = ticket.AssignedToUserId;
                    TN.User = db.Users.Find(ticket.AssignedToUserId);
                    db.TicketNotifications.Add(TN);



                    TicketAttachment TA = new TicketAttachment();                   
                    if (File != null)
                    {
                        if (FileUploadValidator.IsWebFriendlyFile(File))

                        {
                            var fileName = Path.GetFileName(File.FileName);
                            var customName = string.Format(Guid.NewGuid() + fileName);
                            File.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), customName));
                            TA.FilePath = "~/Uploads/" + customName;
                            TA.TicketId = ticket.Id;
                            TA.Description = "";
                            TA.CreatedDate = DateTimeOffset.Now;
                            TA.UserId = userId;
                            db.TicketAttachments.Add(TA);
                        }

                        else
                        {

                            ViewBag.ErrorMessage = "Please select an image between 1KB-2MB and in an approved format (.jpg, .bmp, .png, .gif)";
                            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
                            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                            ViewBag.SubmitterUserId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterUserId);
                            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                            return View(ticket);
                        }
                    }


                    db.Entry(ticket).State = EntityState.Modified;

                    db.SaveChanges();

                    ticket.TicketStatus = db.TicketStatuses.Find(ticket.TicketStatusId); // Creating an instance of Ticket Status object to be able to access name from Id in code below
                    ticket.AssignedToUser = db.Users.Find(ticket.AssignedToUserId);
                    ticket.Project = db.Projects.Find(ticket.ProjectId);
                    ticket.SubmitterUser = db.Users.Find(ticket.SubmitterUserId);
                    ticket.TicketPriority = db.TicketPriorities.Find(ticket.TicketPriorityId);
                    ticket.TicketType = db.TicketTypes.Find(ticket.TicketTypeId);

                    if (oldTicket.Description != ticket.Description)  //Checking to see if New Description is Equal to old Description
                    {
                        TicketHistory th = new TicketHistory

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Description",
                            OldValue = oldTicket.Description,
                            NewValue = ticket.Description,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th);
                        }
                    }

                    if (oldTicket.Title != ticket.Title)
                    {
                        TicketHistory th2 = new TicketHistory  //Checking to see if New Title is Equal to old Title

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Title",
                            OldValue = oldTicket.Title,
                            NewValue = ticket.Title,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th2);
                        }
                    }
                    if (helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator") || helper.IsUserInRole(userId, "Project Manager1") || helper.IsUserInRole(userId, "Project Manager2") || helper.IsUserInRole(userId, "Project Manager3"))
                    {
                        if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                        {
                            TicketHistory th3 = new TicketHistory  //Checking to see if New Assigned To User is equal to the Old Assigned to User

                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Assigned To User",
                                OldValue = oldTicket.AssignedToUser.LastName,
                                NewValue = ticket.AssignedToUser.LastName,
                                DateChanged = DateTime.Now,

                            };
                            {
                                db.TicketHistories.Add(th3);
                            }
                        }
                    }



                    if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                    {
                        TicketHistory th5 = new TicketHistory  //Checking to see if New Ticket Priority is Equal to old Ticket Priority

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Ticket Priority",
                            OldValue = oldTicket.TicketPriority.Name,
                            NewValue = ticket.TicketPriority.Name,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th5);
                        }
                    }
                    if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                    {
                        TicketHistory th6 = new TicketHistory  //Checking to see if New Ticket Status is Equal to old Ticket Status

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Ticket Status",
                            OldValue = oldTicket.TicketStatus.Name,
                            NewValue = ticket.TicketStatus.Name,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th6);
                        }
                    }


                    if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                    {
                        TicketHistory th7 = new TicketHistory  //Checking to see if New Ticket Type is Equal to old Ticket Type

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Ticket Type",
                            OldValue = oldTicket.TicketType.Name,
                            NewValue = ticket.TicketType.Name,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th7);
                        }
                    }

                    if (oldTicket.ProjectId != ticket.ProjectId)
                    {
                        TicketHistory th8 = new TicketHistory  //Checking to see if New Title is Equal to old Title

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Project",
                            OldValue = oldTicket.Project.Name,
                            NewValue = ticket.Project.Name,
                            DateChanged = DateTime.Now,

                        };
                        {
                            db.TicketHistories.Add(th8);
                        }
                    }

                    db.SaveChanges();
                    if (TN != null)
                    {
                        var message = new IdentityMessage
                        {
                            Body = "The ticket" + " " + ticket.Title + " " + "from project" + " " + ticket.Project.Name + " " + "has been edited by" + " " + user.FirstName + user.LastName + "." + "  " +
                                "Please see the ticket history to view the specific change",
                            Subject = "Your ticket has been edited",
                            Destination = TN.User.Email
                        };
                        EmailService email = new EmailService();
                        await email.SendAsync(message);
                    }

                    return RedirectToAction("Index");
                }
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.SubmitterUserId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterUserId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
            }

            // GET: Tickets/Delete/5 
            [Authorize(Roles = "Admin, Administrator, Project Manager1, Project Manager2, ProjectManager3")]
            public ActionResult Archive(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ticket ticket = db.Tickets.Find(id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                return View(ticket);
            }

            // POST: Tickets/Delete/5
            [HttpPost, ActionName("Archive")]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ArchiveConfirmed(int id)
            {
                if (ModelState.IsValid)
                {
                    Ticket ticket = db.Tickets.Find(id);
                    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id.Equals(ticket.Id));
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    int archivedId = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Archived").Id;
                    ticket.TicketStatusId = archivedId;
                    db.Entry(ticket).Property("TicketStatusId").IsModified = true;

                ticket.AssignedToUser = db.Users.Find(ticket.AssignedToUserId);
                if (ticket.AssignedToUser == null)
                {
                    ticket.AssignedToUser = db.Users.Find(ticket.SubmitterUserId);
                }


                TicketNotification TN = new TicketNotification();
                    TN.TicketId = ticket.Id;
                    TN.UserId = ticket.AssignedToUserId;
                    TN.User = ticket.AssignedToUser;
                    db.TicketNotifications.Add(TN);

                    db.SaveChanges();
                                
                    if (oldTicket.TicketStatus.Name != ticket.TicketStatus.Name)  //Checking to see if New Description is Equal to old Description
                    {
                        TicketHistory th = new TicketHistory

                        {
                            TicketId = ticket.Id,
                            UserId = userId,
                            Property = "Ticket Status",
                            OldValue = oldTicket.TicketStatus.Name,
                            NewValue = ticket.TicketStatus.Name,
                            DateChanged = DateTime.Now,

                        };

                        db.TicketHistories.Add(th);
                    }


                    db.SaveChanges();

                    if (TN != null)
                    {
                    //getting hung up - commenting out for now 10/11/2018
                        //EmailService email = new EmailService();
                        //var message = new IdentityMessage
                        //{
                        //    Body = "Your ticket titled" + " " + oldTicket.Title + " " + "from project" + " " + oldTicket.Project.Name + " " + "has been closed by" + " " + user.FirstName != null ? user.FirstName : "" + user.LastName != null? user.LastName : user.DisplayName + "." + "  " +
                        //        "Please see your tickets to view the change.",
                        //    Subject = "Your ticket assignment has been changed.",
                        //    Destination = TN.User.Email
                        //};
                        //await email.SendAsync(message);
                    }

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }

   