// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Determines the correct node to create for the tree.
    /// </summary>
    internal class ExpressionTreeFactory
    {
        /// <summary>
        /// Initializes correct operator node.
        /// </summary>
        /// <param name="op"> Operator to be made. </param>
        /// <returns> Appropriate Operator Node. </returns>
        public static OperatorNode CreateOperatorNode(char op)
        {
            switch (op)
            {
                case '+': return new AdditionNode();
                case '-': return new SubtractionNode();
                case '*': return new MultiplicationNode();
                case '/': return new DivisionNode();
            }

            // I don't know how it would ever get here when the operator is always checked
            // to see if it is valid prior to creating the respective node.
            throw new Exception("Invalid operator");
        }

        /// <summary>
        /// This function makes it so parenthesis are accepted by expressions.
        /// </summary>
        /// <param name="ch"> Character being checked. </param>
        /// <returns> If the character is an accepted one or not. </returns>
        public static bool MatchesCharacter(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '(':
                case ')': return true;
            }

            return false;
        }

        /// <summary>
        /// MatchesOperator makes sure the operator is an accepted operator.
        /// </summary>
        /// <param name="op"> Operator being checked.</param>
        /// <returns> True if op is a defined operator, false if not. </returns>
        public static bool MatchesOperator(char op)
        {
            switch (op)
            {
                case '+':
                case '-':
                case '*':
                case '/': return true;
            }

            return false;
        }
    }
}
