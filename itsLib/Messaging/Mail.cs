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
            : this(Session, "/Messages/View_Message.aspx?MessageFolderID=" + FolderId.ToString() + "&MessageID=" + Id.ToString())
        {
        }

        public User From
        {
            get
            {
                var nodesWithJSOnclick = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("onclick", "").StartsWith("javascript:") select node;
                var nodesWithJSOnclickToPersons = from node in nodesWithJSOnclick where node.GetAttributeValue("onclick", "").StartsWith("javascript:window.open('/Person/show_person.aspx") select node;
                return User.fromUid(_Session, int.Parse(nodesWithJSOnclickToPersons.First().GetAttributeValue("onclick", "").Substring("javascript:window.open('/Person/show_person.aspx?".Length).Split(new char[] { '=', '&' })[1]));
            }
        }

        public string To
        {
            get { throw new NotImplementedException(); }
        }

        public string Contents
        {
            get { throw new NotImplementedException(); }
        }
    }
}