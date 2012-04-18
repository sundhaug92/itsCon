using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace itsMailTestCon
{
    class Program
    {
        static void Main(string[] args)
        {
            smtpService service = new smtpService();
            service.OnStart(args);
            while (Console.Read() != 'Q') ;
            service.OnStop();
        }
    }
}
