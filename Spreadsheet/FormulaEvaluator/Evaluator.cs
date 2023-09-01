using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FormulaEvaluator;
/// <summary>
/// Class that provides the methods required to evaluate an arithmetic expressions using standard infix notation.
/// </summary>
public static class Evaluator
{
    /// <summary>
    /// Delegate that will retrieve the value of a reference provided in the equation.
    /// </summary>
    /// <param name="v">The reference value in the equation.</param>
    /// <returns>An integer value of the reference value.</returns>
    public delegate int Lookup(String v);

    /// <summary>
    /// Method for evaluating the integer value of an arithmetic expressions using standard infix notation.
    /// </summary>
    /// <param name="exp">The expression to be evaluated.</param>
    /// <param name="variableEvaluator">The delegate that will return the value of a reference value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static int Evaluate(String exp, Lookup variableEvaluator)
    {
        string trimmed = String.Concat(exp.Where(c => !Char.IsWhiteSpace(c)));
        string[] substrings = Regex.Split(trimmed, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
        Stack<int> values = new Stack<int>();
        Stack<string> op = new Stack<string>();
        foreach (string s in substrings)
        {
            string type = stringType(s);
            switch (type)
            {
                case "Integer":
                    if (topStack(op) is "/" or "*")
                    {
                        string operate = op.Pop();
                        if (values.Count <= 0)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        if (int.Parse(s) == 0)
                        {
                            throw new ArgumentException("Invalid 0 division.");
                        }
                        if (operate == "/")
                        {
                            values.Push(value / int.Parse(s));
                        }
                        if (operate == "*")
                        {
                            values.Push(value * int.Parse(s));
                        }
                    }
                    else
                    {
                        values.Push(int.Parse(s));
                    }
                    break;
                case "Reference":
                    if (topStack(op) is "/" or "*")
                    {
                        string operate = op.Pop();
                        if (values.Count <= 0)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        if (variableEvaluator(s) == 0)
                        {
                            throw new ArgumentException("Invalid 0 division.");
                        }
                        if (operate == "/")
                        {
                            values.Push(value / variableEvaluator(s));
                        }
                        if (operate == "*")
                        {
                            values.Push(value * variableEvaluator(s));
                        }
                    }
                    else
                    {
                        values.Push(variableEvaluator(s));
                    }
                    break;
                case "Add":
                    if (topStack(op) is "+" or "-")
                    {
                        string operate = op.Pop();
                        if (values.Count <= 1)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        int value2 = values.Pop();
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
                        if (values.Count <= 1)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        int value2 = values.Pop();
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
                        if (values.Count <= 1)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        int value2 = values.Pop();
                        if (operate == "+")
                        {
                            values.Push(value2 + value);
                        }
                        if (operate == "-")
                        {
                            values.Push(value2 - value);
                        }
                        if (topStack(op) != "(")
                        {
                            throw new ArgumentException("equation format out of order.");
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
                        if (values.Count <= 1)
                        {
                            throw new ArgumentException("equation format out of order.");
                        }
                        int value = values.Pop();
                        int value2 = values.Pop();
                        if (value == 0)
                        {
                            throw new ArgumentException("Invalid 0 division.");
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
        if (op.Count <= 0&&values.Count==1)
        {
            return values.Pop();
        }
        if(op.Count==1&&values.Count==2)
        {
            string operate = op.Pop();
            if (values.Count <= 1)
            {
                throw new ArgumentException("equation format out of order.");
            }
            int value = values.Pop();
            int value2 = values.Pop();
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
        throw new ArgumentException("equation format out of order.");
    }
        
    /// <summary>
    /// Helper method that finds the type of a character in the expression.
    /// </summary>
    /// <param name="s">The string representation of the character.</param>
    /// <returns>The type of the character as a string.</returns>
    public static string stringType(String s)
    {
        if( int.TryParse(s, out _))
        {
            return "Integer";
        }
        if (Regex.IsMatch(s, "^[A-Za-z]+[0-9]+$"))
        {
            return "Reference";
        }
        if (s == "+" )
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
        if (s == ")")
        {
            return "RightParenthesis";
        }
        else
        {
            return "";
        }
    }
    /// <summary>
    /// Helper method that returns the top value of a stack without removing it.
    /// </summary>
    /// <param name="s">The stack that needs to be checked.</param>
    /// <returns>The value on the top of the stack.</returns>
    public static string topStack(Stack<string> s)
    {
        if (s.Count <= 0){
            return "";
        }
        else
        {
            return s.Peek();
        }
    }
}

