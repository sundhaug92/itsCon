using System;

namespace itsLib
{
    internal class Bulletin
    {
        int Id = 0;
        Session Session; ICourseProjectCommons Parent;

        private Bulletin(Session Session, ICourseProjectCommons Parent, int Id)
        {
            this.Id = Id;
            this.Session = Session;
            this.Parent = Parent;
        }

        public string Text
        {
            get
            {
                string r = "";

                return r;
            }
        }

        private static Bulletin[] inProject(Project Project)
        {
            return inPath(Project.getDashboardPath());
        }

        private static Bulletin[] inCourse(Course Course)
        {
            return inPath(Course.getDashboardPath());
        }

        private static Bulletin[] inPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}