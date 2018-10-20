using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FIT5032_Assignment_New.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FIT5032_Assignment_New.Controllers
{
    public class PostsController : Controller
    {
        private PostDBContext db = new PostDBContext();
        private ApplicationUserManager _userManager;
        // GET: Posts
        //public ActionResult Index()
        //{
        //    return View(db.Posts.ToList());
        //}
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index(string poster, string searchString)
        {
            //var posterList = new List<string>();
            //var posterEmailList = new List<string>();
            //var PosterQuery = from d in db.Posts
            //                //orderby d.AuthorId
            //                select d.AuthorId;

            //posterList.AddRange(PosterQuery.Distinct());

            //foreach (string x in posterList)
            //{
            //    posterEmailList.Add(GetUserName(x));
            //}
            //ViewBag.poster = new SelectList(posterEmailList);


            var posts = from m in db.Posts
                        select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Title.Contains(searchString));
            }

            ViewBag.posts = posts;

            //if (!String.IsNullOrEmpty(poster))
            //{
            //    posts = posts.Where(x => x.AuthorId == poster);
            //}

            return View(posts);
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,PostDate,AuthorId,Content")] Post post)
        {
            if (string.IsNullOrEmpty(post.Title))
            {
                ModelState.AddModelError("Title", "Give your recipe a proper name.");
            }

            if (string.IsNullOrEmpty(post.Content))
            {
                ModelState.AddModelError("Content", "You haven't written anything.");
            }

            if (ModelState.IsValid)
            {
                post.AuthorId = User.Identity.GetUserId();
                post.PostDate = DateTime.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post.AuthorId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,PostDate,AuthorId,Content")] Post post)
        {
            if (string.IsNullOrEmpty(post.Title))
            {
                ModelState.AddModelError("Title", "Give your recipe a proper name.");
            }

            if (string.IsNullOrEmpty(post.Content))
            {
                ModelState.AddModelError("Content", "You haven't written anything.");
            }
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post.AuthorId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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

        public string GetUserName(string userId)
        {
            // return UserManagerExtensions.GetEmail(userId);
            try
            {
                return UserManager.GetEmail(userId);
            }
            catch
            {
                return "Unkown User";
            }
        }
    }
}
