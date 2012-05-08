using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using itsLib;
using System.Web.SessionState;

namespace itsLibMembershipProvider
{
    public class itsMembershipProvider : MembershipProvider
    {
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }
        public override bool RequiresUniqueEmail
        {
            get
            {
                return false;
            }
        }
        public override int MinRequiredPasswordLength
        {
            get
            {
                return 6;
            }
        }
        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return "";
            }
        }
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return int.MaxValue;
            }
        }
        public override int PasswordAttemptWindow
        {
            get
            {
                return int.MaxValue;
            }
        }
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }
        public override string ApplicationName
        {
            get
            {
                return "itsLib-Xporter";
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override MembershipUserCollection FindUsersByName(string s, int a, int b, out int c)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByEmail(string s, int a, int b, out int c)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, Object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.ProviderError;
            return null;
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return false;
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string username)
        {
            return false;
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        public override bool ValidateUser(string username, string password)
        {
            Session sess = new Session();
            string realUsername = username.Substring(username.IndexOf("\\") + 1);
            uint CustomerId = uint.Parse(username.Substring(0, username.IndexOf("\\")));
            sess.Customer = new Customer(sess, CustomerId);
            try
            {
                sess.Login(realUsername, password);
            }
            catch (Exception) { return false; }
            return sess.LoggedIn;
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(object o, bool b)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(string s, bool b)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteUser(string s, bool b)
        {
            throw new NotImplementedException();
        }
        public override string GetUserNameByEmail(string s)
        {
            throw new NotImplementedException();
        }
    }
}
