using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xporter;

namespace itslFtpCon
{
    internal class Program
    {
        private static void Connection(object o)
        {
            TcpClient tc = (TcpClient)o;
            NetworkStream ns = new NetworkStream(tc.Client);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            Session sess = new Session();
            sess.Create();
            string user = "", pass = "";
            uint CustomerId;

            sw.WriteLine("220 its ftpd");
            while (tc.Connected)
            {
                sw.Flush();
                string command = sr.ReadLine().Trim();
                string cmd = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();
                if (cmd == "USER")
                {
                    user = command.Substring(cmd.IndexOf("USER ") + "USER ".Length).Split('@')[0];
                    CustomerId = uint.Parse(command.Substring(cmd.IndexOf("USER ") + "USER ".Length).Split('@')[1]);
                    Customer Customer = new Xporter.Customer();
                    Customer.fromId(CustomerId);
                    sess.setCustomer(Customer);
                    sw.WriteLine("331 Password required");
                }
                if (cmd == "PASS")
                {
                    pass = command.Substring(cmd.IndexOf("PASS ") + "PASS ".Length);
                    sess.Login(user, pass);
                    if (sess.getLoginState()) sw.WriteLine("230 You may continue");
                    else sw.WriteLine("530 Denied");
                }
            }
        }

        private static void Main(string[] args)
        {
            TcpListener tl = new TcpListener(21);
            tl.Start();
            while (true)
            {
                new Thread(Connection).Start(tl.AcceptTcpClient());
            }
        }
    }
}