using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* This can be modified any number of ways. Here's a start. */

namespace Message_Class
{
    public class Message
    {
        string delimiter = ":";
        string in_string = "";

        public Message()
        { }

        public Message(string m_in)
        {
            in_string = m_in;
        }

        void send_message()
        {
            // Plug into socket somehow. Must see socket!
        }

        void receive_message()
        {
            // in_string = m_in;

            // Trim off the \n
            string[] substrings = Regex.Split(in_string, delimiter);
            System.Console.Write(substrings[0] + "\n");

            int condition = substrings.Length;
            switch (condition)
            {
                case 1:
                    if (substrings[0].Equals("INVALID"))
                    {   // TO DO 
                        System.Console.Write("'INVALID' command recognized.");
                    }
                    if (substrings[0].Equals("UPDATE"))
                    {   // TO DO
                        System.Console.Write("'UPDATE' command recognized.");
                    }
                    if (substrings[0].Equals("SAVED"))
                    {   // TO DO
                        System.Console.Write("'SAVED' command recognized.");
                    }
                    break;
                case 2:
                    if (substrings[0].Equals("DISCONNECT"))
                    {   // TO DO
                        System.Console.Write("'DISCONNECT' command recognized.");
                    }
                    if (substrings[0].Equals("PASSWORD"))
                    {   // TO DO
                        System.Console.Write("'PASSWORD' command recognized.");
                    }
                    if (substrings[0].Equals("ERROR"))
                    {   // TO DO
                        System.Console.Write("'ERROR' command recognized.");
                    }
                    if (substrings[0].Equals("OPEN"))
                    {   // TO DO
                        System.Console.Write("'OPEN' command recognized.");
                    }
                    if (substrings[0].Equals("CREATE"))
                    {   // TO DO
                        System.Console.Write("'CREATE' command recognized.");
                    }
                    break;
                case 3:
                    if (substrings[0].Equals("FILELIST"))
                    {   // TO DO
                        System.Console.Write("'FILELIST' command recognized.");
                    }
                    if (substrings[0].Equals("ENTER"))
                    {   // TO DO
                        System.Console.Write("'ENTER' command recognized.");
                    }
                    if (substrings[0].Equals("UPDATE"))
                    {   // TO DO
                        System.Console.Write("'UPDATE' command recognized.");
                    }
                    break;
                default:
                    System.Console.Write("ERROR! INVALID COMMAND!");
                    break;
            }

            // Case substrings length = 1
        }

        static void Main(string[] args)
        {
            Message newMes = new Message("ENTER:BLAH:BLAH\n");
            newMes.receive_message();
            string line = Console.ReadLine();
        }
    }
}
