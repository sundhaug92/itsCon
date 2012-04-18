﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace itsLib
{
    public class Session
    {
        string _Id = "";
        public CookieContainer Cookies = new CookieContainer();
        KeepAlive _KeepAlive;
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
            MakeHttpGetRequestGetCookies("/");
        }

        private void MakeHttpGetRequestGetCookies(string p)
        {
            HttpWebResponse resp = (HttpWebResponse)GetHttpWebRequest(p).GetResponse();
            _Id = resp.Cookies["ASP.NET_SessionId"].Value;
            resp.Close();
        }
        public HttpWebRequest GetHttpWebRequest(string p)
        {
            Uri uri = new Uri(Properties.Settings.Default.Domain + p);
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
            hwr.UserAgent = Properties.Settings.Default.UA_String;

            hwr.Timeout = 60 * 1000;
            hwr.ContinueTimeout = 60 * 1000;
            hwr.CookieContainer = Cookies;
            return hwr;
        }
        public Customer Customer;
        public User Me
        {
            get
            {
                return User.Me(this);
            }
        }

        public void Login(string Username, string Password)
        {
            HttpWebRequest InitialLoginRequest = (HttpWebRequest)HttpWebRequest.Create(Properties.Settings.Default.Domain + "/");
            Cookies.Add(new Cookie("login", "CustomerId=" + Customer.Id + "&LanguageId=0&ssl=True", "/", Properties.Settings.Default.Domain.Substring("https://".Length)));
            InitialLoginRequest.CookieContainer = Cookies;
            InitialLoginRequest.UserAgent = Properties.Settings.Default.UA_String;
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
                    //Console.WriteLine("<form method=\"" + Form.GetAttributeValue("method", "") + "\" action=\"" + Form.GetAttributeValue("action", "") + "\" id=\"" + Form.GetAttributeValue("id", "")+"\">");
                    
                    foreach (var inp in initialLoginScreen.DocumentNode.Descendants("input"))
                    {
                        if (!LoginFormData.ContainsKey(inp.GetAttributeValue("name", ""))) LoginFormData.Add(inp.GetAttributeValue("name", ""), inp.GetAttributeValue("value", ""));
                    }
                    foreach (var inp in LoginFormData)
                    {
                        NewLoginFormData.Add(inp.Key, WebUtility.UrlEncode(inp.Value));
                        //Console.WriteLine(inp.Key + "=" + WebUtility.UrlEncode(inp.Value));
                    }
                    LoginFormData = NewLoginFormData;
                    string LoginUrl = Properties.Settings.Default.Domain;
                    LoginUrl += Form.GetAttributeValue("action", "")[0] != '/' ? "/" + Form.GetAttributeValue("action", "") : Form.GetAttributeValue("action", "");
                    HttpWebRequest secondRequest = (HttpWebRequest)HttpWebRequest.Create(LoginUrl);
                    secondRequest.CookieContainer = Cookies;
                    secondRequest.UserAgent = Properties.Settings.Default.UA_String;
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


        // System.Timers.Timer KeepAliveTimer;
        bool _LoggedIn = false, _AutoKeepAlive = false;

        public bool LoggedIn
        {
            get
            {
                return _LoggedIn;
            }
        }
    }
}