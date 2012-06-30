using System;
using itsLib;

namespace webApp.Person
{
    public partial class Me : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session sess = ((Session)Session["Xporter::Session"]);
            uint Id = sess.Me.Id;
            Response.Redirect("Person.aspx?PersonId=" + Id.ToString(), true);
        }
    }
}