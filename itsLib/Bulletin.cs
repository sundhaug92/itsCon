using System;

namespace itsLib
{
    internal class Bulletin
    {
        int _Id = 0;

        private Bulletin(int Id)
        {
            _Id = Id;
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