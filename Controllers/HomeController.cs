using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
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

        public ActionResult Lobby()
        {

            ViewBag.CurrentUser = UserManager.FindById(User.Identity.GetUserId()).FirstName;

            return View();
        }

        public ActionResult Dashboard()
        {
            var model = new DashboardViewModel();
            
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.MyHouseId == null)
            {
                return RedirectToAction("Lobby", "Home");
            }

            var householdId = user.MyHouseId;
            var houseAccounts = db.Households.Find(householdId);

            model.Household = houseAccounts;
            model.BankAccounts = db.BankAccounts.ToList();
            model.Transactions = db.Transactions.OrderByDescending(t => t.Created).ToList();

            ViewBag.CurrentUser = UserManager.FindById(User.Identity.GetUserId()).FirstName;

            return View(model);
        }

        public ActionResult GetDisplayName()
        {
            var displayname = UserManager.FindById(User.Identity.GetUserId()).DisplayName;

            return Content(displayname);
        }

        public ActionResult GetFirstName()
        {
            var firstname = UserManager.FindById(User.Identity.GetUserId()).FirstName;

            return Content(firstname);
        }

        public ActionResult GetLastName()
        {
            var lastname = UserManager.FindById(User.Identity.GetUserId()).LastName;

            return Content(lastname);
        }

        public ActionResult GetFirstInitial()
        {
            var getfirstinitial = UserManager.FindById(User.Identity.GetUserId()).FirstName;

            var firstinitial = getfirstinitial.Substring(0, 1);

            return Content(firstinitial);
        }

        public ActionResult GetAvatar()
        {
            var avatar = UserManager.FindById(User.Identity.GetUserId()).AvatarUrl;

            return Content(avatar);
        }
    }
}