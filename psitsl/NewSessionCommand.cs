using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace psitsl
{
    [Cmdlet(VerbsCommon.New, "ItslearningSession")]
    public class NewSessionCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new itsLib.Session());
        }
    }
}