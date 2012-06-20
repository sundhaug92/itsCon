using System;
using System.Collections.Generic;
using itsLib;
using itsLib.Messaging;

namespace itsCon
{
    internal class Program
    {
        //Get password, without displaying it
        //Like Console.ReadLine, but using Console.ReadKey(true), instead of Console.ReadKey(false)
        private static string getPassword()
        {
            string s = "";  //Start of with a blank string
            int pos = 0;    //At position 0
            while (true)    //(Theoretically) forever
            {
                ConsoleKeyInfo cki = Console.ReadKey(true); //Get a key
                if (cki.Key == ConsoleKey.Backspace)        //backspace
                {
                    if (pos > 0)                            //And theres something to the left of our position
                    {
                        pos--;                              //Reduce the count of characters to our left
                        s = s.Substring(0, pos) + s.Substring(pos + 1); //Remove the character that used to be to our left
                    }
                    continue;
                }
                if (cki.Key == ConsoleKey.Enter)            //enter
                {
                    Console.WriteLine();                    //Print a new line (like Console.ReadLine() does)
                    return s;                               //Return what we've got
                }
                if (cki.Key == ConsoleKey.RightArrow)       //Right arrow key
                {
                    if (pos < (s.Length - 1)) pos++;        //If possible, move one step to the right
                    continue;
                }
                if (cki.Key == ConsoleKey.LeftArrow)        //Left arrow key
                {
                    if (pos > 0) pos--;                     //If possible, move one step to the left
                    continue;
                }

                //It's a non-control character (or an un-detected control-character)
                if (s.Length == pos)                        //if we are at the end of the string
                {
                    s += cki.KeyChar.ToString();            //Append the new character
                }
                else if ((s.Length != pos) && (pos == 0))   //if we are at the beginning of the string
                {
                    s = cki.KeyChar.ToString() + s;         //Add character to the beginnning of the string
                }
                else if ((s.Length != pos) && (pos != 0))   //if we're somewhere else in the string, insert string
                {
                    s = s.Substring(0, pos) + cki.KeyChar.ToString() + s.Substring(pos);
                }
                pos++;  //Record the fact that the number of characters to the left of the marker
            }
        }

        private static void Main(string[] args)
        {
            Session sess = new Session();  //Create a new Session object
            Console.WriteLine("ASP.NET_Id: [" + sess.Id + "]"); //Output the value of the ASP.NET_SessionId cookie
            Console.Write("CustomerId? ");          //Get the customer id from the user
            uint CustomerId = uint.Parse(Console.ReadLine());
            sess.Customer = new Customer(sess, CustomerId);

            Console.WriteLine("Customer: " + sess.Customer.Name); //Output the customer's name
            System.Diagnostics.Debug.Assert((new Customer(sess, sess.Customer.Name).Id == CustomerId) && (new Customer(sess, sess.Customer.Name).Name == sess.Customer.Name));  //Check that creating a customer from that name, gives the same customer as creating one with the CustomerId

            Console.Write("Username: ");    //Get the username from the user
            string Username = Console.ReadLine();

            Console.Write("Password: ");    //Get the password from the user
            string Password = getPassword();

            sess.Login(Username, Password); //Login

            Person Me = itsLib.Person.Me(sess);     //Who am I?
            Console.WriteLine("Welcome, " + Me.Name + "!");//Print my name

            while (true) //Until the user exits
            {
                try
                {
                    Console.Write("."); //Print prompt
                    string order = Console.ReadLine(); //Get command
                    string[] orders = order.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   //Split command into parts
                    orders[0] = orders[0].ToLower();//Make part one lowercase (to simplify the if's in our code)
                    if ((orders[0] == "exit") || (orders[0] == "quit") || (orders[0] == "break")) break;
                    if (orders[0] == "project")
                    {
                        if (orders[1] == "set-active") new Project(sess, uint.Parse(orders[2])).setActive();//Set the active context to the project with this id
                        if (orders[1] == "get-active") //Get project id of active context
                        {
                            if (sess.ActiveContext.StartsWith("P")) Console.WriteLine(sess.ActiveContext);
                            else Console.WriteLine("Active context is not a project");
                        }
                        if (orders[1] == "get-root")    //Get the root directory of the active context if it's a project
                        {
                            if (sess.ActiveContext.StartsWith("P"))
                            {
                                Console.WriteLine((new Project(sess, uint.Parse(sess.ActiveContext.Substring(1))).getRootDirectory()).Name);
                            }
                            else Console.WriteLine("Active context is not a project");
                        }
                        if (orders[1] == "active-toggle-archive") //send project in active context to archive
                        {
                            if (sess.ActiveContext.StartsWith("P")) new Project(sess, uint.Parse(sess.ActiveContext.Substring(1))).toArchive();
                            else Console.WriteLine("Active context is not a project");
                        }
                    }
                    if (orders[0] == "course")
                    {
                        if (orders[1] == "set-active") new Course(sess, int.Parse(orders[2])).setActive(); //Set active context course id
                        if (orders[1] == "get-active") //Get the active context course id
                        {
                            if (sess.ActiveContext.StartsWith("C")) Console.WriteLine(sess.ActiveContext);
                            else Console.WriteLine("Active context is not a course");
                        }
                        if (orders[1] == "get-root")        //Get the root directory of the course
                        {
                            if (sess.ActiveContext.StartsWith("C"))
                            {
                                new Course(sess, int.Parse(sess.ActiveContext.Substring(1))).getRootDirectory();
                            }
                            else Console.WriteLine("Active context is not a course");
                        }
                    }
                    if (orders[0] == "keepalive")
                    {
                        if (orders[1] == "get-messenger-status")        //Show as online
                        {
                            Console.WriteLine(sess.KeepAlive.MessengerStatus);
                        }
                        if (orders[1] == "set-messenger-status")        //Show as offline
                        {
                            sess.KeepAlive.MessengerStatus = uint.Parse(orders[2]);
                        }
                    }
                    if (orders[0] == "make-http-request") sess.GetHttpWebRequest(orders[1]).GetResponse().Close();  //Make a HTTP-GET request, with the session cookies
                    if (orders[0] == "session")
                    {
                        if (orders[1] == "logout") sess.Logout();           //Log out
                        if (orders[1] == "login") sess.Login(orders[2], orders[3]); //login <user> <password>
                    }
                    if (orders[0] == "mail")
                    {
                        if (orders[1] == "get-in-folder")       //Get mails in folder by id
                        {
                            itsLib.Messaging.MailBox mb = new itsLib.Messaging.MailBox(sess, int.Parse(orders[2]));
                            Console.WriteLine("Messages in folder \"" + mb.Name + "\"");    //Print folder name
                            List<Mail> mails = mb.GetMails();   //Get the mails in message folder
                            foreach (Mail Mail in mails)
                            {
                                Console.Write("From: " + Mail.From.ShortName + "\t\t");  //Output who sent the mail
                                foreach (Person Person in Mail.To)
                                {
                                    if (Person != null) Console.Write(Person.ShortName + "\t\t"); //And all recipients
                                }
                                Console.WriteLine(Mail.Subject);    //And lastly the subject
                            }
                        }
                        if (orders[1] == "create")
                        {
                            MailCreator NewMail = new MailCreator();
                            do
                            {
                                Console.Write("To: ");
                                order = Console.ReadLine().Trim();
                                if (order != "") NewMail.To.Add(new Person(sess, uint.Parse(order)));
                                else break;
                                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                                Console.WriteLine("To: " + NewMail.To[NewMail.To.Count - 1].Name);
                            } while (true);
                            do
                            {
                                Console.Write("Cc: ");
                                order = Console.ReadLine().Trim();
                                if (order != "") NewMail.Cc.Add(new Person(sess, uint.Parse(order)));
                                else break;
                                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                                Console.WriteLine("Cc: " + NewMail.Cc[NewMail.Cc.Count - 1].Name);
                            } while (true);
                            do
                            {
                                Console.Write("Bcc: ");
                                order = Console.ReadLine().Trim();
                                if (order != "") NewMail.Bcc.Add(new Person(sess, uint.Parse(order)));
                                else break;
                                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                                Console.WriteLine("Bcc " + NewMail.Bcc[NewMail.Bcc.Count - 1].Name);
                            } while (true);
                            Console.Write("Subject: ");
                            NewMail.Subject = Console.ReadLine();
                            do
                            {
                                Console.Write("Text: ");
                                order = Console.ReadLine();
                                if (order != "") NewMail.Text += order + '\n'.ToString();
                                else break;
                            } while (true);
                            NewMail.SendMessage(sess);
                        }
                        if (orders[0] == "bulletin")
                        {
                            if (orders[1] == "in-active")   //Get bulletin (by id) in the currently active context (project or course)
                            {
                                List<Bulletin> bulletins;
                                if (sess.ActiveContext.StartsWith("C")) bulletins = Bulletin.inCP(sess, new Course(sess, int.Parse(sess.ActiveContext.Substring(1))));
                                else bulletins = Bulletin.inCP(sess, new Project(sess, uint.Parse(sess.ActiveContext.Substring(1))));
                                foreach (Bulletin bulletin in bulletins)
                                {
                                    Console.WriteLine(bulletin.By.ShortName + ":" + bulletin.Title + ":" + bulletin.Text);
                                }
                            }
                        }
                        if (orders[0] == "find-person")
                        {
                            if (orders[1] == "any")         //List all persons
                            {
                                PersonSearch ps = PersonSearch.GetAll(sess);
                                foreach (Person Person in ps.Result)
                                {
                                    Console.WriteLine(Person.Name);
                                }
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
    }
}