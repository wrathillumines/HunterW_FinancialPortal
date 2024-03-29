﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HunterW_FinancialPortal.ExtensionMethods;
using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.BankAccount).Include(t => t.Budget).Include(t => t.CreatedBy).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());

            ViewBag.BankAccountsId = new SelectList(db.BankAccounts.Where(b => b.HouseholdId == currentUser.MyHouseId), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.Budgets.Where(b => b.HouseholdId == currentUser.MyHouseId), "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,TransactionAmount,CreatedById,BankAccountsId,TransactionTypeId,BudgetId,Created,Updated")] Transaction transaction)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                transaction.CreatedById = User.Identity.GetUserId();
                transaction.Created = DateTimeOffset.Now;
                db.Transactions.Add(transaction);
                db.SaveChanges();

                transaction.UpdateBalance();

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.BankAccountsId = new SelectList(db.BankAccounts.Where(b => b.HouseholdId == currentUser.MyHouseId), "Id", "Name", transaction.BankAccountsId);
            ViewBag.BudgetId = new SelectList(db.Budgets.Where(b => b.HouseholdId == currentUser.MyHouseId), "Id", "Name", transaction.BudgetId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
