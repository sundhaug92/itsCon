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
                HttpWebRequest hwr = Session.GetHttpWebRequest("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId.ToString());
                HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                HtmlDocument doc = new HtmlDocument();
                doc.Load(resp.GetResponseStream());
                resp.Close();
                Dictionary<string, string> FormData = new Dictionary<string, string>();
                foreach (var Form in doc.DocumentNode.Descendants("form"))
                {
                    if (Form.GetAttributeValue("id", "") == "ctl03")
                    {
                        Dictionary<string, string> NewLoginFormData = new Dictionary<string, string>();

                        foreach (var inp in doc.DocumentNode.Descendants("input"))
                        {
                            if (!FormData.ContainsKey(inp.GetAttributeValue("name", ""))) FormData.Add(inp.GetAttributeValue("name", ""), inp.GetAttributeValue("value", ""));
                        }
                        foreach (var inp in FormData)
                        {
                            NewLoginFormData.Add(inp.Key, WebUtility.UrlEncode(inp.Value));
                        }
                        FormData = NewLoginFormData;
                        string LoginUrl = Properties.Settings.Default.urlBase;
                        LoginUrl += Form.GetAttributeValue("action", "")[0] != '/' ? "/" + Form.GetAttributeValue("action", "") : Form.GetAttributeValue("action", "");
                        HttpWebRequest secondRequest = Session.GetHttpWebRequest("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId.ToString());
                        secondRequest.Method = "POST";
                        secondRequest.ContentType = "application/x-www-form-urlencoded";
                        string data = "";
                        foreach (var inp in FormData)
                        {
                            data += inp.Key + "=" + inp.Value + "&";
                        }
                        if (data[data.Length - 1] == '&') data.Substring(0, data.Length - 1);
                        secondRequest.ContentLength = System.Text.ASCIIEncoding.ASCII.GetByteCount(data);
                        secondRequest.GetRequestStream().Write(System.Text.ASCIIEncoding.ASCII.GetBytes(data), 0, (int)secondRequest.ContentLength);
                        HttpWebResponse Resp2 = (HttpWebResponse)secondRequest.GetResponse();
                        Resp2.Close();
                    }
                    throw new Exception("failed");
                }
            }
        }

        public Mail[] GetMails()
        {
            //Pagesize = 50; //Set Pagesize to a sensible, low value
            HttpWebResponse resp = (HttpWebResponse)Session.GetHttpWebRequest("/Messages/InternalMessages.aspx?MessageFolderId=" + MessageFolderId).GetResponse();
            HtmlDocument Document = new HtmlDocument();
            Document.Load(resp.GetResponseStream());
            var Nodes = from m in Document.DocumentNode.DescendantNodes() where (m.Name == "tr") && (m.GetAttributeValue("id", "").StartsWith("_table_")) && (m.GetAttributeValue("id", "") != "_table_0") select m;
            Mail[] Mails = new Mail[Nodes.Count()];
            foreach (var v in Nodes)
            {
                Mails[uint.Parse(v.GetAttributeValue("id", "").Substring("_table_".Length)) - 1] = new Mail(Session, uint.Parse(v.ChildNodes[5].GetAttributeValue("onclick", "").Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries)[1]), MessageFolderId);
            }
            return Mails;
        }
    }
}