using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ClientModel;
using System.Text.RegularExpressions;

namespace AvailableFiles {

    public partial class Form1 : Form {

        // Register for this event to be motified when a line of text arrives.
        public event Action<bool> ClickedCancel;
        public delegate void CloseFileList(bool clickedCancel);

        private char esc = (char)27;

        int index = 1;
        Client client;
        String SSName;
        String defaultText;

		private string[] filelist;

        public Form1() {
            InitializeComponent();
            this.Text = "Available Files";
            DM.Text = "Click on a Spreadsheet to open it or Click \"New\" to create a new one";
            defaultText = "Provide a name for the new spreadsheet";
        }

        /// <summary>
        /// Create a new instance of the form given a client.
        /// </summary>
        /// <param name="c"></param>
        public Form1(Client c) {
            client = c;
            InitializeComponent();
            DM.Text = "Click on a Spreadsheet to open it or Click \"New\" to create a new one";
            this.ControlBox = false;            
        }

        /// <summary>
        /// Uses the spreadsheet'ss client.
        /// </summary>
        /// <param name="c"></param>
        public void setClient(Client c) {
            this.client = c;
        }

        /// <summary>
        /// Add clickable labels dynamically 
        /// </summary>
        /// <param name="s"></param>
        public void AddSS(string[] s) {
			filelist = s;
            for (int i = 1; i < s.Length; i++) {
                btnAdd_Click(s[i]);
            }
        }

        /// <summary>
        /// Adds a new click event handler on the label.
        /// </summary>
        /// <param name="s"></param>
        private void btnAdd_Click(String s) {
            Label b = new Label();
            b.Text = s;
            b.Name = "btn" + index;
            b.Location = new Point(DM.Location.X, DM.Location.Y + 30 * index);
            b.Click += new EventHandler(b_Click);

            index++;
            groupBox1.Controls.Add(b);
        }

        /// <summary>
        /// Opens a the spreadsheet clicked by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_Click(object sender, EventArgs e) {
            // it's all buttons event handler
            // Buttons can be recognized by ((Button)sender).Text value or my ((Button)sender).Name

            //Request to the server the spreadsheet chosen by user.
            //OPEN[esc]spreadsheet_name\n 
            SSName = ((Label)sender).Text;

            //Send to the message class.
            client.SendMessage("OPEN" + esc + SSName + "\n");

            Close();
        }

        /// <summary>
        /// Opens a new empty spreadsheet when the new button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewBtn_Click(object sender, EventArgs e) {
            string ssName = newSSName.Text;

            // The user must not use an extension for the name of the 
            //  spreadsheet. Deny if user provides extension.
            if (ssName.Contains('.')) {
                MessageBox.Show(
                  "Wrong file name. Please don't include extension",
                  "Error",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
                return;
            }

			foreach (string str in filelist) {
				if (str.Equals(ssName + ".ss")) {
					newSSName.Text = "Name already exists";
					return;
				}
			}

            SSName = newSSName.Text;

            //CREATE[esc]spreadsheet_name\n
            client.SendMessage("CREATE" + esc + newSSName.Text + "\n");

            Close();
        }

        /// <summary>
        /// Returns the name of the spreadsheet.
        /// </summary>
        /// <returns></returns>
        public String getSSName() {
            return SSName;
        }

        private void CancelBtn_Click(object sender, EventArgs e) {
            Close();
            ClickedCancel(true);
        }

    }


}

