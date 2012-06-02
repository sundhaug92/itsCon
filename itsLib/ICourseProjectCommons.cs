using itsLib.fs;

namespace itsLib
{
    public interface ICourseProjectCommons
    {
        string getDashboardPath();

        Directory getRootDirectory();

        void setActive();
    }
}