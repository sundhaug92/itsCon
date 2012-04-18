using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace itsMailTestCon
{
    public partial class smtpService
    {
        smtpServer serverInstance = new smtpServer();
        public smtpService()
        {
        }

        /*protected override */
        public void OnStart(string[] args)
        {
            serverInstance.Start();
        }

        /*protected override */ public void OnStop()
        {
            serverInstance.Stop();
        }
    }
}
