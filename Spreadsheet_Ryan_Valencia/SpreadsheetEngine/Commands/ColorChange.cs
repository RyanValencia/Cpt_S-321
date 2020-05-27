// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Changes color property of cell.
    /// </summary>
    public class ColorChange : InterfaceCommands
    {
        private readonly uint newColor;
        private readonly List<uint> oldColors = new List<uint>();
        private readonly List<Cell> cells = new List<Cell>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChange"/> class.
        /// </summary>
        /// <param name="newColor"> new color of the cell. </param>
        /// <param name="oldColors"> previous colors of the cell. </param>
        /// <param name="cells"> the cell.  </param>
        public ColorChange(uint newColor, List<uint> oldColors, List<Cell> cells)
        {
            this.newColor = newColor;
            this.oldColors = oldColors;
            this.cells = cells;
        }

        /// <summary>
        /// Changes each cell to their new colors.
        /// </summary>
        public void Change()
        {
            for (int i = 0; i < this.cells.Count; i++)
            {
                this.cells[i].BGColor = this.newColor;
            }
        }

        /// <summary>
        /// Changes each cell to their old colors.
        /// </summary>
        public void UndoChange()
        {
            for (int i = 0; i < this.cells.Count; i++)
            {
                this.cells[i].BGColor = this.oldColors[i];
            }
        }
    }
}
