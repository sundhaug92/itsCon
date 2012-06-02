using System;
using System.Web;

namespace webApp.Person
{
    public partial class Person : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Uri user_info_Uri = Request.Url;
            uint Uid = uint.Parse(HttpUtility.ParseQueryString(user_info_Uri.Query).Get("PersonId"));
        }
    }
}