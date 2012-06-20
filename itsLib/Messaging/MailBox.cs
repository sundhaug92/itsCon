using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace itsLib.Messaging
{
    public class MailBox
    {
        private int MessageFolderId;
        private Session Session;

        public MailBox(Session Session, int MessageFolderId)
        {
            this.Session = Session;
            this.MessageFolderId = MessageFolderId;
        }

        public string Name
        {
            get
            {
                HttpWebRequest hwr = Session.GetHttpWebRequest("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId);
                HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                HtmlDocument Doc = new HtmlDocument();
                try
                {
                    Doc.Load(resp.GetResponseStream());
                    foreach (var v in Doc.DocumentNode.DescendantNodes())
                    {
                        if (v.Name != "span") continue;
                        if (v.Id == "ctl05_TT") return v.InnerText;
                    }
                    return "";
                }
                finally
                {
                    resp.Close();
                }
            }
        }

        public uint Pagesize
        {
            set
            {
                HtmlDocument doc = Session.GetDocument("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId.ToString());
                Dictionary<string, string> FormData = new Dictionary<string, string>();

                FormData.Add("__EVENTTARGET", "_table$4:Pagesize:" + value.ToString());
                FormData.Add("__EVENTARGUMENT", "");
                FormData.Add("_table$4:Pagesize:", value.ToString());

                foreach (var Form in doc.DocumentNode.Descendants("form"))
                {
                    if (Form.GetAttributeValue("id", "") == "ctl03")
                    {
                        foreach (var inp in doc.DocumentNode.Descendants("input"))
                        {
                            if (!FormData.ContainsKey(inp.GetAttributeValue("name", ""))) FormData.Add(inp.GetAttributeValue("name", ""), inp.GetAttributeValue("value", ""));
                        }
                        Session.PostData("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId.ToString(), FormData);
                    }
                }
            }
        }

        public List<Mail> GetMails()
        {
            Pagesize = 50; //Set Pagesize to a sensible, low value
            HttpWebResponse resp = (HttpWebResponse)Session.GetHttpWebRequest("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId).GetResponse();
            HtmlDocument Document = new HtmlDocument();
            Document.Load(resp.GetResponseStream());
            var Nodes = from m in Document.DocumentNode.DescendantNodes() where (m.Name == "tr") && (m.GetAttributeValue("id", "").StartsWith("_table_")) && (m.GetAttributeValue("id", "") != "_table_0") select m;
            List<Mail> Mails = new List<Mail>(Nodes.Count());
            foreach (var v in Nodes)
            {
                Mails.Add(new Mail(Session, uint.Parse(v.ChildNodes[5].GetAttributeValue("onclick", "").Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries)[1]), MessageFolderId));
            }
            return Mails;
        }
    }
}