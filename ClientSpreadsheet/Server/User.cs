using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNetworking;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace CServer
{
    class User
    {
        //TODO: Once the message is received display it on the clients side console.

        public StringSocket socket { get; private set; }    // Socket associated with the player
        public string Name { get; private set; }            // Player's name
        private readonly object Lock = new object();
        private Lobby lobby;                                // Copy of the lobby user belongs to.
        private char esc = (char)27;
        private string previousCommand;
        bool Terminated;

        public User(string _name, StringSocket _socket)
        {
            Name = _name;
            socket = _socket;
            Terminated = false;

            socket.BeginReceive(LineReceivedFromUser, socket);
        }

        /// <summary>
        /// Send a line of text to the player.
        /// </summary>
        public void SendMessageToUser(String line)
        {
            lock (Lock)
            {
                // Check to make sure the socket is still open
                if (Terminated)
                    return;
                if (!socket.SocketOpen)
                {
                    Terminated = true;
                    return;
                }

                if (socket != null)
                {
                    socket.BeginSend(line + "\n", (e, p) => { }, socket);
                }
            }
        }

        /// <summary>
        /// Called when a game is terminated by the other player.
        /// </summary>
        public void TerminatePlayer()
        {
            socket.BeginSend("TERMINATED" + "\n", (e, p) => { }, socket);
            socket.CloseSocket();
            Terminated = true;
        }

        /// <summary>
        /// Deal with an arriving line of text.
        /// </summary>
        public void LineReceivedFromUser(String s, Exception e, object p)
        {
            lock (Lock)
            {
                // Check to make sure the socket is still open
                if (Terminated)
                    return;

                if (ReferenceEquals(s, null) || e is SocketException || !socket.SocketOpen)
                {
                    Terminated = true;
                    return;
                }

                string[] words = Regex.Split(s, esc.ToString());

                Console.WriteLine(s);

                //for (int i = 0; i < 1; i++)
                //{
                SendMessages(words[0], words);
                //}

                //SendMessagesToAll(s);

                socket.BeginReceive(LineReceivedFromUser, socket);
            }
        }

        /// <summary>
        /// Sends the scores to the players.
        /// </summary>
        public void SendMessagesToAll(string s)
        {

            lock (Lock)
            {
                foreach (User usr in lobby.getUsers())
                {
                    //Do not send to the user who just sent the message
                    if (!usr.Equals(this))
                    {
                        usr.SendMessageToUser(s + "\n");
                    }
                }
            }

        }

        /// <summary>
        /// Sends the scores to the players.
        /// </summary>
        public void SendMessages(string s, string[] arr)
        {

            lock (Lock)
            {
                foreach (User usr in lobby.getUsers())
                {
                    //Do not send to the user who just sent the message
                    //if (!usr.Equals(this))
                    //{
                    //usr.SendMessageToUser(s + "\n");
                    if (s.Equals("CREATE"))
                    {
                        usr.SendMessageToUser("UPDATE" + esc + "1\n");
                    }
                    //}

                    if (s.Equals("OPEN"))
                    {
                        usr.SendMessageToUser("UPDATE" + esc + "8" + esc + "A1" + esc + "1" + esc + "B1" + esc + "2\n");

                    }
                    //ENTER[esc]version_number[esc]cell_name[esc]cell_content\n 
                    if (s.Equals("ENTER"))
                    {
                        int ver = Convert.ToInt32(arr[1]);
                        string[] contents = Regex.Split(arr[3], @":");
                        string conts = "";
                        foreach(string str in contents){
                            conts += str;
                        }
                        previousCommand = "UPDATE" + esc + ver + esc + arr[2] + esc + conts + "\n";
                        usr.SendMessageToUser("UPDATE" + esc + ver + esc + arr[2] + esc + conts + "\n");

                    }
                    //ERROR[esc]error_message\n 
                    if (arr.Length >= 4)
                    {
                        if (arr[3].Equals("circular"))
                        {
                            usr.SendMessageToUser("ERROR" + esc + "The update results in circular dependency\n");
                        }
                    }
                    if (s.Equals("RESYNC")) {
                        usr.SendMessageToUser(previousCommand);
                    }
                }
            }
        }

        /// <summary>
        /// Get a coppy of the lobby
        /// </summary>
        public void setLobby(Lobby lobby)
        {
            this.lobby = lobby;
        }
    }
}
