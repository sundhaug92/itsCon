using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return inPath(Project.DashboardPath);
        }

        private static Bulletin[] inPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}