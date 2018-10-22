using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_Assignment_New.Models;
using FIT5032_Assignment_New.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FIT5032_Assignment_New.Controllers
{
    public class LocationsController : Controller
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

        // GET: Locations
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var locations = db.Locations.Where(s => s.InviterId == userId).ToList();
            //var locations = from m in db.Locations
            //                where m.
            return View(locations);
        }

        // GET: Locations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            ViewBag.Invitees = null;
            IEnumerable<Invitee> invitees = db.Invitees.Where(s => s.InvitationId == id);
            if (invitees.Count() != 0)
            {
                ViewBag.Invitees = invitees;
            }
            
            return View(location);
        }

        // GET: Locations/Create
        [Authorize]
        public ActionResult Create()
        {
            var location = new Location();
            //var invitees = new List<Invitee>();
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,LocationName,Description,Latitude,Longitude,Date, InviterId")] Location location)
        {
            location.InviterId = User.Identity.GetUserId();
            ModelState.Clear();
            TryValidateModel(location);
            //List<Invitee> invitees = new List<Invitee>();//
            //if (selectedInvitees != null)//
            //{//

            //    foreach (var invitee in selectedInvitees)//
            //    {//
            //        var inviteeToAdd = new Invitee();//
            //        inviteeToAdd.Email = invitee;//
            //        invitees.Add(inviteeToAdd);//

            //    }//
            //}//
            if (string.IsNullOrEmpty(location.LocationName))
            {
                ModelState.AddModelError("LocationName", "Tell us the location name.");
            }

            if (string.IsNullOrEmpty(location.Description))
            {
                ModelState.AddModelError("Description", "Tell us what the event is.");
            }

            if (string.IsNullOrEmpty(location.Date.ToString()))
            {
                ModelState.AddModelError("Date", "Tell us when it is.");
            }

            if (string.IsNullOrEmpty(location.Latitude.ToString()))
            {
                ModelState.AddModelError("Latitude", "Tell us where it is.");
            }

            if (string.IsNullOrEmpty(location.Longitude.ToString()))
            {
                ModelState.AddModelError("Longitude", "Tell us where it is.");
            }

            if (DateTime.Compare(DateTime.Now, location.Date) > 0)
            {
                ModelState.AddModelError("Date", "You can't pick a date earlier than now.");
            }

            if (location.Latitude < -90 || location.Latitude > 90)
            {
                ModelState.AddModelError("Latitude", "Latitude should be in range of [-90,90]");
            }
            if (location.Longitude < -180 || location.Longitude > 180)
            {
                ModelState.AddModelError("Longitude", "Longitude should be in range of [-180,180]");
            }
            if (ModelState.IsValid)
            {
                db.Locations.Add(location);
                //foreach (var m in invitees)
                //    db.Invitees.Add(m);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }

        // GET: Locations/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location.InviterId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }

            if (DateTime.Compare(location.Date, DateTime.Now) < 0)
            {
                return RedirectToAction("Index");
            }

            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LocationName,Description,Latitude,Longitude,Date, InviterId")] Location location)
        {
            if (string.IsNullOrEmpty(location.LocationName))
            {
                ModelState.AddModelError("LocationName", "Tell us the location name.");
            }

            if (string.IsNullOrEmpty(location.Description))
            {
                ModelState.AddModelError("Description", "Tell us what the event is.");
            }

            if (string.IsNullOrEmpty(location.Date.ToString()))
            {
                ModelState.AddModelError("Date", "Tell us when it is.");
            }

            if (string.IsNullOrEmpty(location.Latitude.ToString()))
            {
                ModelState.AddModelError("Latitude", "Tell us where it is.");
            }

            if (string.IsNullOrEmpty(location.Longitude.ToString()))
            {
                ModelState.AddModelError("Longitude", "Tell us where it is.");
            }

            if (DateTime.Compare(DateTime.Now, location.Date) > 0)
            {
                ModelState.AddModelError("Date", "You can't pick a date earlier than now.");
            }

            if (location.Latitude < -90 || location.Latitude > 90)
            {
                ModelState.AddModelError("Latitude", "Latitude should be in range of [-90,90]");
            }
            if (location.Longitude < -180 || location.Longitude > 180)
            {
                ModelState.AddModelError("Longitude", "Longitude should be in range of [-180,180]");
            }
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                string sender = GetUserName(location.InviterId);
                string des = location.Description;
                foreach (Invitee invitee in location.Invitees)
                {
                    
                   // if (invitee.Status.Equals("Accepted"))
                   // {
                        try
                        {
                            String toEmail = invitee.Email;
                            String subject = "An event's detail has changed!";
                            String contents = sender + "has changed event " + des + "'s detail. Check the link below!";
                            String link = Url.Action("Details", "Invitees", new { id = invitee.Id }, "https");

                            EmailSender es = new EmailSender();
                            es.Send(toEmail, subject, contents, link);
                        }
                        catch {
                            return RedirectToAction("Index", "Home");
                        }
                   // }
                    //else
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                    
                }
                
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location.InviterId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);
            var invitees = db.Invitees.Where(s => s.InvitationId == id);
            foreach(var invitee in invitees)
            {
                db.Invitees.Remove(invitee);
            }
            db.Locations.Remove(location);
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
            catch (Exception)
            {
                return "Unknown User";
            }
        }
    }
}
