using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HunterW_FinancialPortal.Models;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems/Create
        [Authorize(Roles = "House Head")]
        public ActionResult Create()
        {
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            return View();
        }

        // POST: BudgetItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BudgetId,Name,Description,Amount,Payee")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();

                var budgetId = budgetItem.BudgetId;

                return RedirectToAction("Details", "Budgets", new { id = budgetId });
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            return RedirectToAction("Dashboard", "Home");
        }

        // GET: BudgetItems/CreateFirst
        [Authorize(Roles = "House Head")]
        public ActionResult CreateFirst()
        {
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            return View();
        }

        // POST: BudgetItems/CreateFirst
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFirst([Bind(Include = "Id,BudgetId,Name,Description,Amount,Payee")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            return RedirectToAction("Dashboard", "Home");
        }

        // GET: BudgetItems/Delete/5
        [Authorize(Roles = "House Head")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            var itemId = budgetItem.Id;
            db.BudgetItems.Remove(budgetItem);
            db.SaveChanges();
            return RedirectToAction("Details", "Budgets", new { id = itemId });
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
