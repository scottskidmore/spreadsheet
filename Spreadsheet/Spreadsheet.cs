using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{
    /// <summary>
    /// Class that handles contructing cells for
    /// the spread sheet to use.
    /// </summary>
    public class Cell
    {

        public object contents;
        public string StringForm { get; set; }
        /// <summary>
        /// Constructor used to convert Json back 
        /// into cell objects.
        /// </summary>
        /// <param name="StringForm"></param>
        [JsonConstructor]
        public Cell(string StringForm)
        {
            this.StringForm = StringForm;
            contents = "";

        }
        /// <summary>
        /// Constructor used by the spreadsheet to add
        /// cells.
        /// </summary>
        /// <param name="content"></param>
        public Cell(object content)
        {
            StringForm = "";
            if (content is Formula)
            {
                StringForm = "=" + ((Formula)content).ToString();


            }
            if (content is double)
            {
                StringForm = ((double)content).ToString();

            }
            if (content is string)
            {
                StringForm = ((string)content).ToString();


            }


            contents = content;





        }
    }
    /// <summary>
    /// Spreadsheet that has the functionality described in
    /// AbstractSpreadsheet class. Used to create a spreadsheet
    /// model and store the relationships between cells.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        public Dictionary<string, Cell> Cells { get; }
        private DependencyGraph dg;
        private Func<string, string> normalizer;
        private Func<string, bool> validator;
        /// <summary>
        /// Constructor used to convert Json into a spreadsheet
        /// with a backing dictionary for the cells and a version
        /// loaded in from Json.
        /// </summary>
        [JsonConstructor]
        public Spreadsheet(Dictionary<string, Cell> cells, string Version) : base(Version)
        {
            this.Cells = cells;
            dg = new DependencyGraph();
            normalizer = s => s;
            validator = s => true;

        }
        /// <summary>
        /// Constructor creates an empty spreadsheet
        /// with a default version and cell name functions
        /// </summary>
        public Spreadsheet() : base("default")
        {
            Cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            normalizer = s => s;
            validator = s => true;

        }
        /// <summary>
        /// Constructor that creates an empty spread sheet
        /// with defined cell name functions and a version
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(version)
        {
            Cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            normalizer = normalize;
            validator = isValid;


        }
        /// <summary>
        /// Constructor used to load a spreadsheet from a file
        /// and add functions to control cell names and a version.
        /// </summary>
        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(version)
        {
            Cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            string f;
            Spreadsheet? s;
            try
            {
                f = File.ReadAllText(file);
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Failed to read from file");
            }
            try
            {


                s = JsonSerializer.Deserialize<Spreadsheet>(f);
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Failed to deserialize from json");
            }
            if (s is null)
            {
                throw new SpreadsheetReadWriteException("File returned null");
            }
            if (s.Cells is null)
            {
                normalizer = normalize;
                validator = isValid;
            }
            if (s.Version !=version)
            {
                throw new SpreadsheetReadWriteException("Version miss match");
            }
            else
            {

                normalizer = normalize;
                validator = isValid;

                List<KeyValuePair<string, Cell>> list = s.Cells.ToList();
                foreach (KeyValuePair<string, Cell> v in list)
                {
                    SetContentsOfCell(v.Key, v.Value.StringForm);
                }
            }




        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.
        /// The return value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }
            if (!validator(name))
            {
                throw new InvalidNameException();
            }
            name = normalizer(name);
            if (Cells.ContainsKey(name))
            {
                return Cells[name].contents;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return Cells.Keys.ToArray();
        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {

            if (Cells.ContainsKey(name))
            {
                if (GetCellContents(name) is Formula)
                {
                    Formula? formula = GetCellContents(name) as Formula;
                    if (!object.Equals(formula, null))
                    {
                        IEnumerable<string> vars = formula.GetVariables();

                        foreach (string var in vars)

                        {
                            dg.RemoveDependency(name, var);
                        }
                    }



                }
                Cells[name].contents = number;
                Cells[name].StringForm = number.ToString();

            }
            else
            {
                Cell cell = new Cell(number);
                Cells.Add(name, cell);
            }
            List<string> list = DependencyList(name);
            list.Add(name);
            return list;
        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {

            if (Cells.ContainsKey(name))
            {
                if (GetCellContents(name) is Formula)
                {
                    Formula? formula = GetCellContents(name) as Formula;
                    if (!object.Equals(formula, null))
                    {
                        IEnumerable<string> vars = formula.GetVariables();

                        foreach (string var in vars)

                        {
                            dg.RemoveDependency(name, var);
                        }
                    }



                }
                if (text == "")
                {
                    Cells.Remove(name);

                    return GetDirectDependents(name).ToList();
                }
                Cells[name].contents = text;
                Cells[name].StringForm = text;
                GetCellsToRecalculate(name);
            }

            else
            {
                if (text == "")
                {

                    return GetDirectDependents(name).ToList();
                }
                Cell cell = new Cell((object)text);
                Cells.Add(name, cell);
            }
            List<string> list = DependencyList(name);
            list.Add(name);
            return list;
        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            if (Cells.ContainsKey(name))
            {
                if (GetCellContents(name) is Formula)
                {
                    Formula? currentformula = GetCellContents(name) as Formula;
                    if (!object.Equals(currentformula, null))
                    {
                        IEnumerable<string> currentvars = currentformula.GetVariables();

                        foreach (string var in currentvars)

                        {
                            dg.RemoveDependency(var, name);
                        }
                    }



                }

                IEnumerable<string> vars = formula.GetVariables();

                foreach (string var in vars)

                {
                    if (var == name)
                    {
                        throw new CircularException();
                    }
                    dg.AddDependency(var, name);
                }

                Cells[name].contents = formula;
                Cells[name].StringForm = "="+formula.ToString();
                GetCellsToRecalculate(name);
            }
            else
            {
                IEnumerable<string> vars = formula.GetVariables();
                Cell cell = new Cell(formula);
                foreach (string var in vars)

                {
                    if (var == name)
                    {
                        throw new CircularException();
                    }

                }
                foreach (string var in vars)

                {
                    List<string> dependent = DependencyList(var);
                    if (dependent.Contains(name))
                    {
                        throw new CircularException();
                    }

                    dg.AddDependency(var, name);
                }

                Cells.Add(name, cell);
            }
            List<string> dependents = DependencyList(name);

            dependents.Add(name);
            return dependents;


        }
        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
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
        protected override IEnumerable<string> GetDirectDependents(string name)
        {


            if (Cells.TryGetValue(name, out Cell? cell))
            {
                return dg.GetDependents(name);
            }

            return new List<string>();

        }
        /// <summary>
        /// Checks if the name is a valid cell name
        /// </summary>

        private bool Valid(string cellname)
        {
            return Regex.IsMatch(cellname, @"^[A-Za-z_][0-9A-Za-z_]*$");
        }
        /// <summary>
        /// Creates a list of all the cells that use name cell directly
        /// and indirectly
        /// </summary>
        private List<string> DependencyList(string name)
        {

            List<string> dependents = new List<string>();
            if (Cells.TryGetValue(name, out Cell? cell))
            {


                foreach (string dep in dg.GetDependees(name))
                {

                    dependents.Add(dep);
                    dependents = dependents.Union(DependencyList(dep)).ToList();

                }
            }
            return dependents;
        }
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using a JSON format.
        /// The JSON object should have the following fields:
        /// "Version" - the version of the spreadsheet software (a string)
        /// "Cells" - a data structure containing 0 or more cell entries
        ///           Each cell entry has a field (or key) named after the cell itself 
        ///           The value of that field is another object representing the cell's contents
        ///               The contents object has a single field called "StringForm",
        ///               representing the string form of the cell's contents
        ///               - If the contents is a string, the value of StringForm is that string
        ///               - If the contents is a double d, the value of StringForm is d.ToString()
        ///               - If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
        /// 
        /// For example, if this spreadsheet has a version of "default" 
        /// and contains a cell "A1" with contents being the double 5.0 
        /// and a cell "B3" with contents being the Formula("A1+2"), 
        /// a JSON string produced by this method would be:
        /// 
        /// {
        ///   "Cells": {
        ///     "A1": {
        ///       "StringForm": "5"
        ///     },
        ///     "B3": {
        ///       "StringForm": "=A1+2"
        ///     }
        ///   },
        ///   "Version": "default"
        /// }
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {

            string json = JsonSerializer.Serialize(this);

            if (json != null)
            {
                try
                {
                    File.WriteAllText(filename, json);
                    this.Changed = false;
                }
                catch
                {
                    throw new SpreadsheetReadWriteException("Failed to write to file");
                }
            }
        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }
            if (!validator(name))
            {
                throw new InvalidNameException();
            }
            name = normalizer(name);
            if (Cells.ContainsKey(name))
            {

                if (Cells[name].contents is Formula)
                {
                    return ((Formula)Cells[name].contents).Evaluate(LookUp);
                }
                return Cells[name].contents;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
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
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }
            if (!validator(name))
            {
                throw new InvalidNameException();
            }
            name = normalizer(name);
            this.Changed = true;
            if (double.TryParse(content, out double d))
            {
                return SetCellContents(name, d);
            }
            if (content.Length > 0 && content.ToArray()[0] == '=')
            {
                return SetCellContents(name, new Formula(content.Substring(1, (content.Length) - 1), normalizer, validator));
            }
            else
            {
                return SetCellContents(name, content);
            }
        }
        /// <summary>
        /// Used to Lookup references for the evaluate formula method.
        /// returns an argument exception if the value is invalid.
        /// </summary>
        public double LookUp(string name)

        {
            object ans = GetCellValue(name);
            if (ans is double)
            {
                return (double)ans;
            }


            else
            {
                throw new ArgumentException("Invalid Formula or Bad Reference");
            }
        }
    }
}

