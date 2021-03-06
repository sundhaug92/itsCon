﻿using System.Collections.Generic;

namespace itsLib
{
    public class MailCreator
    {
        public MailCreator()
        {
            To = new List<Person>();
            Cc = new List<Person>();
            Bcc = new List<Person>();
        }

        public List<Person> Bcc { get; set; }

        public List<Person> Cc { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public List<Person> To { get; set; }

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
            Dictionary<string, string> MailData = new Dictionary<string, string>(5);
            string Persons = "";
            foreach (Person Person in To)
            {
                Persons += Person.Username + ";";
            }
            MailData.Add("to", Persons);

            Persons = "";
            foreach (Person Person in Cc)
            {
                Persons += Person.Username + ";";
            }
            MailData.Add("cc", Persons);

            Persons = "";
            foreach (Person Person in Bcc)
            {
                Persons += Person.Username + ";";
            }
            MailData.Add("bcc", Persons);

            MailData.Add("operationId", (1000).ToString());
            MailData.Add("id", (0).ToString());
            MailData.Add("text", Text);
            MailData.Add("subject", Text);
            MailData.Add("files", "");
            MailData.Add("messageMeasurement", (2).ToString());
            MailData.Add("_", "");
            Session.PostData("/XmlHttp/Api.aspx?Function=MessagingValidateRecipients", MailData);
            Session.PostData("/XmlHttp/Api.aspx?Function=MessagingSendMessage&MessageOperationID=" + (1000).ToString(), MailData);
        }
    }
}