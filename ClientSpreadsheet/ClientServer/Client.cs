using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using CustomNetworking;

namespace ClientModel
{
    public class Client
    {

        // The socket used to communicate with the server.  If no connection has been
        // made yet, this is null.
        private StringSocket socket;

        // Register for this event to be motified when a line of text arrives.
        public event Action<String, Exception> IncomingLineEvent;

        public delegate void MessageReceived(String s, Exception e);

        private bool receiving = true;

        /// <summary>
        /// Creates a not yet connected client model.
        /// </summary>
        public Client()
        {
            socket = null;
        }

        /// <summary>
        /// Connect to the server at the given hostname and port and with the given name of username and
        /// password.
        /// </summary>
        public bool Connect(String hostname, int port, String password)
        {
            try
            {
                TcpClient client;
                //Case 1: Using IP
                IPAddress address;
                if (IPAddress.TryParse(hostname, out address))
                {
                    client = new TcpClient();
                    client.Connect(address, port);
                }
                else
                {
                    //Case 2: Using DNS
                    client = new TcpClient(hostname, port);
                }

                socket = new StringSocket(client.Client, UTF8Encoding.Default);
                //PASSWORD[esc]password\n 
                char esc = (char)27;
                socket.BeginSend("PASSWORD" + esc + password + "\n", (e, p) => { }, null);
                socket.BeginReceive(LineReceived, null);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send a line of text to the server.
        /// </summary>
        /// <param name="line"></param>
        public void SendMessage(String line)
        {
            if (socket != null)
            {
                socket.BeginSend(line + "\n", (e, p) => { }, null);
            }
        }

        /// <summary>
        /// Deal with an arriving line of text.
        /// </summary>
        private void LineReceived(String s, Exception e, object p)
        {
            if (!receiving)
                return;

            if (IncomingLineEvent != null)
            {
                IncomingLineEvent(s, e);
            }

            // Check if the socket has been closed, stopping the receiving if it has
            if (!ReferenceEquals(s, null) && !(e is SocketException))
                socket.BeginReceive(LineReceived, null);
        }

        /// <summary>
        /// Closes the socket when finished with it.
        /// </summary>
        public void CloseSocket()
        {
            socket.CloseSocket();
            receiving = false;
        }


    }
}

