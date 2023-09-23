using SpreadsheetUtilities;
using SS;
using static System.Net.Mime.MediaTypeNames;

namespace SpreadsheetTests;

[TestClass]
public class UnitTest1
{
    Spreadsheet test = new Spreadsheet();

    [TestMethod]
    public void SetCellDoubleTest()
    {
        test.SetCellContents("a1", 2.0);
        IEnumerable<string> names = test.GetNamesOfAllNonemptyCells();
        foreach (string var in names)
        {
            Assert.AreEqual("a1", var);
        }
    }

    [TestMethod]
    public void SetCellFormulaTest()
    {
        Formula f = new Formula("a3+a2");
        test.SetCellContents("a1", f);
        IEnumerable<string> names = test.GetNamesOfAllNonemptyCells();
        foreach (string var in names)
        {
            Assert.AreEqual("a1", var);
        }
    }
    [TestMethod]
    public void SetCellStringTest()
    {

        test.SetCellContents("a1", "hello");
        IEnumerable<string> names = test.GetNamesOfAllNonemptyCells();
        foreach (string var in names)
        {
            Assert.AreEqual("a1", var);

        }

    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameStringTest()
    {
        test.SetCellContents("#", "hello");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameFormulaTest()
    {
        Formula f = new Formula("a3+a2");
        test.SetCellContents("-9", f);
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameDoubleTest()
    {
        test.SetCellContents("a+", 2.0);
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameCellContentsTest()
    {
        Spreadsheet test1 = new Spreadsheet();
        test1.GetCellContents("a+");
    }
    [TestMethod]
    public void CellContentsTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        test1.SetCellContents("a1", 2.0);
        test1.SetCellContents("a2", "hello");
        Formula f = new Formula("a3+a2");
        test1.SetCellContents("a4", f);
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.IsTrue(f.Equals(test1.GetCellContents("a4")));

    }
    [TestMethod]
    public void CellContentsOnEmptyTest()
    {

        Spreadsheet test1 = new Spreadsheet();

        Assert.AreEqual("", test1.GetCellContents("a1"));


    }
    [TestMethod]
    public void SetCellContentsEmptyTest()
    {

        Spreadsheet test1 = new Spreadsheet();

        Assert.AreEqual(0, test1.SetCellContents("a1","").ToList().Count);


    }
    [TestMethod]
    public void CellUpdate1Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1+a8+a9");
        test1.SetCellContents("a1", 3.0);
        test1.SetCellContents("a2", "he");
        test1.SetCellContents("a3", a);
        test1.SetCellContents("a1", 2.0);
        test1.SetCellContents("a2", "");
        Formula f = new Formula("a1+a4+a7");
        test1.SetCellContents("a3", f);
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("", test1.GetCellContents("a2"));
        Assert.IsTrue(f.Equals(test1.GetCellContents("a3")));


    }
    [TestMethod]
    public void CellUpdate2Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1+a8+a9");
        test1.SetCellContents("a1", 3.0);
        test1.SetCellContents("a2", "he");
        test1.SetCellContents("a3", a);
        test1.SetCellContents("a1", 2.0);
        test1.SetCellContents("a2", "hello");
        
        test1.SetCellContents("a3", 3.0);
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.AreEqual(3.0,test1.GetCellContents("a3"));


    }
    [TestMethod]
    public void CellUpdate3Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1+a8+a9");
        test1.SetCellContents("a1", 3.0);
        test1.SetCellContents("a2", "he");
        test1.SetCellContents("a3", a);
        test1.SetCellContents("a1", 2.0);
        test1.SetCellContents("a2", "hello");
       
        test1.SetCellContents("a3", "");
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.AreEqual("",test1.GetCellContents("a3"));


    }
    public void CellUpdateTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1");
        test1.SetCellContents("a1", 3.0);
        test1.SetCellContents("a2", "he");
        test1.SetCellContents("a3", a);
        test1.SetCellContents("a1", 2.0);
        test1.SetCellContents("a2", "hello");
        Formula f = new Formula("a1+a4");
        test1.SetCellContents("a3", f);
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.IsTrue(f.Equals(test1.GetCellContents("a3")));


    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void CircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1");

        test1.SetCellContents("a3", a);

        Formula f = new Formula("a3+a2");
        test1.SetCellContents("a1", f);


    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void Circular1Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1");

        test1.SetCellContents("a3", a);

        Formula f = new Formula("a3+a2");
        test1.SetCellContents("a3", f);


    }
    
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void LongCircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1");

        test1.SetCellContents("a3", a);

        Formula f = new Formula("a4+a5");
        test1.SetCellContents("a1", f);
        Formula s = new Formula("a8+a7");
        test1.SetCellContents("a4", s);
        Formula d = new Formula("a3+a12");
        test1.SetCellContents("a7", d);



    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void RefToSelfCircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a3");

        test1.SetCellContents("a3", a);





    }
    [TestMethod]

    public void SetCellReturnTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a4+a5*a7");
        List<string> match = new List<string> { "a1", "a2","a4","a5","a7" ,"a3" };
        IList<string> list = test1.SetCellContents("a3", a);

        for (int i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(match[i], list[i]);
        }




    }
}
