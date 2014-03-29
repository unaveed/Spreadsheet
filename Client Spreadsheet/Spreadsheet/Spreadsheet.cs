using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS {
    // PARAGRAPHS 2 and 3 modified for PS5.
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet{

        private DependencyGraph dg;				// Maps the dependencies
        private Dictionary<string, Cell> cells;	// Dictionary<name, Cell>
        private bool changed;					// Keeps track of whether or not the spreadsheet has been changed since last save

		/// <summary>
		/// Creates a new Spreadsheet object.  This constructor sets the validity function
        /// to return true in every case, the normalize function to return the original
        /// string, and sets the version to 'default'.
		/// </summary>
        public Spreadsheet()
            : this(x => true, s => s, "default") {
        }

		/// <summary>
		/// Creates a new Spreadsheet object.  This constructor allows the user to provide
        /// a validity function, a normalizing function, and a version.
		/// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version) {
            dg = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }
  
		/// <summary>
		/// Creates a new Spreadsheet object.  This constructor allows the user to provide the
        /// path to a spreadsheet file, as well as the validity function, normalizing function,
        /// and version.
		/// </summary>
        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : this(isValid, normalize, version) {
			try {
				using (XmlReader reader = XmlReader.Create(file)) {
					string name = null;
					string contents = null;
					while (reader.Read()) {
						if (reader.IsStartElement()) {
							switch (reader.Name) {
								case "spreadsheet":
									string v = reader["version"];
									if (v != version)
										throw new SpreadsheetReadWriteException("Invalid version");
									break;
								case "cell":
									break;
								case "name":
									reader.Read();
									name = reader.Value;
									break;
								case "contents":
									reader.Read();
									contents = reader.Value;
									break;
							}
						}
						else
							if (reader.Name == "cell")
								SetContentsOfCell(name, contents);
					}
				}
			}
			catch (Exception e) {
				throw new SpreadsheetReadWriteException(e.Message);
			}
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells() {
            List<string> lst = new List<string>(cells.Keys);
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(lst);
            return set;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name) {
			// If name isn't in cells, then it is either invalid, or it was never added in the first place.
			if (name == null || !ValidName(Normalize(name)))
                throw new InvalidNameException();
            Cell cell;
            cells.TryGetValue(Normalize(name), out cell);
            if (ReferenceEquals(cell, null))
                return "";
            return cell.Content;
        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number) {
            if (name == null || !ValidName(name))
                throw new InvalidNameException();


            // Clear previous dependencies
            if (cells.ContainsKey(name))
                RemoveDependency(name);

            dg.ReplaceDependents(name, new HashSet<string>());
            HashSet<string> a = (HashSet<string>)dg.GetDependents(name);
            bool b = dg.HasDependents(name);
            bool c = dg.HasDependees(name);

            // Cell already exists
            if (cells.ContainsKey(name)) {
                Cell cell;
                cells.TryGetValue(name, out cell);
                cell.Content = number;
            }
            // Cell doesn't exist, so create it
            else
                cells.Add(name, new Cell(number, GetCell));

            LinkedList<string> lst = (LinkedList<string>)GetCellsToRecalculate(name);
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(lst);
            return set;
        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text) {
            if (text == null)
                throw new ArgumentNullException();

            if (name == null || !ValidName(name))
                throw new InvalidNameException();

            // Clear previous dependencies
            if (cells.ContainsKey(name))
                RemoveDependency(name);

            if (text == "") {
                cells.Remove(name);
                return new HashSet<string>(GetCellsToRecalculate(name));
            }

            // Cell already exists
            if (cells.ContainsKey(name)) {
                Cell cell;
                cells.TryGetValue(name, out cell);
                cell.Content = text;
            }
            // Cell doesn't exist, so create it
            else
                cells.Add(name, new Cell(text, GetCell));

            LinkedList<string> lst = (LinkedList<string>)GetCellsToRecalculate(name);
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(lst);
            return set;

        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, SpreadsheetUtilities.Formula formula) {
            if (formula == null)
                throw new ArgumentNullException("Error: Formula paramter was null.");

            // Checks if cell name is valid and not null
            if (name == null || !ValidName(name))
                throw new InvalidNameException();

            List<string> cellNames = new List<string>();
            Cell cellValue;

            // Holds the cell names that will be returned
            HashSet<string> result = new HashSet<string>();

            // Holds the contents of the original cell
            object o = null; 


            if (cells.ContainsKey(name)) {
                o = cells[name].Content;
                RemoveDependency(name);
            }

            // Save the content of the cell to store in the dictionary
            cellValue = new Cell(formula, GetCell);
            cells.Add(name, cellValue);

            // cellNames gets a list of all of the cell names in the formula and creates a list 
            // to add to the dependency graph
            cellNames = formula.GetVariables().ToList();

            // Add the values associated to name to the dependency graph
            foreach (string n in cellNames) {
                dg.AddDependency(name, n);
            }

            try {
                //saves the direct and indirect values into tempI and then put them into my returnSet
                //also checks for circular dependency and recalculates the values in the cells associate with the name
                IEnumerable<string> tempI = GetCellsToRecalculate(name);//recalculates the values and checks for circular dependency
                foreach (string n in tempI)
                    result.Add(n);
            }
            catch (CircularException c) {

                //method comes here if a circularexeption is found when calling the GetCellsToRecalculate method
                //originalCellContent is saved earlier in the code to allow us to go back to the original values to get
                //rid of the circular dependency
                if (o != null) {
                    if (o is Formula) {
                        Formula originalFormula = new Formula(o.ToString());

                        SetCellContents(name, originalFormula);
                    }
                    else if (o is String) {
                        SetCellContents(name, o.ToString());
                    }
                    else if (o is double) {
                        SetCellContents(name, Convert.ToDouble(o));
                    }

                }
                else
                    SetCellContents(name, "");
                throw c;
            }
            return result;
        }

        /// <summary>
        /// Removes all of the dependencies from the cell.
        /// </summary>
        private void RemoveDependency(string name) {
            if (dg.HasDependents(name)) {
                List<string> dgDependents = dg.GetDependents(name).ToList();
                foreach (string n in dgDependents)
                    dg.RemoveDependency(name, n);
            }
            // Remove the cell
            cells.Remove(name);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name) {
            if (name == null)
                throw new ArgumentNullException();

			// Change name to normalized version
            name = Normalize(name);

            if (!ValidName(name) || !IsValid(name))
                throw new InvalidNameException();

            return dg.GetDependees(name);
        }

		/// <summary>
		/// Returns true if s is considered a valid cell name.
		/// </summary>
        private bool ValidName(string s) {
            return Regex.IsMatch(s, "^[a-zA-Z][a-zA-Z0-9]*$");
        }

        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed {
            get {
                return changed;
            }
            protected set {
                changed = value;
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename) {
            string version;
            try {
                using (XmlReader reader = XmlReader.Create(filename)) {
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    // Get version
                    version = reader.GetAttribute(0);
                }
            }
            catch (Exception) {
                throw new SpreadsheetReadWriteException("Unable to read the xml file");
            }
            return version;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            try {
                using (XmlWriter writer = XmlWriter.Create(filename, settings)) {
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (KeyValuePair<string, Cell> kv in cells) {
                        // Start new cell
                        writer.WriteStartElement("cell");

                        writer.WriteElementString("name", kv.Key);

                        // Write contents
                        if (kv.Value.Content is Formula)
                            writer.WriteElementString("contents", "=" + kv.Value.Content.ToString());
                        else
                            writer.WriteElementString("contents", kv.Value.Content.ToString());

                        // End cell tag
                        writer.WriteEndElement();
                    }

                    // End spreadsheet tag
                    writer.WriteEndElement();
                }

                // Change spreadsheet state to not changed
                Changed = false;
            }
            catch (Exception) {
                throw new SpreadsheetReadWriteException("Invalid file name");
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name) {
            if (name == null)
                throw new InvalidNameException();

			// Change name to normalized version
            name = Normalize(name);

            if (!ValidName(name) || !IsValid(name))
                throw new InvalidNameException();

            Cell cell;
            if (cells.TryGetValue(name, out cell))
                return cell.Value;

			// Cell doesn't exist, so return an empty string
            return "";
        }

		/// <summary>
		/// Function used to lookup variable values for formulas.  Throws ArgumentException
        /// if cell contents can't be parsed to a double.
		/// </summary>
		private double GetCell(string name) {
			name = Normalize(name);
            if (cells.ContainsKey(name))
                if (cells[name].Value is double)
                    return (double)cells[name].Value;
            throw new ArgumentException();
        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content) {
            if (content == null)
                throw new ArgumentNullException();

            if (name == null || name == "")
                throw new InvalidNameException();

			// Change name to normalized version
            name = Normalize(name);

            if (!ValidName(name) || !IsValid(name))
                throw new InvalidNameException();

            ISet<string> set;

			// Check if content is a double
            double value;
            if (double.TryParse(content, out value))
                set = SetCellContents(name, value);

			// Check if content is a formula
            else if (content.StartsWith("="))
                set = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));

			// Content must be a string
            else
                set = SetCellContents(name, content);
			
			ReevaluateCells(set);

			// Change spreadsheet state to 'changed'
            Changed = true;

			return set;
        }

        private void ReevaluateCells(ISet<string> set) {
			Cell cell;
			foreach (string name in set) {
				cells.TryGetValue(name, out cell);
				if (cell != null)
					cell.Reevaluate();
            }
        }

    }

	/// <summary>
	/// Represents a single spreadsheet cell.
	/// </summary>
	class Cell {

        private object content;
        private Func<string, double> Lookup;

		public object Content {
			get {
				return content;
            }
			set {
                content = value;

				// Content is a formula, so compute
				if (content is Formula) {
                    Formula f = (Formula)content;
                    Value = f.Evaluate(Lookup);
                }

				else
					Value = content;
			}
        }

        public object Value { get; private set; }	// Value of the cell, which can be a double, string, or Formula

        public Cell(object c, Func<string, double> lookup) {
            Lookup = lookup;
            Content = c;
        }

		/// <summary>
		/// Reevaluates the value for this cell.
		/// </summary>
		/// <param name="f"></param>
        public void Reevaluate() {
			// Check if cell contents are a formula, and if so, reevaluate
            if (Content is Formula)
                Content = Content;
            return;
        }

    }
}
