using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using VodadModel;
using VodadModel.Repository;

namespace Vodad.Models
{
    public class VodadMembershipUser : MembershipUser
    {
        public int roleId;

        public VodadMembershipUser(string providerName, string name, object providerUserKey, string email, int roleId, string passwordQuestion, string comment, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
            : base(providerName, name, providerUserKey, email, passwordQuestion, comment, false, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
            this.roleId = roleId;
        }
    }
}