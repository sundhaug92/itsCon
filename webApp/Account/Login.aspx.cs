using System;
using System.Web.Security;
using System.Web.UI;
using itsLib;

namespace webApp.Account
{
    public partial class Login : Page
    {
        protected void Login_OnAuthentificate(object sender, System.Web.UI.WebControls.AuthenticateEventArgs e)
        {
            bool auth = LogMeIn(uint.Parse(Request["ctl00$MainContent$loginCtl$CustomerId"]), loginCtl.UserName, loginCtl.Password, loginCtl.RememberMeSet);
            if (auth)
            {
                FormsAuthentication.RedirectFromLoginPage(loginCtl.UserName, loginCtl.RememberMeSet);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private bool LogMeIn(uint p1, string p2, string p3, bool b)
        {
            try
            {
                Session sess = ((Session)Session["Xporter::Session"]);
                Customer Customer = new Customer(sess, p1);
                sess.Customer = Customer;
                sess.Login(p2, p3);
                return sess.LoggedIn;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}