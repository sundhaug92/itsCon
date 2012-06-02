using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Xporter;

namespace itslFtpCon
{
    internal class Program
    {
        private static void Connection(object o)
        {
            TcpClient tc = (TcpClient)o;
            TcpClient dataClient = null;
            TcpListener pasvListener = null;
            TcpClient pasvClient;
            NetworkStream ns = new NetworkStream(tc.Client);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            StreamReader dataStreamReader;
            StreamWriter dataStreamWriter = null;

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
                    Console.WriteLine(">" + command);
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
                if (cmd == "FEAT")
                {
                    sw.WriteLine("211 No features"); //Should probably use some other code
                    continue;
                }
                if (cmd == "PASV")
                {
                    try
                    {
                        sw.WriteLine("227 =127,0,0,1,0,20"); // Implement proper code for finding IP/Port of server
                    }
                    catch (Exception)
                    {
                    }
                    if (pasvListener == null)
                    {
                        pasvListener = new TcpListener(20);
                        pasvListener.Start();
                    }
                    dataClient = pasvListener.AcceptTcpClient();
                    continue;
                }
                if (cmd == "PORT")
                {
                    string uri = command.Substring("PORT".Length + 1);
                    string[] parts = uri.Split(',');
                    int port = int.Parse(parts[4]) * 256 + int.Parse(parts[5]);
                    string ip = "";
                    foreach (string part in parts)
                    {
                        ip += part + ".";
                    }
                    ip = ip.Substring(0, ip.LastIndexOf('.') - 1);
                    ip = ip.Substring(0, ip.LastIndexOf('.'));
                    ip = ip.Substring(0, ip.LastIndexOf('.'));
                    dataClient = new TcpClient(ip, port);
                    sw.WriteLine("200 Connected");
                    dataStreamReader = new StreamReader(new NetworkStream(dataClient.Client));
                    dataStreamWriter = new StreamWriter(new NetworkStream(dataClient.Client));

                    continue;
                }
                if (cmd == "NLST")
                {
                    if (wd == "/")
                    {
                        sw.WriteLine("150");
                        dataStreamWriter.WriteLine("eportfolio\r\ncourses\r\nprojects\r\n");
                        dataStreamWriter.Flush();
                        dataStreamWriter.Close();
                        dataClient.Close();
                        sw.WriteLine("226");
                    }
                    continue;
                }
                if (command.Length >= (cmd.Length + 2)) Console.WriteLine("UNKNOWN \"" + cmd + "\" in \"" + command.Substring(cmd.Length + 1) + "\"");
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