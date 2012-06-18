using System;

namespace itsLib
{
    internal class MailCreator
    {
        public Person[] Bcc { get; set; }

        public Person[] Cc { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public Person[] To { get; set; }

        public void SendMessage(Session Session)
        {
            /*
             * /XmlHttp/Api.aspx?Function=MessagingValidateRecipients
             * operationId: 1000
             * to:
             * cc:
             * bcc:
             * subject:
             * text:
             * files:
             * id: 0
             * messageMeasurement: 2
             * _:
            */
        }
    }
}