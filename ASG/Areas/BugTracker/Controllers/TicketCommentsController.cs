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
using System.Threading.Tasks;
using ASG.Models;

namespace ASG.Areas.BugTracker.Models.Controllers
{
        public class TicketCommentsController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: TicketComments
            public ActionResult Index()
            {
                var ticketComments = db.TicketComments.Include(t => t.Ticket).Include(t => t.User);
                return View(ticketComments.ToList());
            }

            // GET: TicketComments/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketComment ticketComment = db.TicketComments.Find(id);
                if (ticketComment == null)
                {
                    return HttpNotFound();
                }
                return View(ticketComment);
            }

            // GET: TicketComments/Create
            public ActionResult Create()
            {
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId");
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
                return View();
            }

            // POST: TicketComments/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Create([Bind(Include = "Id,TicketId,Comment")] TicketComment ticketComment, int Id)
            {

                if (ModelState.IsValid)
                {

                    UserRolesHelper helper = new UserRolesHelper(db);
                    ProjectAssignHelper ph = new ProjectAssignHelper();
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    var ticket = db.Tickets.Find(Id);


                    TicketNotification TN = new TicketNotification();
                    TN.TicketId = ticket.Id;
                    TN.UserId = ticket.AssignedToUserId;
                    TN.User = ticket.AssignedToUser;
                    db.TicketNotifications.Add(TN);

                    var message = new IdentityMessage
                    {
                        Body = "The ticket" + " " + ticket.Title + " " + "from project" + " " + ticket.Project.Name + " " + "has been commented on by" + " " + user.FirstName + user.LastName + "." + "  " +
                        "Please see your ticket comments to view the comment.",
                        Subject = "Your ticket has been commented on.",
                        Destination = TN.User.Email
                    };
                    EmailService email = new EmailService();
                    await email.SendAsync(message);
                
                    var userroleAdmin = helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Administrator");
                    var userroleProjectManager1 = helper.IsUserInRole(userId, "Project Manager1");
                    var userroleProjectManager2 = helper.IsUserInRole(userId, "Project Manager2");
                    var userroleProjectManager3 = helper.IsUserInRole(userId, "Project Manager3");
                    var userroleDeveloper1 = helper.IsUserInRole(userId, "Developer1");
                    var userroleDeveloper2 = helper.IsUserInRole(userId, "Developer2");
                    var userroleDeveloper3 = helper.IsUserInRole(userId, "Developer3");
                    var userroleDeveloper4 = helper.IsUserInRole(userId, "Developer4");
                    var userroleSubmitter = helper.IsUserInRole(userId, "Submitter");

                    int ticketid = Id;
                    int projectid = ticket.Project.Id;

                    if (userroleAdmin || userroleProjectManager1 && ph.IsUserOnAProject(userId, projectid) || userroleProjectManager2 && ph.IsUserOnAProject(userId, projectid) || userroleProjectManager3 && ph.IsUserOnAProject(userId, projectid) ||
                        userroleDeveloper1 && ticket.AssignedToUserId == userId || userroleDeveloper2 && ticket.AssignedToUserId == userId || userroleDeveloper3 && ticket.AssignedToUserId == userId ||
                        userroleDeveloper4 && ticket.AssignedToUserId == userId || userroleSubmitter && ticket.SubmitterUserId == userId)
                    {


                        ticketComment.Ticket = ticket;
                        ticketComment.UserId = userId;
                        ticketComment.User = user;
                        ticketComment.TicketId = ticket.Id;
                  
                        ticketComment.CreatedDate = DateTime.UtcNow;
                        TimeSpan elapsed = DateTime.UtcNow - ticketComment.CreatedDate;

                    
                        double hours = elapsed.TotalHours;
                        double minutes = elapsed.TotalMinutes;
                        string s = elapsed.ToString();

                        db.TicketComments.Add(ticketComment);


                    TicketHistory th = new TicketHistory  //filling in all the properties of ticket history
                    {
                        TicketId = ticket.Id,
                        UserId = userId,
                        Property = "Comment Added",
                        OldValue = "N/A",
                        NewValue = ticketComment.Comment,
                        DateChanged = DateTime.Now,

                    };
                    db.TicketHistories.Add(th);
                    db.SaveChanges();    

                    return RedirectToAction("Details", "Tickets", new { id = Id });
                    }
                                        
                    else
                    {
                        var Temporary = "You cannot add comments to this ticket.  Please revisit your role assignment.  Tickets can only be commented on by their creator, a developer assigned to the ticket, a project manager, or an administrator.";
                        TempData["message"] = Temporary;
                    }
                }
                return RedirectToAction("Details", "Tickets", new { id = Id });

            }

            // GET: TicketComments/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketComment ticketComment = db.TicketComments.Find(id);
                if (ticketComment == null)
                {
                    return HttpNotFound();
                }
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId", ticketComment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
                return View(ticketComment);
            }

            // POST: TicketComments/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Comment,CreatedDate")] TicketComment ticketComment)
            {
                if (ModelState.IsValid)
                {

                    db.Entry(ticketComment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId", ticketComment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
                return View(ticketComment);
            }

            // GET: TicketComments/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketComment ticketComment = db.TicketComments.Find(id);
                if (ticketComment == null)
                {
                    return HttpNotFound();
                }
                return View(ticketComment);
            }

            // POST: TicketComments/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                TicketComment ticketComment = db.TicketComments.Find(id);
                db.TicketComments.Remove(ticketComment);
                db.SaveChanges();
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

