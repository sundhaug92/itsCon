using System;
using itsLib.fs;

namespace itsLib
{
    public class Project : ICourseProjectCommons
    {
        uint Id;
        Session Session;

        public Project(Session Session, uint Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?ProjectID=" + Id).GetResponse().Close();
        }

        public string getDashboardPath()
        {
            setActive();
            return "/Project/project.aspx";
        }

        public Directory getRootDirectory()
        {
            throw new NotImplementedException();
        }

        public void toArchive()
        {
            setActive();
            Session.GetHttpWebRequest("/project/about.aspx?Archive=toggle").GetResponse().Close();
        }
    }
}