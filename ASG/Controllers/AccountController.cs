using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ASG.Models;
using ASG.Areas.BugTracker.Models;
using System.Collections.Generic;
using System.Net.Mail;
using System.Configuration;
using System.Web.Security;

namespace ASG.Controllers
{
    //[RequireHttps]
    //[Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper helper = new UserRolesHelper(db);

        public AccountController()
        {
        }

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
        public ActionResult Login(string returnUrl, string Application)
        {
            ViewBag.Application = Application;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public JsonResult Post(FormCollection form)
        {
            var item = form["Blah1"];
            return Json(new { success = true });
        }
        //
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                if (model.Application != null && model.Application == "Blog")
                {                                
                    return Json(new { success = false, ex = "Invalid login attempt" });
                }
                if (model.Application != null && model.Application == "FinancialPlanner")
                {             
                    return Json(new { success = false, ex = "Invalid login attempt" });
                }
                if (model.Application != null && model.Application == "BugTracker")
                {
                    return Json(new { success = false, ex = "Invalid login attempt" });
                }
            
                return View(model);
            }

            else
            {
                var ApplicationToLoginTo = model.Application;
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false); 
                if (ApplicationToLoginTo == "BugTracker")
                {
                    switch (result)
                    {
                        case SignInStatus.Success:
                            var roleCheck = RoleCheck("BugtrackerUser");
                            return RedirectToAction("Landing", "Home", new { area = "BugTracker" });
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            TempData["shortMessage"] = "Invalid login attempt.";                         
                            return RedirectToAction("Index", "Home", new { area = "BugTracker", model });              
                    }
                }
             
                else if (ApplicationToLoginTo == "FinancialPlanner")
                {
                    switch (result)
                    {
                        case SignInStatus.Success:
                            var roleCheck = RoleCheck("FinancialPlannerUser");
                            return Json(new { success = true, roleCheck = roleCheck });                         
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            TempData["shortMessage"] = "Invalid login attempt.";  
                            return RedirectToAction("Index", "Home", new { area = "FinancialPlanner" });             
                    }
                }

                else if (ApplicationToLoginTo == "Blog")
                {
                    switch (result)
                    {
                        case SignInStatus.Success:
                            var roleCheck = RoleCheck("BlogUser");                       
                            return Json(new { success = true, roleCheck = roleCheck });                                
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
                else
                {
                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToAction("Index", "Home");
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
            }

         
        }

        public bool RoleCheck(string Role)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                userId = SignInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId();
            }
            var roleCheck = helper.IsUserInRole(userId, Role);
            return roleCheck;
        }



        public ActionResult Rerender()
        {
            if (Request.IsAjaxRequest())
            {

                var userId1 = User.Identity.GetUserId();     
                var userRoles = helper.ListUserRoles(userId1).ToList();
                return Json(new {userRoles = userRoles });    
            }
            else
            {
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult LogOut()
        {

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Json(new { success = true, ex = "Invalid login attempt" });         
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
       
        public async Task<ActionResult> SignIn(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                if(model.Application != null && model.Application == "BugTracker")
                {
                    return Json(new { success = false, ex = "Invalid login attempt" });
                }
                return View(model);
            }

            var ApplicationToLoginTo = model.Application;
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            var result2 = SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);          
            if (ApplicationToLoginTo == "BugTracker")
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        var roleCheck = RoleCheck("BugtrackerUser");
                        return Json(new { success = true, roleCheck = roleCheck });                    
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        TempData["shortMessage"] = "Invalid login attempt.";                      
                        return Json(new { success = false, ex = "Invalid login attempt" });                      
                }
            }

            else if (ApplicationToLoginTo == "FinancialPlanner")
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "Home", new { area = "FinancialPlanner" });
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

            else if (ApplicationToLoginTo == "Blog")
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "BlogPosts");
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
            else
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "Home");
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
        }
        [AllowAnonymous]

        public async Task<ActionResult> GuestLogin(string userName, string type)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var ApplicationToLoginTo = type;

            var Password = "Login1111!";
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(userName, Password, false, shouldLockout: false);

            if (ApplicationToLoginTo == "BugTracker")
            {
                switch (result)
                {

                    case SignInStatus.Success:
                        return RedirectToAction("Landing", "Home", new { area = "BugTracker" });

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return RedirectToAction("Index");
                }
            }

            else if (ApplicationToLoginTo == "FinancialPlanner")
            {
                switch (result)
                {

                    case SignInStatus.Success:                    
                        var user = db.Users.Where(u => u.UserName == userName);
                        var householdId = db.Households.Where(h => h.Users.Select(q => q.UserName).Contains(userName)).Select(h => h.Id).FirstOrDefault();

                        if (householdId > 0)
                        {
                            return RedirectToAction("BudgetHousehold", "Budgets", new { area = "FinancialPlanner", id = householdId });
                        }
                      else
                        {
                            return RedirectToAction("Index", "Home", new { area = "FinancialPlanner" });
                        }                      
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return RedirectToAction("Index");
                }
            }

            else if (ApplicationToLoginTo == "Blog")
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "Home", new { area = "Blog" });

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return RedirectToAction("Index");
                }
            }
            
            else
            {
                switch (result)
                {

                    case SignInStatus.Success:
                        return RedirectToAction("Index", "Home");

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return RedirectToAction("Index");
                }
            }           
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string Application)
        {
            ViewBag.Application = Application;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {         
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Application != null)
                    {
                        if (model.Application == "BugTracker")
                        {
                            helper.AddUserToRole(user.Id, "BugtrackerUser");
                            helper.AddUserToRole(user.Id, "Submitter");                          
                        }
                        if (model.Application == "Blog")
                        {
                            helper.AddUserToRole(user.Id, "BlogUser"); 
                        }
                        if (model.Application == "FinancialPlanner")
                        {
                            helper.AddUserToRole(user.Id, "FinancialPlannerUser");   
                        }
                        db.SaveChanges();
                      
                    }
                   
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    var userRoles = helper.ListUserRoles(user.Id);                   
                    if (model.Application != null)
                        {
                            if (model.Application == "BugTracker")
                            {                          
                            return Json(new { success = true, roles = userRoles });                        
                            }

                            if (model.Application == "Blog")
                            {
                            return Json(new { success = true, roles = userRoles });                          
                            }
                            if (model.Application == "FinancialPlanner")
                            {
                            return Json(new { success = true, roles = userRoles });                         
                            }

                    }                
                    
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
                if (model.Application != null && model.Application == "BugTracker")
                {
                    var errors = result.Errors.ToList();
                    TempData["registryResponse"] = errors;
                    return Json(new { success = false, ex = "Invalid login attempt" });               
                }

            }
            if (!ModelState.IsValid)
            {                
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                if (model.Application != null && model.Application == "BugTracker")
                {
                    TempData["registryResponse"] = modelErrors;
                    return Json(new { success = false, ex = modelErrors });                 
                }

                if (model.Application != null && model.Application == "Blog")
                {
                    TempData["registryResponse"] = modelErrors;
                    return Json(new { success = false, ex = modelErrors });                  
                }


                if (model.Application != null && model.Application == "FinancialPlanner")
                {
                    TempData["registryResponse"] = modelErrors;
                    return Json(new { success = false, ex = modelErrors });                   
                }         
            }      
                    
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]

        public ActionResult AddRegistration(AddRegistrationViewModel additionalAccounts)
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());            
                // Request a redirect to the external login provider
                var accountList = new List<string>();
                if (additionalAccounts.Blog)
                {
                    helper.AddUserToRole(user.Id, "BlogUser");
                    accountList.Add("Blog");
                }
               
                if (additionalAccounts.BugTracker)
                {
                    helper.AddUserToRole(user.Id, "BugtrackerUser");
                    helper.AddUserToRole(user.Id, "Submitter");
                    helper.AddUserToRole(user.Id, "Developer1");
                    accountList.Add("BugTracker");
                }
               
                if (additionalAccounts.FinancialPlanner)
                {
                    helper.AddUserToRole(user.Id, "FinancialPlannerUser");
                    accountList.Add("FinancialPlanner");
                }              

                db.SaveChanges();
                SignInManager.SignIn(user, false, false);
                return Json(new { success = true, accountList });
            }
            catch
            {
                return Json(new { success = false });
            }
        }    

        [HttpPost]
        [AllowAnonymous]

        public ActionResult AddMultipleRegistration(AddRegistrationViewModel additionalAccounts)
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());             
                // Request a redirect to the external login provider
                var accountList = new List<string>();
                if (additionalAccounts.Blog)
                {
                    helper.AddUserToRole(user.Id, "BlogUser");
                    accountList.Add("Blog");
                }
                else
                {
                    if (helper.IsUserInRole(user.Id,"BlogUser"))
                    {
                        //helper.RemoveUserFromRole(user.Id, "BlogUser");
                    }
                }
                if (additionalAccounts.BugTracker)
                {
                    helper.AddUserToRole(user.Id, "BugtrackerUser");
                    helper.AddUserToRole(user.Id, "Submitter");
                    helper.AddUserToRole(user.Id, "Developer1");
                    accountList.Add("BugTracker");
                }
                else
                {
                    if (helper.IsUserInRole(user.Id, "BugtrackerUser"))
                    {
                        //helper.RemoveUserFromRole(user.Id, "BugtrackerUser");
                    }
                }
                if (additionalAccounts.FinancialPlanner)
                {
                    helper.AddUserToRole(user.Id, "FinancialPlannerUser");
                    accountList.Add("FinancialPlanner");
                }
                else
                {
                    if (helper.IsUserInRole(user.Id, "FinancialPlannerUser"))
                    {
                        //helper.RemoveUserFromRole(user.Id, "FinancialPlannerUser");
                    }
                }

                db.SaveChanges();
                SignInManager.SignIn(user, false, false);
                return Json(new { success = true, accountList });
            }
            catch
            {
                return Json(new { success = false });
            }     
        }


        //GET: /Account/ResendEmailConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResendEmailConfirmation()
        {
            return View();
        }

        //POST: /Acount/ResendEmailConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>ResendEmailConfirmation(ForgotPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            return RedirectToAction("ConfirmationSent");
        }
        //GET: /Account/ConfirmationSent
        public ActionResult ConfirmationSent()
        {
            return View();
        }
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
            var modelErrors = new List<string>();
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    modelErrors.Add("Invalid attempt");
                    if (model.Application == "Blog")
                    {
                        return Json(new { success = false, ex = modelErrors});
                    }
                    if (model.Application == "FinancialPlanner")
                    {
                        return Json(new { success = false, ex = modelErrors });
                    }
                    if (model.Application == "BugTracker")
                    {
                        return Json(new { success = false, ex = modelErrors });
                    }
                    else
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return View("ForgotPasswordConfirmation");
                    }
                    
                }

            

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, Application = model.Application }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please confirm your account:" + callbackUrl); /*"Please reset your password by clicking<a href=\"" + callbackUrl + "\">here</a>");*/

                if (model.Application == "Blog")
                {
                    return Json(new { success = true });
                }
                if (model.Application == "BugTracker")
                {
                    return Json(new { success = true });
                }
                if (model.Application == "FinancialPlanner")
                {
                    return Json(new { success = true });
                }
                else
                {
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
              

            }
           
            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    modelErrors.Add(modelError.ErrorMessage);
                }
            }

            if (model.Application != null && model.Application == "BugTracker")
            {
                TempData["registryResponse"] = modelErrors;
                return Json(new { success = false, ex = modelErrors });             
            }

            if (model.Application != null && model.Application == "Blog")
            {             
                return Json(new { success = false, ex = modelErrors });              
            }

            if (model.Application != null && model.Application == "FinancialPlanner")
            {
                TempData["registryResponse"] = modelErrors;
                return Json(new { success = false, ex = modelErrors });              
            }
            else
            {
                // If we got this far, something failed, redisplay form
                return View(model);
            }

           
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string Application)
        {
            if (Application == "Blog")
            {
                TempData["code"] = code;
                TempData["resetPassword"] = true;
                  return RedirectToAction("BlogPosts", "Blog");
            }
            if (Application == "BugTracker")
            {
                TempData["code"] = code;
                TempData["resetPassword"] = true;
                return RedirectToAction("Index", "Home", new { area = "BugTracker" });
            }
            if (Application == "FinancialPlanner")
            {
                TempData["code"] = code;
                TempData["resetPassword"] = true;
                return RedirectToAction("Index", "Home", new { area = "FinancialPlanner" });
            }
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

                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                if (model.Application != null && model.Application == "BugTracker")
                {
                    TempData["registryResponse"] = modelErrors;
                    return Json(new { success = false, ex = modelErrors });                   
                }

                if (model.Application != null && model.Application == "Blog")
                {
                    TempData["code"] = model.Code;
                    return Json(new { success = false, ex = modelErrors });                  
                }

                if (model.Application != null && model.Application == "FinancialPlanner")
                {
                    TempData["registryResponse"] = modelErrors;
                    return Json(new { success = false, ex = modelErrors });                  
                }
                else
                {
                    return View(model);
                }
               
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                if (model.Application == "Blog")
                {
                    return Json(new { success = true });
                }
                if (model.Application == "BugTracker")
                {
                    return Json(new { success = true });
                }
                if (model.Application == "FinancialPlanner")
                {
                    return Json(new { success = true });
                }
                else
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
               
            }
            //AddErrors(result);

            else
            {
                if (model.Application == "Blog")
                {                
                    TempData["code"] = model.Code;

                    var errors = result.Errors.ToList();
                    return Json(new { success = false, ex = errors });
                }
                if (model.Application == "BugTracker")
                {                   
                    TempData["code"] = model.Code;

                    var errors = result.Errors.ToList();
                    return Json(new { success = false, ex = errors });
                }
                if (model.Application == "FinancialPlanner")
                {                 
                    TempData["code"] = model.Code;

                    var errors = result.Errors.ToList();
                    return Json(new { success = false, ex = errors });
                }
            }
            
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string Application)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);


            if (Application == "BugTracker")
            {
                return RedirectToAction("Index", "Home", new { area = "BugTracker" });
            }
            else if (Application == "FinancialPlanner")
            {
                return RedirectToAction("Index", "Home", new { area = "FinancialPlanner" });
            }
            else if (Application == "Blog")
            {
                return RedirectToAction("Index", "BlogPosts", new { area = "Blog" });
            }
            else if (Application == "Portfolio")
            {
                return RedirectToAction("Index", "Home", new { area = "Portfolio" });  
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
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