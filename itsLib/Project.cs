using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    internal class Project
    {
        int Id;
        Session Session;

        public Project(Session Session, int Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public string DashboardPath
        {
            get
            {
                Session.GetHttpWebRequest("/main.aspx?ProjectID=" + Id).GetResponse().Close();
                return "/Project/project.aspx";
            }
        }
    }
}