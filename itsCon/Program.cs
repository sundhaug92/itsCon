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
            Console.WriteLine("Welcome, " + Me.Name);
            while (true)
            {
                Console.Write(".");
                string order = Console.ReadLine();
                string[] orders = order.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                orders[0] = orders[0].ToLower();
                if ((orders[0] == "exit") || (orders[0] == "quit") || (orders[0] == "break")) break;
                if (orders[0] == "project")
                {
                    if (orders[1] == "set-active") ;
                    if (orders[1] == "get-active") ;
                }
            }
        }
    }
}