using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itsLib
{
    internal class PersonSearch
    {
        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId, int CourseId, PersonType PersonType)
        {
        }
        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId, int CourseId)
            :this(Session, Forname, Surname, HierarchyId, CourseId, PersonType.administrator| PersonType.employee | PersonType.examinator | PersonType.guest| PersonType.parent| PersonType.student | PersonType.sysadmin)
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId)
            : this(Session, Forname, Surname, HierarchyId, -1)
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname)
            : this(Session, Forname, Surname, -1)
        {
        }

        public static PersonSearch GetAll(Session Session)
        {
            return new PersonSearch(Session, "", " ");
        }
    }
}