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
    public class TransactionCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TransactionCategories
        public ActionResult Index()
        {
            return View(db.TransactionCategories.ToList());
        }

        // GET: TransactionCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return HttpNotFound();
            }
            return View(transactionCategory);
        }

        // GET: TransactionCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransactionCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,CreatedDate,UpdatedDate")] TransactionCategory transactionCategory)
        {
            if (ModelState.IsValid)
            {
                db.TransactionCategories.Add(transactionCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transactionCategory);
        }

        // GET: TransactionCategories/Edit/5
        public PartialViewResult Edit(int? id, int? transactionid)
        {
            if (id == null)
            {
                return PartialView();
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return PartialView();
            }
            ViewBag.transactionid = transactionid;
            return PartialView(transactionCategory);
        }

        public PartialViewResult NewCategory(int? transactionid)
        {

            if (transactionid == null)
            {
                return PartialView();
            }
            Transaction transaction = db.Transactions.Find(transactionid);
            TransactionCategory transactionCategory = db.TransactionCategories.Find(transactionid);
            if (transaction.TransactionCategoryId == null)
            {
                ViewBag.transactionnull = "N/A";
                ViewBag.transactionid = transactionid;
                return PartialView(transaction.TransactionCategory);
            }
            return PartialView();
        }


        //POST

        public ActionResult NewCategoryConfirm([Bind(Include = "Name")] TransactionCategory transactionCategory)
        {
            if (ModelState.IsValid)

            {
                var userId = User.Identity.GetUserId();
                int y = Convert.ToInt32(TempData["Data1"]);
                var Transaction = db.Transactions.FirstOrDefault(t => t.Id == y);
                var budgetid = Transaction.BudgetId;

                var existingNames = db.TransactionCategories.Where(tc => tc.CreatedByUserId == userId).Select(tc => tc.Name).ToList();
                if (existingNames.Contains(transactionCategory.Name))
                {

                    Transaction.TransactionCategoryId = db.TransactionCategories.FirstOrDefault(tc => tc.Name == transactionCategory.Name).Id;
                    db.Transactions.Attach(Transaction);
                    db.Entry(Transaction).Property("TransactionCategoryId").IsModified = true;
                    db.SaveChanges();
                }
                else
                {

                    TransactionCategory TransactionCat = new TransactionCategory();
                    TransactionCat.CreatedDate = DateTime.Now;
                    TransactionCat.CreatedByUserId = userId;
                    TransactionCat.Name = transactionCategory.Name;
                    db.TransactionCategories.Add(TransactionCat);
                    db.SaveChanges();


                    Transaction.TransactionCategoryId = TransactionCat.Id;
                    db.Transactions.Attach(Transaction);
                    db.Entry(Transaction).Property("TransactionCategoryId").IsModified = true;


                    db.SaveChanges();

                }

                return RedirectToAction("Details", "Budgets", new { id = budgetid });
            }
            return RedirectToAction("Index", "Home");
        }



        // POST: TransactionCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CreatedDate")] TransactionCategory transactionCategory, string button)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                int y = Convert.ToInt32(TempData["Data1"]);
                var Transaction = db.Transactions.FirstOrDefault(t => t.Id == y);
                var BudgetId = Transaction.BudgetId;

                if (button == "Save")
                {

                    transactionCategory.UpdatedDate = DateTime.Now;

                    db.Entry(transactionCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Budgets", new { id = BudgetId });
                }
                else if (button == "Update just this transaction")
                {
                    var existingNames = db.TransactionCategories.Where(tc => tc.CreatedByUserId == userId).Select(tc => tc.Name).ToList();
                    if (existingNames.Contains(transactionCategory.Name))
                    {
                        var oldTransaction = db.TransactionCategories.Find(transactionCategory.Id);
                        transactionCategory.CreatedDate = oldTransaction.CreatedDate;
                        transactionCategory.UpdatedDate = DateTime.Now;
                        transactionCategory.Id = db.TransactionCategories.FirstOrDefault(tc => tc.Name == transactionCategory.Name).Id;
                        db.Entry(transactionCategory).State = EntityState.Modified;
                    }
                    else
                    {

                        TransactionCategory TransactionCat = new TransactionCategory();
                        TransactionCat.CreatedDate = DateTime.Now;
                        TransactionCat.CreatedByUserId = userId;
                        TransactionCat.Name = transactionCategory.Name;
                        db.TransactionCategories.Add(TransactionCat);
                        db.SaveChanges();


                        Transaction.TransactionCategoryId = TransactionCat.Id;
                        db.Transactions.Attach(Transaction);
                        db.Entry(Transaction).Property("TransactionCategoryId").IsModified = true;
                        db.SaveChanges();
                    }

                }
                else
                {
                    var userstransactions = db.Transactions.Where(t => t.SubmitterUserId == userId);
                    var transactionCategoryd = db.TransactionCategories.FirstOrDefault(t => t.Id == transactionCategory.Id).Id;
                    foreach (var transaction in userstransactions)
                    {
                        if (transaction.TransactionCategoryId == transactionCategoryd)
                        {
                            transaction.TransactionCategoryId = null;
                            transaction.TransactionCategory.Name = "N/A";
                            db.Entry(transaction).Property("TransactionCategoryId").IsModified = true;
                        }
                    }


                    db.SaveChanges();
                    return RedirectToAction("Details", "Budgets", new { id = BudgetId });
                }

                return RedirectToAction("Details", "Budgets", new { id = BudgetId });
            }
            return View(transactionCategory);
        }

        // GET: TransactionCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return HttpNotFound();
            }
            return View(transactionCategory);
        }

        // POST: TransactionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            db.TransactionCategories.Remove(transactionCategory);
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
