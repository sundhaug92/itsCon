using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    public enum PersonType
    {
        sysadmin = 1,
        examinator = 2,
        administrator = 4,
        employee = 8,
        student = 16,
        parent = 32,
        guest = 64
    }
}