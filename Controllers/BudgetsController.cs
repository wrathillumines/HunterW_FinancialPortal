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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var budgets = db.Budgets.Include(b => b.Household);
            return View(budgets.ToList());
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int id)
        {
            var model = new BudgetViewModel();
            var budget = db.Budgets.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            model.Budget = budget;
            model.BudgetItems = db.BudgetItems.ToList();

            return View(model);
        }

        // GET: Budgets/Create
        [Authorize(Roles = "House Head")]
        public ActionResult Create(string userId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            var houseId = db.Households.Where(h => h.MyHouse == currentUser);

            ViewBag.BankAccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(houseId, "Id", "Name");
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Description")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var house = db.Users.Find(userId).MyHouseId;
                budget.HouseholdId = house.Value;

                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("CreateFirst", "BudgetItems");
            }

            return RedirectToAction("Dashboard", "Home");
        }

        // GET: Budgets/Edit/5
        [Authorize(Roles = "House Head")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Description")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        [Authorize(Roles = "House Head")]
        public ActionResult Delete(int? id)
        {
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

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
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
