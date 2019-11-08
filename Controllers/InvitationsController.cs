using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize(Roles = "House Head")]
    public class InvitationsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Create Invitation in DB
        public ActionResult SendHouseInvitation()
        {
            return View();
        }

        // POST: Create Invitation in DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendHouseInvitation([Bind(Include = "UserId,Body,Created,SentBy,RecipientEmail")]Invitation invite, string messageBody, string recipient)
        {

            var time = DateTimeOffset.Now;
            var useBy = time.AddDays(5);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var from = currentUser.Email;
            var code = invite.Code;
            string cb = Url.Action("AcceptAndRegister", "Account", new { id = invite.HouseholdId, code = code }, protocol: HttpContext.Request.Url.Scheme);

            if (ModelState.IsValid)
            {
                invite.Subject = $"Invitation From {currentUser.FirstName}";
                invite.SentBy = currentUser.Email;
                invite.RecipientEmail = recipient;
                invite.TTL = useBy;
                invite.Code = Guid.NewGuid();
                invite.HouseholdId = currentUser.MyHouse.Id;
                invite.IsValid = true;
                invite.Created = time;
                invite.Body = $"{messageBody} <hr /> To join {currentUser.FirstName}'s household, <a href='{cb}' target='_blank'>click here</a>!";
                db.Invitations.Add(invite);

                db.SaveChanges();
            }

            MailMessage mailMessage = new MailMessage(from, invite.RecipientEmail)
            {
                IsBodyHtml = true,
                Subject = invite.Subject,
                Body = invite.Body
            };
            var service = new EmailConfirm();
            await service.SendAsync(mailMessage);

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