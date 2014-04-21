using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServer
{
    class Lobby
    {

        // Represents the players
        private User user;

        private List<User> Users;

        // Overall correct words seen so far
        private HashSet<string> overallCorrect;

        // Object used for locking
        private readonly object Lock = new object();

        private char esc = (char)27;

        public Lobby()
        {
            Users = new List<User>();
        }

        public void addUser(User user)
        {
            user.setLobby(this);
            Users.Add(user);
            //user.SendMessageToUser("Welcome!");
            user.SendMessageToUser("FILELIST" + esc + "exponential" + esc + "quadratic" + esc + "fibonacci\n");
            user.socket.BeginReceive(user.LineReceivedFromUser, user.socket);
        }

        /// <summary>
        /// Sends the scores to the players.
        /// </summary>
        public void SendMessagesToAll(string s, bool IsValid)
        {
            lock (Lock)
            {

                foreach (User usr in Users)
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
        /// If one of the player sockets  closes, then they will send
        /// themselves to this method, saying the game needs to be
        /// terminated.  Sends a message to the remaining player letting
        /// them know and closes the game.
        /// </summary>
        /// <param name="u"></param>
        public void SessionTerminated(User u)
        {
            lock (Lock)
            {
                if (ReferenceEquals(user.socket, null) && (ReferenceEquals(user.socket, null)))
                    return;
                if (ReferenceEquals(u, user))
                {
                    user.TerminatePlayer();
                }
                else
                {
                    user.TerminatePlayer();
                }
            }
        }

        public List<User> getUsers()
        {
            return Users;
        }
    }
}


