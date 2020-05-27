// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Made this class to be able to access Cell.
    /// </summary>
    public class SpreadsheetCell : Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetCell"/> class.
        /// </summary>
        /// <param name="rowIndex">Number of Rows.</param>
        /// <param name="colIndex">Number of Columns.</param>
        public SpreadsheetCell(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
        }
    }
}
