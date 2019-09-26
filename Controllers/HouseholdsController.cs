using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HunterW_FinancialPortal.Helpers;
using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace HunterW_FinancialPortal.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Household Member List
        public ActionResult Members()
        {
            //should we make a view model?

            var userId = User.Identity.GetUserId();
            var houseId = db.Users.Find(userId).MyHouseId;
            var members = db.Users.Where(u => u.MyHouseId == houseId).ToList();

            return View(members);
        }

        // GET: Households/Join
        public ActionResult Join()
        {
            return View();
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }


        //POST: Households/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Households.Add(household);
                db.SaveChanges();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                user.MyHouseId = household.Id;
                db.SaveChanges();
                roleHelper.RemoveUserFromRole(userId, roleName: "User");
                roleHelper.AddUserToRole(userId, roleName: "House Head");
                return RedirectToAction("CreateFirst", "BankAccounts");
            }

            return RedirectToAction("Lobby", "Home");
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
