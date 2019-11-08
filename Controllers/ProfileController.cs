using HunterW_FinancialPortal.Helpers;
using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyProfile
        public ActionResult MyProfile()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.Select(user => new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
            }).FirstOrDefault(u => u.Id == userId);

            return View(member);
        }

        // GET: Other User Profile
        public ActionResult Profile(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Dashboard", "Home");
            };
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Edit Profile
        [Authorize]
        public ActionResult EditProfile()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.Select(user => new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
            }).FirstOrDefault(u => u.Id == userId);

            return View(member);
        }

        //POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserProfileViewModel member)
        {
            var user = db.Users.Find(member.Id);
            user.FirstName = member.FirstName;
            user.LastName = member.LastName;
            user.DisplayName = member.DisplayName;
            user.Email = member.Email;
            user.UserName = member.Email;

            if (UploadHelper.IsWebFriendlyImage(member.Avatar))
            {
                var fileName = Path.GetFileName(member.Avatar.FileName);
                member.Avatar.SaveAs(Path.Combine(Server.MapPath("~/Avatars/"), fileName));
                user.AvatarUrl = "/Avatars/" + fileName;
            }
            db.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }

    }
}