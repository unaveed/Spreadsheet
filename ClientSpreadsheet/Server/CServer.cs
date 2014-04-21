using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CustomNetworking;
using System.Text.RegularExpressions;
using System.IO;


namespace CServer
{
    public class Server
    {
        // Listens for incoming actions
        private TcpListener server;

        // Keeps track of the players that are waiting to be connected to a game
        private Queue<User> User;

        //Only one lobby at a time for now.
        private Lobby lobby;

        private readonly object locker = new object();  // Used for locking (threading)

        private char esc = (char)27;

        /// <summary>
        /// Launches a chat server that listens on port 2000
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            int port = 12345;

            {
                //Call the three argument constructor. 
                Server server = new Server(port);

                Console.ReadKey();
            }
            Console.Read();
        }

        /// <summary>
        /// Constructs a new boggle server.
        /// </summary>
        public Server(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            server.BeginAcceptSocket(ConnectionReceived, null);

            // Initialize the new Queue.
            User = new Queue<User>();

            //Initialize the new Lobby
            lobby = new Lobby();
        }

        /// <summary>
        /// Deals with connection requests
        /// </summary>
        private void ConnectionReceived(IAsyncResult ar)
        {
            Socket socket = server.EndAcceptSocket(ar);
            StringSocket ss = new StringSocket(socket, UTF8Encoding.Default);
            ss.BeginReceive(VerificationReceived, ss);
            server.BeginAcceptSocket(ConnectionReceived, ss);
        }

        /// <summary>
        /// Receives the first line of text from the client, which contains the name of the remote
        /// user.  Uses it to compose and send back a welcome message.
        /// </summary>
        private void VerificationReceived(String StartMessage, Exception e, object p)
        {
            //Split the message by spaces.
            string[] keyWords = Regex.Split(StartMessage, esc.ToString());
            String username = "";
            String password = "";

            //Username and password validation
            Boolean uval = false;
            Boolean upas = false;

            for (int i = 0; i < keyWords.Length; i++)
            {
                if (keyWords[i].Equals("<USERNAME>"))
                {
                    username = keyWords[i + 1];

                    //Check if username is valid
                    if (username.Equals("Chuy"))
                        uval = true;

                    Console.WriteLine(keyWords[i + 1]);
                }
                else if (keyWords[i].Equals("PASSWORD"))
                {
                    password = keyWords[i + 1];

                    //Check if password is valid
                    if (password.Equals("j"))
                        upas = true;

                    //Validate password
                    Console.WriteLine(keyWords[i + 1]);
                }
            }
            if (keyWords[1].Equals("j"))
            {
                upas = true;
            }

            StringSocket ss = (StringSocket)p;

            if (upas)
            {
                lock (locker)
                {
                    lobby.addUser(new User(username, ss));
                    //CheckQueue();
                }
            }
            else
            {
                //Sent to the user that we are ignoring their input
                ss.BeginSend("INVALID\n", null, p);
                ss.BeginReceive(VerificationReceived, p);
            }
        }

        /// <summary>
        /// Checks if there are 2 or more players in the queue, pairing up the
        /// players if there are.  Does nothing otherwise.
        /// </summary>
        private void CheckQueue()
        {
            //while (players.Count > 1)
            //{
            //    BoggleBoard bb;
            //    if (userBoardLayout == "")
            //        bb = new BoggleBoard();
            //    else
            //        bb = new BoggleBoard(userBoardLayout);

            //    new Game(players.Dequeue(), players.Dequeue(), timer, bb);
            //}

        }

        /// <summary>
        /// Finds the player that clicked cancel and removes them from the queue.
        /// </summary>
        private bool PlayerCancel(User p)
        {
            //// Get the first player in the queue and check if they are the one that
            //// clicked cancel.
            //Player first = players.Dequeue();
            //if (ReferenceEquals(first, p))
            //{
            //    // TODO: socket might not exist anymore
            //    first.socket.CloseSocket();
            //    return true;
            //}
            //// First player wasn't the one, so put them back in the queue
            //players.Enqueue(first);

            //// Loop through and find the player that clicked cancel
            //while (true)
            //{
            //    Player temp = players.Dequeue();
            //    if (ReferenceEquals(temp, p))
            //    {
            //        temp.socket.CloseSocket();
            //        break;
            //    }
            //    players.Enqueue(temp);
            //}

            //// Loop through and put the first player at the front of the queue
            //while (true)
            //{
            //    Player temp = players.Peek();
            //    if (ReferenceEquals(temp, first))
            //        break;

            //    // Take the person in front and move them to the back
            //    temp = players.Dequeue();
            //    players.Enqueue(temp);
            //}

            return true;


        }

    }

}
