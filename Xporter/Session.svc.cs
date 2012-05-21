﻿using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Xporter
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Session
    {
        itsLib.Session _Session;

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void Create()
        {
            _Session = new itsLib.Session();
        }

        [OperationContract]
        public void setCustomer(Customer Customer)
        {
            _Session.Customer = Customer.itsLibCustomer();
        }

        [OperationContract]
        public itsLib.Customer getCustomer()
        {
            return _Session.Customer;
        }

        [OperationContract]
        public bool Login(string Username, string Password)
        {
            _Session.Login(Username, Password);
            return getLoginState();
        }

        [OperationContract]
        public bool getLoginState()
        {
            return _Session.LoggedIn;
        }

        // Add more operations here and mark them with [OperationContract]
    }
}