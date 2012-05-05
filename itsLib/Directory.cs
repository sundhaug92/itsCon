using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Directory
    {
        Session Session;
        ICourseProjectCommons Parent;
        uint Id;

        public Directory(Session Session, ICourseProjectCommons Parent, uint Id)
        {
            this.Session = Session;
            this.Parent = Parent;
            this.Id = Id;
        }

        public Directory[] Subdirectories()
        {
            HtmlDocument Document = new HtmlDocument();
            WebResponse resp = Session.GetHttpWebRequest("/Folder/process_folder.aspx?FolderID=" + Id.ToString()).GetResponse();
            Document.Load(resp.GetResponseStream());
            resp.Close();
            var nodesWithHrefToDirectory = from node in Document.DocumentNode.DescendantNodes() where node.Name == "a" && node.GetAttributeValue("href", "").Contains("/Folder/process_folder.aspx") select node.GetAttributeValue("href", "");
            Directory[] Subdirectories = new Directory[nodesWithHrefToDirectory.Count()];
            uint i = 0;
            foreach (string relPath in nodesWithHrefToDirectory)
            {
                Uri uri = relPath.StartsWith("/") ? new Uri(Properties.Settings.Default.urlBase + relPath) : new Uri(relPath);
                Subdirectories[i++] = new Directory(Session, Parent, uint.Parse(HttpUtility.ParseQueryString(uri.Query).Get("FolderID")));
            }
            return Subdirectories;
        }
    }
}