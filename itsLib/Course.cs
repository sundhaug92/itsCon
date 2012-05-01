using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    public class Course : ICourseProjectCommons
    {
        uint Id;
        Session Session;

        public Course(Session Session, uint Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?CourseID=" + Id).GetResponse().Close();
        }

        public string getDashboardPath()
        {
            setActive();
            return "/Course/course.aspx";
        }

        public Directory getRootDirectory()
        {
            throw new NotImplementedException();
        }
    }
}