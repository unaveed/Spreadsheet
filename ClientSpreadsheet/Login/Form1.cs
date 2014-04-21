using ClientModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class Form1 : Form
    {
        private Client client;
        private bool loginSuccessful = false;
        //private int Port = 30000;
        private int Port = 12345;
        private bool closeLogin = false;

        // Register for this event to be motified when a line of text arrives.
        public event Action<bool> ClickedCancel;
        public delegate void CloseLogin(bool clickedCancel);

        //AvailableFiles.Form1 availableFile;
        public static Form1 Form1Instance;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new instance of the login form given the client that the
        ///  spreadsheet is using.
        /// </summary>
        /// <param name="c"></param>
        public Form1(Client c)
        {
            client = c;
            InitializeComponent();
            this.Text = "Login";
            this.ControlBox = false;
        }


        /// <summary>
        /// Creates a new instance of the login given a client the ip and the password.
        ///  This is done so that the user will not have to input that information again.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ip"></param>
        /// <param name="password"></param>
        public Form1(Client c, string ip, string password)
        {
            client = c;
            InitializeComponent();
            NewSS(ip, password);
        }

        /// <summary>
        /// Creates a new socket for this initial spreadsheet when
        ///   the login button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (ServerIPAddress.Text == "" || Password.Text == "")
            {
                MessageBox.Show(
                    "Please enter both fields.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            //The ip address, port 2000, and the name of the player.
            if (!client.Connect(ServerIPAddress.Text, Port, Password.Text))
            {
                MessageBox.Show("Please enter a valid IP address.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Close();
        }

        /// <summary>
        /// Creates a new socket for this new spreadsheet given an ip address 
        ///  and a password.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="pass"></param>
        public void NewSS(String ip, String pass)
        {
            //The ip address, port 2000, and the name of the player.
            if (!client.Connect(ip, Port, pass))
            {
                MessageBox.Show("Please enter a valid IP address.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            Close();
        }

        /// <summary>
        /// Closes login window when user clicks the cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e) {
            Close();

            ClickedCancel(true);
            return;
        }

        public bool clickedCancell() {
            return closeLogin;
        }

        /// <summary>
        /// Shows Invalid login or password message box.
        /// </summary>
        public void Invalid() {
            MessageBox.Show("Incorrect Login Please Try Again.",
                      "Message",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            return;
        }

        /// <summary>
        /// Shows Server has closed message box.
        /// </summary>
        public void ServerClosedMSG() {
            MessageBox.Show("Server has closed.  Please try again later.",
                        "Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            return;
        }

        /// <summary>
        /// Show invalid login text when login is incorrect.
        /// </summary>
        public void LoginError() {
            InvalidLogin.Text = "Incorrect Login Please Try Again";
        }

        /// <summary>
        /// Sets this client to the client being used by the spreadsheet.
        /// </summary>
        /// <param name="c"></param>
        public void setClient(Client c) {
            client = c;
        }

        /// <summary>
        /// Gets the client that is being used by this login and spreadsheet.
        /// </summary>
        /// <returns></returns>
        public Client getClient()
        {
            return client;
        }

        /// <summary>
        /// Method that lets the login window if the login was successful.
        /// </summary>
        /// <param name="TF"></param>
        public void setLogin(bool TF)
        {
            loginSuccessful = TF;
        }

        /// <summary>
        /// Get the password that the user input.
        /// </summary>
        /// <returns></returns>
        public String getPassword()
        {
            return Password.Text;
        }

        /// <summary>
        /// Gets the ip Address that the user input.
        /// </summary>
        /// <returns></returns>
        public String getIp()
        {
            return ServerIPAddress.Text;
        }

        

    }
}
