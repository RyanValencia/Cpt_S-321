// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Command that changes cell text.
    /// </summary>
    public class TextChange : InterfaceCommands
    {
        private readonly string oldText = string.Empty;
        private readonly string newText = string.Empty;
        private readonly Cell cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextChange"/> class.
        /// </summary>
        /// <param name="oldText"> previous text of the cell. </param>
        /// <param name="newText"> new text of the cell. </param>
        /// <param name="cell"> specific cell. </param>
        public TextChange(string oldText, string newText, Cell cell)
        {
            this.oldText = oldText;
            this.newText = newText;
            this.cell = cell;
        }

        /// <summary>
        /// Change Text property to new text.
        /// </summary>
        public void Change()
        {
            this.cell.Text = this.newText;
        }

        /// <summary>
        /// Change Text property to old text.
        /// </summary>
        public void UndoChange()
        {
            this.cell.Text = this.oldText;
        }
    }
}
