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

        private SS.Spreadsheet spreadsheet;		// Spreadsheet model

        private static int totalSpreadsheets = 1;	// Keeps track of the number of spreadsheet open at one time
        private string titleFile;				// Name of the file, displayed at the top
		private string saveFile;				// The path of the saved file, initially set to ""
        private bool insertMode;				// Variable for InsertMode

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
                }
                else {
                    spreadsheetPanel1.Focus();
                    insertMode = false;
                }
            }
        }

        /// <summary>
        /// Constructs a new spreadsheet
        /// </summary>
        public Form1() {
            InitializeComponent();


            // This an example of registering a method so that it is notified when
            // an event happens.  The SelectionChanged event is declared with a // delegate that specifies that all methods that register with it must // take a SpreadsheetPanel as its parameter and return nothing.  So we
            // register the displaySelection method below.

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



		/************************** VISUAL CODE **************************/



							/*** New ***/

        // Deals with the New menu
        private void newToolStripMenuItem1_Click(object sender, EventArgs e) {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            totalSpreadsheets++;
            MyApplicationContext.getAppContext().RunForm(new Form1());
        }

							/*** Open ***/

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            totalSpreadsheets++;
            Open();
        }

							/*** Save ***/

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
			Save();
        }

							/*** Save As ***/

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveAs();
        }

							/*** Exit ***/

        // Deals with the Exit menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
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

			if (spreadsheet.Changed) {
                DialogResult result = MessageBox.Show(
                    "Do you want to save the changes you made to '" + titleFile + "'?",
                    "Spreadsheet",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                switch (result) {
					case DialogResult.Yes:
                        if (Save() == DialogResult.Cancel)
                            e.Cancel = true;
                        break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
                        break;
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
					ChangeCellValue(ss);

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
            ChangeCellValue(spreadsheetPanel1);
        }

        private void cellContentsText_KeyDown(object sender, KeyEventArgs e) {
			int col, row;
			switch (e.KeyCode) {
				case Keys.Return:
					if (!ChangeCellValue(spreadsheetPanel1))
						break;
					spreadsheetPanel1.GetSelection(out col, out row);
					ChangeSelection(col, row + 1);
					break;
				case Keys.Tab:
					if (!ChangeCellValue(spreadsheetPanel1))
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
        private bool ChangeCellValue(SS.SpreadsheetPanel ss) {
			int row, col;
			ss.GetSelection(out col, out row);

            string name = GetCellName(col, row);

            ISet<string> set = new HashSet<string>();
            try {
                // Set the contents of the spreadsheet cell
                set = spreadsheet.SetContentsOfCell(name, contentsTextBox.Text);
                foreach (string s in set) {
                    DisplayValueInTable(s);
                }

                // Display the new value in both the cell and the Value text box
                DisplayValueInTable(name);
                valueTextBox.Text = GetValue(name);
            }
            catch (SS.CircularException e) {
                MessageBox.Show(
                    "ERROR: The formula you entered causes a circular dependency.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
				return false;
            }
            catch (SpreadsheetUtilities.FormulaFormatException e) {
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
            if (spreadsheet.Changed)
                this.Text = titleFile + "*";
            else
                this.Text = titleFile;

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
            }
            catch (SS.SpreadsheetReadWriteException e) {
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

    }

}
