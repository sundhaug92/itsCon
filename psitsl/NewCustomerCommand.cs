using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace psitsl
{
    [Cmdlet(VerbsCommon.New, "ItslearningCustomer")]
    internal class NewCustomerCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject();
        }
    }
}