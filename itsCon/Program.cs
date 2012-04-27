using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;

using HtmlAgilityPack;
using itsLib;

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
            sess.Customer = new Customer(CustomerId);
            Console.WriteLine("Customer: " + sess.Customer.Name);
            Console.Write("Username: ");
            string Username = Console.ReadLine();
            Console.Write("Password: ");
            string Password = Console.ReadLine();
            sess.Login(Username, Password);
            User Me = itsLib.User.Me(sess);
            for (int i = -5; i <= 5; i++)
            {
                try
                {
                    itsLib.Messaging.MailBox MailBox = new itsLib.Messaging.MailBox(sess, i);
                    Console.WriteLine("Mailbox \"" + MailBox.Name + "\"");
                }
                catch (Exception) { Console.WriteLine("Error looking for MailBox with id=" + i.ToString()); }
            }

            while (true)
            {
                Console.ReadKey(true);
                if (sess.KeepAlive.MessengerStatus == 1) sess.KeepAlive.MessengerStatus = 0;
                else sess.KeepAlive.MessengerStatus = 1;
            }
        }
    }
}