using System.Management.Automation;

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