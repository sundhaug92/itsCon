using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Bulletin
    {
        private uint Id = 0;
        private ICourseProjectCommons Parent;
        private Session Session;

        public Bulletin(Session Session, ICourseProjectCommons Parent, uint Id)
        {
            this.Id = Id;
            this.Session = Session;
            this.Parent = Parent;
        }

        public Person By
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = Session.GetDocument("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2");
                var nodesWithJSOnclick = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("onclick", "").StartsWith("javascript:") select node;
                var nodesWithJSOnclickToPersons = from node in nodesWithJSOnclick where node.GetAttributeValue("onclick", "").StartsWith("javascript:window.open('/Person/show_person.aspx") select node;
                return new Person(Session, uint.Parse(nodesWithJSOnclickToPersons.First().GetAttributeValue("onclick", "").Substring("javascript:window.open('/Person/show_person.aspx?".Length).Split(new char[] { '=', '&' })[1]));
            }
        }

        public string Text
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = Session.GetDocument("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2");
                try
                {
                    var userinput = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("class", "") == "userinput" && node.Name == "div" select node;
                    return userinput.First().InnerHtml.Trim();
                }
                catch (InvalidOperationException) { }
                return "<p></p>";
            }
        }

        public string Title
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = Session.GetDocument("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2");
                try
                {
                    var ctl05_TT = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("id", "") == "ctl05_TT" && node.Name == "span" select node;
                    return ctl05_TT.First().InnerText;
                }
                catch (InvalidOperationException) { }
                return "";
            }
        }

        public static List<Bulletin> inCourse(Session Session, Course Course)
        {
            return inCP(Session, Course);
        }

        public static List<Bulletin> inCP(Session Session, ICourseProjectCommons Parent)
        {
            string path = Parent.getDashboardPath();
            HtmlDocument Document = Session.GetDocument(path);
            var nodesWithHrefToBulletin = from node in Document.DocumentNode.DescendantNodes() where node.Name == "a" && node.GetAttributeValue("href", "").Contains("/Bulletin/View") select node.GetAttributeValue("href", "");
            List<Bulletin> Bulletins = new List<Bulletin>(nodesWithHrefToBulletin.Count());
            int i = 0;
            foreach (string uri_string in nodesWithHrefToBulletin)
            {
                Uri uri = uri_string.StartsWith("/") ? new Uri(Properties.Settings.Default.urlBase + uri_string) : new Uri(uri_string);
                Bulletins[i++] = new Bulletin(Session, Parent, uint.Parse(HttpUtility.ParseQueryString(uri.Query).Get("BulletinId")));
            }
            return Bulletins;
        }

        public static List<Bulletin> inProject(Session Session, Project Project)
        {
            return inCP(Session, Project);
        }
    }
}