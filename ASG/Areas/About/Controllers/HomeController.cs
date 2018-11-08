using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASG.Areas.About.Controllers
{
    public class HomeController : Controller
    {
        // GET: About/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Resume()
        {
            return View();
        }

        public ActionResult Timeline()
        {
            return View();
        }
        // GET: About/Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: About/Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: About/Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {         
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: About/Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: About/Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: About/Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: About/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
