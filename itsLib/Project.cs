using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    public class Project
    {
        int Id;
        Session Session;

        public Project(Session Session, int Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?ProjectID=" + Id).GetResponse().Close();
        }

        public string DashboardPath
        {
            get
            {
                setActive();
                return "/Project/project.aspx";
            }
        }
    }
}