using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Xml.Linq;

namespace itsLib
{
    public class KeepAlive:Timer
    {
        public delegate void valueChangedEventHandler(object o, int e);

        event valueChangedEventHandler OnlineUsersChange;
        event valueChangedEventHandler MessengerStatusChange;
        event valueChangedEventHandler UnreadMessagesChange;
        event valueChangedEventHandler UnreadCloudEmailMessagesChange;

        Session _Parent;
        public Session Parent{
            get
            {
                return _Parent;
            }
        }

        public KeepAlive(Session Parent)
            : base(100)
        {
            this._Parent = Parent;
            this.Elapsed += Timer_Elapsed;
            this.AutoReset = true;
        }
        public void SendKeepAlive()
        {
            try
            {
                HttpWebRequest hwr = Parent.GetHttpWebRequest("/XmlHttp/KeepAlive.asmx/OnlineInfo");
                //hwr.Accept = "application/json, text/javascript, */*; q=0.01";
                hwr.ContentLength = 2;
                //hwr.ContentType = "application/json; charset=utf-8";
                hwr.Method = "POST";
                hwr.GetRequestStream().Write(new byte[] { Convert.ToByte('{'), Convert.ToByte('}') }, 0, 2);
                //Console.WriteLine("SendKeepAlive Waiting");
                HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                //Console.WriteLine("SKA Done");
                if (!resp.ResponseUri.ToString().Contains(Properties.Settings.Default.urlBase + "/XmlHttp/KeepAlive.asmx/OnlineInfo")) throw new Exception("KA FAIL");
                XElement xdoc = XElement.Load(resp.GetResponseStream());
                int newOnlineUsers = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "OnlineUsers" select xml.Value).First());
                int newUnreadMessages = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "UnreadMessages" select xml.Value).First());
                int newMessengerStatus = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "MessengerStatus" select xml.Value).First());
                int newUnreadCloudEmailMessages = int.Parse((from xml in xdoc.Descendants() where xml.Name.LocalName == "UnreadCloudEmailMessages" select xml.Value).First());

                if (OnlineUsers != newOnlineUsers)if(OnlineUsersChange != null) OnlineUsersChange(this, newOnlineUsers);
                if (UnreadMessages != newUnreadMessages) if (UnreadMessagesChange != null) UnreadMessagesChange(this, newOnlineUsers);
                if (MessengerStatus != newOnlineUsers) if (MessengerStatusChange != null) MessengerStatusChange(this, newOnlineUsers);
                if (UnreadCloudEmailMessages != newOnlineUsers) if (UnreadCloudEmailMessagesChange != null) UnreadCloudEmailMessagesChange(this, newOnlineUsers);

                _OnlineUsers = newOnlineUsers;
                _UnreadMessages = newUnreadMessages;
                _MessengerStatus = newMessengerStatus;
                _UnreadCloudEmailMessages = newUnreadCloudEmailMessages;

                resp.Close();
                //Console.WriteLine("OnlineUsers:" + OnlineUsers.ToString() + " UnreadMessages:" + UnreadMessages.ToString() + " MessengerStatus:" + MessengerStatus.ToString() + " UnreadCloudEmailMessages:" + UnreadCloudEmailMessages.ToString());
            }
            catch (WebException) { }
        }
        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Stop();
            //Console.WriteLine("KeepAliveTimer Elapsed");
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
                        HttpWebRequest hwr = Parent.GetHttpWebRequest("/OnlineUsers.aspx?Status=" + value);
                        hwr.Referer = Properties.Settings.Default.urlBase + "/OnlineUsers.aspx?Status=" + ((value == 0) ? 1 : 0).ToString();
                        //Console.WriteLine("MessengerStatus (MS) waiting for response");
                        HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                        //Console.WriteLine("MS Done");
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
