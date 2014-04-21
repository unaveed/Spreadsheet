using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Login;
using AvailableFiles;
using SpreadsheetGUI;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using ClientModel;
using System.Windows.Forms;


namespace Message
{
    public class MessageClass
    {
        private AvailableFiles.Form1 af;
        private Login.Form1 login;
        private Client client;
        private SpreadsheetGUI.Form1 spreadsheet;

        public MessageClass()
        {
            Client client = new Client();
            client.IncomingLineEvent += MessageReceived;

            //Create a new login window and set its client to this client.
            login = new Login.Form1();
            login.setClient(client);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            //MyApplicationContext appContext = MyApplicationContext.getAppContext();
            Application.Run(login);


            //Create new available file window.
            af = new AvailableFiles.Form1();
            af.setClient(client);
            //Application.Run(af);

            spreadsheet = new SpreadsheetGUI.Form1();
        }

        private void MessageReceived(string line, Exception e)
        {
            if (ReferenceEquals(line, null) || line == "")
            {
                if (e is SocketException)
                {
                    login.ServerClosedMSG();
                }
                return;
            }

            string[] lineReceived = Regex.Split(line, @"\[esc\]");

            string protocol = lineReceived[0];

            //INVALID\n 
            if (protocol.Equals("INVALID"))
            {
                //Call the login method which will display this window.
                login.Invalid();
            }
            //FILELIST[esc]list_of_existing_filenames\n ( each name delimited by [esc])
            else if (protocol.Equals("FILELIST"))
            {
                //af = new AvailableFiles.Form1();
                //af.setClient(client);

                //Add the items to the list of items you can open
                this.af.AddSS(lineReceived);

                //Run the files available prompt window.
                Application.Run(af);

                //Application.Run(spreadsheet);
                System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ThreadStart(RunAppSpreadsheet));
                t.Start();

                //Open the spreadsheet at this point
                //SpreadsheetGUI.Form1 Spreadsheet;
                //Spreadsheet = new SpreadsheetGUI.Form1(client);
                //Spreadsheet.setClient(client);
                //////CheckForIllegalCrossThreadCalls = false;


                //Spreadsheet.Closed += (sender, args) => this.Close();
                //Spreadsheet.ShowDialog();

                //Application.Run(Spreadsheet);


                //Close();
            }
            else if (protocol.Equals("UPDATE"))
            {
                HashSet<string> cells = new HashSet<string>();
                try
                {
                    int versionNumber = Convert.ToInt32(lineReceived[1]);

                    spreadsheet.setVersion(versionNumber);
                }
                catch
                {
                    Console.WriteLine("Cannot convert string into a number");
                }

                for (int i = 2; i < lineReceived.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        //It is a cell name
                        string cellName = lineReceived[i];
                        cells.Add(cellName);
                        i++;
                    }
                    if ((i % 2) == 1 && i < lineReceived.Length)
                    {
                        //It is the contents of the cell.
                        string cellCont = lineReceived[i];
                    }
                }

                spreadsheet.Invoke((MethodInvoker)delegate
                {
                    spreadsheet.Update();
                });

                //this.Invoke((MethodInvoker)delegate
                //{
                //    spreadsheet.Update();

                //});
                //CheckForIllegalCrossThreadCalls = false;
            }

        }

        private void RunAppSpreadsheet()
        {
            Application.Run(spreadsheet);
        }
    }
}