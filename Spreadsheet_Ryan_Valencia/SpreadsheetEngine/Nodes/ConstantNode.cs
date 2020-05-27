// Ryan Valencia - 1164233

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Holds a constant value in the tree.
    /// </summary>
    internal class ConstantNode : Node
    {
        /// <summary>
        /// Gets or sets value of Constant Node.
        /// </summary>
        public double Value { get; set; }
    }
}
