using System.Management.Automation;

namespace psitsl
{
    [Cmdlet(VerbsCommon.New, "ItslearningCustomer")]
    public class NewCustomerCommand : Cmdlet
    {
        private uint _Id = 0;
        private itsLib.Session _Session = null;

        [Parameter(
                                    Mandatory = true,
                                    ValueFromPipeline = true)]
        public uint Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        [Parameter(
                                    Mandatory = true,
                                    ValueFromPipeline = true)]
        public itsLib.Session Session
        {
            get
            {
                return _Session;
            }
            set
            {
                _Session = value;
            }
        }

        protected override void ProcessRecord()
        {
            WriteObject(new itsLib.Customer(Session, Id));
        }
    }
}