using ASG.Areas.FinancialPlanner.Models;
using ASG.Controllers;
using ASG.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASG.Areas.FinancialPlanner.Controllers
{
    public class HomeController : Controller
    {
        // GET: FinancialPlanner/Home
        public ActionResult Index()
        {
            ExpireOldBudgets();
            CreateDemoBudget();          
            return View();
        }

        public static void ExpireOldBudgets()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var today = DateTime.Now;
            foreach (var budget in db.Budgets.Where(b => b.Expired == false))
            {
                if (budget.BudgetEnd < today)
                {
                    budget.Expired = true;
                }
            }
            db.SaveChanges();
        }

        
        public double GetRandomDouble(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }    

        public void CreateDemoBudget()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var guestUserHousehold = db.Users.Where(u => u.UserName == "GuestUser@Mailinator.com").FirstOrDefault().Household;
            if (!(guestUserHousehold.Budgets.Where(b => b.Expired == false).Any()))
            {                
                var guestUserId = db.Users.Where(u => u.UserName == "GuestUser@Mailinator.com").FirstOrDefault().Id;
                var TransactionCategoriesForGuestUser = (db.TransactionCategories.Where(tc => tc.CreatedByUserId == guestUserId)).ToArray();
                var numberOfTransactionCategories = TransactionCategoriesForGuestUser.Count();  
                var householdFinancialAccounts = db.Accounts.Where(a => a.HouseholdId == guestUserHousehold.Id).ToList();

                if (householdFinancialAccounts != null)
                {
                    foreach (var account in householdFinancialAccounts)
                    {
                        account.ActualBalance = 5000.00M;
                        account.ReconciledBalance = 5000.00M;
                        account.UpdatedDate = DateTime.Now;
                    }
                }

                double numberOfDaysAgoBudgetWasCreated = GetRandomDouble(1.00, 25.00);
                var allHouseholdUsers = guestUserHousehold.Users.ToArray();
                var numberOfHouseholdUsers = allHouseholdUsers.Count();
                Budget budget = new Budget();             
                
       
                budget.Expired = false;
                budget.HouseholdId = guestUserHousehold.Id;
                budget.BudgetStartDate = DateTime.Now.AddDays(-numberOfDaysAgoBudgetWasCreated);
                budget.BudgetEnd = budget.BudgetStartDate.AddDays(30);
                budget.BudgetDurationPeriodId = 3;
                budget.UserId = guestUserHousehold.UserId;                
                budget.BudgetEndDate = 0;
                budget.Amount = 5000;
                budget.BudgetRemaining = 5000;
                budget.Description = "General Monthly Expenses";
                budget.Name = "Demo Users' Budget";

                db.Budgets.Add(budget);
                db.SaveChanges();

                
                Random r = new Random();             
                
                //Create a random number of Transactions between 1-10
                int randomNumberOfTransactions = r.Next(1, 10);               
                

                //This will track all of the household expenses
                double totalOfAllTransactions = 0.00;    

                //Keep track of expenses for each account in the household, starting at 0.00
                Dictionary<int, double> householdAccountsAndExpenses = new Dictionary<int, double>();
                foreach (var account in householdFinancialAccounts)
                {
                    householdAccountsAndExpenses.Add(account.Id, 0.00);
                }

                int i = 0;

                //Create transactions
                while (i < randomNumberOfTransactions)
                {

                    int randomHouseholdUser = r.Next(0, numberOfHouseholdUsers);                    
                    int randomTransactionCategoryRecord = r.Next(0, numberOfTransactionCategories);

                    //Since a budget has been created, we only want to create transactions between today and the day that the budget was created
                    double numberOfDaysAgoTransactionWasCreated = GetRandomDouble(1.00, numberOfDaysAgoBudgetWasCreated);

                    //Create a random Expense amount between 1-100
                    var expenseAmount = GetRandomDouble(1.00, 100.00);

                    //Using the array of all householdusers, pick a random member to give the transaction to
                    var currentUser = allHouseholdUsers[randomHouseholdUser];

                    //Create the random Transactions and add them to the database
                    Transaction T = new Transaction();
                    T.Amount = ((decimal)expenseAmount * -1);
                    T.BudgetId = budget.Id;                    
                    T.Reconciled = false;
                    T.SubmitterUserId = currentUser.Id;
                    T.FinancialAccountId = householdFinancialAccounts.Where(h => h.AccountHolderUserId == currentUser.Id).FirstOrDefault().Id;             
                    T.Title = "Demo Expense" +"" + i;
                    T.TransactionCategoryId = TransactionCategoriesForGuestUser[randomTransactionCategoryRecord].Id;
                    T.TransactionTypeId = 1;
                    T.Void = false;
                    T.CreatedDate = DateTime.Now.AddDays(-numberOfDaysAgoTransactionWasCreated);
                    db.Transactions.Add(T);

                    //Here you are keeping track of all of the transactions
                    totalOfAllTransactions += (expenseAmount * -1);
                    //Here you are keeping track of all of the transactions per Acount
                    householdAccountsAndExpenses[T.FinancialAccountId] += (expenseAmount * -1);

                    i++;
                }

                //Now update the budget remaining based on how many transactions have occurred.
                budget.BudgetRemaining = budget.BudgetRemaining + (decimal)totalOfAllTransactions;

                //Update each demo users financial Account balance to take into consideration the expenses / transactions generated above
               foreach (var AccountExpenseTotal in householdAccountsAndExpenses)
                {
                    var financialAccount = db.Accounts.Where(a => a.Id == AccountExpenseTotal.Key).FirstOrDefault();    
                    if (financialAccount !=null)
                    {
                        financialAccount.ActualBalance += (decimal)AccountExpenseTotal.Value;
                    }                    
                }
               
                db.SaveChanges();

            }

        }

        public PartialViewResult FPLogin()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPLogin.cshtml");
        }

        public PartialViewResult FPRegister()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPRegister.cshtml");
        }

        public PartialViewResult FPForgotPassword()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPForgotPassword.cshtml");
        }

        public PartialViewResult FPForgotPasswordConfirmation()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPForgotPasswordConfirmation.cshtml");
        }
        public PartialViewResult FPResetPassword()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPResetPassword.cshtml");
        }

        public PartialViewResult FPResetPasswordConfirmation()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPResetPasswordConfirmation.cshtml");
        }

        public PartialViewResult FPAccess()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPLoginAddRole.cshtml");
        }

        public PartialViewResult FPRegistrationConfirmation()
        {
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPRegistrationConfirmation.cshtml");
        }

        public PartialViewResult FPBudgetOptions()
        {
            ApplicationDbContext db = new ApplicationDbContext();
        
                ViewBag.BudgetDurationPeriodId = new SelectList(db.BudgetDurationPeriods, "Id", "Description");
          
               
            return PartialView("~/Areas/FinancialPlanner/Views/Home/_FPBudgetOptions.cshtml");
        }

        // GET: FinancialPlanner/Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FinancialPlanner/Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FinancialPlanner/Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FinancialPlanner/Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FinancialPlanner/Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FinancialPlanner/Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FinancialPlanner/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
