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
            TcpListener pasvListener = null;
            NetworkStream ns = new NetworkStream(tc.Client);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            Session sess = new Session();
            string user = "", pass = "", usernameEncoded, usernameDecoded;
            sw.AutoFlush = true;
            string tmpS = "";
            uint CustomerId = 0;
            string wd = "/";
            bool binaryFlag = false;
            short Port = 0;

            sw.WriteLine("220 its ftpd");
            while (tc.Connected)
            {
                string command;
                try
                {
                    sw.Flush();
                    if ((sr == null) || (sw == null)) return;
                    command = sr.ReadLine().Trim();
                }
                catch (IOException) { return; }
                string cmd = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();
                if (cmd == "USER")
                {
                    try
                    {
                        usernameEncoded = command.Substring(cmd.IndexOf("USER ") + "USER ".Length + 1);
                        usernameDecoded = Base16.from16(usernameEncoded);
                        user = usernameDecoded.Split('@')[0];
                        CustomerId = uint.Parse(usernameDecoded.Split('@')[1]);
                        sw.WriteLine("331 Password required");
                    }
                    catch (Exception e) { sw.WriteLine("530 " + e.Message); }
                    continue;
                }
                if (cmd == "PASS")
                {
                    pass = command.Substring(cmd.IndexOf("PASS ") + "PASS ".Length + 1);
                    try
                    {
                        sess.Create();
                        Customer Customer = new Xporter.Customer();
                        Customer.fromId(CustomerId);
                        sess.setCustomer(Customer);
                        sess.Login(user, Base16.from16(pass));
                        if (sess.getLoginState()) sw.WriteLine("230 You may continue");
                        else sw.WriteLine("530 Denied");
                    }
                    catch (Exception e) { sw.WriteLine("530 " + e.Message); }
                    continue;
                }
                if (cmd == "PWD")
                {
                    sw.WriteLine("257 \"" + wd + "\"");
                    continue;
                }
                if (cmd == "CWD")
                {
                    wd = command.Substring("CWD ".Length + 1);
                    if (wd == "") wd = "/";
                    sw.WriteLine("250 " + wd);
                    continue;
                }
                if (cmd == "TYPE")
                {
                    tmpS = command.Substring("TYPE ".Length + 1);
                    if ((tmpS == "A") || (tmpS == "A N")) binaryFlag = false;
                    if ((tmpS == "I") || (tmpS == "L 8")) binaryFlag = true;
                    sw.WriteLine("200 " + binaryFlag.ToString());
                    continue;
                }
                if (cmd == "PASV")
                {
                    if (pasvListener != null)
                    {
                        try
                        {
                            pasvListener.Stop();
                        }
                        catch (SocketException) { }
                    }
                    pasvListener = null;
                    Random r = new Random();
                    while (pasvListener == null)
                    {
                        try
                        {
                            pasvListener = new TcpListener(IPAddress.Loopback, r.Next() & 65535);
                            pasvListener.Start();
                        }
                        catch (SocketException) { }
                    }
                    tmpS = pasvListener.LocalEndpoint.ToString();
                    Port = (short)ushort.Parse(tmpS.Substring(tmpS.IndexOf(':') + 1));
                    tmpS = tmpS.Replace(".", ",").Substring(0, tmpS.IndexOf(':'));
                    tmpS += ("," + (Port / 256).ToString());
                    tmpS += ("," + ((short)((Port << 8) / 256)).ToString());
                    sw.WriteLine("227 =" + tmpS);
                    new Thread(Connection).Start(pasvListener.AcceptTcpClient());
                    continue;
                }
                if (cmd == "SYST")
                {
                    sw.WriteLine("215 ITSL");
                    continue;
                }
                if (cmd == "EPSV")
                {
                    sw.WriteLine("502 NO IPv6");
                    continue;
                }
                if (command.Length >= (cmd.Length + 2)) Console.WriteLine("UNKNOWN \"" + cmd + "\" in \"" + command.Substring(cmd.Length + 2) + "\"");
                else Console.WriteLine("UNKNOWN \"" + cmd + "\"");
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