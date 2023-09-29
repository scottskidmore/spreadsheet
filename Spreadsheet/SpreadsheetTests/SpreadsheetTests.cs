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
        test.SetContentsOfCell("a1", "2.0");
        IEnumerable<string> names = test.GetNamesOfAllNonemptyCells();
        foreach (string var in names)
        {
            Assert.AreEqual("a1", var);
        }
    }

    [TestMethod]
    public void SetCellFormulaTest()
    {
       
        test.SetContentsOfCell("a1", "=a3+a2");
        IEnumerable<string> names = test.GetNamesOfAllNonemptyCells();
        foreach (string var in names)
        {
            Assert.AreEqual("a1", var);
        }
    }
    [TestMethod]
    public void SetCellStringTest()
    {

        test.SetContentsOfCell("a1", "hello");
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
        test.SetContentsOfCell("#", "hello");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameFormulaTest()
    {
       
        test.SetContentsOfCell("-9", "=a3+a2");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameDoubleTest()
    {
        test.SetContentsOfCell("a+", "2.0");
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
        test1.SetContentsOfCell("a1", "2.0");
        test1.SetContentsOfCell("a2", "hello");
        Formula f = new Formula("a3+a2");
        test1.SetContentsOfCell("a4", "=a3+a2");
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
    public void SetContentsOfCellEmptyTest()
    {

        Spreadsheet test1 = new Spreadsheet();

        Assert.AreEqual(0, test1.SetContentsOfCell("a1","").ToList().Count);


    }
    [TestMethod]
    public void CellUpdate1Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        
        test1.SetContentsOfCell("a1", "3.0");
        test1.SetContentsOfCell("a2", "he");
        test1.SetContentsOfCell("a3", "=a1+a2+a1+a8+a9");
        test1.SetContentsOfCell("a1", "2.0");
        test1.SetContentsOfCell("a2", "");
        Formula f = new Formula("a1+a4+a7");
        test1.SetContentsOfCell("a3", "=a1+a4+a7");
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("", test1.GetCellContents("a2"));
        Assert.IsTrue(f.Equals(test1.GetCellContents("a3")));


    }
    [TestMethod]
    public void CellUpdate2Test()
    {

        Spreadsheet test1 = new Spreadsheet();
       
        test1.SetContentsOfCell("a1", "3.0");
        test1.SetContentsOfCell("a2", "he");
        test1.SetContentsOfCell("a3", "=a1+a2+a1+a8+a9");
        test1.SetContentsOfCell("a1", "2.0");
        test1.SetContentsOfCell("a2", "hello");
        
        test1.SetContentsOfCell("a3", "3.0");
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.AreEqual(3.0,test1.GetCellContents("a3"));


    }
    [TestMethod]
    public void CellUpdate3Test()
    {

        Spreadsheet test1 = new Spreadsheet();
        
        test1.SetContentsOfCell("a1", "3.0");
        test1.SetContentsOfCell("a2", "he");
        test1.SetContentsOfCell("a3", "=a1+a2+a1+a8+a9");
        test1.SetContentsOfCell("a1", "2.0");
        test1.SetContentsOfCell("a2", "hello");
       
        test1.SetContentsOfCell("a3", "");
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.AreEqual("",test1.GetCellContents("a3"));


    }
    public void CellUpdateTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        Formula a = new Formula("a1+a2+a1");
        test1.SetContentsOfCell("a1", "3.0");
        test1.SetContentsOfCell("a2", "he");
        test1.SetContentsOfCell("a3", "=a1+a2+a1");
        test1.SetContentsOfCell("a1", "2.0");
        test1.SetContentsOfCell("a2", "hello");
        Formula f = new Formula("a1+a4");
        test1.SetContentsOfCell("a3", "=a1+a4");
        Assert.AreEqual(2.0, test1.GetCellContents("a1"));
        Assert.AreEqual("hello", test1.GetCellContents("a2"));
        Assert.IsTrue(f.Equals(test1.GetCellContents("a3")));


    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void CircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
       

        test1.SetContentsOfCell("a3", "=a1+a2+a1");

       
        test1.SetContentsOfCell("a1", "=a3+a2");


    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void Circular1Test()
    {

        Spreadsheet test1 = new Spreadsheet();
       

        test1.SetContentsOfCell("a3", "=a1+a2+a1");

       
        test1.SetContentsOfCell("a3", "=a3+a2");


    }
    
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void LongCircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
     

        test1.SetContentsOfCell("a3", "=a1+a2+a1");

        
        test1.SetContentsOfCell("a1", "=a4+a5");
      
        test1.SetContentsOfCell("a4", "=a8+a7");
     
        test1.SetContentsOfCell("a7", "=a8+a7");



    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void RefToSelfCircularTest()
    {

        Spreadsheet test1 = new Spreadsheet();
     

        test1.SetContentsOfCell("a3", "=a1+a2+a3");





    }
    [TestMethod]

    public void SetCellReturnTest()
    {

        Spreadsheet test1 = new Spreadsheet();
        
        List<string> match = new List<string> { "a1", "a2","a4","a5","a7" ,"a3" };
        IList<string> list = test1.SetContentsOfCell("a3", "=a1+a2+a4+a5*a7");

        for (int i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(match[i], list[i]);
        }




    }
    [TestMethod]

    public void SaveTest()
    {

        Spreadsheet test1 = new Spreadsheet();

        
        IList<string> list = test1.SetContentsOfCell("a3", "2.0");

        test1.Save("save.txt");
        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "1");
        Assert.AreEqual(2.0, test.GetCellValue("a3"));
        
        foreach(string var in test.GetNamesOfAllNonemptyCells())
        Assert.AreEqual("a3",var );



    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveInvalidAddressTest()
    {

        Spreadsheet test1 = new Spreadsheet();


        IList<string> list = test1.SetContentsOfCell("a3", "2.0");

        test1.Save("/missing/save.txt");
       



    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadInvalidAddressTest()
    {

        Spreadsheet test1 = new Spreadsheet("/missing/save.txt", s => true, s => s, "1");


       



    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadEmptyFileTest()
    {

        File.WriteAllText("save.txt","");


        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "1");
        



    }
    [TestMethod]
    
    public void LoadEmptySpreadsheetTest()
    {

        File.WriteAllText("save.txt", "{}");


        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "1");
       

       




    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadNullTest()
    {

        File.WriteAllText("save.txt", "null");


        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "1");
     




    }
    [TestMethod]
   
    public void OtherConstructorsTest()
    {

       


        Spreadsheet test = new Spreadsheet( s => true, s => s, "1");
        Spreadsheet test1 = new Spreadsheet();
        Assert.AreEqual(test1.Version, "default");
        Assert.AreEqual(test.Version, "1");






    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContentsValidatorFalseTest()
    {




        Spreadsheet test = new Spreadsheet(s => false, s => s, "1");
        test.GetCellContents("a1");






    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContentsValidatorFalseTest()
    {




        Spreadsheet test = new Spreadsheet(s => false, s => s, "1");
        test.SetContentsOfCell("a1","21");






    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueValidatorFalseTest()
    {




        Spreadsheet test = new Spreadsheet(s => false, s => s, "1");
        test.GetCellValue("a1");






    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueInvalidNameTest()
    {




        Spreadsheet test = new Spreadsheet(s => false, s => s, "1");
        test.GetCellValue("a>1");






    }
    [TestMethod]
    public void GetCellValueEmptyCellTest()
    {




        Spreadsheet test = new Spreadsheet(s => true, s => s, "1");
        Assert.AreEqual("",test.GetCellValue("a1"));






    }
    [TestMethod]
    public void GetCellValueFormulaCellTest()
    {




        Spreadsheet test = new Spreadsheet(s => true, s => s, "1");
        test.SetContentsOfCell("a2", "2.0");
        test.SetContentsOfCell("a3", "5");
        test.SetContentsOfCell("a4", ".5");
        test.SetContentsOfCell("a1", "=a2+a3+a4");
        Assert.AreEqual(7.5, test.GetCellValue("a1"));






    }
    [TestMethod]
    public void GetCellValueDoubleFormulaCellTest()
    {




        Spreadsheet test = new Spreadsheet(s => true, s => s, "1");
        test.SetContentsOfCell("a2", "2.0");
        test.SetContentsOfCell("a3", "5");
        test.SetContentsOfCell("a6", "=a2+a3");
        test.SetContentsOfCell("a5", "=a6+a2");
        test.SetContentsOfCell("a1", "=a2+a3+a5");
        Assert.AreEqual(16.0, test.GetCellValue("a1"));






    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetCellValueDoubleFormulaAddStringErrCellTest()
    {




        Spreadsheet test = new Spreadsheet(s => true, s => s, "1");
        test.SetContentsOfCell("a2", "2.0");
        test.SetContentsOfCell("a3", "5");
        test.SetContentsOfCell("a4", "hi");
        test.SetContentsOfCell("a5", "=a4");
        test.SetContentsOfCell("a1", "=a2+a3+a5");
        Assert.AreEqual(7.5, test.GetCellValue("a1"));






    }

}
