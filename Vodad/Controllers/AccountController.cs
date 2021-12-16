using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Vodad.Models;
using System.Web.Routing;
using VodadModel;
using VodadModel.Repository;
using VodadModel.Helpers;
using System.Transactions;

namespace Vodad.Controllers
{

    [Authorize]
    public class AccountController : BaseController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IRoleService RoleService { get; set; }
        public IMembershipService MembershipService { get; set; }

        [AllowAnonymous]
        public ActionResult Activate(string code)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            code = HttpUtility.HtmlEncode(code);

            var user = userRepository.GetSingle(w => w.VerificationKey.Equals(code));

            if (user != null)
            {
                user.VerificationKey = null;
                user.IsActivated = true;

                userRepository.Save();

                Logger.Info("User {0} {1} has been activated.", user.Id, user.Email);

                return RedirectToAction("Login", "Account", new { success = "accountactivated" });
            }

            return RedirectToAction("Login", "Account", new { success = "accountnotactivated" });
        }

        public void AccountActivation(string email)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Email.Equals(email));

            if (user != null)
            {
                user.VerificationKey = null;
                user.IsActivated = true;

                userRepository.Save();

                Logger.Warn("User {0} {1} has been activated by administrator.", user.Id, user.Email);
            }
        }

        [Authorize]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult AttachAccountByIP()
        {
            var user = UserHelper.GetUserByEmail(User.Identity.Name);
            ViewBag.UserId = user.Id;

            Repository<User> userRepository = new Repository<User>(Entities);

            ViewBag.AttachedIp = userRepository.GetSingle(w => w.Id == user.Id).AccountAttachedIP;

            return View();
        }

        [HttpPost]
        [Authorize]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult AttachAccountByIP(AttachAccountByIPModel model)
        {
            if (ModelState.IsValid)
            {
                Repository<User> userRepository = new Repository<User>(Entities);

                var user = userRepository.GetSingle(w => w.Id == model.UserId);

                if (user != null)
                {
                    using (var transaction = new TransactionScope())
                    {
                        user.AccountAttachedIP = model.UserIp;

                        userRepository.Save();

                        Logger.Info("User {0} {1} has attached his account to IP {2}", user.Id, user.Email, user.AccountAttachedIP);

                        transaction.Complete();

                        return RedirectToAction("ChangeProfile", "Account", new { success = "accountattached" });
                    }
                }
            }

            return RedirectToAction("ChangeProfile", "Account", new { success = "accountnotattached" });
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Administrator")]
        public ActionResult BanUser(int userId)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Id == userId);

            if (user != null)
            {
                ViewBag.BanUserId = user.Id;
                ViewBag.BanUserName = user.Name;
                ViewBag.BanUserEmail = user.Email;

                return View();
            }
            else
            {
                return RedirectToAction("Error404", "Error");
            }
        }

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Administrator")]
        public ActionResult BanUser(BanUserModel model)
        {
            Repository<Ban> banRepository = new Repository<Ban>(Entities);
            Repository<User> userRepository = new Repository<User>(Entities);
            Repository<VodadModel.Roles> rolesRepository = new Repository<VodadModel.Roles>(Entities);

            var user = userRepository.GetSingle(w => w.Id == model.BanUserId);

            if (user != null && !user.Email.Equals(VodadModel.Utilities.Constants.AdminConstants.Email))
            {
                using (var transaction = new TransactionScope())
                {
                    Ban ban = new Ban();

                    OrderController orderController = new OrderController();

                    ban.UserId = model.BanUserId;
                    ban.RoleId = user.RoleId;
                    ban.BanDateTime = DateTime.Now;
                    ban.BanReason = HttpUtility.HtmlEncode(model.Reason);
                    ban.CanBeUnbanned = !model.CanBeUnbanned;

                    //Закрыть все кампании и ордера для Advertiser'а, PerformerPlatform'ы для Performer'а и OrderPerformed, связанные с пользователем
                    //CampaignController campaignController = new CampaignController();
                    PerformerPlatformController performerPlatformController = new PerformerPlatformController();

                    if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Performer))
                    {
                        OrderPerformedController orderPerformedController = new OrderPerformedController();

                        var orderPerformed = user.OrderPerformed.ToList();

                        foreach (var op in orderPerformed)
                            orderPerformedController.ChangeOrderPerformedStatus((int)op.Id,
                                                                                VodadModel.Utilities.Constants.OrdersStatuses.
                                                                                    Pay, (int)op.User.Id);
                    }


                    var order = user.Order.ToList();
                    var performerPlatform = user.PerformerPlatform.ToList();

                    foreach (var o in order)
                        orderController.ChangeOrderStatus(User.Identity.Name, (int)o.Id,
                                                                VodadModel.Utilities.Constants.OrdersStatuses.Closed);

                    foreach (var pp in performerPlatform)
                        performerPlatformController.ChangePerformerPlatformStatus((int)pp.Id,
                                                                                VodadModel.Utilities.Constants.
                                                                                    OrdersStatuses.Deleted);


                    user.RoleId = rolesRepository.GetSingle(w => w.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Banned)).Id;

                    var admin = UserHelper.GetUserByEmail(User.Identity.Name);

                    ban.AdminBanId = admin.Id;

                    userRepository.Save();

                    banRepository.Add(ban);
                    banRepository.Save();

                    Logger.Info("User {0} {1} has been banned by {2} {3}, reason {4}, can be unbanned? {5}", user.Id,
                                user.Email, admin.Id, admin.Email, model.Reason, model.CanBeUnbanned);


                    transaction.Complete();
                    return RedirectToAction("Profile", "User", new { id = model.BanUserId, success = "userbanned" });
                }
            }
            else
            {
                return RedirectToAction("Error404", "Error");
            }
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Administrator")]
        public ActionResult ChangeUsersRole(int? userId)
        {
            if (userId != null)
            {
                Repository<User> userRepository = new Repository<User>(Entities);

                var user = userRepository.GetSingle(w => w.Id == userId);

                if (user != null)
                {
                    ViewBag.UserId = user.Id;
                    ViewBag.UserName = user.Name;
                    ViewBag.UserEmail = user.Email;
                    ViewBag.UserRolename = user.Roles.RoleName;
                    ViewBag.RolesList = GetRolesList();

                    return View();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Administrator")]
        public ActionResult ChangeUsersRole(ChangeUsersRoleModel model)
        {
            Repository<User> userRepository = new Repository<User>(Entities);
            Repository<VodadModel.Roles> rolesRepository = new Repository<VodadModel.Roles>(Entities);

            var user = userRepository.GetSingle(w => w.Id == model.UserId);
            var role = rolesRepository.GetSingle(w => w.Id == model.RoleId);

            if (user != null && !user.Email.Equals(VodadModel.Utilities.Constants.AdminConstants.Email) && !user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Banned) && role != null && !role.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Banned))
            {
                using (var transaction = new TransactionScope())
                {
                    if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator))
                    {
                        // Отправить письмо об удалении админ
                        string messageBody = user.Email + ", " + user.Name + " role has been changed FROM Administrator by " + User.Identity.Name;
                        const string messageSubject = "Administrator deleted";
                        //UserHelper.SendMessage("380964869236@sms.kyivstar.net", "Kostya", messageBody, messageSubject);
                    }

                    if (role.RoleName.Equals("Administrator"))
                    {
                        // Отправить письмо о пополнении админов
                        string messageBody = user.Email + ", " + user.Name + "role has been changed TO Administrator by " + User.Identity.Name;
                        const string messageSubject = "Administrator added";
                        //UserHelper.SendMessage("380964869236@sms.kyivstar.net", "Kostya", messageBody, messageSubject);
                    }

                    bool b = user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Helper) || user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator);

                    string userRole = user.Roles.RoleName;

                    user.RoleId = model.RoleId;

                    userRepository.Save();

                    if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Helper) || user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator) || b)
                        Logger.Warn("User {0} {1} role {2} has changed role to {3} by {4}", user.Id, user.Email, userRole, user.Roles.RoleName, User.Identity.Name);
                    else
                        Logger.Info("User {0} {1} role {2} has changed role to {3} by {4}", user.Id, user.Email, userRole, user.Roles.RoleName, User.Identity.Name);

                    transaction.Complete();

                    return RedirectToAction("Profile", "User", new { id = model.UserId, success = "rolechanged" });
                }
            }
            return RedirectToAction("Profile", "User", new { id = model.UserId, success = "rolenotchanged" });
        }

        [Authorize]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult DeattachAccountByIP()
        {
            using (var transaction = new TransactionScope())
            {
                Repository<User> userRepository = new Repository<User>(Entities);

                var user = userRepository.GetSingle(w => w.Email.Equals(User.Identity.Name));

                user.AccountAttachedIP = "";

                userRepository.Save();

                Logger.Info("Account {0} {1} has been deattached by IP", user.Id, user.Email);

                transaction.Complete();
            }

            return RedirectToAction("ChangeProfile", "Account", new { success = "deattached" });
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public ActionResult ForgotPassword(string success)
        {
            if (success != null)
            {
                if (success.Equals(VodadModel.Utilities.Constants.AlertMessages.Success))
                {
                    ViewBag.AlertMessage = "Message with Your password has been send to Your Email.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("wrongemail"))
                {
                    ViewBag.AlertMessage = "Wrong Email! Please enter correct Email!";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("userisbanned"))
                {
                    ViewBag.AlertMessage = "User is banned.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            FormsAuthentication.SignOut();

            return View();
        }

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            Repository<User> userRepository = new Repository<User>(Entities);
            Repository<Ban> banRepository = new Repository<Ban>(Entities);

            var user = userRepository.GetSingle(w => w.Email.Equals(model.Email));
            var bannedUser = banRepository.GetSingle(w => w.UserId == user.Id);

            string success = "wrongemail";

            if (user != null)
            {
                if ((bannedUser != null && bannedUser.CanBeUnbanned == true) || bannedUser == null)
                {
                    using (var transaction = new TransactionScope())
                    {
                        var userRep = new UserRepository();

                        string newPassword = RandomPassword.Generate(8, 12);

                        userRep.ChangePassword(model.Email, newPassword);
                        user.IsLockedOut = false;

                        userRepository.Save();
                        success = VodadModel.Utilities.Constants.AlertMessages.Success;

                        // Отправить письмо с новым паролем
                        string messageBody = user.Name + ",\r\n Your new password: " + newPassword +
                                             "\r\n\r\n Thank you. \r\n ShowNGain team.";
                        const string messageSubject = "ShowNGain password change";
                        UserHelper.SendMessage(model.Email, user.Name, messageBody, messageSubject);

                        Logger.Info(string.Format("{0} has changed his password by ForgotPassword", model.Email));

                        transaction.Complete();
                    }
                }
                else
                {
                    success = "userisbanned";
                }
            }

            FormsAuthentication.SignOut();

            return RedirectToAction("ForgotPassword", "Account", new { success });
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null)
                FormsService = new FormsAuthenticationService();
            if (MembershipService == null)
                MembershipService = new AccountMembershipService();
            if (RoleService == null)
                RoleService = new AccountRoleService();

            base.Initialize(requestContext);
        }

        //
        // GET: /Account/Login

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string success)
        {
            if (success != null)
            {
                if (success.Equals("accountactivated"))
                {
                    ViewBag.AlertMessage = "Your account has been activated, please login.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("accountnotactivated"))
                {
                    ViewBag.AlertMessage = "Your account is not activated!";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("accountalreadyactive"))
                {
                    ViewBag.AlertMessage = "Your account is already active.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("coderesent"))
                {
                    ViewBag.AlertMessage = "Activation code has been resent. Please check Your E-mail.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
            }

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var repo = new UserRepository();

            if (ModelState.IsValid)
            {
                var userRepository = new Repository<User>(Entities);
                var user = userRepository.GetSingle(w => w.Email.Equals(model.Email));

                if (user != null)
                {
                    if (user != null && user.IsLockedOut == false)
                    {
                        if (repo.ValidateUser(model.Email, model.Password))
                        {
                            if (user.IsActivated)
                            {
                                if ((!user.AccountAttachedIP.Equals("") && user.AccountAttachedIP.Equals(model.UserIp)) || user.AccountAttachedIP.Equals(""))
                                {
                                    Repository<Ban> banRepository = new Repository<Ban>(Entities);

                                    var bannedUser = banRepository.GetSingle(w => w.UserId == user.Id);

                                    if (bannedUser == null || (bannedUser != null && bannedUser.CanBeUnbanned == true))
                                    {
                                        user.LastLoginDate = DateTime.Today;

                                        user.LastLoginIp = model.UserIp;

                                        FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

                                        if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator))
                                        {
                                            // Отправить письмо о входе админа
                                            string messageBody = user.Email + ", " + user.Name + " has entered system. IP: " + model.UserIp;
                                            const string messageSubject = "Administrator entered";
                                            //UserHelper.SendMessage("380964869236@sms.kyivstar.net", "Kostya", messageBody, messageSubject);
                                        }

                                        Logger.Info("User {0} has been logged in with ip {1}", model.Email, model.UserIp);

                                        userRepository.Save();

                                        if (Url.IsLocalUrl(returnUrl))
                                        {
                                            return Redirect(returnUrl);
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "Home");
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "User is banned");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "You are trying to login with different IP, it's forbidden.");
                                }
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account", new { success = "accountnotactivated" });
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", "The provided user name or password  is incorrect.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User is locked out. Too many failed login tries attempted. Reset your password by click \"Forgot password?\"");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password isn't correct.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public ActionResult Register(int? r)
        {
            if (!User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Advertiser) && !User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Performer))
            {
                ViewBag.Referal = r;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Account/Register

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email, model.Role, model.Referal, model.UserIp);

                const string TEST_USER_EMAIL_PART = "tu_1_";
                bool isTestUser = model.Email.StartsWith(TEST_USER_EMAIL_PART);

                /*
                 * For test purposes
                 * If user email starts from TU_1_ , then email will not be sent
                */

                if (isTestUser && createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Welcome", "Home");
                }

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Repository<User> userRepository = new Repository<User>(Entities);

                    var user = userRepository.GetSingle(w => w.Email.Equals(model.Email));

                    if (user != null)
                    {

                        // Отправить письмо с новым паролем
                        string messageBody = model.UserName + ",\r\n we thank you for registering at \"Vodad\". \r\n Your login: " + model.Email + "\r\n Your password: " + model.Password + "\r\n\r\n You need to activate Your account using this link: " + Request.Url.Scheme + "://" + Request.Url.Host + "/Account/Activate?code=" + user.VerificationKey + "\r\n\r\n Thank you. \r\n Vodad team.";
                        const string messageSubject = "Vodad registration";
                        UserHelper.SendMessage(model.Email, model.UserName, messageBody, messageSubject);
                        return RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [AllowAnonymous]
        public ActionResult ResendActivationCode(string success)
        {
            if (success != null)
            {
                if (success.Equals("wrongemail"))
                {
                    ViewBag.AlertMessage = "You have entered wrong email, or time limit has been expired.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResendActivationCode(ResendActivationCodeModel model)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Email.Equals(model.Email));

            if (user != null)
            {
                if (!user.IsActivated)
                {
                    UserRepository userRepository_ = new UserRepository();

                    user.VerificationKey = userRepository_.RandomString(8);

                    userRepository.Save();

                    // Send message with new activation code
                    string messageBody = "You need to activate Your account using this link: " + Request.Url.Scheme + "://" + Request.Url.Host + "/Account/Activate?code=" + user.VerificationKey + "\r\n\r\n Thank you. \r\n Vodad team.";
                    const string messageSubject = "Vodad registration";
                    UserHelper.SendMessage(model.Email, user.Name, messageBody, messageSubject);
                    return RedirectToAction("Login", "Account", new { success = "coderesent" });
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { success = "accountalreadyactive" });
                }
            }
            else
            {
                return RedirectToAction("ResendActivationCode", "Account", new { success = "wrongemail" });
            }

        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    var userRepo = new UserRepository();
                    changePasswordSucceeded = userRepo.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword, userIsOnline: true);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password isn't correct.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ViewResult ChangeProfile(string success)
        {
            List<MerchantsListModel> merchantsListModels = new List<MerchantsListModel>();
            var userRepository = new Repository<User>(Entities);
            var user = userRepository.GetSingle(w => w.Email == User.Identity.Name);
            Repository<Merchants> merchantRepository = new Repository<Merchants>(Entities);

            // Проверка и активирование изменений кошелька
            WalletController walletController = new WalletController();
            walletController.ActivateNewUsersMerchantAccount((int)user.Id);

            var merchants = merchantRepository.GetAll();

            if (merchants != null)
            {
                foreach (var merchant in merchants)
                {
                    var userMerchant = merchant.UserMerchants.FirstOrDefault(w => w.UserId == user.Id);

                    MerchantsListModel merchantsListModelElement = new MerchantsListModel();

                    merchantsListModelElement.MerchantName = merchant.MerchantName;

                    if (userMerchant != null)
                    {
                        merchantsListModelElement.MerchantAccount = userMerchant.Account;
                        merchantsListModelElement.UserMerchantId = (int)userMerchant.Id;
                        merchantsListModelElement.AccountStatus = userMerchant.Status;
                        merchantsListModelElement.NextAccount = userMerchant.NextAccount;
                    }

                    merchantsListModels.Add(merchantsListModelElement);
                }

                ViewBag.Merchants = merchantsListModels;
            }

            ViewBag.Timezones = GetTimezoneList();

            ViewBag.UserName = user.Name;
            ViewBag.UserId = user.Id;
            ViewBag.Email = User.Identity.Name;

            if (success != null)
            {
                if (success.Equals("accountchanged"))
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                    ViewBag.AlertMessage = "Your account changes has been saved";
                }
                else if (success.Equals("accountnotchanged"))
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                    ViewBag.AlertMessage = "Something went wrong, please try again later";
                }
                else if (success.Equals("accountattached"))
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                    ViewBag.AlertMessage = "Your account has been attached by IP";
                }
                else if (success.Equals("accountnotattached"))
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                    ViewBag.AlertMessage = "Something went wrong, please try again later";
                }
                else if (success.Equals("walletexists"))
                {
                    ViewBag.AlertMessage = "Wallet already exists";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("usermerchantadded"))
                {
                    ViewBag.AlertMessage = "Wallet has been added";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("usermerchantchanged"))
                {
                    ViewBag.AlertMessage = "Wallet has been changed";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("deattached"))
                {
                    ViewBag.AlertMessage = "Account has been deattached";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
            }

            ViewBag.ReferralsList = GetUsersReferralList((int)user.Id);
            ViewBag.ReferralsIncome = user.Wallet.FirstOrDefault().ReferralsIncome;

            ChangeProfileModel changeProfileModel = new ChangeProfileModel();

            changeProfileModel.Comments = "";

            if (user.Comments != null)
            {
                changeProfileModel.Comments = HttpUtility.HtmlDecode(user.Comments);
            }

            return View(changeProfileModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        public ActionResult ChangeProfile(ChangeProfileModel model)
        {
            string success = "accountnotchanged";

            if (ModelState.IsValid)
            {
                bool changeProfileSucceeded;

                try
                {
                    var userRepo = new UserRepository();
                    changeProfileSucceeded = userRepo.ChangeProfile(User.Identity.Name, 19, model.Comments, userIsOnline: true);
                }
                catch (Exception)
                {
                    changeProfileSucceeded = false;
                }

                if (changeProfileSucceeded)
                {
                    success = "accountchanged";

                    return RedirectToAction("ChangeProfile", "Account", new { success });
                }
                else
                {
                    return RedirectToAction("ChangeProfile", "Account", new { success });
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("ChangeProfile", "Account", new { success });
        }

        public ActionResult ChangeProfileSuccess()
        {
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        public static List<VodadModel.Roles> GetRolesList()
        {
            var rolesRepository = new Repository<VodadModel.Roles>(new VodadEntities());

            return rolesRepository.GetAll().ToList();
        }

        private IEnumerable<Timezone> GetTimezoneList()
        {
            var repo = new Repository<Timezone>(Entities);
            IEnumerable<Timezone> timezones = repo.GetAll().ToList();

            return timezones;
        }

        private List<ReferralsListModel> GetUsersReferralList(int uid)
        {
            var userRepository = new Repository<User>(Entities);

            var users = userRepository.GetAll(w => w.ReferrerId == uid);

            var referralList = new List<ReferralsListModel>();

            if (users.Any())
            {
                foreach (var u in users)
                {
                    var referral = new ReferralsListModel();

                    referral.ReferralId = (int)u.Id;
                    referral.ReferralName = u.Name;
                    referral.ReferralLastActivityDateTime = u.LastOnlineDateTime.ToString();
                    referral.ReferralRole = u.Roles.RoleName;

                    referralList.Add(referral);
                }
            }

            return referralList;
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Helper, Administrator")]
        public ActionResult UnbanUser(int userId)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Id == userId);

            if (user != null)
            {
                if (user.Ban.LastOrDefault(w => w.UserId == userId).CanBeUnbanned == true)
                {
                    ViewBag.UnbanUserId = user.Id;
                    ViewBag.UnbanUserName = user.Name;
                    ViewBag.UnbanUserEmail = user.Email;

                    return View();
                }
                else
                {
                    return RedirectToAction("Profile", "User", new { userId, success = "canntbeunbanned" });
                }
            }
            else
            {
                return RedirectToAction("Error404", "Error");
            }
        }

        [HttpPost]
        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [Authorize(Roles = "Helper, Administrator")]
        public ActionResult UnbanUser(UnbanUserModel model)
        {
            Repository<Ban> banRepository = new Repository<Ban>(Entities);
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Id == model.UnbanUserId);

            if (user != null)
            {
                Ban ban = banRepository.GetAll(w => w.UserId == model.UnbanUserId).ToList().Last();

                if (ban != null)
                {
                    using (var transaction = new TransactionScope())
                    {
                        ban.UserId = model.UnbanUserId;
                        ban.RoleId = user.RoleId;
                        ban.UnbanDateTime = DateTime.Now;
                        ban.UnbanReason = HttpUtility.HtmlEncode(model.Reason);
                        user.RoleId = user.RegisteredRoleId;

                        var admin = UserHelper.GetUserByEmail(User.Identity.Name);

                        ban.AdminUnbanId = admin.Id;

                        userRepository.Save();

                        banRepository.Save();

                        Logger.Info("User {0} {1} has been unbanned by {2} {3}, reason {4}.", user.Id,
                                    user.Email, admin.Id, admin.Email, model.Reason);

                        transaction.Complete();
                    }
                    return RedirectToAction("Profile", "User", new { id = model.UnbanUserId, success = "userunbanned" });
                }
                else
                {
                    return RedirectToAction("Profile", "User", new { id = model.UnbanUserId, success = "userisnotbanned" });
                }
            }
            else
            {
                return RedirectToAction("Error404", "Error");
            }
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public JsonResult ValidateEmail(string Email)
        {
            var user = UserHelper.GetUserByEmail(Email);

            if (user != null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //#if (!DEBUG)
        //[RequireHttps]
        //#endif
        [AllowAnonymous]
        public JsonResult ValidateName(string UserName)
        {
            var userRepository = new Repository<User>(Entities);

            string userNameEncode = HttpUtility.HtmlEncode(UserName);
            var user = userRepository.GetSingle(w => w.Name.Equals(userNameEncode));

            if (user != null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);

        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}