namespace SpreadsheetConsoleApp;
/// <summary>
/// Class for testing the evaluator methods.
/// </summary>
class Program
{
    /// <summary>
    /// A test delegate created to reflect how one would really function.
    /// </summary>
    /// <param name="v">The string of the reference we are looking for.</param>
    /// <returns>The integer value of a reference</returns>
    public static int Lookup(string v)
    {
        if (v == "A1")
        {
            return 10;
        }
        if (v == "a2")
        {
            return 2;
        }
        else
        {
            return 5;
        }
    }
    /// <summary>
    /// Main method for running tests on the evaluator class.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        //int var = FormulaEvaluator.Evaluator.Evaluate("(2+2)*2", null);
        //Console.WriteLine("(2+2)*2"+" Expected:8 Got:"+var);
        FormulaEvaluator.Evaluator.Lookup test = Lookup;
        int var1 = FormulaEvaluator.Evaluator.Evaluate("A1+5*2", test);
        Console.WriteLine("A1+5*2" + " Expected:20 Got:" + var1);
        int var2 = FormulaEvaluator.Evaluator.Evaluate("5/(a2-1)", test);
        Console.WriteLine("5/(a2-1)" + " Expected:5 Got:" + var2);
        int var3 = FormulaEvaluator.Evaluator.Evaluate("10/5", test);
        Console.WriteLine("10/5" + " Expected:2 Got:" + var3);
        int var4 = FormulaEvaluator.Evaluator.Evaluate("10/a2", test);
        Console.WriteLine("10/a2" + " Expected:5 Got:" + var4);
        int var5 = FormulaEvaluator.Evaluator.Evaluate("10-a2-5", test);
        Console.WriteLine("10-a2-5" + " Expected:3 Got:" + var5);
        int var6 = FormulaEvaluator.Evaluator.Evaluate("(1*1)-2/2", test);
        Console.WriteLine("(1*1)-2/2" + " Expected:2 Got:" + var6);
        //int var7 = FormulaEvaluator.Evaluator.Evaluate("(10-a2)/0", test);
        //Console.WriteLine("(10-a2)/0" + " Expected: Got:" + var7);
    }
}

