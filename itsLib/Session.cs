using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace itsLib
{
    public class Session : IDisposable
    {
        string _Id = "";
        public CookieContainer Cookies = new CookieContainer();
        KeepAlive _KeepAlive;
        string _UserAgent = Properties.Settings.Default.UA_String;

        public string UserAgent
        {
            get
            {
                return _UserAgent;
            }
            set
            {
                _UserAgent = value;
            }
        }

        public KeepAlive KeepAlive
        {
            get { return _KeepAlive; }
        }

        public string Id
        {
            get
            {
                return _Id;
            }
        }

        public Session()
        {
            MakeHttpGetRequestGetCookies("/XmlHttp/SessionLessApi.aspx");
        }

        private void MakeHttpGetRequestGetCookies(string p)
        {
            HttpWebResponse resp = (HttpWebResponse)GetHttpWebRequest(p).GetResponse();
            _Id = resp.Cookies["ASP.NET_SessionId"].Value;
            resp.Close();
        }

        string _ActiveContext = "";

        public string ActiveContext
        {
            get
            {
                return _ActiveContext;
            }
        }

        public HttpWebRequest GetHttpWebRequest(string p)
        {
            if (!(p.Contains("XmlHttp") || (p == "/")))
            {
                Debug.WriteLine("Navigating to " + p);
                NameValueCollection NVC = (p.IndexOf("?") >= 0) ? HttpUtility.ParseQueryString(p.Substring(p.IndexOf("?"))) : HttpUtility.ParseQueryString(p);
                if (NVC != null)
                {
                    if (NVC.Count != 0)
                    {
                        if (NVC["CourseID"] != null) _ActiveContext = "C" + NVC["CourseID"];
                        if (NVC["ProjectID"] != null) _ActiveContext = "P" + NVC["ProjectID"];
                    }
                }
            }

            Uri uri = new Uri(Properties.Settings.Default.urlBase + p);
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
            hwr.UserAgent = UserAgent;

            hwr.Timeout = 60 * 1000;
            hwr.ContinueTimeout = 60 * 1000;
            hwr.CookieContainer = Cookies;
            return hwr;
        }

        public Customer Customer;

        public Person Me
        {
            get
            {
                return Person.Me(this);
            }
        }

        public void Login(string Username, string Password)
        {
            HttpWebRequest InitialLoginRequest = (HttpWebRequest)HttpWebRequest.Create(Properties.Settings.Default.urlBase + "/");
            Cookies.Add(new Cookie("login", "CustomerId=" + Customer.Id + "&LanguageId=0&ssl=True", "/", Properties.Settings.Default.urlBase.Substring("https://".Length)));
            InitialLoginRequest.CookieContainer = Cookies;
            InitialLoginRequest.UserAgent = UserAgent;
            HtmlDocument initialLoginScreen = new HtmlDocument();
            HttpWebResponse FirstResponse = (HttpWebResponse)InitialLoginRequest.GetResponse();
            initialLoginScreen.Load(FirstResponse.GetResponseStream());

            Dictionary<string, string> LoginFormData = new Dictionary<string, string>();

            LoginFormData.Add("ctl03$Login$site$input", Customer.Id.ToString());
            LoginFormData.Add("ctl03$Login$username$input", Username);
            LoginFormData.Add("ctl03$Login$password$input", Password);

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
                    string LoginUrl = Properties.Settings.Default.urlBase;
                    LoginUrl += Form.GetAttributeValue("action", "")[0] != '/' ? "/" + Form.GetAttributeValue("action", "") : Form.GetAttributeValue("action", "");
                    HttpWebRequest secondRequest = (HttpWebRequest)HttpWebRequest.Create(LoginUrl);
                    secondRequest.CookieContainer = Cookies;
                    secondRequest.UserAgent = UserAgent;
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

                    if ((loginResp.ResponseUri.PathAndQuery.Contains("StartUrl")))
                    {
                        _LoggedIn = true;
                        _KeepAlive = new KeepAlive(this);
                        _KeepAlive.Start();
                        return;
                    }
                    loginResp.Close();
                }
                throw new Exception("Login failed");
            }
        }

        bool _LoggedIn = false;

        public bool LoggedIn
        {
            get
            {
                return _LoggedIn;
            }
        }

        public void Logout()
        {
            GetHttpWebRequest("/log_out.aspx").GetResponse().Close();
            _LoggedIn = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _KeepAlive.Dispose();
            }
        }

        ~Session()
        {
            Dispose(false);
        }
    }
}