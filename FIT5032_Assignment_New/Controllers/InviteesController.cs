using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FIT5032_Assignment_New.Models;
using FIT5032_Assignment_New.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FIT5032_Assignment_New.Controllers
{
    public class InviteesController : Controller
    {
        private Maps db = new Maps();
        private ApplicationUserManager _userManager;

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
        // GET: Invitees
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userEmail = GetUserName(userId);
            var invitees = db.Invitees.Where(s => s.Email == userEmail).ToList();
            //var invitees = db.Invitees.Include(i => i.Location);
            return View(invitees);
        }

        // GET: Invitees/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitee invitee = db.Invitees.Find(id);
            if (invitee == null)
            {
                return HttpNotFound();
            }
            return View(invitee);
        }

        // GET: Invitees/Create
        [Authorize]
        public ActionResult Create(int invitationId)
        {
            //SelectList(db.Locations, "Id", "LocationName");
            var invitee = new Invitee();
            var invitation = db.Locations.Find(invitationId);
            ViewBag.InvitationId = invitation.Id;
            return View();
        }

        public ActionResult Error(int id, string condition)
        {
            ViewBag.Message = "Error";
            if (condition.Equals("exist"))
            {
                ViewBag.Message = "You can't invite same person twice!";
            }
            return View(id);
        }

        // POST: Invitees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Email,Status,InvitationId")] Invitee invitee)
        {
            String sender = GetUserName(db.Locations.Find(invitee.InvitationId).InviterId);
            invitee.Status = "Pending";
            var foo = db.Invitees.Where(v => v.Email.Equals(invitee.Email) && v.InvitationId == invitee.InvitationId);
            

            ModelState.Clear();
            TryValidateModel(invitee);
            if (foo.Count() != 0)
            {
                //return RedirectToAction("Error", new { id = invitee.InvitationId, condition = "exist" });
                ModelState.AddModelError("Email", "You can't invite a person twice.");
            }

            if (invitee.Email.Equals(sender))
            {
                ModelState.AddModelError("Email", "You can't invite yourself.");
            }

            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
            if (!r.IsMatch(invitee.Email))
            {
                ModelState.AddModelError("Email", "Please input a valid Email address.");
            }

            if (ModelState.IsValid)
            {
                db.Invitees.Add(invitee);
                db.SaveChanges();
                try
                {
                    String toEmail = invitee.Email;
                    String subject = "You Recieved An Invitation on TINV!";
                    String contents = sender + " is inviting you to an exciting event. Check the link below!";
                    String link = Url.Action("Details", "Invitees", new { id = invitee.Id }, "http");
                    
                    EmailSender es = new EmailSender();
                    es.Send(toEmail, subject, contents, link);
                }
                catch { }
                return RedirectToAction("Details", "Locations", new { id = invitee.InvitationId});
            }

            //ViewBag.InvitationId = new SelectList(db.Locations, "Id", "LocationName", invitee.InvitationId);
            return View(invitee);
        }

        // GET: Invitees/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Accept", Value = "Accepted" });
            items.Add(new SelectListItem { Text = "Not sure", Value = "Pending" });
            items.Add(new SelectListItem { Text = "Decline", Value = "Declined" });
            ViewBag.Status = items;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitee invitee = db.Invitees.Find(id);
            if (invitee.Email != UserManager.GetEmail(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "Home");
            }
            if (invitee == null)
            {
                return HttpNotFound();
            }
            //ViewBag.InvitationId = new SelectList("accepted", "pending", "declined");
            return View(invitee);
        }

        // POST: Invitees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Email,Status,InvitationId")] Invitee invitee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.InvitationId = new SelectList("accepted", "pending", "declined");
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Accept", Value = "Accepted" });
            items.Add(new SelectListItem { Text = "Not sure", Value = "Pending" });
            items.Add(new SelectListItem { Text = "Decline", Value = "Declined" });
            ViewBag.Status = items;
            return View(invitee);
        }

        // GET: Invitees/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitee invitee = db.Invitees.Find(id);
            if (invitee == null)
            {
                return HttpNotFound();
            }
            return View(invitee);
        }

        // POST: Invitees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitee invitee = db.Invitees.Find(id);
            db.Invitees.Remove(invitee);
            db.SaveChanges();
            return RedirectToAction("Index", "Invitees");
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
            catch (Exception)
            {
                return "Unknown User";
            }
        }

    }
}
