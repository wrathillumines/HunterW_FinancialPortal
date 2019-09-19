using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccounts bankAccounts = db.BankAccounts.Find(id);
            if (bankAccounts == null)
            {
                return HttpNotFound();
            }
            return View(bankAccounts);
        }

        // GET: BankAccounts/CreateFirst
        [Authorize(Roles = "House Head")]
        public ActionResult CreateFirst(string userId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            var houseId = db.Households.Where(h => h.MyHouse == currentUser);   

            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(houseId, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/CreateFirst
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFirst([Bind(Include = "Id,BankAccountTypeId,Name,Balance")] BankAccounts bankAccounts)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var house = db.Users.Find(userId).MyHouseId;
                bankAccounts.HouseholdId = house.Value;

                db.BankAccounts.Add(bankAccounts);
                db.SaveChanges();
                return RedirectToAction("Create", "Budgets");
            }

            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name", bankAccounts.BankAccountTypeId);
            return RedirectToAction("Dashboard", "Home");
        }

        // GET: BankAccounts/Create
        [Authorize(Roles = "House Head")]
        public ActionResult Create(string userId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            var houseId = db.Households.Where(h => h.MyHouse == currentUser);

            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(houseId, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankAccountTypeId,Name,Balance")] BankAccounts bankAccounts)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var house = db.Users.Find(userId).MyHouseId;
                bankAccounts.HouseholdId = house.Value;

                db.BankAccounts.Add(bankAccounts);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name", bankAccounts.BankAccountTypeId);
            return RedirectToAction("Dashboard", "Home");
        }

        // GET: BankAccounts/Edit/5
        [Authorize(Roles = "House Head")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccounts bankAccounts = db.BankAccounts.Find(id);
            if (bankAccounts == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name", bankAccounts.BankAccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccounts.HouseholdId);
            return View(bankAccounts);
        }

        // POST: BankAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,BankAccountTypeId,Name,Balance")] BankAccounts bankAccounts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name", bankAccounts.BankAccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccounts.HouseholdId);
            return View(bankAccounts);
        }

        // GET: BankAccounts/Delete/5
        [Authorize(Roles = "House Head")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccounts bankAccounts = db.BankAccounts.Find(id);
            if (bankAccounts == null)
            {
                return HttpNotFound();
            }
            return View(bankAccounts);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccounts bankAccounts = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccounts);
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Home");
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
