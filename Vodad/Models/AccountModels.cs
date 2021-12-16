using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using VodadModel;

namespace Vodad.Models
{
    public class AttachAccountByIPModel
    {
        public int UserId { get; set; }

        public string UserIp { get; set; }
    }

    public class BanUserModel
    {
        public int BanUserId { get; set; }

        [Required]
        [Display(Name = "Ban without unban?")]
        public bool CanBeUnbanned { get; set; }

        [Required]
        [Display(Name = "Reason:")]
        public string Reason { get; set; }
    }

    public class MerchantsListModel
    {
        public int UserMerchantId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantAccount { get; set; }

        public string AccountStatus { get; set; }

        public string NextAccount { get; set; }
    }

    public class UnbanUserModel
    {
        public int UnbanUserId { get; set; }

        [Required]
        [Display(Name = "Reason:")]
        public string Reason { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
    }

    public class ChangeProfileModel
    {
        /*[Required]
        [Display(ResourceType = typeof (Internationalization), Name = "ChangeProfileModel_Timezone_Select_your_timezone")]
        public int Timezone { get; set; }*/

        [Display(Name = "Profile comments")]
        public string Comments { get; set; }

        [Display(Name = "Last login IP")]
        public string LastLoginIp { get; set; }
    }

    public class ChangeUsersRoleModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Choose new user's role")]
        public int RoleId { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "Please enter Your Email here:")]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Wrong Email!")]
        public string Email { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display( Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string UserIp { get; set; }
    }

    public class RegisterModel
    {
        [Display(Name = "User name")]
        [StringLength(20, MinimumLength = 4, ErrorMessage= "Too short name")]
        [Remote("ValidateName", "Account", ErrorMessage= "This name is in use")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail address")]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage= "Wrong Email")]
        [Remote("ValidateEmail", "Account", ErrorMessage = "This Email is in use")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Choose Your role")]
        public string Role { get; set; }

        public int? Referal { get; set; }

        public string UserIp { get; set; }
    }

    public class ReferralsListModel
    {
        public int ReferralId { get; set; }

        public string ReferralName { get; set; }

        public string ReferralLastActivityDateTime { get; set; }

        public string ReferralRole { get; set; }
    }

    public class ResendActivationCodeModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public interface IRoleService
    {
        bool AdminExists();
        void AddUsersToRoles(string[] usernames, string[] rolenames);
        void RemoveUsersFromRoles(string[] usernames, string[] rolenames);
        void CreateRole(string roleName);
    }

    public class AccountRoleService : IRoleService
    {
        private readonly RoleProvider _provider;

        public AccountRoleService()
            : this(null)
        {
        }

        public AccountRoleService(RoleProvider provider)
        {
            _provider = provider ?? new VodadRoleProvider();
        }

        public bool AdminExists()
        {
            var users = _provider.GetUsersInRole("Administrator");

            if (!users.Any())
                return false;

            return true;
        }

        public void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            _provider.AddUsersToRoles(usernames, rolenames);
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            _provider.RemoveUsersFromRoles(usernames, rolenames);
        }

        public void CreateRole(string roleName)
        {
            _provider.CreateRole(roleName);
        }
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userLogin, string password)
        {
            if (String.IsNullOrEmpty(userLogin)) throw new ArgumentException("Value cannot be null or empty.", "userLogin");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userLogin, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, string userRoleName, int? referrer, string userIp)
        {
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            var provider = (VodadMembershipProvider)_provider;
            MembershipCreateStatus status = provider.CreateUser(userName, password, email, userRoleName, referrer, null, null, null, out status, userIp);
            return status;
        }

        public bool ChangePassword(string userLogin, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userLogin)) throw new ArgumentException("Value cannot be null or empty.", "userLogin");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userLogin, true /* userIsOnline */);
                return currentUser != null && currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userLogin, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email, string userRoleName, int? referrer, string userIp);
        bool ChangePassword(string userLogin, string oldPassword, string newPassword);
    }

    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                //case MembershipCreateStatus.DuplicateUserName:
                //return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

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
    }
}
