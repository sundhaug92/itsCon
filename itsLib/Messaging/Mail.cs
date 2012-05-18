using System;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace itsLib.Messaging
{
    public class Mail
    {
        string _Path = "";

        Session _Session;
        HtmlDocument Document = new HtmlDocument();

        public Mail(Session Session, string Path)
        {
            _Path = Path;
            _Session = Session;
            WebResponse resp = _Session.GetHttpWebRequest(Path).GetResponse();
            Document.Load(resp.GetResponseStream());
            resp.Close();
        }

        public Mail(Session Session, uint Id, int FolderId)
            : this(Session, "/Messages/View_Message.aspx?MessageFolderID=" + FolderId.ToString() + "&MessageID=" + Id.ToString() + "&ShowAll=1")
        {
        }

        public Person From
        {
            get
            {
                var nodesWithJSOnclick = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("onclick", "").StartsWith("javascript:") select node;
                var nodesWithJSOnclickToPersons = from node in nodesWithJSOnclick where node.GetAttributeValue("onclick", "").StartsWith("javascript:window.open('/Person/show_person.aspx") select node;
                return new Person(_Session, uint.Parse(nodesWithJSOnclickToPersons.First().GetAttributeValue("onclick", "").Substring("javascript:window.open('/Person/show_person.aspx?".Length).Split(new char[] { '=', '&' })[1]));
            }
        }

        public Person[] To
        {
            get
            {
                var Description = (from node in Document.DocumentNode.DescendantNodes() where node.Name == "table" && node.GetAttributeValue("class", "") == "description" select node).First();
                var recipientList = (from node in Description.DescendantNodes() where node.Name == "td" select node.InnerText).ToArray()[1];
                string[] Names = recipientList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Person[] r = new Person[Names.Length];
                int i = 0;
                foreach (string s in Names)
                {
                    //r[i++] = new Person(_Session.Customer, s.Trim());
                    //Get Person id!
                }
                return r;
            }
        }

        public string Contents
        {
            get
            {
                var contents = from node in Document.DocumentNode.DescendantNodes() where node.Name == "div" && node.GetAttributeValue("class", "") == "userinput" select node;
                return contents.First().InnerHtml;
            }
        }

        public string Subject
        {
            get
            {
                var titles = from node in Document.DocumentNode.DescendantNodes() where node.Name == "span" && node.GetAttributeValue("id", "") == "ctl05_TT" select node;
                return titles.First().InnerText;
            }
        }
    }
}