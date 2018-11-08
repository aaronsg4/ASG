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
using ASG.Models;
using Newtonsoft.Json;

namespace ASG.Areas.FinancialPlanner.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.TransactionCategory).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }


        //Get: ModalPartial

        public PartialViewResult TransactionModal(string id, int? budgetId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var categories = db.TransactionCategories.Where(c => c.CreatedByUserId == userId).ToList();

            var categoryOptions = (from tc in db.TransactionCategories
                                   where tc.CreatedByUserId == userId
                                   select new
                                   {
                                       label = tc.Name,
                                       value = tc.Name
                                   }).ToArray();
       
            string json = JsonConvert.SerializeObject(categoryOptions, Formatting.Indented);    
            ViewBag.autocompleteOptions = json;
          
            var household = user.Household;
            var accounts = db.Accounts.Where(a => a.AccountHolderUserId == userId);
            var userscategories = db.TransactionCategories.Where(t => t.CreatedByUserId == userId).ToList();
            var userhouseholdId = user.HouseholdId;
            var usersinhousehold = db.Users.Where(u => u.HouseholdId == userhouseholdId);
            var UserHousehold = user.Household.Name;
            var General = db.TransactionCategories.Where(t => t.Name == "General");
            var UserCategories = db.TransactionCategories.Where(t => t.CreatedByUserId == userId);
            ViewBag.UserCategories = userscategories;

            ViewBag.currentBudgetId = budgetId;
            if (!household.Budgets.Any())
            {
                ViewBag.BudgetAlert = "Oops, you haven't established any Budgets yet.  Click here to set up your budget to utilize the planner.";
            }

            if (!accounts.Any())
            {
                UrlHelper uh = new UrlHelper(this.ControllerContext.RequestContext);
                string url = uh.Action("Create", "FinancialAccounts", null);
                ViewBag.url = url;
                ViewBag.AccountAlert = "First, lets set up an account to start tracking your transactions.";

            }            

            var budgetsNotExpired = household.Budgets.Where(b => b.Expired == false);
            if (budgetsNotExpired != null)
            {
                ViewBag.BudgetId = new SelectList(budgetsNotExpired, "Id", "Name", budgetId);
            }
           
            ViewBag.FinancialAccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            return PartialView();
        }


        //GET Reconcile Modal
        public PartialViewResult ReconcileTransaction(int? id)
        {

            if (id == null)
            {
                return PartialView();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return PartialView(HttpNotFound());
            }

            return PartialView(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReconcileTransactionConfirm([Bind(Include = "Id,Title,Amount,CreatedDate,UpdatedDate,SubmitterUserId,TransactionTypeId,TransactionCategoryId,FinancialAccountId,BudgetId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var thistransaction = db.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
                thistransaction.Reconciled = true;
                db.Entry(thistransaction).Property("Reconciled").IsModified = true;

                var budget = db.Transactions.Find(transaction.Id).Budget;
                var BudgetId = db.Transactions.Find(transaction.Id).BudgetId;
                var FinancialAccountId = db.Transactions.Find(transaction.Id).FinancialAccountId;
                var SubmitterUserId = db.Transactions.Find(transaction.Id).SubmitterUserId;
                var TransactionCategoryId = db.Transactions.Find(transaction.Id).TransactionCategoryId;
                var TransactionTypeId = db.Transactions.Find(transaction.Id).TransactionTypeId;
                var TransactionTitle = db.Transactions.Find(transaction.Id).Title;
                var oldtransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id.Equals(transaction.Id));
                var newtransactionamount = transaction.Amount;

                decimal difference = 0;

                if (oldtransaction.Amount == newtransactionamount)
                {
                    transaction.ReconciledAmount = 0;
                    transaction.Reconciled = true;
                }

                else if (oldtransaction.Amount != newtransactionamount)
                {
                    difference = newtransactionamount - oldtransaction.Amount;


                    Transaction newtransaction = new Transaction();
                    newtransaction.Amount = difference;
                    newtransaction.BudgetId = BudgetId;
                    newtransaction.CreatedDate = DateTime.Now;
                    newtransaction.FinancialAccountId = FinancialAccountId;
                    newtransaction.Reconciled = true;
                    newtransaction.SubmitterUserId = SubmitterUserId;
                    newtransaction.TransactionCategoryId = TransactionCategoryId;
                    newtransaction.TransactionTypeId = TransactionTypeId;
                    newtransaction.Title = TransactionTitle + " " + "reconciled amount";
                    newtransaction.ReconciledAmount = difference;
                    db.Transactions.Add(newtransaction);
                    if (budget.BudgetRemaining > 0)
                    {
                        if ((budget.BudgetRemaining + difference) > 0)
                        {
                            budget.BudgetRemaining = budget.BudgetRemaining + difference;
                        }
                        else
                        {
                            budget.BudgetRemaining = 0;
                        }
                    }                 
                }

                db.SaveChanges();

                return RedirectToAction("Details", "Budgets", new { id = BudgetId });

            }
            return View(transaction);
        }



        // GET: Transactions/Create
        public ActionResult CreateBudgetTransaction(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(Id);

            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = Id.GetValueOrDefault();
            ViewBag.WhichBudget = budget.Name;
            ViewBag.TransactionCategoryId = new SelectList(db.TransactionCategories, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBudgetTransaction([Bind(Include = "Id,Title,Amount,TransactionTypeId, BudgetId, FinancialAccountId")] Transaction transaction, string TransactionCategoryName)
        {
            if (ModelState.IsValid)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var useraccounts = db.Accounts.Where(a => a.AccountHolderUserId == userId).ToList();
                if (useraccounts != null)
                {
                    transaction.Reconciled = false;
                    transaction.SubmitterUserId = userId;
                    transaction.CreatedDate = DateTime.Now;
                    Budget budget = db.Budgets.Find(transaction.BudgetId);
                    FinancialAccount financialaccount = db.Accounts.Find(transaction.FinancialAccountId);

                    var TransactionCategory = db.TransactionCategories.FirstOrDefault(tc => tc.Name == TransactionCategoryName);
                    if (TransactionCategory != null)
                    {
                        transaction.TransactionCategoryId = TransactionCategory.Id;
                    }
                    else
                    {
                        TransactionCategory transactionCategory = new TransactionCategory();
                        transactionCategory.Name = TransactionCategoryName;
                        transactionCategory.CreatedByUserId = userId;
                        transactionCategory.CreatedDate = DateTime.Now;
                        db.TransactionCategories.Add(transactionCategory);

                        db.SaveChanges();
                        transaction.TransactionCategoryId = transactionCategory.Id;
                    }

                    var creditId = db.TransactionTypes.FirstOrDefault(t => t.Name == "Credit").Id;
                    var debitId = db.TransactionTypes.FirstOrDefault(t => t.Name == "Debit").Id;
                    if (transaction.TransactionTypeId == debitId)
                    {
                        transaction.Amount = transaction.Amount * -1;
                    }


                    budget.BudgetRemaining = budget.BudgetRemaining + transaction.Amount;
                    financialaccount.ActualBalance = financialaccount.ActualBalance + transaction.Amount;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();

                    List<decimal> expenses = new List<decimal>();
                    foreach (var expense in budget.Transactions)
                    {
                        if (expense.CreatedDate <= budget.BudgetEnd && expense.SubmitterUserId == userId)
                        {
                            expenses.Add(expense.Amount);
                        }
                    }
                    var expensetotal = expenses.Sum();
                    ViewBag.Expenses = expensetotal;
                    return Json(new { success = true, expensetotal = expensetotal });           
                }
                else ViewBag.NoAccounts = "You have not set up any accounts.  Please set up a bank account before entering transactions.";
            }


            ViewBag.TransactionCategoryId = new SelectList(db.TransactionCategories, "Id", "Name", transaction.TransactionCategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);

            var modelErrors = new List<string>();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    modelErrors.Add(modelError.ErrorMessage);
                }
            }
            return Json(new { success = false, ex = modelErrors });         
        }

        // GET: Transactions/Edit/5
        public PartialViewResult Edit(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return PartialView();
            }

            ViewBag.TransactionCategoryId = new SelectList(db.TransactionCategories, "Id", "Name", transaction.TransactionCategoryId);
            return PartialView(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title, TransactionCategoryId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var household = user.Household;
                var budget = db.Transactions.FirstOrDefault(t => t.Id == transaction.Id).Budget;
                db.SaveChanges();
                return RedirectToAction("Details", "Budgets", new { id = budget.Id });
            }
            ViewBag.TransactionCategoryId = new SelectList(db.TransactionCategories, "Id", "Name", transaction.TransactionCategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public PartialViewResult Void(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return PartialView();
            }
            return PartialView(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            Budget budget = db.Budgets.Find(transaction.BudgetId);
            FinancialAccount financialaccount = db.Accounts.Find(transaction.FinancialAccountId);
            budget.BudgetRemaining = budget.BudgetRemaining - transaction.Amount;
            financialaccount.ActualBalance = financialaccount.ActualBalance + transaction.Amount;
            transaction.Void = true;

            db.Transactions.Attach(transaction);
            db.Entry(transaction).Property("Void").IsModified = true;
            db.SaveChanges();

            return RedirectToAction("Details", "Budgets", new { id = budget.Id });
        }

        public PartialViewResult Unvoid(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return PartialView();
            }
            return PartialView(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnvoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            Budget budget = db.Budgets.Find(transaction.BudgetId);
            FinancialAccount financialaccount = db.Accounts.Find(transaction.FinancialAccountId);
            budget.BudgetRemaining = budget.BudgetRemaining + transaction.Amount;
            financialaccount.ActualBalance = financialaccount.ActualBalance - transaction.Amount;
            transaction.Void = false;

            db.Transactions.Attach(transaction);
            db.Entry(transaction).Property("Void").IsModified = true;

            db.SaveChanges();
            return RedirectToAction("Details", "Budgets", new { id = budget.Id });
        }

        // GET: Transactions/Delete/5
        public PartialViewResult Delete(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return PartialView();
            }
            return PartialView(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            Budget budget = db.Budgets.Find(transaction.BudgetId);
            FinancialAccount financialaccount = db.Accounts.Find(transaction.FinancialAccountId);
            budget.BudgetRemaining = budget.BudgetRemaining - transaction.Amount;
            financialaccount.ActualBalance = financialaccount.ActualBalance + transaction.Amount;
            db.Transactions.Remove(transaction);

            db.SaveChanges();
            return RedirectToAction("Details", "Budgets", new { id = budget.Id });
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
