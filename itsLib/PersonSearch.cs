using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class PersonSearch
    {
        private List<Person> _Result;

        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId, Course Course, PersonType PersonType)
        {
            HttpWebRequest InitialLoginRequest = Session.GetHttpWebRequest("/search/search_person.aspx");
            HtmlDocument initialLoginScreen = new HtmlDocument();
            HttpWebResponse FirstResponse = (HttpWebResponse)InitialLoginRequest.GetResponse();
            initialLoginScreen.Load(FirstResponse.GetResponseStream());

            Dictionary<string, string> LoginFormData = new Dictionary<string, string>();

            LoginFormData.Add("FirstName", Forname);
            LoginFormData.Add("Lastname", Surname);
            LoginFormData.Add("CourseID", Course.Id.ToString());
            LoginFormData.Add("HierarchyId", HierarchyId.ToString());

            LoginFormData.Add("idProfileID_7", (((PersonType & PersonType.sysadmin) > 0) ? 7 : 0).ToString());
            LoginFormData.Add("idProfileID_14", (((PersonType & PersonType.examinator) > 0) ? 14 : 0).ToString());
            LoginFormData.Add("idProfileID_8", (((PersonType & PersonType.administrator) > 0) ? 8 : 0).ToString());
            LoginFormData.Add("idProfileID_9", (((PersonType & PersonType.employee) > 0) ? 9 : 0).ToString());
            LoginFormData.Add("idProfileID_10", (((PersonType & PersonType.student) > 0) ? 10 : 0).ToString());
            LoginFormData.Add("idProfileID_62007", (((PersonType & PersonType.parent) > 0) ? 62007 : 0).ToString());
            LoginFormData.Add("idProfileID_11", (((PersonType & PersonType.guest) > 0) ? 11 : 0).ToString());

            LoginFormData.Add("Search", "Søk");
            LoginFormData.Add("Advanced", "0");

            foreach (var Form in initialLoginScreen.DocumentNode.Descendants("form"))
            {
                if (Form.GetAttributeValue("name", "") == "form")
                {
                    Dictionary<string, string> NewLoginFormData = new Dictionary<string, string>();

                    foreach (var inp in initialLoginScreen.DocumentNode.Descendants("input"))
                    {
                        if (!LoginFormData.ContainsKey(inp.GetAttributeValue("name", ""))) LoginFormData.Add(inp.GetAttributeValue("name", ""), inp.GetAttributeValue("value", ""));
                    }
                    foreach (var inp in LoginFormData)
                    {
                        NewLoginFormData.Add(inp.Key, WebUtility.UrlEncode(inp.Value));
                    }
                    LoginFormData = NewLoginFormData;
                    string LoginUrl = Properties.Settings.Default.urlBase + "/search/search_person.aspx";
                    LoginUrl += Form.GetAttributeValue("action", "")[0] != '/' ? "/" + Form.GetAttributeValue("action", "") : Form.GetAttributeValue("action", "");
                    HttpWebRequest secondRequest = Session.GetHttpWebRequest("/search/search_person.aspx");
                    secondRequest.Method = "POST";
                    secondRequest.ContentType = "application/x-www-form-urlencoded";
                    string data = "";
                    foreach (var inp in LoginFormData)
                    {
                        data += inp.Key + "=" + inp.Value + "&";
                    }
                    if (data[data.Length - 1] == '&') data.Substring(0, data.Length - 1);
                    secondRequest.ContentLength = System.Text.ASCIIEncoding.ASCII.GetByteCount(data);
                    secondRequest.GetRequestStream().Write(System.Text.ASCIIEncoding.ASCII.GetBytes(data), 0, (int)secondRequest.ContentLength);
                    HttpWebResponse loginResp = (HttpWebResponse)secondRequest.GetResponse();

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(loginResp.GetResponseStream());
                    loginResp.Close();
                    _Result = new List<Person>(10);
                    foreach (var row in (from element in doc.DocumentNode.DescendantNodes() where element.GetAttributeValue("id", "").StartsWith("row_") select element))
                    {
                        string href = row.ChildNodes[1].FirstChild.GetAttributeValue("href", "");
                        _Result.Add(new Person(Session, uint.Parse(HttpUtility.ParseQueryString(new Uri(Properties.Settings.Default.urlBase + href.Substring(href.IndexOf('/'))).Query).Get("PersonID"))));
                    }
                }
            }
        }

        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId, Course Course)
            : this(Session, Forname, Surname, HierarchyId, Course, PersonType.administrator | PersonType.employee | PersonType.examinator | PersonType.guest | PersonType.parent | PersonType.student | PersonType.sysadmin)
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId)
            : this(Session, Forname, Surname, HierarchyId, new Course(Session, -1))
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname)
            : this(Session, Forname, Surname, -1)
        {
        }

        public Person[] Result
        {
            get
            {
                return _Result.ToArray();
            }
        }

        public static PersonSearch GetAll(Session Session)
        {
            return new PersonSearch(Session, "", " ");
        }
    }
}