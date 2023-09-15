// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!
// Last updated: August 2023 (small tweak to API)

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities;

/// <summary>
/// Represents formulas written in standard infix notation using standard precedence
/// rules.  The allowed symbols are non-negative numbers written using double-precision
/// floating-point syntax (without unary preceeding '-' or '+');
/// variables that consist of a letter or underscore followed by
/// zero or more letters, underscores, or digits; parentheses; and the four operator
/// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
/// and "x 23" consists of a variable "x" and a number "23".
///
/// Associated with every formula are two delegates: a normalizer and a validator.  The
/// normalizer is used to convert variables into a canonical form. The validator is used to
/// add extra restrictions on the validity of a variable, beyond the base condition that
/// variables must always be legal: they must consist of a letter or underscore followed
/// by zero or more letters, underscores, or digits.
/// Their use is described in detail in the constructor and method comments.
/// </summary>
public class Formula
{
    private List<string> formula;
    public List<string> get()
    {
        return formula;
    }
    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    ///  string lpPattern = @"\(";
    public Formula(string formula) :
        this(formula, s => s, s => true)
    {  
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.
    ///
    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    ///
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
        this.formula = new List<string>();
        if (formula == "")
        {
            throw new FormulaFormatException("Formula was empty");
        }
        int lpcount = 0;
        int rpcount = 0;
        int count = 0;
        string previous = "";
        foreach (string s in GetTokens(formula))
        {
            string v=normalize(s);
            if (!isValid(v))
            {
                throw new FormulaFormatException("Passed in validation function failed");
            }
            if (rpcount > lpcount)
            {
                throw new FormulaFormatException("Paranthesis out of order");
            }
            if (Regex.IsMatch(v,@"\("))
            {
                if (Regex.IsMatch(previous, @"\)") || Regex.IsMatch(previous, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || double.TryParse(previous, out _))
                {
                    throw new FormulaFormatException("Missing operator or closing paranthesis");
                }
                if (count == GetTokens(formula).Count() - 1)
                {
                    throw new FormulaFormatException("Paranthesis out of order");
                }
                count++;
                lpcount++;
                previous = v;
                this.formula.Add(v);
                continue;
            }
            if (Regex.IsMatch(v,@"\)"))
            {

                if (Regex.IsMatch(previous, @"[\+\-*/]") || Regex.IsMatch(previous, @"\("))
                {
                    throw new FormulaFormatException("Empty parenthesis");
                }
                if (count == 0)
                {
                    throw new FormulaFormatException("Paranthesis out of order");
                }
                count++;
                rpcount++;
                previous = v;
                this.formula.Add(v);
                continue;
            }
            if (double.TryParse(v, out _))
            {
                if (Regex.IsMatch(previous, @"\)") || Regex.IsMatch(previous, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || double.TryParse(previous, out _))
                {
                    throw new FormulaFormatException("Missing operator or closing paranthesis");
                }
                count++;
                previous = v;
                this.formula.Add(v);
                continue;
            }
            if (Regex.IsMatch(v, @"^[+=\-*/]"))
            {
                if (Regex.IsMatch(previous, @"^[+=\-*/]") || Regex.IsMatch(previous, @"\("))
                {
                    throw new FormulaFormatException("Operators out of order");
                }
                if (count == GetTokens(formula).Count() - 1)
                {
                    throw new FormulaFormatException("Cannot end with an operator");
                }
                if (count == 0)
                {
                    throw new FormulaFormatException("Cannot start with an operator");
                }
                count++;
                previous = v;
                this.formula.Add(v);
                continue;
            }
            if (Regex.IsMatch(v,@"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
            {
                if (Regex.IsMatch(previous, @"\)") || Regex.IsMatch(previous, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || double.TryParse(previous, out _))
                {
                    throw new FormulaFormatException("Missing operator or closing paranthesis");
                }
                count++;
                previous = v;
                this.formula.Add(v);
                continue;
            }
           
            
            else
            {
                throw new FormulaFormatException("At least one invalid token in formula");
            }


        }
        if (lpcount != rpcount)
        {
            throw new FormulaFormatException("Paranthesis count missmatch");
        }

        
    }

    /// <summary>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.  When a variable symbol v needs to be determined, it should be looked up
    /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
    /// the constructor.)
    ///
    /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
    /// in a string to upper case:
    ///
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    ///
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise).
    ///
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
    /// The Reason property of the FormulaError should have a meaningful explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
    {
        
        List<string> substrings = this.formula;
        Stack<double> values = new Stack<double>();
        Stack<string> op = new Stack<string>();
        foreach (string s in substrings)
        {
            string type = stringType(s);
            switch (type)
            {
                case "Double":
                    if (topStack(op) is "/" or "*")
                    {
                        string operate = op.Pop();
                       
                        double value = values.Pop();
                        if (double.Parse(s) == 0.0)
                        {
                            return new FormulaError("Invalid 0 division.");
                        }
                        if (operate == "/")
                        {
                            values.Push(value / double.Parse(s));
                        }
                        if (operate == "*")
                        {
                            values.Push(value * double.Parse(s));
                        }
                    }
                    else
                    {
                        values.Push(double.Parse(s));
                    }
                    break;
              
                case "Reference":
                    if (topStack(op) is "/" or "*")
                    {
                        string operate = op.Pop();
                       
                        double value = values.Pop();
                        try

                        {

                            lookup(s);

                        }

                        catch (ArgumentException)
                        {
                            return new FormulaError("No reference found.");
                        }

                        
                            if (lookup(s) == 0.0)
                        {
                            return new FormulaError("Invalid 0 division.");
                        }
                        if (operate == "/")
                        {
                            values.Push(value / lookup(s));
                        }
                        if (operate == "*")
                        {
                            values.Push(value * lookup(s));
                        }
                    }
                    else
                    {
                        values.Push(lookup(s));
                    }
                    break;
                case "Add":
                    if (topStack(op) is "+" or "-")
                    {
                        string operate = op.Pop();
                       
                        double value = values.Pop();
                        double value2 = values.Pop();
                        if (operate == "+")
                        {
                            values.Push(value2 + value);
                        }
                        if (operate == "-")
                        {
                            values.Push(value2 - value);
                        }
                    }
                    op.Push(s);
                    break;
                case "Subtract":
                    if (topStack(op) is "+" or "-")
                    {
                        string operate = op.Pop();
                        
                        double value = values.Pop();
                        double value2 = values.Pop();
                        if (operate == "+")
                        {
                            values.Push(value2 + value);
                        }
                        if (operate == "-")
                        {
                            values.Push(value2 - value);
                        }
                    }
                    op.Push(s);

                    break;
                case "Multiply":
                    op.Push(s);
                    break;
                case "Divide":
                    op.Push(s);
                    break;
                case "LeftParenthesis":
                    op.Push(s);
                    break;
                case "RightParenthesis":
                    if (topStack(op) is "+" or "-")
                    {
                        string operate = op.Pop();
                       
                        double value = values.Pop();
                        double value2 = values.Pop();
                        if (operate == "+")
                        {
                            values.Push(value2 + value);
                        }
                        if (operate == "-")
                        {
                            values.Push(value2 - value);
                        }
                        
                        op.Pop();

                    }
                    else if (topStack(op) == "(")
                    {
                        op.Pop();
                    }
                    if (topStack(op) is "/" or "*")
                    {
                        string operate = op.Pop();
                       
                        double value = values.Pop();
                        double value2 = values.Pop();
                        if (value == 0.0)
                        {
                            return new FormulaError("Invalid 0 division.");
                        }
                        if (operate == "*")
                        {
                            values.Push(value2 * value);
                        }
                        if (operate == "/")
                        {
                            values.Push(value2 / value);
                        }



                    }



                    break;
            }
        }
        if (op.Count <= 0 && values.Count == 1)
        {
            return values.Pop();
        }
        else
        {
            string operate = op.Pop();
            
            double value = values.Pop();
            double value2 = values.Pop();
            if (operate == "+")
            {
                values.Push(value2 + value);
            }
            if (operate == "-")
            {
                values.Push(value2 - value);
            }
            return values.Pop();
        }
        
    }
    /// <summary>
    /// Helper method that finds the type of a character in the expression.
    /// </summary>
    /// <param name="s">The string representation of the character.</param>
    /// <returns>The type of the character as a string.</returns>
    private static string stringType(String s)
    {
        if (double.TryParse(s, out _))
        {
            return "Double";
        }
        if (Regex.IsMatch(s, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
        {
            return "Reference";
        }
        if (s == "+")
        {
            return "Add";
        }
        if (s == "-")
        {
            return "Subtract";
        }
        if (s == "*")
        {
            return "Multiply";
        }
        if (s == "/")
        {
            return "Divide";
        }
        if (s == "(")
        {
            return "LeftParenthesis";
        }
        else
        {
            return "RightParenthesis";
        }
       
        
    }
    /// <summary>
    /// Helper method that returns the top value of a stack without removing it.
    /// </summary>
    /// <param name="s">The stack that needs to be checked.</param>
    /// <returns>The value on the top of the stack.</returns>
    private static string topStack(Stack<string> s)
    {
        if (s.Count <= 0)
        {
            return "";
        }
        else
        {
            return s.Peek();
        }
    }

    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this
    /// formula.  No normalization may appear more than once in the enumeration, even
    /// if it appears more than once in this Formula.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<string> GetVariables()
    {
        List<string> List = new List<string>();
        foreach (string v in formula)
        {

           
            if (Regex.IsMatch(v, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
            {
                bool match = false;
                foreach (string var in List)
                {
                    if (v == var)
                    {
                        match=true;
                    }
                }
                if (match)
                {
                    continue;
                }
                List.Add(v);
                continue;
            }
           
            
        }
        return List;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
        string formula = "";
        foreach(string var in this.formula)
        {
            formula += var;
        }
        return formula;
    }

    /// <summary>
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    ///
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" by
    /// using C#'s standard conversion from string to double (and optionally back to a string).
    /// Variable tokens are considered equal if their normalized forms are equal, as
    /// defined by the provided normalizer.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>

    public override bool Equals(object? obj)
    {
        if (obj==null|| obj.GetType() != typeof(Formula))
        {
            return false;
        }
        Formula f = (Formula)obj;
        string[] arr =f.get().ToArray();
        string[] arr1 = formula.ToArray();
        
        if (arr.Length != arr1.Length)
        {
            return false;
        }
        else
        {
            int i = 0;
            while (i < arr.Length){
                if (Regex.IsMatch(arr[i], @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                {
                   if (arr[i] == arr1[i])
                    {
                        i++;
                        continue;
                    }
                    return false;
                }
                if (Regex.IsMatch(arr[i], @"\("))
                {
                    if (arr[i] == arr1[i])
                    {
                        i++;
                        continue;
                    }
                    return false;
                }
                if (Regex.IsMatch(arr[i], @"\)"))
                {
                    if (arr[i] == arr1[i])
                    {
                        i++;
                        continue;
                    }
                    return false;
                }
                if (Regex.IsMatch(arr[i], @"[\+\-*/]"))
                {
                    if (arr[i] == arr1[i])
                    {
                        i++;
                        continue;
                    }
                    return false;
                }
               
                if (double.TryParse(arr[i], out double var))
                {
                    if(!double.TryParse(arr1[i], out double var1))
                    {
                        return false;
                    }
                    if (var == var1)
                    {
                        i++;
                        continue;
                    }
                    return false;
                }
                
                i++;
            }
            return true;
        }


       


        
    }

    /// <summary>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    /// Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {

        return !f1.Equals(f2);
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
       
        string[] arr = formula.ToArray();

       
            int i = 0;
        string final = "";
            while (i < arr.Length)
            {
                
                if (double.TryParse(arr[i], out double var))
                {
                arr[i] = var.ToString();
                    
                    
                }

            final += arr[i];
                i++;
            }

       
        return final.GetHashCode();
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a legal variable token;
    /// a double literal; and anything that doesn't match one of those patterns.
    /// There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(string formula)
    {
        // Patterns for individual tokens
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                yield return s;
            }
        }

    }
}

/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(string message) : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(string reason) : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}