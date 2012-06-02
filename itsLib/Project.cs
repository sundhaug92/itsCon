using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Project : ICourseProjectCommons
    {
        private uint Id;
        private Session Session;

        public Project(Session Session, uint Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public string getDashboardPath()
        {
            setActive();
            return "/Project/project.aspx";
        }

        public fs.Directory getRootDirectory()
        {
            setActive();
            HtmlDocument Document = new HtmlDocument();
            WebResponse resp = Session.GetHttpWebRequest("/ContentArea/ContentAreaTreeMenu.aspx?LocationID=" + Id.ToString() + "&LocationType=2").GetResponse();
            Document.Load(resp.GetResponseStream());
            resp.Close();
            var jsFunction = (from node in Document.DocumentNode.DescendantNodes() where node.Name == "script" select node.InnerHtml).Last();
            jsFunction = jsFunction.Substring(jsFunction.IndexOf("\"data\" : \"") + "\"data\" : \"".Length);
            jsFunction = jsFunction.Substring(0, jsFunction.IndexOf('"'));
            HtmlDocument sidebar_menu = new HtmlDocument();
            sidebar_menu.Load(new StringReader(jsFunction));
            string DirectoryLinkString = (from node in sidebar_menu.DocumentNode.DescendantNodes() where node.Name == "a" && node.GetAttributeValue("href", "").Contains("/process_folder.aspx") select node.GetAttributeValue("href", "")).First();
            Uri uri = DirectoryLinkString.StartsWith("/") ? new Uri(Properties.Settings.Default.urlBase + DirectoryLinkString) : new Uri(DirectoryLinkString);
            return new fs.Directory(Session, this, uint.Parse(HttpUtility.ParseQueryString(uri.Query).Get("FolderID")));
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?ProjectID=" + Id).GetResponse().Close();
        }

        public void toArchive()
        {
            setActive();
            Session.GetHttpWebRequest("/project/about.aspx?Archive=toggle").GetResponse().Close();
        }
    }
}