﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace itsLib.Messaging
{
    public class Mail
    {
        private string _Path = "";
        private Session _Session;
        private HtmlDocument Document = new HtmlDocument();

        public Mail(Session Session, string Path)
        {
            _Path = Path;
            _Session = Session;
            Document = Session.GetDocument(Path);
        }

        public Mail(Session Session, uint Id, int FolderId)
            : this(Session, "/Messages/View_Message.aspx?MessageFolderID=" + FolderId.ToString() + "&MessageID=" + Id.ToString() + "&ShowAll=1")
        {
        }

        public string Contents
        {
            get
            {
                var contents = from node in Document.DocumentNode.DescendantNodes() where node.Name == "div" && node.GetAttributeValue("class", "") == "userinput" select node;
                return contents.First().InnerHtml;
            }
        }

        public Person From
        {
            get
            {
                var nodesWithJSOnclick = from node in Document.DocumentNode.DescendantNodes() where node.GetAttributeValue("onclick", "").StartsWith("javascript:") select node;
                var nodesWithJSOnclickToPersons = from node in nodesWithJSOnclick where node.GetAttributeValue("onclick", "").StartsWith("javascript:window.open('/Person/show_person.aspx") select node;
                return new Person(_Session, uint.Parse(nodesWithJSOnclickToPersons.First().GetAttributeValue("onclick", "").Substring("javascript:window.open('/Person/show_person.aspx?".Length).Split(new char[] { '=', '&' })[1]));
            }
        }

        public string Subject
        {
            get
            {
                var titles = from node in Document.DocumentNode.DescendantNodes() where node.Name == "span" && node.GetAttributeValue("id", "") == "ctl05_TT" select node;
                return HttpUtility.HtmlDecode(titles.First().InnerText);
            }
        }

        public List<Person> To
        {
            get
            {
                var Description = (from node in Document.DocumentNode.DescendantNodes() where node.Name == "table" && node.GetAttributeValue("class", "") == "description" select node).First();
                var recipientList = (from node in Description.DescendantNodes() where node.Name == "td" select node.InnerText).ToArray()[1];
                string[] _Names = recipientList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), Names = new string[_Names.Length];
                int i = 0;
                foreach (string s in _Names) { Names[i++] = s.Trim(); }
                List<Person> r = new List<Person>(Names.Length);
                ConcurrentBag<Person> _r = new ConcurrentBag<Person>();
                Parallel.ForEach(Names, (s) =>
                {
                    PersonSearch PS = new PersonSearch(_Session, s.Substring(0, s.LastIndexOf(' ')), s.Substring(s.LastIndexOf(' ') + 1));
                    if (PS.Result.Count() == 0) _r.Add(Person.Nobody(_Session));
                    else _r.Add(PS.Result[0]);
                });
                foreach (Person p in _r)
                {
                    r.Add(p);
                }
                return r;
            }
        }
    }
}