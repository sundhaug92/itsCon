using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace itsLib.fs
{
    public class Directory
    {
        private uint _Id;
        private ICourseProjectCommons Parent;
        private Session Session;

        public Directory(Session Session, ICourseProjectCommons Parent, uint Id)
        {
            this.Session = Session;
            this.Parent = Parent;
            this._Id = Id;
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
            get
            {
                Parent.setActive();
                HtmlDocument Document = new HtmlDocument();
                WebResponse resp = Session.GetHttpWebRequest("/Folder/process_folder.aspx?FolderID=" + _Id.ToString()).GetResponse();
                Document.Load(resp.GetResponseStream());
                resp.Close();
                try
                {
                    var span = from node in Document.DocumentNode.DescendantNodes() where node.Name == "span" select node;
                    return span.First().InnerText;
                }
                catch (InvalidOperationException) { }
                return "";
                throw new NotImplementedException();
            }
        }

        public List<Directory> Subdirectories()
        {
            HtmlDocument Document = new HtmlDocument();
            WebResponse resp = Session.GetHttpWebRequest("/Folder/process_folder.aspx?FolderID=" + _Id.ToString()).GetResponse();
            Document.Load(resp.GetResponseStream());
            resp.Close();
            var nodesWithHrefToDirectory = from node in Document.DocumentNode.DescendantNodes() where node.Name == "a" && node.GetAttributeValue("href", "").Contains("/Folder/process_folder.aspx") select node.GetAttributeValue("href", "");
            List<Directory> Subdirectories = new List<Directory>(nodesWithHrefToDirectory.Count());
            int i = 0;
            foreach (string relPath in nodesWithHrefToDirectory)
            {
                Uri uri = relPath.StartsWith("/") ? new Uri(Properties.Settings.Default.urlBase + relPath) : new Uri(relPath);
                Subdirectories[i++] = new Directory(Session, Parent, uint.Parse(HttpUtility.ParseQueryString(uri.Query).Get("FolderID")));
            }
            return Subdirectories;
        }
    }
}