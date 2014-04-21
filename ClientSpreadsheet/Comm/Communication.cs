using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientModel;
using CustomNetworking;
using System.Net.Sockets;

namespace Communication
{
    class Communication
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter ipAdress");
            //String ipAdress = Console.ReadLine();
            String ipAdress = "155.99.162.130";

            Console.WriteLine("Enter Username");
            //String userName = Console.ReadLine();
            String userName = "Chuy";

            Console.WriteLine("Enter Password");
            //String password = Console.ReadLine();
            String password = "sfault";

            int port = 12345;

            Client client = new Client();

            client.IncomingLineEvent += MessageReceived;

            //Variable for client's message.
            String message = "";

            //The ip address, port 2000, and the username and password.
            if (!client.Connect(ipAdress, port, password))
            {
                Console.WriteLine("Please enter a valid IP address.");
            }
            else
            {
                Console.WriteLine("Enter Message: ");
                while (true)
                {
                    message = Console.ReadLine();
                    if (message == "!stop")
                    {
                        client.SendMessage("!stop");
                        client.CloseSocket();
                        break;
                    }

                    if (message != "")
                    {
                        //Send the message
                        client.SendMessage( userName + ": " + message);
                    }
                }
            }
        }

        private static void MessageReceived(string line, Exception e)
        {
            if (ReferenceEquals(line, null) || line == "")
            {
                if (e is SocketException)
                {
                    Console.WriteLine("Server has closed.");
                    return;
                }
                return;
            }
            Console.WriteLine(line);
        }
    }
}
