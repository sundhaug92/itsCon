using System;
using System.Net;
using HtmlAgilityPack;

namespace itsLib
{
    public class Customer
    {
        uint _Id;
        string _Name;

        public Customer(Session Session, uint Id)
        {
            HtmlDocument Doc = new HtmlDocument();
            _Id = Id;
            HttpWebRequest hwr = (HttpWebRequest)Session.GetHttpWebRequest("/");
            Doc.Load(hwr.GetResponse().GetResponseStream());
            foreach (var v in Doc.DocumentNode.Descendants("option"))
            {
                if (v.ParentNode.GetAttributeValue("id", "") != "ctl00_ContentPlaceHolder1_LoginSection1_ChooseSite_site_input") continue;
                if (v.GetAttributeValue("value", -1) == Id) _Name = v.NextSibling.InnerHtml;
            }
            if ((_Name == null) || (_Name == "")) throw new ArgumentException("No customer found", "Id");
        }

        public Customer(Session Session, string Name)
        {
            _Name = Name;
            HtmlDocument Doc = new HtmlDocument();
            HttpWebRequest hwr = (HttpWebRequest)Session.GetHttpWebRequest("/");
            Doc.Load(hwr.GetResponse().GetResponseStream());
            foreach (var v in Doc.DocumentNode.Descendants("option"))
            {
                if (v.ParentNode.GetAttributeValue("id", "") != "ctl00_ContentPlaceHolder1_LoginSection1_ChooseSite_site_input") continue;
                if (_Name == v.NextSibling.InnerHtml) _Id = (uint)v.GetAttributeValue("value", 0);
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