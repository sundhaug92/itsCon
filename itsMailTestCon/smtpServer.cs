using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace itsMailTestCon
{
    class smtpServer
    {
        Thread worker_thread;
        void clientWorker(object o)
        {
            string HELO = "", MAIL_FROM = "", Subject="";
            List<string> RCPT = new List<string>();
            TcpClient tc = (TcpClient)o;
            NetworkStream stream = new NetworkStream(tc.Client);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            int Mode = 220;
            do
            {
                switch (Mode)
                {
                    case 220:
                        writer.WriteLine("220 " + Dns.GetHostName() + " (" + smtpSettings.Default.ServerName + ")");
                        Mode = 0;
                        break;
                    case 221:
                        writer.WriteLine("221 BYE");
                        writer.Flush();
                        writer.Close();
                        stream.Close();
                        tc.Close();
                        return;
                    case 250:
                        writer.WriteLine("250 Ok");
                        break;
                    default:
                        Console.Write("Unknown mode: " + Mode.ToString());
                        break;
                }
                Mode = 0;
                writer.Flush();
                string cmd = reader.ReadLine();
                while ((cmd[0] > 127) || (cmd[0] < 32)) cmd = cmd.Substring(1);
                if (cmd.StartsWith("HELO ")) Mode = 250;
                if (cmd.StartsWith("DATA"))
                {
                    writer.WriteLine("354 ");
                    bool HeaderMode = true;
                    do
                    {
                        cmd = reader.ReadLine();
                        while ((cmd[0] > 127) || (cmd[0] < 32)) cmd = cmd.Substring(1);
                        if ((HeaderMode) && (cmd == "")) HeaderMode = false;
                    } while (cmd != ".");
                }
                if (cmd.StartsWith("MAIL FROM:")) { MAIL_FROM = cmd.Split(':')[1]; Mode = 250; }
                if (cmd.StartsWith("RCPT TO:")) { RCPT.Add(cmd.Split(':')[1]); Mode = 250; }
                if (cmd.StartsWith("QUIT")) Mode = 221;
            } while (true);

        }
        void worker()
        {
            TcpListener tl = new TcpListener(25);
            tl.Start();
            while (true)
            {
                (new Thread(clientWorker)).Start(tl.AcceptTcpClient());
            }
        }
        public void Start()
        {
            Thread worker_thread = new Thread(worker);
            worker_thread.Start();
        }
        public void Stop()
        {
            worker_thread.Abort();
        }
    }
}
