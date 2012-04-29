using System;

using itsLib;
using itsLib.Messaging;

namespace itsCon
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Session sess = new Session();
            Console.WriteLine("ASP.NET_Id: [" + sess.Id + "]");
            Console.Write("CustomerId? ");
            uint CustomerId = uint.Parse(Console.ReadLine());
            sess.Customer = new Customer(sess, CustomerId);
            Console.WriteLine("Customer: " + sess.Customer.Name);
            System.Diagnostics.Debug.Assert((new Customer(sess, sess.Customer.Name).Id == CustomerId) && (new Customer(sess, sess.Customer.Name).Name == sess.Customer.Name));
            Console.Write("Username: ");
            string Username = Console.ReadLine();
            Console.Write("Password: ");
            string Password = Console.ReadLine();
            sess.Login(Username, Password);
            User Me = itsLib.User.Me(sess);
            Console.WriteLine("Welcome, " + Me.Name);
            while (true)
            {
                try
                {
                    Console.Write(".");
                    string order = Console.ReadLine();
                    string[] orders = order.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    orders[0] = orders[0].ToLower();
                    if ((orders[0] == "exit") || (orders[0] == "quit") || (orders[0] == "break")) break;
                    if (orders[0] == "project")
                    {
                        if (orders[1] == "set-active") new Project(sess, uint.Parse(orders[2])).setActive();
                        if (orders[1] == "get-active")
                        {
                            if (sess.ActiveContext.StartsWith("P")) Console.WriteLine(sess.ActiveContext);
                            else Console.WriteLine("Active context is not a project");
                        }
                    }
                    if (orders[0] == "course")
                    {
                        if (orders[1] == "set-active") new Course(sess, uint.Parse(orders[2])).setActive();
                        if (orders[1] == "get-active")
                        {
                            if (sess.ActiveContext.StartsWith("C")) Console.WriteLine(sess.ActiveContext);
                            else Console.WriteLine("Active context is not a course");
                        }
                    }
                    if (orders[0] == "keepalive")
                    {
                        if (orders[1] == "get-messenger-status")
                        {
                            Console.WriteLine(sess.KeepAlive.MessengerStatus);
                        }
                        if (orders[1] == "set-messenger-status")
                        {
                            sess.KeepAlive.MessengerStatus = uint.Parse(orders[2]);
                        }
                    }
                    if (orders[0] == "make-http-request") sess.GetHttpWebRequest(orders[1]).GetResponse().Close();
                    if (orders[0] == "session")
                    {
                        if (orders[1] == "logout") sess.Logout();
                        if (orders[1] == "login") sess.Login(orders[2], orders[3]);
                    }
                    if (orders[0] == "mail")
                    {
                        if (orders[1] == "get-in-folder")
                        {
                            itsLib.Messaging.MailBox mb = new itsLib.Messaging.MailBox(sess, int.Parse(orders[2]));
                            Console.WriteLine("Messages in folder \"" + mb.Name + "\"");
                            Mail[] mails = mb.GetMails();
                            foreach (Mail Mail in mails)
                            {
                                Console.Write("From: " + Mail.From.Name + "\t\t");
                                foreach (User Person in Mail.To)
                                {
                                    Console.Write(Person.Name + "\t\t");
                                }
                                Console.WriteLine(Mail.Subject);
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
    }
}