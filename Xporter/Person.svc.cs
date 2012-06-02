using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Xporter
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Person
    {
        private itsLib.Person _Person;

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void fromId(Session Session, uint Id)
        {
            _Person = new itsLib.Person(Session.itslibSession, Id);
        }

        public uint getId()
        {
            return _Person.Id;
        }

        // Add more operations here and mark them with [OperationContract]
    }
}