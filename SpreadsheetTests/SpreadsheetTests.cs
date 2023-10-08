﻿using System.Text.Json;
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
        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "default");
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
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadEmptySpreadsheetTest()
    {

        File.WriteAllText("save.txt", "{}");


        Spreadsheet test = new Spreadsheet("save.txt", s => true, s => s, "defualt");
       

       




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
    public void GetCellValueDoubleFormulaAddStringErrCellTest()
    {




        Spreadsheet test = new Spreadsheet(s => true, s => s, "1");
        test.SetContentsOfCell("a2", "2.0");
        test.SetContentsOfCell("a3", "5");
        test.SetContentsOfCell("a4", "hi");
        test.SetContentsOfCell("a5", "=a4");
        test.SetContentsOfCell("a1", "=a2+a3+a5");
        Assert.IsTrue( test.GetCellValue("a1") is FormulaError);






    }
    // Verifies cells and their values, which must alternate.
    public void VV(AbstractSpreadsheet sheet, params object[] constraints)
    {
        for (int i = 0; i < constraints.Length; i += 2)
        {
            if (constraints[i + 1] is double)
            {
                Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
            }
            else
            {
                Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
            }
        }
    }


    // For setting a spreadsheet cell.
    public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
    {
        List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
        return result;
    }

    // Tests IsValid
    [TestMethod, Timeout(2000)]
    [TestCategory("1")]
    public void IsValidTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "x");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("2")]
    [ExpectedException(typeof(InvalidNameException))]
    public void IsValidTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("A1", "x");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("3")]
    public void IsValidTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "= A1 + C1");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("4")]
    [ExpectedException(typeof(FormulaFormatException))]
    public void IsValidTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("B1", "= A1 + C1");
    }

    // Tests Normalize
    [TestMethod, Timeout(2000)]
    [TestCategory("5")]
    public void NormalizeTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("", s.GetCellContents("b1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("6")]
    public void NormalizeTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("hello", ss.GetCellContents("b1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("7")]
    public void NormalizeTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("8")]
    public void NormalizeTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("A1", "6");
        ss.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
    }

    // Simple tests
    [TestMethod, Timeout(2000)]
    [TestCategory("9")]
    public void EmptySheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        VV(ss, "A1", "");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("10")]
    public void OneString()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneString(ss);
    }

    public void OneString(AbstractSpreadsheet ss)
    {
        Set(ss, "B1", "hello");
        VV(ss, "B1", "hello");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("11")]
    public void OneNumber()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneNumber(ss);
    }

    public void OneNumber(AbstractSpreadsheet ss)
    {
        Set(ss, "C1", "17.5");
        VV(ss, "C1", 17.5);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("12")]
    public void OneFormula()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneFormula(ss);
    }

    public void OneFormula(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "5.2");
        Set(ss, "C1", "= A1+B1");
        VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("13")]
    public void ChangedAfterModify()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Assert.IsFalse(ss.Changed);
        Set(ss, "C1", "17.5");
        Assert.IsTrue(ss.Changed);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("13b")]
    public void UnChangedAfterSave()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "C1", "17.5");
        ss.Save("changed.txt");
        Assert.IsFalse(ss.Changed);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("14")]
    public void DivisionByZero1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero1(ss);
    }

    public void DivisionByZero1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "0.0");
        Set(ss, "C1", "= A1 / B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("15")]
    public void DivisionByZero2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero2(ss);
    }

    public void DivisionByZero2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "5.0");
        Set(ss, "A3", "= A1 / 0.0");
        Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
    }



    [TestMethod, Timeout(2000)]
    [TestCategory("16")]
    public void EmptyArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        EmptyArgument(ss);
    }

    public void EmptyArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("17")]
    public void StringArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        StringArgument(ss);
    }

    public void StringArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "hello");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("18")]
    public void ErrorArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ErrorArgument(ss);
    }

    public void ErrorArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= C1");
        Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("19")]
    public void NumberFormula1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula1(ss);
    }

    public void NumberFormula1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + 4.2");
        VV(ss, "C1", 8.3);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("20")]
    public void NumberFormula2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula2(ss);
    }

    public void NumberFormula2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "= 4.6");
        VV(ss, "A1", 4.6);
    }


    // Repeats the simple tests all together
    [TestMethod, Timeout(2000)]
    [TestCategory("21")]
    public void RepeatSimpleTests()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "17.32");
        Set(ss, "B1", "This is a test");
        Set(ss, "C1", "= A1+B1");
        OneString(ss);
        OneNumber(ss);
        OneFormula(ss);
        DivisionByZero1(ss);
        DivisionByZero2(ss);
        StringArgument(ss);
        ErrorArgument(ss);
        NumberFormula1(ss);
        NumberFormula2(ss);
    }

    // Four kinds of formulas
    [TestMethod, Timeout(2000)]
    [TestCategory("22")]
    public void Formulas()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formulas(ss);
    }

    public void Formulas(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.4");
        Set(ss, "B1", "2.2");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= A1 - B1");
        Set(ss, "E1", "= A1 * B1");
        Set(ss, "F1", "= A1 / B1");
        VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("23")]
    public void Formulasa()
    {
        Formulas();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("24")]
    public void Formulasb()
    {
        Formulas();
    }


    // Are multiple spreadsheets supported?
    [TestMethod, Timeout(2000)]
    [TestCategory("25")]
    public void Multiple()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        AbstractSpreadsheet s2 = new Spreadsheet();
        Set(s1, "X1", "hello");
        Set(s2, "X1", "goodbye");
        VV(s1, "X1", "hello");
        VV(s2, "X1", "goodbye");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("26")]
    public void Multiplea()
    {
        Multiple();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("27")]
    public void Multipleb()
    {
        Multiple();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("28")]
    public void Multiplec()
    {
        Multiple();
    }

    // Reading/writing spreadsheets
    [TestMethod, Timeout(2000)]
    [TestCategory("29")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save(Path.GetFullPath("/missing/save.txt"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("30")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("31")]
    public void SaveTest3()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        Set(s1, "A1", "hello");
        s1.Save("save1.txt");
        s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
        Assert.AreEqual("hello", s1.GetCellContents("A1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("32")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest4()
    {
        using (StreamWriter writer = new StreamWriter("save2.txt"))
        {
            writer.WriteLine("This");
            writer.WriteLine("is");
            writer.WriteLine("a");
            writer.WriteLine("test!");
        }
        AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("33")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest5()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("save3.txt");
        ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("35")]
    public void SaveTest7()
    {
        var sheet = new
        {
            Cells = new
            {
                A1 = new { StringForm = "hello" },
                A2 = new { StringForm = "5.0" },
                A3 = new { StringForm = "4.0" },
                A4 = new { StringForm = "= A2 + A3" }
            },
            Version = ""
        };

        File.WriteAllText("save5.txt", JsonSerializer.Serialize(sheet));


        AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
        VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("36")]
    public void SaveTest8()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "hello");
        Set(ss, "A2", "5.0");
        Set(ss, "A3", "4.0");
        Set(ss, "A4", "= A2 + A3");
        ss.Save("save6.txt");

        string fileContents = File.ReadAllText("save6.txt");
        JsonDocument o = JsonDocument.Parse(fileContents);
        Assert.AreEqual("default", o.RootElement.GetProperty("Version").ToString());
        Assert.AreEqual("hello", o.RootElement.GetProperty("Cells").GetProperty("A1").GetProperty("StringForm").ToString());
        Assert.AreEqual(5.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A2").GetProperty("StringForm").ToString()), 1e-9);
        Assert.AreEqual(4.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A3").GetProperty("StringForm").ToString()), 1e-9);
        Assert.AreEqual("=A2+A3", o.RootElement.GetProperty("Cells").GetProperty("A4").GetProperty("StringForm").ToString().Replace(" ", ""));
    }


    // Fun with formulas
    [TestMethod, Timeout(2000)]
    [TestCategory("37")]
    public void Formula1()
    {
        Formula1(new Spreadsheet());
    }
    public void Formula1(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= b1 + b2");
        Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
        Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
        Set(ss, "a3", "5.0");
        Set(ss, "b1", "2.0");
        Set(ss, "b2", "3.0");
        VV(ss, "a1", 10.0, "a2", 5.0);
        Set(ss, "b2", "4.0");
        VV(ss, "a1", 11.0, "a2", 6.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("38")]
    public void Formula2()
    {
        Formula2(new Spreadsheet());
    }
    public void Formula2(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= a3");
        Set(ss, "a3", "6.0");
        VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
        Set(ss, "a3", "5.0");
        VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("39")]
    public void Formula3()
    {
        Formula3(new Spreadsheet());
    }
    public void Formula3(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a3 + a5");
        Set(ss, "a2", "= a5 + a4");
        Set(ss, "a3", "= a5");
        Set(ss, "a4", "= a5");
        Set(ss, "a5", "9.0");
        VV(ss, "a1", 18.0);
        VV(ss, "a2", 18.0);
        Set(ss, "a5", "8.0");
        VV(ss, "a1", 16.0);
        VV(ss, "a2", 16.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("40")]
    public void Formula4()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formula1(ss);
        Formula2(ss);
        Formula3(ss);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("41")]
    public void Formula4a()
    {
        Formula4();
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("42")]
    public void MediumSheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
    }

    public void MediumSheet(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "1.0");
        Set(ss, "A2", "2.0");
        Set(ss, "A3", "3.0");
        Set(ss, "A4", "4.0");
        Set(ss, "B1", "= A1 + A2");
        Set(ss, "B2", "= A3 * A4");
        Set(ss, "C1", "= B1 + B2");
        VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
        Set(ss, "A1", "2.0");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
        Set(ss, "B1", "= A1 / A2");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("43")]
    public void MediumSheeta()
    {
        MediumSheet();
    }


    [TestMethod]
    [TestCategory("44")]
    public void MediumSave()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
        ss.Save("save7.txt");
        ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("45")]
    public void MediumSavea()
    {
        MediumSave();
    }


    // A long chained formula. Solutions that re-evaluate 
    // cells on every request, rather than after a cell changes,
    // will timeout on this test.
    // This test is repeated to increase its scoring weight
    [TestMethod, Timeout(6000)]
    [TestCategory("46")]
    public void LongFormulaTest()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("47")]
    public void LongFormulaTest2()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("48")]
    public void LongFormulaTest3()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("49")]
    public void LongFormulaTest4()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("50")]
    public void LongFormulaTest5()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    public void LongFormulaHelper(out object result)
    {
        try
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("sum1", "= a1 + a2");
            int i;
            int depth = 100;
            for (i = 1; i <= depth * 2; i += 2)
            {
                s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
            }
            s.SetContentsOfCell("a" + i, "1");
            s.SetContentsOfCell("a" + (i + 1), "1");
            Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + i, "0");
            Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + (i + 1), "0");
            Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
            result = "ok";
        }
        catch (Exception e)
        {
            result = e;
        }
    }

}
