using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASG.Areas.BugTracker.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using ASG.Models;

namespace ASG.Areas.BugTracker.Models.Controllers
{    
        public class TicketAttachmentsController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: TicketAttachments
            public ActionResult Index()
            {
                var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
                return View(ticketAttachments.ToList());
            }

            // GET: TicketAttachments/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
                if (ticketAttachment == null)
                {
                    return HttpNotFound();
                }
                return View(ticketAttachment);
            }

            // GET: TicketAttachments/Create
            public ActionResult Create()
            {
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId");
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
                return View();
            }

            // POST: TicketAttachments/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Create([Bind(Include = "Id,UserId,Description,CreatedDate")] TicketAttachment ticketAttachment, HttpPostedFileBase File, int Id)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                ProjectAssignHelper ph = new ProjectAssignHelper();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                var ticket = db.Tickets.Find(Id);


                if (ModelState.IsValid)
                {

                    if (helper.IsUserInRole(userId, "Administrator") || helper.IsUserInRole(userId, "Admin") || helper.IsUserInRole(userId, "Project Manager1") && ph.IsUserOnAProject(userId, ticket.Project.Id) ||
                   helper.IsUserInRole(userId, "Project Manager2") && ph.IsUserOnAProject(userId, ticket.Project.Id) || helper.IsUserInRole(userId, "Project Manager3") && ph.IsUserOnAProject(userId, ticket.Project.Id) ||
                   helper.IsUserInRole(userId, "Developer1") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Developer2") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Developer3") && ticket.AssignedToUserId == userId ||
                    helper.IsUserInRole(userId, "Developer4") && ticket.AssignedToUserId == userId || helper.IsUserInRole(userId, "Submitter") && ticket.SubmitterUserId == userId)

                        if (File != null)
                        {
                            TicketNotification TN = new TicketNotification();
                            TN.TicketId = ticket.Id;
                            TN.UserId = ticket.AssignedToUserId;
                            TN.User = ticket.AssignedToUser;
                            db.TicketNotifications.Add(TN);

                            var message = new IdentityMessage
                            {
                                Body = "The ticket" + " " + ticket.Title + " " + "from project" + " " + ticket.Project.Name + " " + "has been edited by" + " " + user.FirstName + user.LastName + "." + "  " +
                                "Please see the ticket history to view the specific change.",
                                Subject = "Your ticket has been edited",
                                Destination = TN.User.Email
                            };
                            EmailService email = new EmailService();
                            await email.SendAsync(message);

                            if (FileUploadValidator.IsWebFriendlyFile(File))

                            {
                                var fileName = Path.GetFileName(File.FileName);
                                var customName = string.Format(Guid.NewGuid() + fileName);
                                File.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), customName));
                                ticketAttachment.FilePath = "/Uploads/" + customName;


                                string filename = ticketAttachment.FilePath.ToString();
                                string word = ".docx";
                                string excel = ".xls";
                                string excelx = "xlsx";
                                string pdf = ".pdf";
                                string jpeg = ".jpg";
                                string bmp = ".bmp";
                                string gif = ".gif";
                                string zip = ".zip";
                                string rar = ".rar";
                                string ticketmessage = "";
                                if (filename.Contains(word))
                                {
                                    ticketmessage = "This is a Word Document";
                                }
                                else if (filename.Contains(excel))
                                {
                                    ticketmessage = "This is an Excel File";
                                }
                                else if (filename.Contains(pdf))
                                {
                                    ticketmessage = "This is an PDF File";
                                }
                                else if (filename.Contains(jpeg))
                                {
                                    ticketmessage = "This is a JPEG File";
                                }
                                else if (filename.Contains(bmp))
                                {
                                    ticketmessage = "This is a BMP File";
                                }
                                else if (filename.Contains(gif))
                                {
                                    ticketmessage = "This is a .gif File";
                                }
                                else if (filename.Contains(zip))
                                {
                                    ticketmessage = "This is an ZIP File";
                                }
                                else if (filename.Contains(rar))
                                {
                                    ticketmessage = "This is a RAR File";
                                }
                                else if (filename.Contains(excelx))
                                {
                                    ticketmessage = "This is an .xlsx File";
                                }
                                
                                ticketAttachment.CreatedDate = DateTimeOffset.Now;
                                var ticketdatestring = ticketAttachment.CreatedDate.ToString("MM-dd-yyyy");
                                ticketAttachment.TicketId = Id;
                                ticketAttachment.Description = ticketmessage + " " + "added by" + " " + user.FirstName + " " + user.LastName + " " + "on" + " " + ticketdatestring + ".";
                                ticketAttachment.UserId = userId;

                                db.TicketAttachments.Add(ticketAttachment);


                            TicketHistory th = new TicketHistory  //filling in all the properties of ticket history
                            {
                                TicketId = ticket.Id,
                                UserId = userId,
                                Property = "Attachment Added",
                                OldValue = "N/A",
                                NewValue =ticketAttachment.Description,
                                DateChanged = DateTime.Now,

                            };
                            db.TicketHistories.Add(th);

                            db.SaveChanges();

                            }
                            int id = Id;
                            return RedirectToAction("Details", "Tickets", new { id = id });
                        }
                        else
                        {
                            var Temporary = "Please select an image between 1KB - 2MB and in an approved format(.jpg, .bmp, .png, .gif)";
                            TempData["attachmentmessage"] = Temporary;


                        }
                    var Temporary3 = "You cannot edit this ticket.  Please revisit your role assignment.  Tickets can only be edited by their creator, a developer assigned to the ticket, a project manager, or an administrator.";
                    TempData["message"] = Temporary3;
                    return RedirectToAction("Details", "Tickets", new { id = Id });
                }

                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId", ticketAttachment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
                return View(ticketAttachment);
            }

            // GET: TicketAttachments/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
                if (ticketAttachment == null)
                {
                    return HttpNotFound();
                }
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId", ticketAttachment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
                return View(ticketAttachment);
            }

            // POST: TicketAttachments/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,FilePath,FileUrl,Description,CreatedDate")] TicketAttachment ticketAttachment)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ticketAttachment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterUserId", ticketAttachment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
                return View(ticketAttachment);
            }

            // GET: TicketAttachments/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
                if (ticketAttachment == null)
                {
                    return HttpNotFound();
                }
                return View(ticketAttachment);
            }

            // POST: TicketAttachments/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
                db.TicketAttachments.Remove(ticketAttachment);
                db.SaveChanges();
                return RedirectToAction("Index", "Projects");
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

   