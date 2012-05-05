using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    public class Directory
    {
        Session Session;
        ICourseProjectCommons Parent;

        public Directory(Session Session, ICourseProjectCommons Parent)
        {
            this.Session = Session;
            this.Parent = Parent;
        }
    }
}