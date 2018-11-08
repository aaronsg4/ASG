using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASG.Models;
using System.IO;
using PagedList;
using System.Web.Security;

namespace ASG.Controllers
{
    //[RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TopicHelper helper = new TopicHelper();       

        // GET: BlogPosts
        public ActionResult Index(int? page, string message = "")
        {
            if (message != "")
            {
                ViewBag.Message = message;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var Published = db.Posts.Where(s => s.Published).ToList();
            return View(Published.OrderByDescending(p=>p.createdDate).ToPagedList(pageNumber, pageSize));    
        }

        public PartialViewResult BlogPartialUpper()
        {
           
            return PartialView("~/Views/BlogPosts/_UpperBlogPartialView.cshtml",db.Topics.ToList()); 
        }
        public PartialViewResult BlogPartialMid()
        {
            return PartialView("~/Views/BlogPosts/_MidBlogPartialView.cshtml", db.Posts.Where(s => s.Published).OrderByDescending(p => p.Comments.Count).Take(3).ToList());
        }
        public PartialViewResult BlogPartialLower()
        {

            return PartialView("~/Views/BlogPosts/_LowerBlogPartialView.cshtml",db.Posts.Where(s => s.Published).OrderByDescending(p=>p.createdDate).Take(6).ToList());
        }
        public PartialViewResult BlogPartialLowLower()
        {

            return PartialView("~/Views/BlogPosts/_LowLowerBlogPartialView.cshtml", db.Topics.ToList()); 
        }

        public ActionResult Search(int? page, string query)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Query = query;
            var queryposts = db.Posts.AsQueryable();
            var Published = db.Posts.Where(s => s.Published).ToList();
            if (!String.IsNullOrWhiteSpace(query)) //means if the query is coming in
            {
                queryposts = queryposts.Where(p => p.Title.Contains(query) || p.Body.Contains(query) || p.Comments.Any(c => c.Body.Contains(query) || c.Commenter.DisplayName.Contains(query)));
            }
            
            return View("Index", queryposts.OrderByDescending(p => p.createdDate).ToPagedList(pageNumber, pageSize));
        }

        //GET
        public ActionResult PendingPosts(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var unpublished = db.Posts.Where(s => s.Published == false).ToList();
            return View(unpublished.OrderByDescending(p => p.createdDate).ToPagedList(pageNumber, pageSize));        
        }
    

        public ActionResult PublishPending(int? PendingPostId)
        {
            if (PendingPostId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var PendingPost = db.Posts.FirstOrDefault(s => s.Id == PendingPostId);
            if (PendingPost == null)
            {
                return HttpNotFound();
            }

            PendingPost.createdDate = DateTime.Now;
            PendingPost.Published = true;
            db.Entry(PendingPost).Property("Published").IsModified = true;
            db.SaveChanges();
       
            return RedirectToAction("Index");
        }
        
       

        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug
            );
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        [Authorize(Roles ="Admin")]
        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "TopicName");
            return View();
        }     


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Post, Topics")] BlogTopicsViewModel model, HttpPostedFileBase Image, string button)            
        {
            if (ModelState.IsValid)
            {              
                
                var Slug = StringUtilities.URLFriendly(model.Post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(model);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(model);
                }
                var ErrorMessage = "";
                if (ImageUploadValidator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var customName = string.Format(Guid.NewGuid() + fileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/NEWIMAGES/"), customName));
                    model.Post.MediaURL = "~/NEWIMAGES/" + customName;
                }
                else
                {
                    ViewBag.ErrorMessage = "Please select an image between 1KB-2MB and in an approved format (.jpg, .bmp, .png, .gif)";
                    model.Post.MediaURL = "images/module-1.jpg"; 
                }
                model.Post.createdDate = DateTime.Now;
                model.Post.Slug = Slug;
                if (button == "Create")
                {
                    model.Post.Published = false;
                }
                else
                {
                    model.Post.Published = true;
                }             
                
                db.Posts.Add(model.Post);
                db.SaveChanges();

                if (model.Topics != null)
                {
                    foreach (var vm in model.Topics)
                    {
                        int topicId = Convert.ToInt32(vm);
                        helper.AddTopicToBlog(topicId, model.Post.Id);
                    }
                }

                return RedirectToAction("Index", new { message = ErrorMessage });               
              
            }
            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "Description");
            return View(model);
        }

       
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            BlogTopicsViewModel vm = new BlogTopicsViewModel()
            {
                Post = blogPost
            };                
            
            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "TopicName");
            return View(vm);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Post, Topics")] BlogTopicsViewModel BlogTopicVM, HttpPostedFileBase image, string button)
        {
             if (ModelState.IsValid)
                {
                    var post = db.Posts.AsNoTracking().First(p=>p.Id==BlogTopicVM.Post.Id);
                    if (BlogTopicVM.Post.Title != post.Title)
                    {
                        var Slug = StringUtilities.URLFriendly(BlogTopicVM.Post.Title);
                        if (String.IsNullOrWhiteSpace(Slug))
                        {
                            ModelState.AddModelError("Title", "Invalid title");
                            return View(BlogTopicVM);
                        }
                        if (db.Posts.Any(p => p.Slug == Slug))
                        {
                            ModelState.AddModelError("Title", "The title must be unique");
                            return View(BlogTopicVM);
                        }
                        BlogTopicVM.Post.Slug = Slug;

                    }
                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {

                    var filePath = Server.MapPath(post.MediaURL);
                    if (filePath != null || filePath != Path.Combine(Server.MapPath("~/ NEWIMAGES/"), "blog.jpg"))
                    {
                        System.IO.File.Delete(filePath);
                    }


                    var fileName = Path.GetFileName(image.FileName);
                        var customName = string.Format(Guid.NewGuid() + fileName);
                        image.SaveAs(Path.Combine(Server.MapPath("~/NEWIMAGES/"), customName));
                        BlogTopicVM.Post.MediaURL = "~/NEWIMAGES/" + customName;                   

                    }
                if (post.Topics != null)
                {
                    foreach (var vm in post.Topics)
                    {
                        helper.RemoveTopicFromBlog(vm.Id, post.Id);
                    }
                    foreach (var vm in BlogTopicVM.Topics)
                    {
                        int topicId = Convert.ToInt32(vm);
                        helper.AddTopicToBlog(topicId, BlogTopicVM.Post.Id);
                    }
                }

                if (button == "PUBLISH") { 
                    BlogTopicVM.Post.Published = true;
                    }
                else {
                    BlogTopicVM.Post.Published = false;
                }
                    BlogTopicVM.Post.updatedDate = DateTime.Now;
                    db.Entry(BlogTopicVM.Post).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }

            ViewBag.Topics = new MultiSelectList(db.Topics, "Id", "TopicName");
            return View(BlogTopicVM);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            var filePath = Server.MapPath(blogPost.MediaURL);           
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "PostId, Id, Body,updatedReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {

                comment.updatedDate = DateTime.Now;
                db.Comments.Attach(comment);
                db.Entry(comment).Property("Body").IsModified = true;
                db.Entry(comment).Property("updatedReason").IsModified = true;
                db.Entry(comment).Property("updatedDate").IsModified = true;                
                db.SaveChanges();
                
                return RedirectToAction("Index", "BlogPosts");
                
            }
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return RedirectToAction("Edit", "Comments");
        }

        // GET: Comments/Delete/5
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditReply(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = db.Replies.Find(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpecificCommentId = new SelectList(db.Comments, "Id", "Body", reply.SpecificCommentId);
            return View(reply);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReply([Bind(Include = "Id, Body,updatedReason")] Reply reply)
        {
            if (ModelState.IsValid)
            {

                reply.updatedDate = DateTime.Now;
                db.Replies.Attach(reply);
                db.Entry(reply).Property("Body").IsModified = true;
                db.Entry(reply).Property("updatedReason").IsModified = true;
                db.Entry(reply).Property("updatedDate").IsModified = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.SpecificCommentId = new SelectList(db.Posts, "Id", "Title", reply.SpecificCommentId);
            return RedirectToAction("Edit", "Comments");
        }

        // GET: Comments/Delete/5
        public ActionResult DeleteReply(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = db.Replies.Find(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            return View(reply);
        }

        // POST: Comments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReply(int id)
        {
            Reply reply = db.Replies.Find(id);
            db.Replies.Remove(reply);
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
