using System;
using System.Linq;
using System.Net;
using System.Timers;
using System.Xml.Linq;

namespace itsLib
{
    public class KeepAlive : Timer
    {
        public delegate void valueChangedEventHandler(object o, int e);

        private event valueChangedEventHandler OnlineUsersChange;

        private event valueChangedEventHandler MessengerStatusChange;

        private event valueChangedEventHandler UnreadMessagesChange;

        private event valueChangedEventHandler UnreadCloudEmailMessagesChange;

        Session _Session;

        public Session Session
        {
            get
            {
                return _Session;
            }
        }

        public KeepAlive(Session Session)
            : base(100)
        {
            this._Session = Session;
            this.Elapsed += Timer_Elapsed;
            this.AutoReset = true;
        }

        public void SendKeepAlive()
        {
            try
            {
                HttpWebRequest hwr = Session.GetHttpWebRequest("/XmlHttp/KeepAlive.asmx/OnlineInfo");
                hwr.ContentLength = 2;
                hwr.Method = "POST";
                hwr.GetRequestStream().Write(new byte[] { Convert.ToByte('{'), Convert.ToByte('}') }, 0, 2);
                HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                if (!resp.ResponseUri.ToString().Contains(Properties.Settings.Default.urlBase + "/XmlHttp/KeepAlive.asmx/OnlineInfo")) throw new Exception("KA FAIL");
                XElement xdoc = XElement.Load(resp.GetResponseStream());
                int newOnlineUsers = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "OnlineUsers" select xml.Value).First());
                int newUnreadMessages = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "UnreadMessages" select xml.Value).First());
                int newMessengerStatus = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "MessengerStatus" select xml.Value).First());
                int newUnreadCloudEmailMessages = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "UnreadCloudEmailMessages" select xml.Value).First());

                if (OnlineUsers != newOnlineUsers) if (OnlineUsersChange != null) OnlineUsersChange(this, newOnlineUsers);
                if (UnreadMessages != newUnreadMessages) if (UnreadMessagesChange != null) UnreadMessagesChange(this, newOnlineUsers);
                if (MessengerStatus != newOnlineUsers) if (MessengerStatusChange != null) MessengerStatusChange(this, newOnlineUsers);
                if (UnreadCloudEmailMessages != newOnlineUsers) if (UnreadCloudEmailMessagesChange != null) UnreadCloudEmailMessagesChange(this, newOnlineUsers);

                _OnlineUsers = newOnlineUsers;
                _UnreadMessages = newUnreadMessages;
                _MessengerStatus = newMessengerStatus;
                _UnreadCloudEmailMessages = newUnreadCloudEmailMessages;

                resp.Close();
            }
            catch (WebException) { }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Stop();
            this.SendKeepAlive();
            this.Start();
        }

        int _OnlineUsers, _UnreadMessages, _MessengerStatus, _UnreadCloudEmailMessages;

        public int OnlineUsers
        {
            get
            {
                return _OnlineUsers;
            }
        }

        public int UnreadMessages
        {
            get
            {
                return _UnreadMessages;
            }
        }

        public int MessengerStatus
        {
            set
            {
                if (value != MessengerStatus)
                {
                    try
                    {
                        HttpWebRequest hwr = Session.GetHttpWebRequest("/OnlineUsers.aspx?Status=" + value);
                        hwr.Referer = Properties.Settings.Default.urlBase + "/OnlineUsers.aspx?Status=" + ((value == 0) ? 1 : 0).ToString();
                        HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                        if (resp.ResponseUri.OriginalString != Properties.Settings.Default.urlBase + "/OnlineUsers.aspx?Status=" + value) throw new Exception("MessengerStatus error");
                        resp.Close();
                        SendKeepAlive();
                    }
                    catch (WebException) { Console.WriteLine("Retrying MessengerStatus"); MessengerStatus = value; }
                }
            }
            get
            {
                return _MessengerStatus;
            }
        }

        public int UnreadCloudEmailMessages
        {
            get
            {
                return _UnreadCloudEmailMessages;
            }
        }
    }
}