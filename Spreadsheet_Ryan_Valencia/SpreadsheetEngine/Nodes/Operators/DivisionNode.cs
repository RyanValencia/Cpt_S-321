// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Defines the Division Operation.
    /// </summary>
    internal class DivisionNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionNode"/> class.
        /// </summary>
        public DivisionNode()
             : base('/')
        {
            this.Precedence = 2;
            this.Associativity = Associative.Left;
        }

        /// <summary>
        /// Divides the constants to the left and right of the node.
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

            return leftValue / rightValue;
        }
    }
}
