using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;
using Vodad.Controllers;

public class VodadMembershipProvider : MembershipProvider
{

    //&nbsp;// Свойства из web.config
    private string _ApplicationName;
    private bool _EnablePasswordReset;
    private bool _EnablePasswordRetrieval = false;
    private bool _RequiresQuestionAndAnswer = false;
    private bool _RequiresUniqueName = true;
    private bool _RequiresUniqueEmail = true;
    private int _MaxInvalidPasswordAttempts;
    private int _PasswordAttemptWindow;
    private int _MinRequiredPasswordLength;
    private int _MinRequiredNonalphanumericCharacters;
    private string _PasswordStrengthRegularExpression;
    private MembershipPasswordFormat _PasswordFormat = MembershipPasswordFormat.Hashed;


    public override string ApplicationName
    {
        get
        {
            return _ApplicationName;
        }
        set
        {
            _ApplicationName = value;
        }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
        return false;
    }

    public MembershipCreateStatus CreateUser(string username, string password, string email, string roleName, int? referrer, string passwordQuestion, string passwordAnswer, object providerUserKey, out MembershipCreateStatus status, string userIp)
    {
        var args = new ValidatePasswordEventArgs(username, password, true);
        OnValidatingPassword(args);
        if (args.Cancel)
        {
            status = MembershipCreateStatus.InvalidPassword;
            return status;
        }
        if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
        {
            status = MembershipCreateStatus.DuplicateEmail;
            return status;
        }
        var u = GetUser(username, false);
        if (u == null)
        {
            var _user = new UserRepository();
            _user.CreateUser(username, password, email, roleName, referrer, userIp);
            status = MembershipCreateStatus.Success;

            return status;
        }
        else
        {
            status = MembershipCreateStatus.DuplicateUserName;
        }

        return status;
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isActivated, object providerUserKey, out MembershipCreateStatus status)
    {
        throw new NotImplementedException("Must be any role");
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        throw new NotImplementedException();
    }

    public override bool EnablePasswordReset
    {
        get
        {
            return _EnablePasswordReset;
        }
    }

    public override bool EnablePasswordRetrieval
    {
        get
        {
            return _EnablePasswordRetrieval;
        }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    // Хелпер для получения значений из конфигурационного файла
    private string GetConfigValue(string configValue, string defaultValue)
    {
        if (string.IsNullOrEmpty(configValue))
            return defaultValue;
        return configValue;
    }

    public override int GetNumberOfUsersOnline()
    {
        throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string userEmail, bool userIsOnline)
    {
        UserRepository _user = new UserRepository();
        return _user.GetUser(userEmail);
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email)
    {
        UserRepository _user = new UserRepository();
        return _user.GetUserNameByEmail(email);
    }

    // Метод инициализации
    public override void Initialize(string name, NameValueCollection config)
    {
        if (config == null)
            throw new ArgumentNullException("config");
        if (name == null || name.Length == 0)
            name = "VodadMembershipProvider";
        if (String.IsNullOrEmpty(config["description"]))
        {
            config.Remove("description");
            config.Add("description", "Vodad Membership Provider");
        }
        base.Initialize(name, config);
        _ApplicationName = GetConfigValue(config["applicationName"],
                        System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        _MaxInvalidPasswordAttempts = Convert.ToInt32(
                        GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
        _PasswordAttemptWindow = Convert.ToInt32(
                        GetConfigValue(config["passwordAttemptWindow"], "10"));
        _MinRequiredNonalphanumericCharacters = Convert.ToInt32(
                        GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
        _MinRequiredPasswordLength = Convert.ToInt32(
                        GetConfigValue(config["minRequiredPasswordLength"], "6"));
        _EnablePasswordReset = Convert.ToBoolean(
                        GetConfigValue(config["enablePasswordReset"], "true"));
        _PasswordStrengthRegularExpression = Convert.ToString(
                        GetConfigValue(config["passwordStrengthRegularExpression"], ""));
    }


    public override int MaxInvalidPasswordAttempts
    {
        get
        {
            return _MaxInvalidPasswordAttempts;
        }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get
        {
            return _MinRequiredNonalphanumericCharacters;
        }
    }

    public override int MinRequiredPasswordLength
    {
        get
        {
            return _MinRequiredPasswordLength;
        }
    }

    public override int PasswordAttemptWindow
    {
        get
        {
            return _PasswordAttemptWindow;
        }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
        get
        {
            return _PasswordFormat;
        }
    }

    public override string PasswordStrengthRegularExpression
    {
        get
        {
            return _PasswordStrengthRegularExpression;
        }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get
        {
            return _RequiresQuestionAndAnswer;
        }
    }

    public override bool RequiresUniqueEmail
    {
        get
        {
            return _RequiresUniqueEmail;
        }
    }

    public override string ResetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
        throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
        throw new NotImplementedException();
    }

    public override bool ValidateUser(string username, string password)
    {
        UserRepository _user = new UserRepository();

        return _user.ValidateUser(username, password);
    }
}