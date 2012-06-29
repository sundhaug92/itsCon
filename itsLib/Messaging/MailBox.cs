using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
                HtmlDocument Doc = Session.GetDocument("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId);
                foreach (var v in Doc.DocumentNode.DescendantNodes())
                {
                    if (v.Name != "span") continue;
                    if (v.Id == "ctl05_TT") return v.InnerText;
                }
                return "";
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
            Pagesize = uint.MaxValue; //Set Pagesize as high as possible (to get all messages)
            HtmlDocument Document = Session.GetDocument("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId);
            var Nodes = from m in Document.DocumentNode.DescendantNodes() where (m.Name == "tr") && (m.GetAttributeValue("id", "").StartsWith("_table_")) && (m.GetAttributeValue("id", "") != "_table_0") select m;

            ConcurrentDictionary<uint, Mail> _Mails = new ConcurrentDictionary<uint, Mail>();
            Parallel.ForEach(Nodes, (v) =>
                {
                    _Mails.AddOrUpdate(
                        uint.Parse(v.GetAttributeValue("id", "").Substring("_table_".Length)),
                        new Mail(Session, uint.Parse(v.ChildNodes[5].GetAttributeValue("onclick", "").Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries)[1]), MessageFolderId),
                        (key, oldValue) => oldValue);
                });

            List<Mail> Mails = new List<Mail>(Nodes.Count());
            foreach (var v in _Mails)
            {
                Mails.Add(v.Value);
            }
            return Mails;
        }
    }
}