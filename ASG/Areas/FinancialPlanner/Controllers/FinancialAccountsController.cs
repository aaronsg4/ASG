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
    public class FinancialAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FinancialAccounts
        [Authorize(Roles="Administrator")]
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(f => f.AccountType).Include(f => f.Household);
            return View(accounts.ToList());
        }

        // GET: FinancialAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinancialAccount financialAccount = db.Accounts.Find(id);
            financialAccount.Transactions = db.Transactions.Where(t => t.FinancialAccountId == financialAccount.Id).Where(t => t.Void != true).ToList();
            if (financialAccount == null)
            {
                return HttpNotFound();
            }
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Create
        public PartialViewResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var householdId = user.HouseholdId;
            var householdIdb = db.Households.Where((System.Linq.Expressions.Expression<Func<Household, bool>>)(h => h.Id == user.HouseholdId));
            var users = db.Users.Where(u => u.HouseholdId == householdId);
            var usersb = users.Where(u => u.Id == userId);

            string nameToDisplay;
            if (usersb.First()?.FullName.Length <= 1)
            {
                nameToDisplay = "Email";
            }
            else
            {
                nameToDisplay = "DisplayName";
            }

            ViewBag.AccountHolderUserId = new SelectList(usersb, "Id", nameToDisplay);
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(householdIdb, "Id", "Name");
            return PartialView();
        }

        // POST: FinancialAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ActualBalance,AccountTypeId, AccountHolderUserId")] FinancialAccount financialAccount)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
                financialAccount.HouseholdId = householdId.Value;
                financialAccount.AccountHolderUserId = userId;
                financialAccount.ReconciledBalance = financialAccount.ActualBalance;
                financialAccount.AccountHolderUserId = userId;
                financialAccount.CreatedDate = DateTime.Now;


                db.Accounts.Add(financialAccount);
                db.SaveChanges();

                return RedirectToAction("BudgetHousehold", "Budgets", new { id = householdId });
            }

            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name", financialAccount.AccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", financialAccount.HouseholdId);
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinancialAccount financialAccount = db.Accounts.Find(id);
            if (financialAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name", financialAccount.AccountTypeId);
            return View(financialAccount);
        }

        // POST: FinancialAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ActualBalance,ReconciledBalance,CreatedDate,UpdatedDate,Include,AccountHolderUserId, AccountTypeId, HouseholdId")] FinancialAccount financialAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(financialAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BudgetHousehold", "Budgets", new { id = financialAccount.HouseholdId });
            }
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name", financialAccount.AccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", financialAccount.HouseholdId);
            return View(financialAccount);
        }

        // GET: FinancialAccounts/Delete/5
        public PartialViewResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var householdId = db.Users.Find(userId).HouseholdId;
            if (id == null)
            {
                return PartialView("Reroute");
            }
            FinancialAccount financialAccount = db.Accounts.Find(id);
            if (financialAccount == null)
            {
                return PartialView("Reroute");
            }
            if (financialAccount.AccountHolderUserId == userId)
            {
                return PartialView(financialAccount);
            }

            var NotYourAccount = "Sorry.  You can only delete your own accounts.";
            TempData["NotYourAccountmessage"] = NotYourAccount;
            return PartialView("Reroute");

        }

        // POST: FinancialAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
            FinancialAccount financialAccount = db.Accounts.Find(id);
            db.Accounts.Remove(financialAccount);
            db.SaveChanges();
            return RedirectToAction("Details", "Households", new { id = householdId });
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
