using System;
using System.Linq;
using System.Net;
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

        public string Text
        {
            get
            {
                Parent.setActive();
                HtmlDocument Document = new HtmlDocument();
                WebResponse resp = Session.GetHttpWebRequest("/Bulletin/View.aspx?BulletinId=" + Id.ToString() + "&LocationType=2").GetResponse();
                Document.Load(resp.GetResponseStream());
                resp.Close();
                var userinput = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("class", "") == "userinput" && node.Name == "div" select node;
                return userinput.First().InnerHtml;
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

        public static Bulletin[] inProject(Project Project)
        {
            return inPath(Project.getDashboardPath());
        }

        public static Bulletin[] inCourse(Course Course)
        {
            return inPath(Course.getDashboardPath());
        }

        public static Bulletin[] inPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}