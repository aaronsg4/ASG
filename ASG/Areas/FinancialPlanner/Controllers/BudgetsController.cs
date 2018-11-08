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

namespace ASG.Areas.FinancialPlanner.Controllers
{
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var allUsers = db.Users.ToList();
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var household = db.Users.FirstOrDefault(u => u.Id == userId).Household;
            var accounts = db.Accounts.FirstOrDefault(h => h.AccountHolderUserId == userId);
            List<Budget> MemberBudgets = new List<Budget>();
            if (!household.Budgets.Any())
            {
                var NoBudgets = "Oops, looks like you don't have any budgets yet.  Please go to your household and set up your budget(s)";
                TempData["budgetmessage"] = NoBudgets;
                ViewBag.BudgetAlert = "Oops, you haven't established any Budgets yet.  Click here to set up your budget to utilize the planner.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var budget in
                household.Budgets)
            {
                MemberBudgets.Add(budget);
            }
            return View(MemberBudgets);
        }

        public ActionResult BudgetPartial()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (userId != null)
            {
                var householdId = db.Users.FirstOrDefault(u => u.Id == userId).HouseholdId;
                if (householdId != null)
                {
                    Household household = db.Households.Find((object)householdId);
                    return PartialView("~/Areas/FinancialPlanner/Views/Budgets/_BudgetPartial.cshtml", household);
                }
                else return PartialView("~/Areas/FinancialPlanner/Views/Budgets/_BudgetPartial.cshtml");
            }
            else return PartialView("~/Areas/FinancialPlanner/Views/Budgets/_BudgetPartial.cshtml");
        }


        public ActionResult BudgetHousehold(int? id)
        {
            BudgetHouseholdViewModel MemberBudget = new BudgetHouseholdViewModel();
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

            MemberBudget.household = db.Households.Find(id);
            if (MemberBudget.household != null)
            {
                var users = db.Users.Where(u => u.HouseholdId == MemberBudget.household.Id).ToList();
                MemberBudget.household.Users = users;
            }

            else
            {
                var NotPartofHousehold = "You must be part of a household to view it's details.  If you have been invited to join a household, please join first by clicking 'Join Household', or create a new household.";
                TempData["message"] = NotPartofHousehold;
                return RedirectToAction("Index", "Home");
            }

            var household = db.Users.FirstOrDefault(u => u.Id == userId).Household;
            var accounts = db.Accounts.FirstOrDefault(h => h.AccountHolderUserId == userId);
            MemberBudget.budgets = db.Budgets.Where(b => b.HouseholdId == household.Id).ToList();
            MemberBudget.household = household;
            List<ApplicationUser> BudgetUsers = new List<ApplicationUser>();
            foreach (var member in MemberBudget.household.Users)
            {
                BudgetUsers.Add(member);
            }
            ViewBag.BudgetCreator = BudgetUsers;
            return View(MemberBudget);
        }
        
        // GET: Budgets
        public ActionResult BudgeterIndex()
        {
            var allUsers = db.Users.ToList();
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var household = db.Users.FirstOrDefault(u => u.Id == userId).Household;
            var accounts = db.Accounts.FirstOrDefault(h => h.AccountHolderUserId == userId);
            List<ApplicationUser> HouseholdMembers = new List<ApplicationUser>();
            List<Budget> MemberBudgets = new List<Budget>();
            if (!household.Budgets.Any())
            {
                var NoBudgets = "Oops, looks like you don't have any budgets yet.  Please set up a budget before proceeding.";
                TempData["budgetmessage"] = NoBudgets;
                ViewBag.BudgetAlert = "Oops, you haven't established any Budgets yet.  Click here to set up your budget to utilize the planner.";
            }

            if (!household.Accounts.Any())
            {
                var NoAccounts = "Oops, looks like you don't have any accounts yet.  Please set up a bank account before proceeding";
                TempData["accountmessage"] = NoAccounts;
                ViewBag.AccountAlert = "Oops, you haven't established any Accounts yet.  Click here to set up your accounts to utilize the planner.";
            }

            if (household.Budgets.Any() && household.Accounts.Any())
            {
                foreach (var member in db.Users.AsNoTracking())
                {
                    if (member.HouseholdId == user.HouseholdId)
                    {
                        HouseholdMembers.Add(member);
                    }
                }

                foreach (var budget in household.Budgets)
                {
                    MemberBudgets.Add(budget);
                }
                return View(MemberBudgets);
            }
            else
            {
                return RedirectToAction("Details", "Households", new { id = household.Id });
            }
        }
        
        //GET
        public PartialViewResult BudgetsCreateModal()
        {
            ViewBag.BudgetDurationPeriodId = new SelectList(db.BudgetDurationPeriods, "Id", "Description");
            ViewBag.ItemizeMessage = "Would you like to itemize this budget? By itemizing you can create categories to budget, like food, entertainment, etc.  If you wish to simply keep the budget as a total monthly budget without itemizing, press complete";
            return PartialView();
        }
        //POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BudgetsCreate([Bind(Include = "Id,Name,Amount,Description,BudgetDurationPeriodId, Household")] Budget budget)
        {
            if (ModelState.IsValid)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var household = user.Household;
                var days = db.BudgetDurationPeriods.FirstOrDefault(b => b.Id == budget.BudgetDurationPeriodId).NumberOfDays;

                if (user.HouseholdId == null)
                {
                    household = new Household();
                    household.CreatedDate = DateTime.Now;
                    household.UserId = userId;
                    db.Households.Add(household);
                    db.SaveChanges();
                    user.HouseholdId = household.Id;

                    budget.Household = db.Households.Find(household.Id);          
                    var UsersFinancialAccounts = db.Users.Find(userId).Accounts;
                    foreach (var account in UsersFinancialAccounts)
                    {
                        account.HouseholdId = household.Id;
                    }
                    db.SaveChanges();               
                }
               
                
                budget.BudgetStartDate = DateTime.Now;
                double numberofdays = Convert.ToDouble(days);
                budget.BudgetEnd = budget.BudgetStartDate.AddDays(numberofdays);
                budget.HouseholdId = household.Id;
                budget.UserId = userId;
                budget.BudgetRemaining = budget.Amount;
                budget.BudgetEndDate = 0;
                
                List<decimal> expenses = new List<decimal>();
                foreach (var transaction in db.Transactions)
                {
                    if (transaction.CreatedDate > budget.BudgetEnd && transaction.SubmitterUserId == userId)
                    {
                        expenses.Add(transaction.Amount);
                    }
                }
                var expensetotal = expenses.Sum();
                db.Budgets.Add(budget);
                db.SaveChanges();
                ViewBag.expenses = expenses;
                return Json(new { success = true });                
            }

            var modelErrors = new List<string>();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    modelErrors.Add(modelError.ErrorMessage);
                }
            }

            ViewBag.BudgetDurationPeriodId = new SelectList(db.BudgetDurationPeriods, "Id", "Description");
            return Json(new { success = false, ex = modelErrors });
        }
              
        public JsonResult CatDropdown(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            //TODO :  change this to get all categories created by anyone on the budget  
            var category = db.TransactionCategories.Where(c => c.CreatedByUserId == userId).Select(c => c.Name);
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult BudgetsItemizeModal(string id)
        {
            ViewBag.ItemizeConfirm = "Please enter categories that you would like to track in the budgeter";
            return PartialView();
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            List<UserData2> UserChartList = new List<UserData2>();   //Finding Chart Data for Users in a household's spending amounts
            var Budget = db.Budgets.Find(id);
            UserData2 Users;
            Household household = db.Users.FirstOrDefault(u => u.Id == userId).Household;
            var userobject = db.Users.Find(userId);
            var userTransactions = db.Transactions.Where(t => t.SubmitterUserId == userId);

            foreach (var user in household.Users)
            {
                Users = new UserData2();
                Users.label = user.FullName;
                Users.value = user.Transactions.Where(t => t.BudgetId == Budget.Id).Where(t => t.Void != true).Select(t => t.Amount).ToList().Sum(s => Convert.ToInt32(s));
              
                UserChartList.Add(Users);
            }

            //for each user in the user chart list, if none of them have a value greater than 0, then give the current user setting it up a value of 100
            bool usersHaveTransactions = false;
            foreach (var user in UserChartList)
            {
                if (user.value != 0)
                {
                    usersHaveTransactions = true;
                }
            }

            //This is because the morris chart disappears and throws an error if there is no data
            if (!usersHaveTransactions)
            {          
                foreach (var user in UserChartList)
                {
                    if (user.label == userobject.FullName)
                    {
                        user.label = "None";
                        user.value = 0;
                        break;
                    }
                }             
            };
            ViewBag.ArrData2 = UserChartList.ToArray();
            List<UserData2> ExpenseChartList = new List<UserData2>();   //Finding Chart Data for a users spending per category
            var Budget2 = db.Budgets.Find(id);
            UserData2 Expenses;
            Household household2 = db.Users.FirstOrDefault(u => u.Id == userId).Household;
            var HouseholdUserIds = db.Users.Where(u => u.HouseholdId == household2.Id).Select(s => s.Id);
            var BudgetCategories = Budget2.Transactions.Select(t => t.TransactionCategory).Distinct().Where(tc => tc != null);    
            var DistinctCategoryNames = BudgetCategories.Select(bc => bc.Name).Distinct().ToList();
            //a new budget does not have any transaction categories
            if (DistinctCategoryNames != null && DistinctCategoryNames.Count == 0)
            {
                DistinctCategoryNames.Add("No Categories");
            }

            foreach (var transactioncategory in DistinctCategoryNames)
            {
                Expenses = new UserData2();
                Expenses.label = transactioncategory;
                var ExpenseList = db.Transactions.ToList().Where(t => t.BudgetId == Budget.Id).Where(t => t.Void != true).Where(t => t.TransactionCategory != null && t.TransactionCategory.Name == transactioncategory).Select(p => p.Amount).Sum(s => Convert.ToInt32(s));

                if (ExpenseList == 0)
                {
                    Expenses.value = 0;
                    Expenses.label = "None";
                }
                else
                {
                    Expenses.value = ExpenseList;
                }
               
                ExpenseChartList.Add(Expenses);
            }

            ViewBag.ArrData3 = ExpenseChartList.ToArray();          

            List<UserData2> DaysLeftList = new List<UserData2>();   //Finding Days Left in Budget Period
            var Budget3 = db.Budgets.Find(id);
            UserData2 Days;
            string start = Budget3.BudgetStartDate.ToShortDateString();
            DateTime startdate = DateTime.Parse(start);
            DateTime expirydate = Budget3.BudgetEnd;
            string enddate = expirydate.ToShortDateString();
            DateTime now = DateTime.Now;
            TimeSpan x = expirydate - now;
            TimeSpan y = now - startdate;
            int daysremain = x.Days;
            int daysintobudget = y.Days;

            Days = new UserData2();
            Days.label = "Days Remaining in Budget";
            Days.value = daysremain;
            DaysLeftList.Add(Days);

            Days = new UserData2();
            Days.label = "Days Into Budget";
            Days.value = daysintobudget;
            DaysLeftList.Add(Days);
            ViewBag.ArrData4 = DaysLeftList.ToArray();

            decimal expenditures = 0;
            if(db.Transactions.Where(t=>t.BudgetId == id).Any())
            {
                expenditures = db.Transactions.Where(t => t.BudgetId == id).Select(t => t.Amount).Sum();
            }      
            if (expenditures ==0)
            {
                ViewBag.TotalExpenses = 0;
            }            
            else
            {
                ViewBag.TotalExpenses = expenditures;
            }
                
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            List<ApplicationUser> Householdmembers = new List<ApplicationUser>();
            foreach (var member in db.Users)
            {
                if (member.HouseholdId == user.HouseholdId)
                {
                    Householdmembers.Add(member);
                }
            }


            ViewBag.UserId = new SelectList(Householdmembers, "Id", "FullName");  //a list of users in the household
            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Amount,Description,SuggestedAmount,BudgetEndDate,BudgetStartDate,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.BudgetRemaining = budget.Amount;
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Users, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public PartialViewResult Edit(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return PartialView();
            }
            ViewBag.BudgetDurationPeriodId = new SelectList(db.BudgetDurationPeriods, "Id", "Description");
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);

            return PartialView(budget);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Amount,Description,BudgetStartDate,HouseholdId,BudgetEnd")] Budget budget)
        {
            if (ModelState.IsValid)
            {


                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/"+budget.Id);
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public PartialViewResult Delete(int? id)
        {
            if (id == null)
            {
                return PartialView();
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return PartialView();
            }
            return PartialView(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            var BudgetTransactions = db.Transactions.Where(t => t.BudgetId == budget.Id).ToList();
            foreach (var transaction in BudgetTransactions)
            {
                db.Transactions.Remove(transaction);
            }
            db.SaveChanges();
            db.Budgets.Remove(budget);
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
