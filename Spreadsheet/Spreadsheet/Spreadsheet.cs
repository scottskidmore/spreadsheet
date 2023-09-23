using System;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{
    public class Cell
    {
       
        public object contents;
        public Cell(object content)
        {
            contents = content;
            



        }
    }
    public class Spreadsheet : AbstractSpreadsheet
	{
        private Dictionary<string, Cell> cells;
        public DependencyGraph dg;
        public Spreadsheet():base("default")
        {
           cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

        }
        public Spreadsheet( Func<string, bool> isValid, Func<string, string> normalize,string version):base(version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

        }
        public Spreadsheet(string file,Func<string, bool> isValid, Func<string, string> normalize, string version):base(version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

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
            if (cells.ContainsKey(name))
            {
                return cells[name].contents;
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
            return cells.Keys.ToArray();
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
           
            if (cells.ContainsKey(name))
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
                cells[name].contents = number;
                GetCellsToRecalculate(name);
            }
            else
            {
                Cell cell = new Cell(number);
                cells.Add(name, cell);
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
           
            if (cells.ContainsKey(name))
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
                    cells.Remove(name);
                    GetCellsToRecalculate(name);
                    return GetDirectDependents(name).ToList();
                }
                cells[name].contents = text;
                GetCellsToRecalculate(name);
            }

            else
            {
                if (text == "")
                {
                    
                    return GetDirectDependents(name).ToList();
                }
                Cell cell = new Cell(text);
                cells.Add(name, cell);
            }
            List<string> list= DependencyList(name);
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
            
            if (cells.ContainsKey(name))
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

                cells[name].contents = formula;
                GetCellsToRecalculate(name);
            }
            else
            {
                IEnumerable<string> vars=formula.GetVariables();
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

                cells.Add(name, cell);
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
           if ( cells.TryGetValue(name,out Cell? cell))
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
            if (cells.TryGetValue(name, out Cell? cell))
            {


                foreach (string dep in dg.GetDependees(name))
                {
                    
                    dependents.Add(dep);
                    dependents=dependents.Union(DependencyList(dep)).ToList();

                }
            }
            return dependents;
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }
            if (cells.ContainsKey(name))
            {
                
                if (cells[name].contents is Formula)
                {
                    return ((Formula)cells[name].contents).Evaluate(LookUp);
                }
                return cells[name].contents;
            }
            else
            {
                return "";
            }
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (!Valid(name))
            {
                throw new InvalidNameException();
            }

            if (double.TryParse(content, out double d))
            {
                return SetCellContents(name, d);
            }
            if (content.ToArray()[0] =='=')
            {
                return SetCellContents(name, new Formula(content.Substring(1, (content.Length)-1)));
            }
            else
            {
                return SetCellContents(name, content);
            }
        }
        public double LookUp(string name)

        {
            object ans = GetCellValue(name);
            if (ans is double)
            {
                return (double)ans;
            }
            object final="";
            if (cells[name].contents is Formula)
            {
                final=((Formula)cells[name].contents).Evaluate(LookUp);
            }
            if (final is double)
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

