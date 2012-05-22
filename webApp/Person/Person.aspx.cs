using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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