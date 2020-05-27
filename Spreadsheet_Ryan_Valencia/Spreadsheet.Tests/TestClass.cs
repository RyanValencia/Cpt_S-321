// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using CptS321;
using NUnit.Framework;

namespace Spreadsheet_Tests
{
    /// <summary>
    /// Test Class.
    /// </summary>
    [TestFixture]
    public class TestClass
    {
        /// <summary>
        /// Test case makes sure value property matches text property.
        /// </summary>
        [Test]
        public void TestValueMatchesText()
        {
        }

        /// <summary>
        /// Test case that checks to make sure Text Property remains formula.
        /// </summary>
        [Test]
        public void TestTextMatchesFormula()
        {
        }

        /// <summary>
        /// Test case to check if color matches after change.
        /// </summary>
        [Test]
        public void TestColor()
        {
            Spreadsheet testSheet = new Spreadsheet(50, 26);
            SpreadsheetCell cell1 = testSheet.GetCell(0, 0) as SpreadsheetCell;

            // FF6600 is a sort of orange from some random color picker I found online. should match.
            cell1.BGColor = 0xFF6600;
            Assert.AreEqual(cell1.BGColor, 0xFF6600);
        }
    }
}
