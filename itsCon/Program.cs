using System;

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
                        if (orders[1] == "set-active") new Project(sess, int.Parse(orders[2])).setActive();
                        if (orders[1] == "get-active")
                        {
                            if (sess.ActiveContext.StartsWith("P")) Console.WriteLine(sess.ActiveContext);
                            else Console.WriteLine("Active context is not a project");
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
    }
}