using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace itsLib
{
    public class Customer
    {
        uint _Id;
        string _Name;

        public Customer(uint Id)
        {
            HtmlDocument Doc = new HtmlDocument();
            _Id = Id;
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(Properties.Settings.Default.Domain);
            hwr.Timeout = 1 * 60 * 1000;
            hwr.ContinueTimeout = 1 * 60 * 1000;
            hwr.UserAgent = Properties.Settings.Default.UA_String;
            Doc.Load(hwr.GetResponse().GetResponseStream());
            foreach (var v in Doc.DocumentNode.Descendants("option"))
            {
                if (v.ParentNode.GetAttributeValue("id", "") != "ctl00_ContentPlaceHolder1_LoginSection1_ChooseSite_site_input") continue;
                if (v.GetAttributeValue("value", -1) == Id) _Name = v.NextSibling.InnerHtml;
            }
            if ((_Name == null) || (_Name == "")) throw new ArgumentException("No customer found", "Id");
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public uint Id
        {
            get
            {
                return _Id;
            }
        }
    }
}