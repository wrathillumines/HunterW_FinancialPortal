using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HunterW_FinancialPortal.Models;
using System.Web.Configuration;
using HunterW_FinancialPortal.Helpers;
using System.IO;
using System.Net.Mail;
using System;
using System.Text;
using System.Linq;

namespace HunterW_FinancialPortal.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            var currentuser = await UserManager.FindByEmailAsync(model.Email);
            switch (result)
            {
                case SignInStatus.Success:
                    if (currentuser.MyHouseId == null)
                    {
                        return RedirectToAction("Lobby", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, int? myHouseId, Guid code)
        {
            if (ModelState.IsValid)
            {
                if (myHouseId == 0)
                    myHouseId = null;

                var user = new ApplicationUser()
                {
                    MyHouseId = myHouseId,
                    UserName = model.Email,
                    DisplayName = model.DisplayName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AvatarUrl = WebConfigurationManager.AppSettings["DefaultAvatar"]
                };

                if (UploadHelper.IsWebFriendlyImage(model.Avatar))
                {
                    //var rootFileName = Path.GetFileNameWithoutExtension(model.Avatar.FileName);
                    //var fileExt = Path.GetExtension(model.Avatar.FileName);

                    //rootFileName = $"{StringUtilities.URLFriendly(rootFileName)}-{DateTime.Now.Ticks}";
                    
                    var fileName = Path.GetFileName(model.Avatar.FileName);
                    model.Avatar.SaveAs(Path.Combine(Server.MapPath("~/Avatars/"), fileName));
                    user.AvatarUrl = "/Avatars/" + fileName;
                }

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (code == new Guid())
                        roleHelper.AddUserToRole(user.Id, "Lobby");
                    else
                    {
                        var invitation = db.Invitations.FirstOrDefault(i => i.Code == code);
                        invitation.IsValid = false;
                        db.SaveChanges();

                        roleHelper.AddUserToRole(user.Id, "User");
                    }

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //        var emailFrom = WebConfigurationManager.AppSettings["emailfrom"];
                    //        MailMessage mailMessage = new MailMessage(emailFrom, model.Email)
                    //        {
                    //            Subject = "Confirm your account",
                    //            Body = "<p><span style=\"font-family: arial;\">Thank you for registering.</span></p><p><span style=\"font-family: arial;\">Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.</span></p>",
                    //            IsBodyHtml = true
                    //        };

                    //        var service = new EmailConfirm();
                    //        await service.SendAsync(mailMessage);

                    return RedirectToAction("EmailConfirmationSent", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Code = code;
            ViewBag.HouseholdId = myHouseId;
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/EmailConfirmationSent
        [AllowAnonymous]
        public ActionResult EmailConfirmationSent()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var emailFrom = WebConfigurationManager.AppSettings["emailfrom"];
                MailMessage mailMessage = new MailMessage(emailFrom, model.Email)
                {
                    Subject = "Reset Password",
                    Body = "<p><span style=\"font-family: arial;\">Reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></span></p>.",
                    IsBodyHtml = true
                };

                var service = new EmailConfirm();
                await service.SendAsync(mailMessage);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Login", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            AddErrors(result);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return RedirectToAction("Login", "Account");
        }

        //
        // Invitation stuff
        [AllowAnonymous]
        public ActionResult AcceptAndRegister(string recipientEmail, Guid code)
        {
            var invitation = db.Invitations.FirstOrDefault(i => i.RecipientEmail == recipientEmail && i.Code == code);
            if (invitation.IsValid && DateTime.Now < invitation.TTL)
            {
                ViewBag.Code = code;
                ViewBag.HouseholdId = invitation.HouseholdId;

                RegisterViewModel registerViewModel = new RegisterViewModel() { Email = recipientEmail };
                return View("Register", registerViewModel);
            }

            var errorMsg = new StringBuilder();
            if (DateTime.Now > invitation.TTL)
            {
                errorMsg.AppendLine("Aww man. That code has expired.");
            }

            if (!invitation.IsValid)
            {
                errorMsg.AppendLine("Aww man. That code is no longer valid.");
            }

            TempData["ErrorMsg"] = errorMsg.ToString();

            return RedirectToAction("Oof", "Admin");
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}