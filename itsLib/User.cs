using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using System.Web;

namespace itsLib
{
    public class User
    {
        Customer _Customer;
        string _Name;
        public string Name
        {
            get { return _Name; }
        }
        public User(Customer customer, string Name)
        {
            this._Customer = customer;
            this._Name = Name;
        }
        public static User Me(Session sess)
        {
            int Uid=0;

            HttpWebRequest hwr = sess.GetHttpWebRequest("/TopMenu.aspx?Course=&CPHFrame=1&item=menu_intranet");
            HtmlDocument top_menu = new HtmlDocument();
            HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
            top_menu.Load(resp.GetResponseStream());
            var e = from element in top_menu.DocumentNode.Descendants("a") where element.GetAttributeValue("class", "") == "user_name" select element.GetAttributeValue("href", "");
            Uri user_info_Uri = new Uri(Properties.Settings.Default.Domain + e.First());
            Uid = int.Parse(HttpUtility.ParseQueryString(user_info_Uri.Query).Get("PersonId"));
            resp.Close();
            return fromUid(sess, Uid);
        }
        public static User fromUid(Session sess, int Uid)
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
            while (Name.EndsWith(" ")) Name = Name.Substring(0, Name.Length - 1);
            return new User(sess.Customer, Name);
        }
    }
}
