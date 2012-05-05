using System;
using System.Linq;
using System.Net;

using System.Net;

using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Bulletin
    {
        int Id = 0;
        Session Session;
        ICourseProjectCommons Parent;

        public Bulletin(Session Session, ICourseProjectCommons Parent, int Id)
        {
            this.Id = Id;
            this.Session = Session;
            this.Parent = Parent;
        }

        public string Title
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = new HtmlDocument();
                WebResponse resp = Session.GetHttpWebRequest("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2").GetResponse();
                Document.Load(resp.GetResponseStream());
                resp.Close();
                try
                {
                    var ctl05_TT = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("id", "") == "ctl05_TT" && node.Name == "span" select node;
                    return ctl05_TT.First().InnerText;
                }
                catch (InvalidOperationException) { }
                return "";
            }
        }

        public string Text
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = new HtmlDocument();
                WebResponse resp = Session.GetHttpWebRequest("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2").GetResponse();
                Document.Load(resp.GetResponseStream());
                resp.Close();
                try
                {
                    var userinput = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("class", "") == "userinput" && node.Name == "div" select node;
                    return userinput.First().InnerHtml.Trim();
                }
                catch (InvalidOperationException) { }
                return "<p></p>";
            }
        }

        public Person By
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = new HtmlDocument();
                WebResponse resp = Session.GetHttpWebRequest("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2").GetResponse();
                Document.Load(resp.GetResponseStream());
                resp.Close();
                var nodesWithJSOnclick = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("onclick", "").StartsWith("javascript:") select node;
                var nodesWithJSOnclickToPersons = from node in nodesWithJSOnclick where node.GetAttributeValue("onclick", "").StartsWith("javascript:window.open('/Person/show_person.aspx") select node;
                return Person.fromUid(Session, int.Parse(nodesWithJSOnclickToPersons.First().GetAttributeValue("onclick", "").Substring("javascript:window.open('/Person/show_person.aspx?".Length).Split(new char[] { '=', '&' })[1]));
            }
        }

        public static Bulletin[] inProject(Session Session, Project Project)
        {
            return inCP(Session, Project);
        }

        public static Bulletin[] inCourse(Session Session, Course Course)
        {
            return inCP(Session, Course);
        }

        public static Bulletin[] inCP(Session Session, ICourseProjectCommons Parent)
        {
            string path = Parent.getDashboardPath();
            HtmlDocument Document = new HtmlDocument();
            WebResponse resp = Session.GetHttpWebRequest(path).GetResponse();
            Document.Load(resp.GetResponseStream());
            resp.Close();
            var nodesWithHrefToBulletin = from node in Document.DocumentNode.DescendantNodes() where node.Name == "a" && node.GetAttributeValue("href", "").Contains("/Bulletin/View") select node.GetAttributeValue("href", "");
            Bulletin[] Bulletins = new Bulletin[nodesWithHrefToBulletin.Count()];
            int i = 0;
            foreach (string uri_string in nodesWithHrefToBulletin)
            {
                Uri uri = uri_string.StartsWith("/") ? new Uri(Properties.Settings.Default.urlBase + uri_string) : new Uri(uri_string);
                Bulletins[i++] = new Bulletin(Session, Parent, int.Parse(HttpUtility.ParseQueryString(uri.Query).Get("BulletinId")));
            }
            return Bulletins;
        }
    }
}