using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using Vodad.Models;
using VodadModel;
using VodadModel.Repository;
using VodadModel.Helpers;
using Roles = VodadModel.Roles;
using System.Text;

namespace Vodad.Controllers
{
    public class UserRepository : BaseController
    {
        private static readonly List<UserLoginDictionary> UserLoginDictionary = new List<UserLoginDictionary>();
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden

        // E-mail verification code generator
        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public MembershipUser CreateUser(string username, string password, string email, string roleName, Int32? referrer, string userIp)
        {
            var userRepository = new Repository<User>(Entities);
            var timezoneRepository = new Repository<Timezone>(Entities);

            var user = new User { Name = username, Email = email, PasswordSalt = CreateSalt() };
            user.Password = CreatePasswordHash(password, user.PasswordSalt);
            user.RegistrationDate = DateTime.Today;

            user.IsLockedOut = false;

            user.LastLockedOutDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;
            user.LastLoginIp = userIp;
            user.RoleId = AccountController.GetRolesList().Single(w => w.RoleName == roleName).Id;
            user.RegisteredRoleId = AccountController.GetRolesList().Single(w => w.RoleName == roleName).Id;
            user.Rating = 0;
            user.Karma = 0;
            user.LastOnlineDateTime = DateTime.Now;
            user.AccountAttachedIP = "";

            // If this user is not for test purposes, the activation process will start
            if (!IsTestUser(ref email))
            {
                user.VerificationKey = RandomString(8);
                user.IsActivated = false;
            }
            else
            {
                user.IsActivated = true;
            }

            user.TimeZoneId = timezoneRepository.GetSingle(w => w.Name.Equals("UTC 0:00")).Id;

            if ((referrer != null) && IsCorrectReferrer((int)referrer))
                user.ReferrerId = referrer;

            userRepository.Add(user);
            userRepository.Save();

            Logger.Info(string.Format("{0} has been registered", user.Email));

            return GetUser(username);
        }

        /// <summary>
        /// Checking if created user is TEST USER
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsTestUser(ref string email)
        {
            const string TEST_USER_EMAIL_PART = "tu_1_";

            return email.StartsWith(TEST_USER_EMAIL_PART);
        }

        public string GetUserNameByEmail(string email)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == email);

            return result == null ? "" : result.Email;
        }

        public string GetUserNameById(int id)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Id == id);

            if (result != null)
                return result.Name;
            return null;
        }

        public string GetUserEmailById(int id)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Id == id);

            if (result != null)
                return result.Email;
            return string.Empty;
        }

        public MembershipUser GetUser(string email)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == email);
            if (result != null)
            {
                var _username = result.Name;
                var _providerUserKey = (int)result.Id;
                string _email = result.Email;
                if (result.RoleId != null)
                {
                    var _roleId = (int)result.RoleId;
                    const string _passwordQuestion = "";
                    string _comment = result.Comments;
                    bool _isLockedOut = result.IsLockedOut;
                    if (result.RegistrationDate != null)
                    {
                        var _creationDate = (DateTime)result.RegistrationDate;
                        DateTime _lastLoginDate = result.LastLoginDate;
                        DateTime _lastActivityDate = DateTime.Now;
                        DateTime _lastPasswordChangeDate = DateTime.Now;
                        DateTime _lastLockedOutDate = result.LastLockedOutDate;
                        var user = new VodadMembershipUser(
                            "VodadMembershipProvider",
                            _username,
                            _providerUserKey,
                            _email,
                            _roleId,
                            _passwordQuestion,
                            _comment,
                            _isLockedOut,
                            _creationDate,
                            _lastLoginDate,
                            _lastActivityDate,
                            _lastPasswordChangeDate,
                            _lastLockedOutDate);
                        return user;
                    }
                }
            }

            return null;
        }

        private static string CreateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[32];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        private static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
// ReSharper disable CSharpWarnings::CS0612
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
// ReSharper restore CSharpWarnings::CS0612
            return hashedPwd;
        }

        public bool ChangePassword(string userEmail, string oldPassword, string newPassword, bool userIsOnline)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == userEmail);

            if (result != null)
            {
                if (result.Password == CreatePasswordHash(oldPassword, result.PasswordSalt))
                {
                    result.PasswordSalt = CreateSalt();
                    result.Password = CreatePasswordHash(newPassword, result.PasswordSalt);
                    userRepository.Save();

                    string messageBody = result.Name + ",\r\n Your new password: " + newPassword + "\r\n\r\n Thank you. \r\n ShowNGain team.";
                    const string messageSubject = "ShowNGain password change";
                    UserHelper.SendMessage(result.Email, result.Name, messageBody, messageSubject);

                    Logger.Info(string.Format("User {0} id = {1} changed password", result.Email, result.Id));
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool ChangePassword(string userEmail, string newPassword)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == userEmail);

            if (result != null)
            {
                var salt = CreateSalt();

                // Необходимо перекодировать пароли для PerformerPlatform, используя новую Salt
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var performerPlatforms = performerPlatformRepository.GetAll(w => w.PerformerId == result.Id);

                foreach (var sp in performerPlatforms)
                {
                    var password = HttpUtility.HtmlDecode(EncryptionHelper.DecryptStringAES(sp.Password, result.PasswordSalt));

                    sp.Password = EncryptionHelper.EncryptStringAES(HttpUtility.HtmlEncode(password), salt);
                }

                result.PasswordSalt = salt;
                result.Password = CreatePasswordHash(newPassword, result.PasswordSalt);
                userRepository.Save();

                Logger.Info(string.Format("User {0} id = {1} changed password", result.Email, result.Id));

                return true;
            }
            return false;
        }

        internal bool ChangeProfile(string userEmail, int timezone, string comment, bool userIsOnline)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == userEmail);

            if (result != null)
            {
                result.TimeZoneId = timezone;

                result.Comments = HttpUtility.HtmlEncode(comment);
                userRepository.Save();

                Logger.Info(string.Format("User {0} id = {1} profile has been modified", result.Email, result.Id));

                return true;
            }
            return false;
        }

        public bool IsCorrectReferrer(int referrer)
        {
            var userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Id == referrer);

            if (user != null && !user.Roles.RoleName.Equals("Helper") && !user.Roles.RoleName.Equals("Banned") && !user.Roles.RoleName.Equals("Administrator"))
                return true;
            return false;
        }

        public bool ValidateUser(string userEmail, string password)
        {
            var userRepository = new Repository<User>(Entities);

            var result = userRepository.GetSingle(u => u.Email == userEmail);

            if (UserLoginDictionary.FirstOrDefault(w => w.Id == (int)result.Id) == null)
            {
                var userLoginDictionaryElement = new UserLoginDictionary { Id = (int)result.Id, PasswordFailedAttempts = 0 };
                UserLoginDictionary.Add(userLoginDictionaryElement);
            }

            var userLoginDictionary = UserLoginDictionary.FirstOrDefault(w => w.Id == (int)result.Id);
            if (userLoginDictionary != null && (result != null && userLoginDictionary.PasswordFailedAttempts < 5))
            {
                if (result.Password == CreatePasswordHash(password, result.PasswordSalt))
                {
                    var firstOrDefault = UserLoginDictionary.FirstOrDefault(w => w.Id == (int)result.Id);
                    if (firstOrDefault != null)
                        firstOrDefault.PasswordFailedAttempts = 0;
                    return true;
                }
                var loginDictionary = UserLoginDictionary.FirstOrDefault(w => w.Id == (int)result.Id);
                if (loginDictionary != null)
                    loginDictionary.PasswordFailedAttempts++;
                return false;
            }
            if (result != null) result.IsLockedOut = true;

            userRepository.Save();

            if (result != null)
                Logger.Info(string.Format("User {0} id = {1} has been locked", result.Email, result.Id));

            return false;
        }

        public User GetDBUser(string userEmail)
        {
            var userRepository = new Repository<User>(Entities);

            return userRepository.GetSingle(u => u.Email == userEmail);
        }

        public Roles GetRole(string name)
        {
            var userRepository = new Repository<Roles>(Entities);

            return userRepository.GetSingle(u => u.RoleName == name);
        }

        public List<User> GetAllUsers()
        {
            var userRepository = new Repository<User>(Entities);

            return userRepository.GetAll().ToList();
        }

        public void AddUsersToRoles(string[] usernames, string[] roleIds)
        {
            foreach (var username in usernames)
            {
                var user = GetDBUser(username);
                if (user != null)
                {
                    foreach (var roleId in roleIds)
                    {
                        var role = GetRole(roleId);
                        if (role != null)
                            if (user.RoleId != null && user.RoleId.Value != role.Id)
                                user.RoleId = role.Id;
                    }
                }
            }
        }

        public void CreateRole(string roleName)
        {
            var rolesRepository = new Repository<Roles>(Entities);

            if (GetRole(roleName) == null)
                rolesRepository.Add(new Roles { RoleName = roleName });
        }

        public void Save()
        {

        }
    }

    public class UserLoginDictionary
    {
        public int Id { get; set; }

        public int PasswordFailedAttempts { get; set; }
    }
}