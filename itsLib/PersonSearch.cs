using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;

namespace itsLib
{
    internal class PersonSearch
    {
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

            LoginFormData.Add("idProfileID_7", (((PersonType&PersonType.sysadmin)>0)?1:0).ToString());
            LoginFormData.Add("idProfileID_14", (((PersonType & PersonType.examinator) > 0) ? 1 : 0).ToString());
            LoginFormData.Add("idProfileID_8", (((PersonType & PersonType.administrator) > 0) ? 1 : 0).ToString());
            LoginFormData.Add("idProfileID_9", (((PersonType & PersonType.employee) > 0) ? 1 : 0).ToString());
            LoginFormData.Add("idProfileID_10", (((PersonType & PersonType.student) > 0) ? 1 : 0).ToString());
            LoginFormData.Add("idProfileID_62007", (((PersonType & PersonType.parent) > 0) ? 1 : 0).ToString());
            LoginFormData.Add("idProfileID_11", (((PersonType & PersonType.guest) > 0) ? 1 : 0).ToString());

            LoginFormData.Add("Search", "Søk");
            LoginFormData.Add("Advanced", "0");

            foreach (var Form in initialLoginScreen.DocumentNode.Descendants("form"))
            {
                if (Form.GetAttributeValue("id", "") == "Form")
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
                    string LoginUrl = Properties.Settings.Default.urlBase+"/search/search_person.aspx";
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
                    loginResp.Close();
                }
        }
            }
        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId, Course Course)
            :this(Session, Forname, Surname, HierarchyId, Course, PersonType.administrator| PersonType.employee | PersonType.examinator | PersonType.guest| PersonType.parent| PersonType.student | PersonType.sysadmin)
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname, int HierarchyId)
            : this(Session, Forname, Surname, HierarchyId, new Course(Session,-1))
        {
        }

        public PersonSearch(Session Session, string Forname, string Surname)
            : this(Session, Forname, Surname, -1)
        {
        }

        public static PersonSearch GetAll(Session Session)
        {
            return new PersonSearch(Session, "", " ");
        }
    }
}