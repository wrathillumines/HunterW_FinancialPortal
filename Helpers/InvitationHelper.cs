using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Helpers
{
    public class InvitationHelper : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public Invitation CreateInvite(Invitation invite)
        {
            var time = DateTimeOffset.Now;
            var useBy = time.AddDays(5);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var newInvite = new Invitation
            {
                Subject = $"{currentUser.FirstName} has sent an invitation",
                Body = $"{currentUser.FirstName} {currentUser.LastName} has invited you to join their household on the HW Budget Assistant! Please <a href=#>click here</a> to join!",
                SentBy = currentUser.Email,
                RecipientEmail = invite.RecipientEmail,
                TTL = useBy,
                Code = Guid.NewGuid(),
                HouseholdId = currentUser.MyHouse.Id,
                IsValid = true,
                Created = time
            };
            db.Invitations.Add(newInvite);
            db.SaveChanges();
            return newInvite;
        }

        public async Task SendHouseInvite(Invitation invite)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var from = currentUser.Email;
            var code = invite.Code;
            string cb = Url.Action("AcceptAndRegister", "Account", new { id = invite.HouseholdId, code }, protocol: HttpContext.Request.Url.Scheme);
            MailMessage mailMessage = new MailMessage(from, invite.RecipientEmail)
            {
                IsBodyHtml = true,
                Subject = $"Invitation From {currentUser.FirstName}",
                Body = $"{invite.Body} <hr /> To join {currentUser.FirstName}'s household, <a href='{cb}' target='_blank'>click here</a>!"
            };
            var service = new EmailConfirm();
            await service.SendAsync(mailMessage);
        }
    }
}