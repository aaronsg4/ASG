using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASG.Areas.BugTracker.Models;
using ASG.Models;

namespace ASG.Areas.BugTracker.Models.Controllers
{
        public class SystemUpdatesController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: SystemUpdates
            public ActionResult Index()
            {
                return View(db.SystemUpdates.ToList());
            }

            // GET: SystemUpdates/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SystemUpdates systemUpdates = db.SystemUpdates.Find(id);
                if (systemUpdates == null)
                {
                    return HttpNotFound();
                }
                return View(systemUpdates);
            }

            // GET: SystemUpdates/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: SystemUpdates/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "Id,Name,Description")] SystemUpdates systemUpdates)
            {
                if (ModelState.IsValid)
                {
                    systemUpdates.UpdateDate = DateTime.Now;
                    db.SystemUpdates.Add(systemUpdates);
                    db.SaveChanges();
                    return RedirectToAction("Landing", "Home");
                }

                return View(systemUpdates);
            }

            // GET: SystemUpdates/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SystemUpdates systemUpdates = db.SystemUpdates.Find(id);
                if (systemUpdates == null)
                {
                    return HttpNotFound();
                }
                return View(systemUpdates);
            }

            // POST: SystemUpdates/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "Id,Name,Description")] SystemUpdates systemUpdates)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(systemUpdates).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(systemUpdates);
            }

            // GET: SystemUpdates/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SystemUpdates systemUpdates = db.SystemUpdates.Find(id);
                if (systemUpdates == null)
                {
                    return HttpNotFound();
                }
                return View(systemUpdates);
            }

            // POST: SystemUpdates/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                SystemUpdates systemUpdates = db.SystemUpdates.Find(id);
                db.SystemUpdates.Remove(systemUpdates);
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

