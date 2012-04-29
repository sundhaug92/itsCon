using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    public class Course
    {
        int Id;
        Session Session;

        public Course(Session Session, int Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?CourseID=" + Id).GetResponse().Close();
        }

        public string DashboardPath
        {
            get
            {
                setActive();
                return "/Course/course.aspx";
            }
        }
    }
}