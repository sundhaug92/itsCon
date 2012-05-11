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