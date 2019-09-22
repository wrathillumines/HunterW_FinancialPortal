using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        InvitationHelper inviteHelper = new InvitationHelper();

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

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        //GET: SendHouseInvitation
        public ActionResult SendHouseInvitation(int id)
        {
            var invite = new Invitation { HouseholdId = id };
            return View(invite);
        }

        //POST: SendHouseInvitation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendHouseInvitation(Invitation model)
        {
            var invite = inviteHelper.CreateInvite(model);
            await inviteHelper.SendHouseInvite(invite);
            return RedirectToAction("Details", "Households", new { id = model.HouseholdId });
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
