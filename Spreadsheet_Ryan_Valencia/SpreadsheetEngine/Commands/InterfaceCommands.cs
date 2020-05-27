// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Command Interface that all commands inherit.
    /// </summary>
    public interface InterfaceCommands
    {
        /// <summary>
        /// Command to update respective change command.
        /// </summary>
        void Change();

        /// <summary>
        /// Command to undo respective change command.
        /// </summary>
        void UndoChange();
    }
}
