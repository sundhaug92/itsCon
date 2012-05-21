using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace webApp.Person
{
    public partial class Me : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Xporter.Session sess = ((Xporter.Session)Session["Xporter::Session"]);
            uint Id = sess.getMe().getId();
            Response.Redirect("Person.aspx?PersonId=" + Id.ToString(), true);
        }
    }
}