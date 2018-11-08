using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASG.Areas.FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using ASG.Models;

namespace ASG.Areas.FinancialPlanner.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitations
        public ActionResult Index()
        {
            var invitations = db.Invitations.Include(i => i.Household);
            return View(invitations.ToList());
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            return View(invitations);
        }

        // GET: Invitations/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        public PartialViewResult CreateInvitationsModal()
        {

            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name");

            return PartialView();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ToEmail")] Invitations invitations)
        {


            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                invitations.SenderUserId = userId;
                invitations.CreatedDate = DateTime.Now;
                invitations.ExpirationDate = DateTime.Now.AddDays(30);
                var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
                invitations.HouseholdId = householdId.GetValueOrDefault();
                invitations.SenderUserId = userId;
                Household household = db.Households.Find(invitations.HouseholdId);
                household.InvitedEmail = invitations.ToEmail;
                db.Invitations.Add(invitations);
                db.SaveChanges();



                db.SaveChanges();


                if (invitations != null)
                {
                    var message = new IdentityMessage
                    {
                        Body = "You have been invited to join the online Financial Planner by " + db.Users.FirstOrDefault(u => u.Id == invitations.SenderUserId).FirstName + ".  Click http://agay-budgeter.azurewebsites.net/ to visit the Financial Planner and proceed.",


                        Subject = "You've been invited to the Financial Planner",
                        Destination = invitations.ToEmail,
                    };
                    EmailService email = new EmailService();
                    await email.SendAsync(message);
                }
                return RedirectToAction("BudgetHousehold", "Budgets", new { id = household.Id });
            }

            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", invitations.HouseholdId);
            return View(invitations);
        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", invitations.HouseholdId);
            return View(invitations);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ToEmail,SenderUserId,Accepted,Expired,CreatedDate,ExpirationDate,HouseholdId")] Invitations invitations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", invitations.HouseholdId);
            return View(invitations);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            return View(invitations);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitations invitations = db.Invitations.Find(id);
            db.Invitations.Remove(invitations);
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
