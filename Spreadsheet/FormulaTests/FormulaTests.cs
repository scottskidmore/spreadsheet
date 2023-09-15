using SpreadsheetUtilities;

namespace FormulaTests;

[TestClass]
public class FormulaTests
{

    /// <summary>
    /// A test delegate created to reflect how one would really function.
    /// </summary>
    /// <param name="v">The string of the reference we are looking for.</param>
    /// <returns>The integer value of a reference</returns>
    public static double Lookups(string v)
    {
        if (v == "A1")
        {
            return 10;
        }
        if (v == "a2")
        {
            return 2;
        }
        if (v == "a4")
        {
            return 0;
        }
        else
        {
            return 5;
        }
    }
    [TestMethod]
    public void BasicConstructionTest()
    {
        Func<string, double> test = Lookups;
        Formula f = new Formula("2+2");
        Formula a = new Formula("(2+2)*q");
        Formula b = new Formula("((2+2))");
        Formula c = new Formula("(2+2-a1)");
        Formula d = new Formula("q*w*s  -2+2");
        Formula e = new Formula("0 / 5");
        Formula h = new Formula("5 + 7 + (5)*8");
        Formula g = new Formula("1*3-8/2+4*(8-9*2)/14*7");
        Formula i = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
        Formula j = new Formula("a4-a4*a4/a4");
        Formula k = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");


        Assert.AreEqual(4.0, f.Evaluate(test));
        Assert.AreEqual(-21.0, g.Evaluate(test));

    }
    [TestMethod]
    public void ScientificNotationTest()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E-06");
        Assert.AreEqual(12.000001234567, a.Evaluate(test));

    }
    [TestMethod]
    public void ScientificNotationTest2()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E06*12");
        Assert.AreEqual(14814816.0, a.Evaluate(test));

    }
    [TestMethod]
    public void ScientificNotationTest3()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E-06*12");
        Assert.AreEqual(12.000014814804, a.Evaluate(test));

    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction()
    {
        Formula formula = new Formula("(");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction1()
    {
        Formula formula = new Formula("++");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction2()
    {
        Formula formula = new Formula(")(");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction3()
    {
        Formula formula = new Formula("12+3/");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction4()
    {
        Formula formula = new Formula("12+3(");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction5()
    {
        Formula formula = new Formula("12+3*)");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction6()
    {
        Formula formula = new Formula("(12+3)4");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction7()
    {
        Formula formula = new Formula("(12+3)(2+5)");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction8()
    {
        Formula formula = new Formula("(12+3)+(2+5))");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction9()
    {
        Formula formula = new Formula("(12+3)**(2+5)");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction10()
    {
        Formula formula = new Formula("a1 12");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction11()
    {
        Formula formula = new Formula("(2+2)a1+a1");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction12()
    {
        Formula formula = new Formula("a1 a1");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction13()
    {
        Formula formula = new Formula("(12+12))-13+(12-12)");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void InvalidFormulaConstruction14()
    {
        Formula formula = new Formula("$+&");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void EmptyFormulaTest()
    {
        Formula formula = new Formula("");
    }
    [TestMethod]
    public void EqualsTest()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E-06");
        Formula b = new Formula("12+   0.000001234567");
        Assert.AreEqual(true, a.Equals(b));

    }
    [TestMethod]
    public void EqualsTest1()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E-06");
        Formula b = new Formula("12+   0.000001234567");
        Assert.AreEqual(true, a == b);

    }
    [TestMethod]
    public void NotEqualsTest()
    {
        Func<string, double> test = Lookups;
        Formula a = new Formula("12 + 1.234567E-06");
        Formula b = new Formula("12+   0.000001234567+5");
        Assert.AreEqual(true, a != b);

    }
    [TestMethod]
    public void HashCodeTest()
    {

        Formula a = new Formula("12 + 1.234567E-06");
        Formula b = new Formula("12+   0.000001234567");
        Assert.AreEqual(true, a.GetHashCode() == b.GetHashCode());

    }
    [TestMethod]
    public void ToStringTest()
    {

        Formula a = new Formula("a1 + a1");

        Assert.AreEqual("a1+a1", a.ToString());

    }
    [TestMethod]
    public void ToStringTest1()
    {

        Formula a = new Formula("a1 + a1+  12.12542");

        Assert.AreEqual("a1+a1+12.12542", a.ToString());

    }

    [TestMethod]
    public void GetVariablesTest()
    {

        Formula a = new Formula("a1 + a2+  z0+z0");
        int count = 0;
        string[] list = new string[] { "a1", "a2", "z0" };

        foreach (string var in a.GetVariables())
        {
            Assert.AreEqual(list[count], var);
            count++;
        }

    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void IsValidTest()
    {

        Formula a = new Formula("a1 + a1+  12.12542", s => s, s => false);

        a.Evaluate(Lookups);

    }
    [TestMethod]
    public void ValidateDivideByZeroTest()
    {
        Formula formula = new Formula("1/0");
        Assert.IsInstanceOfType(formula.Evaluate(Lookups), typeof(FormulaError));
    }
    [TestMethod]
    public void EvaluateWithReferenceTest()
    {
        Formula formula = new Formula("A1*a2+A1+a2");
        Assert.AreEqual(32.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EvaluateWithReferenceDivideByZeroTest()
    {
        Formula formula = new Formula("A1*a2+A1-a2/0");
        Assert.IsInstanceOfType(formula.Evaluate(Lookups), typeof(FormulaError));
    }
    [TestMethod]
    public void EvaluateWithDoubleSubtractTest()
    {
        Formula formula = new Formula("12-2-5");
        Assert.AreEqual(5.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EvaluateWithSubtractAndAddTest()
    {
        Formula formula = new Formula("12+2-5");
        Assert.AreEqual(9.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EvaluateJustParanthesisTest()
    {
        Formula formula = new Formula("(12)");
        Assert.AreEqual(12.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EvaluateTest()
    {
        Formula formula = new Formula("(12/4)");
        Assert.AreEqual(3.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EvaluateDivideByZeroTest()
    {
        Formula formula = new Formula("(12/0)");
        Assert.IsInstanceOfType(formula.Evaluate(Lookups), typeof(FormulaError));
    }
    [TestMethod]
    public void EqualsOnNullTest()
    {
        Formula formula = new Formula("12+5");
        Assert.AreEqual(false, formula.Equals(null));
    }
    [TestMethod]
    public void BigEqualsTest()
    {
        Formula formula = new Formula("12+5*(12+5)-13");
        Formula formula1 = new Formula("12+5*(12+5)-13");
        Assert.AreEqual(true, formula.Equals(formula1));
    }
    [TestMethod]
    public void BigUnEqualsTest()
    {
        Formula formula = new Formula("12+5*(12+5)-13");
        Formula formula1 = new Formula("12+5+(12+5)-13");
        Assert.AreEqual(false, formula.Equals(formula1));
    }
    [TestMethod]
    public void LookupOnZeroDivisionTest()
    {
        Formula formula = new Formula("12/a4");

        Assert.IsInstanceOfType(formula.Evaluate(Lookups), typeof(FormulaError));
    }
    [TestMethod]
    public void EvaluateTest1()
    {
        Formula formula = new Formula("(8-8/2)");
        Assert.AreEqual(4.0, formula.Evaluate(Lookups));
    }
    [TestMethod]
    public void EqualsTest2()
    {
        Formula formula = new Formula("a1+a1");
        Formula formula1 = new Formula("a1+a1");
        Assert.AreEqual(true, formula.Equals(formula1));
    }
    [TestMethod]
    public void UnEqualsTest()
    {
        Formula formula = new Formula("12+13.5");
        Formula formula1 = new Formula("12+13");
        Assert.AreEqual(false, formula.Equals(formula1));
    }
    [TestMethod]
    public void UnEqualsTest1()
    {
        Formula formula = new Formula("a1+a2");
        Formula formula1 = new Formula("a4+a2");
        Assert.AreEqual(false, formula.Equals(formula1));
    }
    [TestMethod]
    public void UnEqualsTest2()
    {
        Formula formula = new Formula("(12)+1");
        Formula formula1 = new Formula("(12+13)");
        Assert.AreEqual(false, formula.Equals(formula1));
    }
    [TestMethod]
    public void UnEqualsTest3()
    {
        Formula formula = new Formula("12+a1");
        Formula formula1 = new Formula("12+13");
        Assert.AreEqual(false, formula.Equals(formula1));
    }
    [TestMethod]
    public void UnEqualsTest4()
    {
        Formula formula = new Formula("(12)+1");
        Formula formula1 = new Formula("12+13-2");
        Assert.AreEqual(false, formula.Equals(formula1));
    }

}
