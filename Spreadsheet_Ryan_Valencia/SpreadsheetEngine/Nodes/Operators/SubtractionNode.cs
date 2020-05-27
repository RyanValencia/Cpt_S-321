// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Defines the Subtraction operation.
    /// </summary>
    internal class SubtractionNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionNode"/> class.
        /// </summary>
        public SubtractionNode()
            : base('-')
        {
            this.Precedence = 1;
            this.Associativity = Associative.Left;
        }

        /// <summary>
        /// Subtracts the constants to the left and right of the node.
        /// </summary>
        /// <returns> The result of the evalution.</returns>
        public override double Evaluate()
        {
            ConstantNode left = this.Left as ConstantNode;
            ConstantNode right = this.Right as ConstantNode;

            double leftValue = 0, rightValue = 0;
            if (this.Left is OperatorNode leftOperator)
            {
                leftValue += leftOperator.Evaluate();
            }
            else
            {
                leftValue = left.Value;
            }

            if (this.Right is OperatorNode rightOperator)
            {
                rightValue += rightOperator.Evaluate();
            }
            else
            {
                rightValue = right.Value;
            }

            return leftValue - rightValue;
        }
    }
}
