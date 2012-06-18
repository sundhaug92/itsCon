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
        public CookieContainer Cookies = new CookieContainer();
        public Customer Customer;
        private string _ActiveContext = "";
        private string _Id = "";
        private KeepAlive _KeepAlive;
        private bool _LoggedIn = false;
        private string _UserAgent = Properties.Settings.Default.UA_String;
        private string _UserName;

        public Session()
        {
            MakeHttpGetRequestGetCookies("/XmlHttp/SessionLessApi.aspx");
        }

        ~Session()
        {
            Dispose(false);
        }

        public string ActiveContext
        {
            get
            {
                return _ActiveContext;
            }
        }

        public string Id
        {
            get
            {
                return _Id;
            }
        }

        public KeepAlive KeepAlive
        {
            get { return _KeepAlive; }
        }

        public bool LoggedIn
        {
            get
            {
                return _LoggedIn;
            }
        }

        public Person Me
        {
            get
            {
                return Person.Me(this);
            }
        }

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

        public string UserName
        {
            get
            {
                return _UserName;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Get/set the UserAgent
        public HtmlDocument GetDocument(string p)//Get HtmlDocument at path p
        {
            HtmlDocument Document = new HtmlDocument();
            HttpWebResponse hwr = (HttpWebResponse)GetHttpWebRequest(p).GetResponse();
            Document.Load(hwr.GetResponseStream());
            hwr.Close();
            return Document;
        }

        //Get the
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

        public void Login(string Username, string Password) //Attempt log in
        {
            HttpWebRequest InitialLoginRequest = GetHttpWebRequest("/");
            Cookies.Add(new Cookie("login", "CustomerId=" + Customer.Id + "&LanguageId=0&ssl=True", "/", Properties.Settings.Default.urlBase.Substring("https://".Length))); //Create login-cookie
            HtmlDocument initialLoginScreen = new HtmlDocument();
            HttpWebResponse FirstResponse = (HttpWebResponse)InitialLoginRequest.GetResponse();
            initialLoginScreen.Load(FirstResponse.GetResponseStream());
            FirstResponse.Close();

            Dictionary<string, string> LoginFormData = new Dictionary<string, string>();

            LoginFormData.Add("ctl03$Login$site$input", Customer.Id.ToString());
            LoginFormData.Add("ctl03$Login$username$input", Username);
            LoginFormData.Add("ctl03$Login$password$input", Password);

            foreach (var Form in initialLoginScreen.DocumentNode.Descendants("form"))
            {
                if (Form.GetAttributeValue("id", "") == "Form")
                {
                    string LoginUrl = "";
                    LoginUrl += Form.GetAttributeValue("action", "")[0] != '/' ? "/" + Form.GetAttributeValue("action", "") : Form.GetAttributeValue("action", "");
                    foreach (HtmlNode Input in initialLoginScreen.DocumentNode.Descendants("input"))
                    {
                        if (!LoginFormData.ContainsKey(Input.GetAttributeValue("name", "")))
                            LoginFormData.Add(Input.GetAttributeValue("name", ""), Input.GetAttributeValue("value", ""));
                    }
                    PostData(LoginUrl, LoginFormData);

                    _LoggedIn = true;
                    _KeepAlive = new KeepAlive(this);
                    _KeepAlive.Start();
                    _UserName = Username;
                    return;
                }
                throw new Exception("Login failed");
            }
        }

        //Create a HttpRequest to the path p
        public void Logout()  //Log out
        {
            GetHttpWebRequest("/log_out.aspx").GetResponse().Close();
            _LoggedIn = false;
        }

        public HtmlDocument PostData(string Path, Dictionary<string, string> Data) //Post dictionary to server
        {
            Dictionary<string, string> _Data = new Dictionary<string, string>();
            foreach (var inp in Data)
            {
                if (!_Data.ContainsKey(inp.Key)) _Data.Add(inp.Key, inp.Value);
            }
            string data = "";
            foreach (var inp in _Data)
            {
                data += inp.Key + "=" + WebUtility.UrlEncode(inp.Value) + "&";
            }
            return PostDocument(Path, data, "application/x-www-form-urlencoded");
        }

        public HtmlDocument PostDocument(string Path, string Content, string ContentType) //Post content to server
        {
            HttpWebRequest secondRequest = GetHttpWebRequest(Path);
            secondRequest.Method = "POST";
            secondRequest.ContentType = ContentType;
            secondRequest.ContentLength = System.Text.ASCIIEncoding.ASCII.GetByteCount(Content);
            secondRequest.GetRequestStream().Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Content), 0, (int)secondRequest.ContentLength);
            HttpWebResponse loginResp = (HttpWebResponse)secondRequest.GetResponse();
            HtmlDocument Document = new HtmlDocument();
            Document.Load(loginResp.GetResponseStream());
            loginResp.Close();
            return Document;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _KeepAlive.Dispose();
            }
        }

        private void MakeHttpGetRequestGetCookies(string p)//Get cookies set by the server when a get request to the path p
        {
            HttpWebResponse resp = (HttpWebResponse)GetHttpWebRequest(p).GetResponse();
            _Id = resp.Cookies["ASP.NET_SessionId"].Value;
            resp.Close();
        }
    }
}