using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Xporter
{
    [ServiceContract(Namespace = "", SessionMode = SessionMode.Required)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Customer
    {
        private itsLib.Customer _Customer;

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void fromName(string Name)
        {
            _Customer = new itsLib.Customer(new itsLib.Session(), Name);
        }

        [OperationContract]
        public void fromId(uint Id)
        {
            _Customer = new itsLib.Customer(new itsLib.Session(), Id);
        }

        [OperationContract]
        public itsLib.Customer itsLibCustomer()
        {
            return _Customer;
        }

        // Add more operations here and mark them with [OperationContract]
    }
}