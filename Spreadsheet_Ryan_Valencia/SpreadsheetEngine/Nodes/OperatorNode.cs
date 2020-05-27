// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Base operator node class that creates the more specific nodes when called.
    /// </summary>
   internal class OperatorNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="op">operator char.</param>
        public OperatorNode(char op)
        {
            this.Operator = op;
            this.Left = this.Right = null;
        }

        /// <summary>
        /// Value of the operator that determines which side of it gets evaluated first.
        /// </summary>
        public enum Associative
        {
            /// <summary>
            /// Left associativity.
            /// </summary>
            Left,

            /// <summary>
            /// Right associativity.
            /// </summary>
            Right,
        }

        /// <summary>
        /// Gets or sets the associativity of the operator.
        /// </summary>
        public Associative Associativity { get; set; }

        /// <summary>
        /// Gets or sets the Precedence of the operator.
        /// </summary>
        public ushort Precedence { get; set; }

        /// <summary>
        /// Gets or Sets the Operator character.
        /// </summary>
        public char Operator { get; set; }

        /// <summary>
        /// Gets or sets the left node.
        /// </summary>
        public Node Left { get; set; }

        /// <summary>
        /// Gets or sets the right node.
        /// </summary>
        public Node Right { get; set; }

        /// <summary>
        /// Base Evaluate where there isn't an evaluation.
        /// </summary>
        /// <returns> Value of the evaluation.</returns>
        public virtual double Evaluate()
        {
            return 0;
        }
    }
}
