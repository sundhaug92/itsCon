using System;
using System.Web.Security;
using System.Web.UI;

namespace webApp
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!((Xporter.Session)Session["Xporter::Session"]).itslibSession.LoggedIn) //Switch to Xporter.getLoginStatus when that's implemented
            {
                if (Request.Path != "/Account/Login.aspx")
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
        }
    }
}