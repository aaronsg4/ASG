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
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Households/Details/5
        //[AuthorizeHouseholdRequired]
        public ActionResult Details(int? id)
        {

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (userId != null)
            {
                var usersbudgets = db.Users.FirstOrDefault(u => u.Id == userId).Budgets;
                ViewBag.UserBudgets = usersbudgets;
            }

            if (userId == null)
            {
                var NotLoggedIn = "You must be logged in to view your household details.";
                TempData["message"] = NotLoggedIn;
                return RedirectToAction("Index", "Home");
            }

            else if (id == null)

            {
                var NoHousehold = "You have not set up a household yet.  Please create a household.";
                TempData["NoHouseholdmessage"] = NoHousehold;
                return RedirectToAction("Index", "Home");

            }

            Household household = db.Households.Find(id);
            if (household != null)
            {
                var users = db.Users.Where(u => u.HouseholdId == household.Id).ToList();
                household.Users = users;
            }

            else
            {
                var NotPartofHousehold = "You must be part of a household to view it's details.  If you have been invited to join a household, please join first by clicking 'Join Household', or create a new household.";
                TempData["message"] = NotPartofHousehold;
                return RedirectToAction("Index", "Home");
            }
            return View(household);
        }
        //Get
        public ActionResult MyHouseholdDetails()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (userId != null)
            {
                var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
                if (householdId != null)
                {
                    Household household = db.Households.Find((object)householdId);
                    return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetails.cshtml", household);
                }

                else return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetails.cshtml");

            }
            else return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetails.cshtml");
        }

        public ActionResult MyHouseholdDetailsb()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (userId != null)
            {
                if (user.HouseholdId != null)
                {
                    var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
                    if (householdId != null)
                    {
                        Household household = db.Households.Find((object)householdId);
                        return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetailsb.cshtml", household);
                    }
                }


                return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetailsb.cshtml");

            }
            return PartialView("~/Areas/FinancialPlanner/Views/Households/_MyHouseholdDetailsb.cshtml");
        }


        // GET: Households/Create
        public PartialViewResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (userId != null)
            {
                if (user.HouseholdId != null)

                {
                    var AlreadyinHousehold = "You are already part of a household.  You can only be in one Financial Planner household at a time.";
                    TempData["AlreadyinHouseholdmessage"] = AlreadyinHousehold;
                    return PartialView("Reroute");
                }
            }

            else
            {
                var NotLoggedIn = "You are not logged in.  Please log in to create a household.";
                TempData["NotLoggedInmessage"] = NotLoggedIn;
                return PartialView("Reroute");

            }
            return PartialView();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                if (userId != null)
                {
                    if (user.HouseholdId == null)
                    {
                        household.CreatedDate = DateTime.Now;
                        household.UserId = userId;
                        db.Households.Add(household);
                        db.SaveChanges();
                        user.HouseholdId = household.Id;
                        var UsersFinancialAccounts = db.Users.Find(userId).Accounts;
                        foreach (var account in UsersFinancialAccounts)
                        {
                            account.HouseholdId = household.Id;
                        }
                        db.SaveChanges();
                        return RedirectToAction("BudgetHousehold", "Budgets", new { id = household.Id });
                    }
                    else
                    {
                        var AlreadyinHousehold = "You are already part of a household.  You can only be in one Financial Planner household at a time.";
                        TempData["message"] = AlreadyinHousehold;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var NotLoggedIn = "You are not logged in.  Please log in to create a household.";
                    TempData["message"] = NotLoggedIn;
                    return RedirectToAction("Index", "Home");
                }                
            }

            return View(household);
        }
        //Get: Households/Join/5
        public PartialViewResult JoinHouseholdPopUp()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (userId != null)
            {
                return PartialView();
            }
            var NotLoggedIn2 = "You are not logged in.  Please log in before joining a household.";
            TempData["NotLoggedIn2message"] = NotLoggedIn2;
            return PartialView("Reroute");
        }

        public async Task<ActionResult> JoinHousehold(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userinHousehold = db.Users.Find(User.Identity.GetUserId());

            
            //took this out to test because refreshauthentication had to be removed due to errors
            //await ControllerContext.HttpContext.RefreshAuthentication(userinHousehold);
            Household household = db.Households.Find(id);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var UsersFinancialAccounts = db.Users.Find(userId).Accounts;
            if (UsersFinancialAccounts != null)
            {
                foreach (var account in UsersFinancialAccounts)
                {
                    account.HouseholdId = household.Id;
                }
            }


            if (household == null)
            {
                return HttpNotFound();
            }

            if (userId != null)
            {                
                return View(household);
            }

            else
            {
                return RedirectToAction("Register", "Account");
            }
        }

        public ActionResult JoinHouseholdEmailConfirm([Bind(Include = "InvitedEmail")] Household household)
        {
            if (ModelState.IsValid)

            {
                var invitedhousehold = db.Households.FirstOrDefault(h => h.InvitedEmail == household.InvitedEmail);
                if (invitedhousehold != null)
                {
                    var householdId = invitedhousehold.Id;
                    Household householdtojoin = db.Households.Find(householdId);
                    return View(householdtojoin);
                }
                else
                {
                    var NotInvited = "Sorry we did not find your invitation in the system.  Please confirm that you have been invited to join a household with the email you are using.";
                    TempData["message"] = NotInvited;
                    return RedirectToAction("Index", "Home");
                }

            }
            return View();
        }
        //POST

        public ActionResult JoinHouseholdConfirm([Bind(Include = "Id, Name,UserId,Description,CreatedDate")] Household household)
        {
            if (ModelState.IsValid)

            {
                var newhousehold = db.Households.AsNoTracking().FirstOrDefault(h => h.Id == household.Id);
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                household.Users = newhousehold.Users;
                user.HouseholdId = household.Id;
                household.Users.Add((ApplicationUser)user);

                db.SaveChanges();
                return RedirectToAction("BudgetHousehold", "Budgets", new { id = household.Id });
            }

            return RedirectToAction("Index");
        }


        //Leave Household GET
        public ActionResult LeaveHousehold(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(Id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }


        //Leave Household POST
        public ActionResult LeaveHouseholdConfirm(int Id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var UsersFinancialAccounts = user.Accounts;
            foreach (var account in UsersFinancialAccounts)
            {
                account.HouseholdId = null;
            }

            user.HouseholdId = null;
            Household household = db.Households.Find(Id);
            household.Users.Remove((ApplicationUser)user);

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {
                var hcd = db.Households.AsNoTracking().FirstOrDefault(h => h.Id == household.Id).CreatedDate;
                household.CreatedDate = hcd;

                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BudgetHousehold", "Budgets", new { id = household.Id });
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            Household household = db.Households.Find(id);
            var householdUsers = household.Users.ToList();
            foreach (var huser in householdUsers)
            {
                huser.HouseholdId = null;
            }

            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Home", "Index");
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
