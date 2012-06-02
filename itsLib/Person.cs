﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Person
    {
        private Customer _Customer;
        private uint _Id;
        private string _Name;

        public Person(Session sess, uint Uid)
        {
            HttpWebRequest hwr = sess.GetHttpWebRequest("/Person/show_person.aspx?PersonId=" + Uid.ToString() + "&Customer=" + sess.Customer.Id);
            HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
            HtmlDocument Personalia = new HtmlDocument();
            Personalia.Load(resp.GetResponseStream());
            resp.Close();
            string Name = (from v in Personalia.DocumentNode.Descendants("span") where v.Id == "ctl00_PageHeader_TT" select v.InnerText).First();
            if (Name.Contains('('))
            {
                Name = Name.Substring(0, Name.IndexOf('('));
            }
            Name = Name.Trim();
            string Forename = Name.Substring(Name.IndexOf(", ") + ", ".Length);
            string Surname = Name.Substring(0, Name.IndexOf(", "));
            this._Customer = sess.Customer;
            this._Name = Name;
            this._Id = Uid;
        }

        public uint Id
        {
            get
            {
                return _Id;
            }
        }

        public string Name
        {
            get { return _Name; }
        }

        public Uri PublicFilesBaseUri
        {
            get
            {
                return new Uri("http://files.itslearning.com/data/" + this._Customer.Id.ToString() + "/" + _Id.ToString());
            }
        }

        public static Person Me(Session sess)
        {
            uint Uid = 0;

            HttpWebRequest hwr = sess.GetHttpWebRequest("/TopMenu.aspx?Course=&CPHFrame=1&item=menu_intranet");
            HtmlDocument top_menu = new HtmlDocument();
            HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
            top_menu.Load(resp.GetResponseStream());
            var e = from element in top_menu.DocumentNode.Descendants("a") where element.GetAttributeValue("class", "") == "user_name" select element.GetAttributeValue("href", "");
            Uri user_info_Uri = new Uri(Properties.Settings.Default.urlBase + e.First());
            Uid = uint.Parse(HttpUtility.ParseQueryString(user_info_Uri.Query).Get("PersonId"));
            resp.Close();
            return new Person(sess, Uid);
        }
    }
}