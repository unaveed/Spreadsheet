using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Threading;
using System.IO;
using System.Xml;

namespace SpreadsheetUnitTests {
    [TestClass]
    public class MyUnitTests {

        [TestMethod]
        public void NewTest1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1");
            Assert.AreEqual(sheet.GetCellValue("B1"), 3.0);
            sheet.SetContentsOfCell("A1", "");
            Assert.IsTrue(sheet.GetCellValue("B1") is FormulaError);
        }


        /***************************** GRADING TESTS *********************************/
        // Simple tests that return FormulaErrors
        [TestMethod()]
        public void Test16() {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test17() {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test18() {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test19() {
            Formula f = new Formula("+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test20() {
            Formula f = new Formula("2+5+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test21() {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test22() {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test23() {
            Formula f = new Formula("5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test24() {
            Formula f = new Formula("5+5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test25() {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test26() {
            Formula f = new Formula("5 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test27() {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test28() {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod()]
        public void Test29() {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod()]
        public void Test30() {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod()]
        public void Test31() {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod()]
        public void Test32() {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod()]
        public void Test33() {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test34() {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test35() {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test36() {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test37() {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod()]
        public void Test38() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void Test39() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod()]
        public void Test40() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod()]
        public void Test41() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void Test42() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod()]
        public void Test43() {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod()]
        public void Test44() {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod()]
        public void Test45() {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod()]
        public void Test46() {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test47() {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test48() {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test49() {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test50() {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod()]
        public void Test51a() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51b() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51c() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51d() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51e() {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52a() {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52b() {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52c() {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52d() {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52e() {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }




        /****************************** TESTS PS5 CODE *******************************/

        [TestMethod]
        public void TestSave1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "12.03");
            sheet.Save("test1.xml");
        }

        [TestMethod]
        public void TestSave2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "12.03");
            sheet.SetContentsOfCell("b2", "hello world!");
            sheet.SetContentsOfCell("c3", "=4+17-3");
            sheet.SetContentsOfCell("d4", "=x3");
            sheet.Save("test2.xml");
        }

        [TestMethod]
        public void TestRead1() {
            string path = "read1.xml";
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "1.5");
            sheet.SetContentsOfCell("b2", "2.6");
            sheet.SetContentsOfCell("c3", "3.7");
            sheet.SetContentsOfCell("d4", "4.8");
            sheet.SetContentsOfCell("e5", "5.9");
            sheet.Save(path);
            sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        [TestMethod]
        public void TestRead2() {
            string path = "read2.xml";
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "1.5");
            sheet.SetContentsOfCell("b2", "2.6");
            sheet.SetContentsOfCell("c3", "3.7");
            sheet.SetContentsOfCell("d4", "4.8");
            sheet.SetContentsOfCell("e5", "5.9");
            sheet.Save(path);
            HashSet<string> original = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();
            sheet = new Spreadsheet(path, x => true, s => s, "default");
            HashSet<string> output = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(original.SetEquals(output));
        }

        [TestMethod]
        public void TestRead3() {
            string path = "read3.xml";
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=1.5+4");
            sheet.SetContentsOfCell("b2", "=a2-2.6");
            sheet.SetContentsOfCell("c3", "=3.7*e5");
            sheet.SetContentsOfCell("d4", "hil");
            sheet.SetContentsOfCell("e5", "5.9");
            sheet.Save(path);
            HashSet<string> original = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();
            Spreadsheet sheet2 = new Spreadsheet(path, x => true, s => s, "default");
            foreach (string s in original) {
                Assert.IsTrue(sheet.GetCellContents(s).Equals(sheet2.GetCellContents(s)));
                Assert.IsTrue(sheet.GetCellValue(s).Equals(sheet2.GetCellValue(s)));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValue1() {
            Spreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.GetCellValue(s);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValue2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue("84");
        }

        [TestMethod]
        public void TestSavedVersion1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hello");
            string path = "savedVersion1.xml";
            sheet.Save(path);
            string version = "default";
            Assert.AreEqual(version, sheet.GetSavedVersion(path));
        }

        [TestMethod]
        public void TestSavedVersion2() {
            string version = "10.2091.23";
            Spreadsheet sheet = new Spreadsheet(x => true, s => s, version);
            sheet.SetContentsOfCell("a1", "hello");
            string path = "savedVersion2.xml";
            sheet.Save(path);
            Assert.AreEqual(version, sheet.GetSavedVersion(path));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSavedVersion3() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetSavedVersion("thisisnotafile.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents1() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", null, 10.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents2() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", null, "Hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents3() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", null, new Formula("5+6"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents4() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", "88", 10.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents5() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", "88", "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents6() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);

            obj.Invoke("SetCellContents", "88", new Formula("5+6"));
        }

        [TestMethod]
        public void TestGetChanged1() {
            string path = "GetChanged1.xml";
            Spreadsheet sheet = new Spreadsheet();
            Assert.IsTrue(!sheet.Changed);
            sheet.SetContentsOfCell("a1", "hello");
            Assert.IsTrue(sheet.Changed);
            sheet.Save(path);
            Assert.IsTrue(!sheet.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBadXml1() {
            string path = "BadXml1.xml";
            Spreadsheet sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBadXml2() {
            string path = "BadXml2.xml";
            Spreadsheet sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBadXml3() {
            string path = "BadXml3.xml";
            Spreadsheet sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBadXml4() {
            string path = "BadXml4.xml";
            Spreadsheet sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBadXml5() {
            string path = "BadXml5.xml";
            Spreadsheet sheet = new Spreadsheet(path, x => true, s => s, "default");
        }

        /***************************** TESTS ALL METHODS *****************************/

        [TestMethod]
        public void TestAddCells1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "5.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("_", "hello");
        }

        [TestMethod]
        public void TestAddCells3() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("b2", "=5 + x4");
        }

        [TestMethod]
        public void TestAddCells4() {
            Spreadsheet sheet = new Spreadsheet();
            HashSet<string> correct = new HashSet<string> { "b2" };
            HashSet<string> output = (HashSet<string>)sheet.SetContentsOfCell("b2", "=5 + x4");
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        public void TestAddCells6() {
            Spreadsheet sheet = new Spreadsheet();
            HashSet<string> correct = new HashSet<string> { "b2" };
            HashSet<string> output = (HashSet<string>)sheet.SetContentsOfCell("b2", "2.3");
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        public void TestAddCells7() {
            Spreadsheet sheet = new Spreadsheet();
            HashSet<string> correct = new HashSet<string> { "X" };
            HashSet<string> output = (HashSet<string>)sheet.SetContentsOfCell("X", "hello");
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells9() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "0.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells10() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "0.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells11() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("83", "0.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells12() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "=5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells13() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "=5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells14() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("83", "=5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells15() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells16() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestAddCells17() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("83", "hello");
        }

        [TestMethod]
        public void TestAddCells18() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "0.0");
            sheet.SetContentsOfCell("a1", "hello");
            sheet.SetContentsOfCell("a1", "=5");
            sheet.SetContentsOfCell("a1", "17.23");
            sheet.SetContentsOfCell("a1", "hi");
            sheet.SetContentsOfCell("a1", "=13+x3");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddCells19() {
            Spreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell("a1", s);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddCells20() {
            Spreadsheet sheet = new Spreadsheet();
            string f = null;
            sheet.SetContentsOfCell("a1", f);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularDependency1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=a1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularDependency2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=c1");
            sheet.SetContentsOfCell("c1", "=a1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularDependency3() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=c1 + d1 - e1");
            sheet.SetContentsOfCell("d1", "=f1");
            sheet.SetContentsOfCell("f1", "=e1");
            sheet.SetContentsOfCell("e1", "=g1");
            sheet.SetContentsOfCell("g1", "=a1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularDependency4() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=c1");
            sheet.SetContentsOfCell("c1", "=b1");
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "0.0");
            HashSet<string> correct = new HashSet<string> { "a1" };
            HashSet<string> output = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "0.0");
            sheet.SetContentsOfCell("b2", "hello");
            sheet.SetContentsOfCell("c3", "=5 + 6");
            sheet.SetContentsOfCell("d4", "5.5");
            sheet.SetContentsOfCell("e5", "new cell");
            HashSet<string> correct = new HashSet<string> { "a1", "b2", "c3", "d4", "e5" };
            HashSet<string> output = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents1() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents2() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("");
        }

        [TestMethod]
        public void TestGetCellContents3() {
            Spreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestGetCellContents4() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "0.0");
            Assert.AreEqual(0.0, sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestGetCellContents5() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a57", "hello");
            Assert.AreEqual("hello", sheet.GetCellContents("a57"));
        }

        [TestMethod]
        public void TestGetCellContents6() {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "5.27");
            sheet.SetContentsOfCell("b2", "6.000");
            sheet.SetContentsOfCell("c3", "hello");
            sheet.SetContentsOfCell("d4", "=5 - 4 + x2");
            sheet.SetContentsOfCell("e5", "=a1 - b2");
            Assert.AreEqual(5.27, sheet.GetCellContents("a1"));
            Assert.AreEqual(6.0, sheet.GetCellContents("b2"));
            Assert.AreEqual("hello", sheet.GetCellContents("c3"));
            Assert.AreEqual(new Formula("5 - 4 + x2"), sheet.GetCellContents("d4"));
            Assert.AreEqual(new Formula("a1 - b2"), sheet.GetCellContents("e5"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPrivateMethod2() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);
            sheet.SetContentsOfCell("a1", "=b1 + c1");
            string s = null;
            obj.Invoke("GetDirectDependents", s);
        }

        [TestMethod]
        public void TestPrivateMethod3() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);
            sheet.SetContentsOfCell("a1", "=b1 + c1");
            HashSet<string> correct = new HashSet<string>();
            HashSet<string> output = (HashSet<string>)obj.Invoke("GetDirectDependents", "x0");
            Assert.IsTrue(correct.SetEquals(output));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestPrivateMethod4() {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject obj = new PrivateObject(sheet);
            sheet.SetContentsOfCell("a1", "=b1 + c1");
            obj.Invoke("GetDirectDependents", "739");
        }

        /***************************** STRESS TESTS *****************************/

        [TestMethod]
        public void StressTest1() {
            Spreadsheet sheet = new Spreadsheet();

            int SIZE = 1000;
            // Add a ton of items
            for (int i = 0; i < SIZE; i++)
                sheet.SetContentsOfCell("a" + i, 1.0 + i + "");

            for (int i = 0; i < SIZE; i++)
                sheet.SetContentsOfCell("b" + i, "hi" + i);

            for (int i = 0; i < SIZE; i++)
                sheet.SetContentsOfCell("c" + i, "=5+" + i);

            // Make correct HashSet
            HashSet<string> correct = new HashSet<string>();
            for (int i = 0; i < SIZE; i++)
                correct.Add("a" + i);
            for (int i = 0; i < SIZE; i++)
                correct.Add("b" + i);
            for (int i = 0; i < SIZE; i++)
                correct.Add("c" + i);

            // Get all of the names of the cells
            HashSet<string> output = (HashSet<string>)sheet.GetNamesOfAllNonemptyCells();

            // Confirm that the names in the spreadsheet are correct
            Assert.IsTrue(correct.SetEquals(output));

            // Check the cell contents of all the cells
            for (int i = 0; i < SIZE; i++)
                Assert.AreEqual(1.0 + i, sheet.GetCellContents("a" + i));
            for (int i = 0; i < SIZE; i++)
                Assert.AreEqual("hi" + i, sheet.GetCellContents("b" + i));
            for (int i = 0; i < SIZE; i++)
                Assert.AreEqual(new Formula("5+" + i), sheet.GetCellContents("c" + i));

            // You did it!
            Assert.IsTrue(true);
        }


        /************************************ GRADING TESTS *******************************/


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test100() {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test200() {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod()]
        public void Test300() {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test400() {
            Spreadsheet s = new Spreadsheet();
            string t = null;
            s.SetContentsOfCell(t, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test500() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod()]
        public void Test600() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test700() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test800() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test900() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod()]
        public void Test1000() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test1100() {
            Spreadsheet s = new Spreadsheet();
            string f = null;
            s.SetContentsOfCell("A8", f);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1200() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1300() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "=2");
        }

        [TestMethod()]
        public void Test1400() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test1500() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test1600() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test1700() {
            Spreadsheet s = new Spreadsheet();
            try {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e) {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test1800() {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test1900() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test2000() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test2100() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test2200() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test2300() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test2400() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void Test2500() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test2600() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void Test2700() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            HashSet<string> set = (HashSet<string>)s.SetContentsOfCell("A5", "10.0");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test2800() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test2900() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test3000() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test3100() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }
        [TestMethod()]
        public void Test3200() {
            Test31();
        }
        [TestMethod()]
        public void Test3300() {
            Test31();
        }
        [TestMethod()]
        public void Test3400() {
            Test31();
        }

        [TestMethod()]
        public void Test3500() {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++) {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }
        [TestMethod()]
        public void Test3600() {
            Test35();
        }
        [TestMethod()]
        public void Test3700() {
            Test35();
        }
        [TestMethod()]
        public void Test3800() {
            Test35();
        }
        [TestMethod()]
        public void Test3900() {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++) {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException) {
            }
        }
        [TestMethod()]
        public void Test4000() {
            Test39();
        }
        [TestMethod()]
        public void Test4100() {
            Test39();
        }
        [TestMethod()]
        public void Test4200() {
            Test39();
        }

        [TestMethod()]
        public void Test4300() {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++) {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            HashSet<string> firstCells = new HashSet<string>();
            HashSet<string> lastCells = new HashSet<string>();
            for (int i = 0; i < 250; i++) {
                firstCells.Add("A1" + i);
                lastCells.Add("A1" + (i + 250));
            }
            HashSet<string> set = (HashSet<string>)s.SetContentsOfCell("A1499", "0");
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SetEquals(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SetEquals(lastCells));
        }
        [TestMethod()]
        public void Test4400() {
            Test43();
        }
        [TestMethod()]
        public void Test4500() {
            Test43();
        }
        [TestMethod()]
        public void Test4600() {
            Test43();
        }

        [TestMethod()]
        public void Test4700() {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test4800() {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test4900() {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test5000() {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest(int seed, int size) {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++) {
                try {
                    switch (rand.Next(3)) {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException) {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand) {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand) {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++) {
                switch (rand.Next(4)) {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2)) {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }




        /// <summary>
        ///This is a test class for SpreadsheetTest and is intended
        ///to contain all SpreadsheetTest Unit Tests
        ///</summary>
        [TestClass()]
        public class GradingTests {


            private TestContext testContextInstance;

            /// <summary>
            ///Gets or sets the test context which provides
            ///information about and functionality for the current test run.
            ///</summary>
            public TestContext TestContext {
                get {
                    return testContextInstance;
                }
                set {
                    testContextInstance = value;
                }
            }

            #region Additional test attributes
            // 
            //You can use the following additional attributes as you write your tests:
            //
            //Use ClassInitialize to run code before running the first test in the class
            //[ClassInitialize()]
            //public static void MyClassInitialize(TestContext testContext)
            //{
            //}
            //
            //Use ClassCleanup to run code after all tests in a class have run
            //[ClassCleanup()]
            //public static void MyClassCleanup()
            //{
            //}
            //
            //Use TestInitialize to run code before running each test
            //[TestInitialize()]
            //public void MyTestInitialize()
            //{
            //}
            //
            //Use TestCleanup to run code after each test has run
            //[TestCleanup()]
            //public void MyTestCleanup()
            //{
            //}
            //
            #endregion

            // Verifies cells and their values, which must alternate.
            public void VV(AbstractSpreadsheet sheet, params object[] constraints) {
                for (int i = 0; i < constraints.Length; i += 2) {
                    if (constraints[i + 1] is double) {
                        Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                    }
                    else {
                        Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                    }
                }
            }


            // For setting a spreadsheet cell.
            public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents) {
                List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
                return result;
            }

            // Tests IsValid
            [TestMethod()]
            public void IsValidTest1() {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "x");
            }

            [TestMethod()]
            [ExpectedException(typeof(InvalidNameException))]
            public void IsValidTest2() {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("A1", "x");
            }

            [TestMethod()]
            public void IsValidTest3() {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "= A1 + C1");
            }

            [TestMethod()]
            [ExpectedException(typeof(FormulaFormatException))]
            public void IsValidTest4() {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("B1", "= A1 + C1");
            }

            // Tests Normalize
            [TestMethod()]
            public void NormalizeTest1() {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("", s.GetCellContents("b1"));
            }

            [TestMethod()]
            public void NormalizeTest2() {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("hello", ss.GetCellContents("b1"));
            }

            [TestMethod()]
            public void NormalizeTest3() {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("a1", "5");
                s.SetContentsOfCell("A1", "6");
                s.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
            }

            [TestMethod()]
            public void NormalizeTest4() {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("a1", "5");
                ss.SetContentsOfCell("A1", "6");
                ss.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
            }

            // Simple tests
            [TestMethod()]
            public void EmptySheet() {
                AbstractSpreadsheet ss = new Spreadsheet();
                VV(ss, "A1", "");
            }


            [TestMethod()]
            public void OneString() {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneString(ss);
            }

            public void OneString(AbstractSpreadsheet ss) {
                Set(ss, "B1", "hello");
                VV(ss, "B1", "hello");
            }


            [TestMethod()]
            public void OneNumber() {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneNumber(ss);
            }

            public void OneNumber(AbstractSpreadsheet ss) {
                Set(ss, "C1", "17.5");
                VV(ss, "C1", 17.5);
            }


            [TestMethod()]
            public void OneFormula() {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneFormula(ss);
            }

            public void OneFormula(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "5.2");
                Set(ss, "C1", "= A1+B1");
                VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
            }


            [TestMethod()]
            public void Changed() {
                AbstractSpreadsheet ss = new Spreadsheet();
                Assert.IsFalse(ss.Changed);
                Set(ss, "C1", "17.5");
                Assert.IsTrue(ss.Changed);
            }


            [TestMethod()]
            public void DivisionByZero1() {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero1(ss);
            }

            public void DivisionByZero1(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "0.0");
                Set(ss, "C1", "= A1 / B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }

            [TestMethod()]
            public void DivisionByZero2() {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero2(ss);
            }

            public void DivisionByZero2(AbstractSpreadsheet ss) {
                Set(ss, "A1", "5.0");
                Set(ss, "A3", "= A1 / 0.0");
                Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
            }



            [TestMethod()]
            public void EmptyArgument() {
                AbstractSpreadsheet ss = new Spreadsheet();
                EmptyArgument(ss);
            }

            public void EmptyArgument(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod()]
            public void StringArgument() {
                AbstractSpreadsheet ss = new Spreadsheet();
                StringArgument(ss);
            }

            public void StringArgument(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "hello");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod()]
            public void ErrorArgument() {
                AbstractSpreadsheet ss = new Spreadsheet();
                ErrorArgument(ss);
            }

            public void ErrorArgument(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= C1");
                Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
            }


            [TestMethod()]
            public void NumberFormula1() {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula1(ss);
            }

            public void NumberFormula1(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + 4.2");
                VV(ss, "C1", 8.3);
            }


            [TestMethod()]
            public void NumberFormula2() {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula2(ss);
            }

            public void NumberFormula2(AbstractSpreadsheet ss) {
                Set(ss, "A1", "= 4.6");
                VV(ss, "A1", 4.6);
            }


            // Repeats the simple tests all together
            [TestMethod()]
            public void RepeatSimpleTests() {
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
            [TestMethod()]
            public void Formulas() {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formulas(ss);
            }

            public void Formulas(AbstractSpreadsheet ss) {
                Set(ss, "A1", "4.4");
                Set(ss, "B1", "2.2");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= A1 - B1");
                Set(ss, "E1", "= A1 * B1");
                Set(ss, "F1", "= A1 / B1");
                VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
            }

            [TestMethod()]
            public void Formulasa() {
                Formulas();
            }

            [TestMethod()]
            public void Formulasb() {
                Formulas();
            }


            // Are multiple spreadsheets supported?
            [TestMethod()]
            public void Multiple() {
                AbstractSpreadsheet s1 = new Spreadsheet();
                AbstractSpreadsheet s2 = new Spreadsheet();
                Set(s1, "X1", "hello");
                Set(s2, "X1", "goodbye");
                VV(s1, "X1", "hello");
                VV(s2, "X1", "goodbye");
            }

            [TestMethod()]
            public void Multiplea() {
                Multiple();
            }

            [TestMethod()]
            public void Multipleb() {
                Multiple();
            }

            [TestMethod()]
            public void Multiplec() {
                Multiple();
            }

            // Reading/writing spreadsheets
            [TestMethod()]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest1() {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save("q:\\missing\\save.txt");
            }

            [TestMethod()]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest2() {
                AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
            }

            [TestMethod()]
            public void SaveTest3() {
                AbstractSpreadsheet s1 = new Spreadsheet();
                Set(s1, "A1", "hello");
                s1.Save("save1.txt");
                s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
                Assert.AreEqual("hello", s1.GetCellContents("A1"));
            }

            [TestMethod()]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest4() {
                using (StreamWriter writer = new StreamWriter("save2.txt")) {
                    writer.WriteLine("This");
                    writer.WriteLine("is");
                    writer.WriteLine("a");
                    writer.WriteLine("test!");
                }
                AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
            }

            [TestMethod()]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest5() {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save("save3.txt");
                ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
            }

            [TestMethod()]
            public void SaveTest6() {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
                ss.Save("save4.txt");
                Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
            }

            [TestMethod()]
            public void SaveTest7() {
                using (XmlWriter writer = XmlWriter.Create("save5.txt")) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "5.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A3");
                    writer.WriteElementString("contents", "4.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A4");
                    writer.WriteElementString("contents", "= A2 + A3");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
                VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
            }

            [TestMethod()]
            public void SaveTest8() {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "A1", "hello");
                Set(ss, "A2", "5.0");
                Set(ss, "A3", "4.0");
                Set(ss, "A4", "= A2 + A3");
                ss.Save("save6.txt");
                using (XmlReader reader = XmlReader.Create("save6.txt")) {
                    int spreadsheetCount = 0;
                    int cellCount = 0;
                    bool A1 = false;
                    bool A2 = false;
                    bool A3 = false;
                    bool A4 = false;
                    string name = null;
                    string contents = null;

                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            switch (reader.Name) {
                                case "spreadsheet":
                                    Assert.AreEqual("default", reader["version"]);
                                    spreadsheetCount++;
                                    break;

                                case "cell":
                                    cellCount++;
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
                        else {
                            switch (reader.Name) {
                                case "cell":
                                    if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                    else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                    else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                    else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                    else Assert.Fail();
                                    break;
                            }
                        }
                    }
                    Assert.AreEqual(1, spreadsheetCount);
                    Assert.AreEqual(4, cellCount);
                    Assert.IsTrue(A1);
                    Assert.IsTrue(A2);
                    Assert.IsTrue(A3);
                    Assert.IsTrue(A4);
                }
            }


            // Fun with formulas
            [TestMethod()]
            public void Formula1() {
                Formula1(new Spreadsheet());
            }
            public void Formula1(AbstractSpreadsheet ss) {
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

            [TestMethod()]
            public void Formula2() {
                Formula2(new Spreadsheet());
            }
            public void Formula2(AbstractSpreadsheet ss) {
                Set(ss, "a1", "= a2 + a3");
                Set(ss, "a2", "= a3");
                Set(ss, "a3", "6.0");
                VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
                Set(ss, "a3", "5.0");
                VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
            }

            [TestMethod()]
            public void Formula3() {
                Formula3(new Spreadsheet());
            }
            public void Formula3(AbstractSpreadsheet ss) {
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

            [TestMethod()]
            public void Formula4() {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formula1(ss);
                Formula2(ss);
                Formula3(ss);
            }

            [TestMethod()]
            public void Formula4a() {
                Formula4();
            }


            [TestMethod()]
            public void MediumSheet() {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
            }

            public void MediumSheet(AbstractSpreadsheet ss) {
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

            [TestMethod()]
            public void MediumSheeta() {
                MediumSheet();
            }


            [TestMethod()]
            public void MediumSave() {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
                ss.Save("save7.txt");
                ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
            }

            [TestMethod()]
            public void MediumSavea() {
                MediumSave();
            }


            // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
            [TestMethod()]
            public void LongFormulaTest() {
                object result = "";
                Thread t = new Thread(() => LongFormulaHelper(out result));
                t.Start();
                t.Join(60 * 1000);
                if (t.IsAlive) {
                    t.Abort();
                    Assert.Fail("Computation took longer than 60 seconds");
                }
                Assert.AreEqual("ok", result);
            }

            public void LongFormulaHelper(out object result) {
                try {
                    AbstractSpreadsheet s = new Spreadsheet();
                    s.SetContentsOfCell("sum1", "= a1 + a2");
                    int i;
                    int depth = 100;
                    for (i = 1; i <= depth * 2; i += 2) {
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
                catch (Exception e) {
                    result = e;
                }
            }
        }
    }

}