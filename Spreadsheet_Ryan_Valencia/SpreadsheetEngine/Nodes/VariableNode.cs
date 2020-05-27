// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Node that holds the name of a variable.
    /// </summary>
    internal class VariableNode : Node
    {
        /// <summary>
        /// Gets or sets name of variable.
        /// </summary>
        public string Name { get; set; }
    }
}
