using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using itsLib.fs;

namespace itsLib
{
    public interface ICourseProjectCommons
    {
        void setActive();

        string getDashboardPath();

        Directory getRootDirectory();
    }
}