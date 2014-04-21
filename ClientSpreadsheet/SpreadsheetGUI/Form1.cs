/*
 * Features I want to add someday:
 * - Much more vim stuff:
 *     + Bar at the bottom that says whether you are in insert mode, or shows the commands in normal mode...
 *     + Visual mode
 *     + Add many more commands/key bindings
 * - Able to select more than one cell at a time
 * - Be able to click while putting in a formula to put that cell into the formula
 * - Add more tools to use (alignment changes, color changes, etc.)
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClientModel;
using System.Net.Sockets;
using Login;
using System.Reflection;
using System.Security.Permissions;
using System.Timers;
using System.Windows.Forms;


namespace SpreadsheetGUI {
    /// <summary>
    /// Example of using a SpreadsheetPanel object
    /// </summary>
    public partial class Form1 : Form {

        // Help message
        private string HelpMessage = @"
Welcome to the Spreadsheetinator!  Here is a list of features and how to use them:

- File menu:
	New: Brings up a new spreadsheet
	Open: Opens an existing spreadsheet
	Save: Saves your spreadsheet
	Save As: Save your spreadsheet to a specific place
	Exit: Close your spreadsheet
- The Cell field (the text box that is grayed out) displays the current cell you have selected.
- The Value field (the text box next to the Cell field, also grayed out) displays the value in the cell.  The value can also be seen in the cell itself.
- The Contents field lets you add content to a cell:
	+ To add something to a cell, simply type what you want into the Contents field and either press enter, tab, or click onto another cell.
- To change a cell, simply select the cell you wish to change and press 'i' to start your input (think of switching to Insert mode in Vim), or click on the contents text box and begin typing.  When you are done, press enter, tab, or click onto another cell.  You will be able to keep typing in the next cell.  If you want to be able to navigate using the arrow keys (or hjkl described below), press escape.
- To input a formula, while typing into the contents field, type '=' then your formula.  For example, '=A1 + B3 - 4 * C5' would be an appropriate formula.  If you made a mistake in your formula, you will see an error in your cell value.  If you made a formula that created a circular reference, the program will tell you there was an error and not input the formula.  You may type lower case letters in your formula.
- To help in navigating, you can navigate around the cells using the 'h', 'j', 'k', and 'l' keys as though you were using Vim!
- If you try to open an invalid or corrupt file, an error will occur and it won't open your file.

Hope these instructions and hints help!  Have fun Spreadsheetinatoring!

Note: Special features added for the assignment

Here is a list of extra features I added for the assignment (you know, the 'above and beyond' stuff):
	- Able to start typing into the contents box when selecting a cell (basically, you don't have to click in the contents box to start editing the contents)
	- The title of the spreadsheet changes to the name of the file the user saved (or just Form if they haven't saved it yet)
	- An asterisk appears next to the title of the spreadsheet if there are unsaved changed.  It goes away when the user saves the file.
	- I overrode the red X (the closing one) in the upper right corner to make sure that the user wants to close if there are unsaved changes.
	- Able to move around using the arrow keys as well as the tab key.
	- Able to start typing into the contents text box from any cell by typing 'i' then typing whatever you want.  User is able to stay in insert mode as long as they like, switching back to normal mode by typing <Escape>.
	- Also able to move around using 'h', 'j', 'k', and 'l' like in Vim!  Yay!
	- Made it so a brand new spreadsheet has the name 'Sheet 1', then 'Sheet 2', and so forth.  It keeps track of the number of spreadsheets that are open and uses that to name the brand new spreadsheets.
	- Added icons to make it pretty.";

        //Initialize the Login window
        //Login.Form1 login;
        AvailableFiles.Form1 availableFile;

        private SS.Spreadsheet spreadsheet;		// Spreadsheet model

        private static int totalSpreadsheets = 1;	// Keeps track of the number of spreadsheet open at one time
        private string titleFile;				// Name of the file, displayed at the top
        private string saveFile;				// The path of the saved file, initially set to ""
        private bool insertMode;				// Variable for InsertMode
        private Client client;                  // Clien used to connect to the server
        private string usrPassword;             // Used to save the users login password
        private string usrIpAddrss;             // Used to save the users ipAdress
        private int versionNumber;              // Keep track of this spreadsheet's version number
        private Login.Form1 login;              // Access to the login.
        private string NameOfSS;
        private bool SSEdit;                    // True is the last command sent was an edit to the spreadsheet.
        private char esc = (char)27;            // Delimeter for the protocol
        private bool serverClosed = false;      // If server closes there is no option of saving work.
        private System.Timers.Timer time;       // Clock for autosave


        // This delegate enables asynchronous calls for setting 
        // the text property on a TextBox control. 
        delegate void SetTextCallback(string text);
        delegate void CloseSSCallback();
        delegate void ShowSSCallback();
        delegate bool ChangeCellVal(SS.SpreadsheetPanel ss, String name, String contents);

        // Boolean that keeps track of whether the user is in insert mode or not, also
        // takes care of switching focus from the content text box to the spreadsheet
        // and back.
        private bool InsertMode {
            get {
                return insertMode;
            }

            set {
                if (value) {
                    contentsTextBox.Focus();
                    insertMode = true;
                } else {
                    //spreadsheetPanel1.Focus();
                    insertMode = false;
                }
            }
        }

        /// <summary>
        /// Constructs a new spreadsheet
        /// </summary>
        public Form1() {
            InitializeComponent();

            //Creates new socket for this specific spreadsheet.
            client = new Client();
            time = new System.Timers.Timer();
            client.IncomingLineEvent += MessageReceived;

            //Creates new login window.
            login = new Login.Form1(client);
            login.ClickedCancel += CancelSpreadSheet; // Will close this spreadsheet if user clicks cancel.
            login.ShowDialog();

            //Save the IpAddress and the password so user will not have to input every
            // time a new spreadsheet is opened.
            usrPassword = login.getPassword();
            usrIpAddrss = login.getIp();
            SSEdit = false;

            //Hide spreadsheet until the login has been verified.
            spreadsheetPanel1.Hide();

            time.Interval = 3000; // Set to 30000 for 30 seconds!!
            time.Elapsed += new ElapsedEventHandler(time_elapsed);

            // This could also be done graphically in the designer, as has been
            // demonstrated in class.
            spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;

            spreadsheet = new SS.Spreadsheet(x => true, (string s) => s.ToUpper(), "ps6");

            titleFile = "Sheet " + totalSpreadsheets;

            saveFile = "";

            SetTitle();

            InsertMode = false;

        }

        /// <summary>
        /// Constructs a new spreadsheet when user clicks open
        /// and user login is no longer required.
        /// </summary>
        public Form1(String ip, String password) {
            InitializeComponent();

            client = new Client();
            time = new System.Timers.Timer();
            client.IncomingLineEvent += MessageReceived;

            usrPassword = password;
            usrIpAddrss = ip;

            login = new Login.Form1(client, ip, password);

            spreadsheetPanel1.Hide();

            time.Interval = 3000; // Set to 30000 for 30 seconds!!
            time.Elapsed += new ElapsedEventHandler(time_elapsed);

            // This could also be done graphically in the designer, as has been
            // demonstrated in class.
            spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;

            spreadsheet = new SS.Spreadsheet(x => true, (string s) => s.ToUpper(), "ps6");

            SetTitle();

            InsertMode = false;

        }

        /// <summary>
        /// Constructs a new spreadsheet
        /// </summary>
        public Form1(string filename, string save, SS.Spreadsheet sheet) {
            InitializeComponent();

            spreadsheet = sheet;

            spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;

            // Initialize all of the non-empty cells
            HashSet<string> cells = new HashSet<string>(spreadsheet.GetNamesOfAllNonemptyCells());
            foreach (string name in cells)
                DisplayValueInTable(name);

            spreadsheetPanel1.SetSelection(0, 0);

            saveFile = filename;

            titleFile = save;

            // Perform an initial save
            Save();

            SetTitle();

            InsertMode = false;
        }

        /// <summary>
        /// Method called when a new message has been received by the server.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="e"></param>
        private void MessageReceived(String line, Exception e) {
            if (ReferenceEquals(line, null) || line == "") {
                if (e is SocketException) {
                    serverClosed = true;
                    MessageBox.Show("Server has closed.  Please try again later.",
                        "Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    if (this.InvokeRequired) {
                        CloseSSCallback d = new CloseSSCallback(CloseSS);
                        this.Invoke(d, new object[] { });
                    } else {
                        CloseSS();
                    }
                    return;
                }
                return;
            }

            string[] lineReceived = Regex.Split(line, esc.ToString());

            string protocol = lineReceived[0];

            //INVALID\n 
            if (protocol.Equals("INVALID")) {
                login.LoginError();
                login = new Login.Form1(client);
                login.ShowDialog();
            }
                //FILELIST[esc]list_of_existing_filenames\n ( each name delimited by [esc])
            else if (protocol.Equals("FILELIST")) {
                availableFile = new AvailableFiles.Form1(client);
                availableFile.ClickedCancel += CancelSpreadSheet;

                //Add the items to the list of items you can open
                availableFile.AddSS(lineReceived);

                //Run the files available prompt window.
                availableFile.ShowDialog();

                NameOfSS = availableFile.getSSName();

                //Shows the spreadsheet again after the user has picked to open 
                // a new spreadsheet or to open an exsisting spreadsheet.
                if (this.InvokeRequired) {
                    ShowSSCallback d = new ShowSSCallback(ShowSS);
                    this.Invoke(d, new object[] { });
                } else {
                    spreadsheetPanel1.Show();
                }
                availableFile.Focus();

                time.Start();

                SetTitle();

            }
                //UPDATE[esc]current_version[esc]cell_name1[esc]cell_content1[esc]cell_name2[esc]…\n
                  //SYNC[esc]current_version[esc]cell_name[esc]cell_content…\n 
            else if (protocol.Equals("UPDATE") || protocol.Equals("SYNC")) {
                Dictionary<string, string> cells = new Dictionary<string, string>();
                String cellName = ""; String cellCont = "";
                try {
                    int currentVersion = Convert.ToInt32(lineReceived[1]);

                    //If the last command sent to server was an edit to the spreadsheet and 
                    // the version + 1 doesn't equal to the version send back by the server demand a Resync to server.
                    if (SSEdit && (versionNumber + 1) != currentVersion) {
                        //Must resync with the server.
                        client.SendMessage("RESYNC\n");
                        SSEdit = false;
                        return;
                    }
                    versionNumber = currentVersion;
                } catch {
                    //TODO:Handle this properly.
                    MessageBox.Show(
                    "Error",
                    "Protocol was not sent properly. No version number was sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
                }
                //Go through the array and place the cellName in the correct container
                // and do the same with the content of that cell.
                for (int i = 2; i < lineReceived.Length; i++) {
                    if (i % 2 == 0) {
                        //It is a cell name
                        cellName = lineReceived[i];
                        i++;
                    }
                    if ((i % 2) == 1 && i < lineReceived.Length) {
                        //It is the contents of the cell.
                        cellCont = lineReceived[i];
                    }
                    cells.Add(cellName, cellCont);
                }
                Update(cells);

                SSEdit = false;
            } else if (protocol.Equals("SAVED")) {
                //TODO: what to do when the saved command is received.
                MessageBox.Show("File Successfully Saved",
                          "Saved",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
                return;
            }
                //ERROR[esc]error_message\n 
          else if (protocol.Equals("ERROR")) {
                MessageBox.Show(lineReceived[1],
                           "Message",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Saves the file every time the increment on the clock elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void time_elapsed(object sender, ElapsedEventArgs e) {
            SendSaveCmd();
        }
  
        /// <summary>
        /// When the user clicks cancel this method will be called and will terminate 
        /// the socket and this spreadsheet.
        /// </summary>
        /// <param name="clickedCancel"></param>
        private void CancelSpreadSheet(bool clickedCancel) {
            if (clickedCancel) {
                if (this.InvokeRequired) {
                    ShowSSCallback d = new ShowSSCallback(CloseSS);
                    this.Invoke(d, new object[] { });
                } else {
                    Close();
                }
            }
            return;
        }

        /************************** THREAD SAFE METHODS  **************************/

        /// <summary>
        /// If user closes available file window has been closed and closes this spreadsheet as well.
        /// </summary>
        private void availableFileClosed(object sender, FormClosedEventArgs e) {
            if (this.InvokeRequired) {
                ShowSSCallback d = new ShowSSCallback(CloseSS);
                this.Invoke(d, new object[] { });
            } else {
                this.Close();
            }
        }

        /// <summary>
        /// Method that is called for Showing the spreadsheet located on a different thread.
        /// </summary>
        private void ShowSS() {
            spreadsheetPanel1.Show();
        }

        /// <summary>
        /// Method that is called for closing a spreadsheet located on a different thread.
        /// </summary>
        private void CloseSS() {
            this.Close();
        }

        /// <summary>
        /// Method that is called for changing the name of the title in a different 
        /// thread.
        /// </summary>
        /// <param name="text"></param>
        private void ChangeSSName(string text) {
            this.Text = text;
        }

        /************************** VISUAL CODE **************************/

        //private void CreateSpreadsheet() {

        //    // This could also be done graphically in the designer, as has been
        //    // demonstrated in class.
        //    spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;

        //    spreadsheet = new SS.Spreadsheet(x => true, (string s) => s.ToUpper(), "ps6");

        //    titleFile = "Sheet " + totalSpreadsheets;

        //    saveFile = "";

        //    SetTitle();

        //    InsertMode = false;
        //}

        /*** New ***/

        // Deals with the New menu
        private void newToolStripMenuItem1_Click(object sender, EventArgs e) {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            totalSpreadsheets++;
            MyApplicationContext.getAppContext().RunForm(new Form1(usrIpAddrss, usrPassword));
        }

        /*** Open ***/

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            totalSpreadsheets++;

            // When opening an existing item provide the password and ip address used when first
            //  loging in so that the user doesn't have to type it in again.
            MyApplicationContext.getAppContext().RunForm(new Form1(usrIpAddrss, usrPassword));
        }

        /*** Save ***/

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
            SendSaveCmd();
            SetTitle();
        }

        /*** Save As ***/

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveAs();
        }

        /*** Exit ***/

        // Deals with the Exit menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {

            DialogResult result = MessageBox.Show(
                "Do you want to save the changes you made to '" + NameOfSS + "'?",
                "Spreadsheet",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            switch (result) {
                case DialogResult.Yes:
                    SendSaveCmd();
                    CloseServerSocket();
                    Close();
                    break;
                case DialogResult.No:
                    CloseServerSocket();
                    Close();
                    break;
                case DialogResult.Cancel:
                    break;
            }

        }

        /*** Red X (closing) ***/

        /// <summary>
        /// Override the red X that closes.  Check with the user if there are
        /// any unsaved changes that need to be saved.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            totalSpreadsheets--;

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (!serverClosed) {
                if (spreadsheet.Changed) {
                    DialogResult result = MessageBox.Show(
                        "Do you want to save the changes you made to '" + NameOfSS + "'?",
                        "Spreadsheet",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning);

                    switch (result) {
                        case DialogResult.Yes:
                            SendSaveCmd();
                            CloseServerSocket();
                            break;
                        case DialogResult.No:
                            CloseServerSocket();
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }

        /*** Help ***/
        /// <summary>
        /// Displays the help message.
        /// </summary>
        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(HelpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        /*** Arrow keys ***/

        private void spreadsheetPanel1_KeyDown(object sender, KeyEventArgs e) {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            switch (e.KeyCode) {
                case Keys.Tab:
                    ChangeSelection(col + 1, row);
                    break;
                case Keys.K:
                case Keys.Up:
                    ChangeSelection(col, row - 1);
                    break;
                case Keys.J:
                case Keys.Down:
                    ChangeSelection(col, row + 1);
                    break;
                case Keys.H:
                case Keys.Left:
                    ChangeSelection(col - 1, row);
                    break;
                case Keys.L:
                case Keys.Right:
                    ChangeSelection(col + 1, row);
                    break;
                case Keys.I:
                    InsertMode = true;
                    break;
            }
        }


        private void spreadsheetPanel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Tab:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }


        /*** Selected Cell ***/

        /// <summary>
        /// Displays the selected cell in the non editable cell box.
        /// </summary>
        private void DisplaySelectedCell(SS.SpreadsheetPanel ss) {
            int row, col;

            // Save the users selection
            ss.GetSelection(out col, out row);

            // Get the old cell and set it's value, skipping if it's the first
            // selection seen so far
            int oldRow, oldCol;
            GetCellCoords(selectedCellText.Text, out oldCol, out oldRow);
            // oldRow and oldCol will be -1 if this is the first selection seen so far
            if (oldRow >= 0 && oldCol >= 0) {
                ss.SetSelection(oldCol, oldRow);

                // Check if the contents have changed, doing nothing if they haven't
                if (CheckForChanges(oldCol, oldRow))
                    ChangeCellValuebyServer(ss);

                // Restore the selection to the user selection
                ss.SetSelection(col, row);
            }


            // Display the cell name in the Cell text box
            string name = GetCellName(col, row);

            selectedCellText.Text = name;

            // Set the valueTextBox to the cell value
            valueTextBox.Text = GetValue(name);

            // Set the cellContents text box to the cell contents
            contentsTextBox.Text = GetContents(name);

            // Make sure focus is in the correct spot
            InsertMode = InsertMode;
        }

        /*** Contents ***/

        private void cellContentsText_Leave(object sender, EventArgs e) {
            ChangeCellValuebyServer(spreadsheetPanel1);
        }

        private void cellContentsText_KeyDown(object sender, KeyEventArgs e) {
            int col, row;
            switch (e.KeyCode) {
                case Keys.Return:
                    if (!ChangeCellValuebyServer(spreadsheetPanel1))
                        break;
                    spreadsheetPanel1.GetSelection(out col, out row);
                    ChangeSelection(col, row + 1);
                    break;
                case Keys.Tab:
                    if (!ChangeCellValuebyServer(spreadsheetPanel1))
                        break;
                    spreadsheetPanel1.GetSelection(out col, out row);
                    ChangeSelection(col + 1, row);
                    break;
                case Keys.Escape:
                    InsertMode = false;
                    break;
            }
        }

        private void contentsTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
            }
        }

        /************************** LOGIC CODE **************************/

        /// <summary>
        /// Changes the cell value to it's contents.  Returns true if no
        /// problems were encountered, false if an exception was thrown
        /// and caught.
        /// </summary>
        private bool ChangeCellValuebyServer(SS.SpreadsheetPanel ss) {
            int row, col;
            ss.GetSelection(out col, out row);

            string name = GetCellName(col, row);

            ISet<string> set = new HashSet<string>();

            String CellCont = contentsTextBox.Text;

            try {
                CellCont = spreadsheet.NormalizeNValidate(CellCont);

            } catch (SpreadsheetUtilities.FormulaFormatException e) {
                MessageBox.Show(
                    e.Message,
                    "Formula Format Exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            List<string> tokens = new List<string>(GetTokens(CellCont));

            CellCont = makeProtocolContent(tokens);

            //Send the edit to the server
            //ENTER[esc]version_number[esc]cell_name[esc]cell_content\n 
            client.SendMessage("ENTER" + esc + versionNumber + esc + name + esc + CellCont + "\n");

            //Set the spreadsheet edited flag to true.s
            SSEdit = true;

            return true;
        }

        /// <summary>
        /// Return the Formula in the format needed for the protocol.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private String makeProtocolContent(List<string> tokens) {
            string protocolFormula = "";
            if (tokens.Count == 1 || tokens[0].Equals('=')) {
                return tokens[0];
            } else {
                //foreach (string s in tokens) {
                for (int i = 0; i < tokens.Count; i++) {
                    if (i != tokens.Count - 1)
                        protocolFormula += tokens[i] + ":";
                    else
                        protocolFormula += tokens[i];
                }
            }
            return protocolFormula;
        }

        /// <summary>
        /// Changes the cell value to it's contents.  Returns true if no
        /// problems were encountered, false if an exception was thrown
        /// and caught.
        /// </summary>
        private bool ChangeCellValue(SS.SpreadsheetPanel ss) {
            int row, col;
            ss.GetSelection(out col, out row);

            string name = GetCellName(col, row);

            ISet<string> set = new HashSet<string>();
            try {
                //Set the contents of the spreadsheet cell
                set = spreadsheet.SetContentsOfCell(name, contentsTextBox.Text);

                foreach (string s in set) {
                    DisplayValueInTable(s);
                }

                // Display the new value in both the cell and the Value text box
                DisplayValueInTable(name);
                valueTextBox.Text = GetValue(name);
            } catch (SS.CircularException e) {
                MessageBox.Show(
                    "ERROR: The formula you entered causes a circular dependency.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            } catch (SpreadsheetUtilities.FormulaFormatException e) {
                MessageBox.Show(
                    e.Message,
                    "Formula Format Exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            // Set title to have asterisk if the file is changed
            SetTitle();

            return true;
        }

        /// <summary>
        /// Changes the cell value to the provided contents.  Returns true if no
        /// problems were encountered, false if an exception was thrown
        /// and caught.
        /// </summary>
        private bool ChangeCellValueGivenContent(SS.SpreadsheetPanel ss, String name, String contents) {
            ISet<string> set = new HashSet<string>();
            try {

                //TESTS:
                //spreadsheet.NormalizeNValidate("a1++a2");
                //spreadsheet.SetContentsOfCell("a1", "=a2+a3");

                // Set the contents of the spreadsheet cell
                set = spreadsheet.SetContentsOfCell(name, contents);
                foreach (string s in set) {
                    DisplayValueInTable(s);
                }

                // Display the new value in both the cell and the Value text box
                DisplayValueInTable(name);
                //this.Invoke(new MethodInvoker(this.ChangeValueTextBox(name)));

                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (this.valueTextBox.InvokeRequired) {
                    SetTextCallback d = new SetTextCallback(ChangeValueTextBox);
                    this.Invoke(d, new object[] { name });
                } else {
                    this.valueTextBox.Text = name;
                }

                //this.valueTextBox.Text = GetValue(name);
            } catch (SS.CircularException e) {
                MessageBox.Show(
                    "ERROR: The formula you entered causes a circular dependency.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            } catch (SpreadsheetUtilities.FormulaFormatException e) {
                MessageBox.Show(
                    e.Message,
                    "Formula Format Exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            // Set title to have asterisk if the file is changed
            SetTitle();

            return true;
        }

        private void ChangeValueTextBox(string name) {
            valueTextBox.Text = GetValue(name);
        }

        /// <summary>
        /// Changes which cell is selected to the row and col.
        /// </summary>
        private void ChangeSelection(int col, int row) {
            spreadsheetPanel1.SetSelection(col, row);
            DisplaySelectedCell(spreadsheetPanel1);
        }

        /// <summary>
        /// Displays the value of the cell in the appropriate cell on the table
        /// </summary>
        private void DisplayValueInTable(string name) {
            int col, row;
            GetCellCoords(name, out col, out row);
            spreadsheetPanel1.SetValue(col, row, GetValue(name));
        }

        /// <summary>
        /// Returns the string format of the given cell, for example col=1 row=0 => "A1".
        /// </summary>
        public string GetCellName(int col, int row) {
            return ((char)(col + 65)).ToString() + (row + 1).ToString();
        }

        /// <summary>
        /// Returns the coordinates of the cell.
        /// </summary>
        private void GetCellCoords(string name, out int col, out int row) {
            // NOTE: Consider making this a member variable
            Regex r = new Regex(@"([a-zA-Z]+)([1-9]\d*)");

            // Split cell name into tokens
            Match result = r.Match(name);

            // 'Return' or out the correct col and row
            col = GetCol(result.Groups[1] + "");
            int.TryParse(result.Groups[2] + "", out row);
            row--;
        }

        /// <summary>
        /// Helper method for GetCellCoords.  Takes the letter portion of a cell name
        /// and converts it to the correct column number.
        /// </summary>
        private static int GetCol(string col) {
            col = col.ToUpper();
            int result = 0;

            char[] arr = col.ToCharArray();

            for (int i = arr.Length - 1, j = 0; i >= 0; i--, j++) {
                result += (int)((arr[i] - 64) * Math.Pow(26, j));
            }

            return result - 1;
        }
        /// <summary>
        /// Returns the contents of a cell as a string.
        /// </summary>
        private string GetContents(string name) {

            // Contents can be a string, double, or formula
            object contents = spreadsheet.GetCellContents(name);

            // If the contents are a formula, prepend '='
            if (contents is SpreadsheetUtilities.Formula)
                return "=" + contents;

            // Otherwise, return the contents
            return contents + "";
        }

        /// <summary>
        /// Returns the value of the cell.
        /// </summary>
        private string GetValue(string name) {
            object value = spreadsheet.GetCellValue(name);

            // If the formula is a FormulaError, return the error
            if (value is SpreadsheetUtilities.FormulaError)
                return ((SpreadsheetUtilities.FormulaError)value).Reason;

            return value + "";
        }

        /// <summary>
        /// Returns true if the cell contents are different than what is
        /// in the contents text box.
        /// </summary>
        private bool CheckForChanges(int col, int row) {
            string name = GetCellName(col, row);
            if (GetContents(name) != contentsTextBox.Text)
                return true;
            return false;
        }

        /// <summary>
        /// Sets the title to have an asterisk if the file has been changed,
        /// or just the name of the file if it hasn't been changed.
        /// </summary>
        private void SetTitle() {
            string title;
            if (!spreadsheet.Changed) {
                title = NameOfSS + "*";
                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (this.InvokeRequired) {
                    SetTextCallback d = new SetTextCallback(ChangeSSName);
                    this.Invoke(d, new object[] { title });
                } else {
                    this.Text = title;
                }
            } else {
                title = NameOfSS;
                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (this.InvokeRequired) {
                    SetTextCallback d = new SetTextCallback(ChangeSSName);
                    this.Invoke(d, new object[] { title });
                } else {
                    this.Text = title;
                }
            }

        }

        /// <summary>
        /// Opens a file if the user chooses, or does nothing if they cancel or exit.
        /// </summary>
        private void Open() {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*";

            // Set the default filter to *.ss
            open.FilterIndex = 1;

            // Open the file chooser
            open.ShowDialog();

            string path = open.FileName;

            // If user doesn't choose a file
            if (path == "")
                return;

            // Set the file title (filename) and save path
            string filename = open.SafeFileName;

            // Try to make a new spreadsheet with the chosen file, then make a new file if
            // it succeeds.  If it is an invalid or corrupt file, then show the message and
            // don't make the new file.
            try {
                SS.Spreadsheet sheet = new SS.Spreadsheet(path, x => true, (string s) => s.ToUpper(), "ps6");

                // Start a new spreadsheet with the chosen filename
                MyApplicationContext.getAppContext().RunForm(new Form1(path, filename, sheet));
            } catch (SS.SpreadsheetReadWriteException e) {
                MessageBox.Show("Invalid or corrupt file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Brings up a saving dialog and sets the path to save and the name of the file.
        /// </summary>
        private DialogResult SaveAs() {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*";

            // Set the default filter to *.ss
            saveDialog.FilterIndex = 1;

            // Open the save dialog
            DialogResult result = saveDialog.ShowDialog();

            string filename = saveDialog.FileName;

            // If user clicked cancel or exited
            if (result == DialogResult.Cancel)
                return result;

            // Check to see if they chose *.ss (1) or *.* (2)
            if (saveDialog.FilterIndex == 1) {
                // Check if the file has the correct extension
                string correctExtension = ".ss";
                int lastDot = filename.LastIndexOf('.');
                if (lastDot > 0)
                    if (filename.Substring(lastDot) != correctExtension)
                        filename += correctExtension;
            }

            // Set file title
            int a = filename.LastIndexOf("\\");
            titleFile = filename.Substring(a + 1);

            // Set the save file name
            saveFile = filename;

            return result;
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        private DialogResult Save() {
            // Have to initialize result (compiling issues)
            DialogResult result = DialogResult.None;
            if (saveFile == "") {
                result = SaveAs();
            }

            if (result == DialogResult.Cancel)
                return result;

            spreadsheet.Save(saveFile);
            SetTitle();

            return result;
        }

        /// <summary>
        /// Sends a save command to the server.
        /// </summary>
        private void SendSaveCmd() {
            //SAVE[esc]version_number\n 
            client.SendMessage("SAVE" + esc + versionNumber);
        }

        /// <summary>
        /// Sends a message to the server requesting to disconnect.
        /// </summary>
        private void CloseServerSocket() {
            client.SendMessage("DISCONNECT\n");
            client.CloseSocket();
        }

        /// <summary>
        /// Method used when the spreadsheet gets an update command from the 
        /// server. It will update the cells that have been modified by other users
        /// as well as this user.
        /// </summary>
        /// <param name="cells"></param>
        private void Update(Dictionary<string, string> cells) {
            spreadsheetPanel1.SelectionChanged += DisplaySelectedCell;

            // Initialize all of the non-empty cells
            foreach (KeyValuePair<string, string> item in cells) {
                //The key is the name of the cell and the value is the contents of the cell.
                if (spreadsheetPanel1.InvokeRequired) {
                    ChangeCellVal d = new ChangeCellVal(ChangeCellValueGivenContent);
                    spreadsheetPanel1.Invoke(d, new object[] { spreadsheetPanel1, item.Key, item.Value });
                } else {
                    ChangeCellValueGivenContent(spreadsheetPanel1, item.Key, item.Value);
                }
            }
            InsertMode = false;
        }

        /// <summary>
        /// Sets the client for this spreadsheet
        /// </summary>
        /// <param name="c"></param>
        public void setClient(Client c) {
            this.client = c;
        }

        /// <summary>
        /// Sets the version number.
        /// </summary>
        /// <param name="vNum"></param>
        public void setVersion(int vNum) {
            versionNumber = vNum;
        }

        /// <summary>
        /// When User presses the Undo button it will undo the last global change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoBtn_Click(object sender, EventArgs e) {
            //UNDO[esc]spreadsheet_name\n
            client.SendMessage("UNDO" + esc + NameOfSS + "\n");
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula) {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace)) {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline)) {
                    yield return s;
                }
            }

        }
    }
}
