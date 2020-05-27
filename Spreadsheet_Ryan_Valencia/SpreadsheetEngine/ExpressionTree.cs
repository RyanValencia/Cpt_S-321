// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    /// <summary>
    /// Expression tree that computes value.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// Cell of the expression.
        /// </summary>
        public Cell ExpressionCell;

        /// <summary>
        /// The expression of the value that fits the simplified format.
        /// </summary>
        private string simplifiedExpression;

        /// <summary>
        /// treeQueue is the order the tree will be evaluated.
        /// </summary>
        private Queue<Node> treeQueue = null;

        /// <summary>
        /// Root Node of the expression tree.
        /// </summary>
        private Node rootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// Constructs the tree from the specific expression.
        /// </summary>
        /// <param name="expression"> expression that the tree is constructed from.</param>
        public ExpressionTree(string expression)
        {
            if (expression.Length != 0)
            {
                this.Expression = expression;
            }
            else
            {
                throw new ArgumentException("No expression");
            }
        }

        /// <summary>
        /// Gets the variables dictionary.
        /// </summary>
        public Dictionary<string, double> Variables { get; } = new Dictionary<string, double>();

        /// <summary>
        /// Gets or sets the expression with specified formatting.
        /// </summary>
        public string Expression
        {
            get
            {
                return this.simplifiedExpression;
            }

            set
            {
                this.simplifiedExpression = value;
                if (this.simplifiedExpression.Length != 0)
                {
                    this.treeQueue = this.ShuntingYardAlgorithm(this.Expression);
                    this.CreateTree();
                }
                else
                {
                    throw new ArgumentException("No expression.");
                }
            }
        }

        /// <summary>
        /// Sets variable in the dictionary as a value.
        /// </summary>
        /// <param name="variableName"> variableName. </param>
        /// <param name="variableValue"> Value of variable named. </param>
        public void SetVariable(string variableName, double variableValue)
        {
            if (this.Variables.ContainsKey(variableName))
            {
                this.Variables[variableName] = variableValue;
                this.CreateTree();
            }
            else
            {
                this.Variables[variableName] = variableValue;
            }
        }

        /// <summary>
        /// Matches the variables to their correct cells in the spreadsheet.
        /// </summary>
        /// <param name="cell"> The correct Cell. </param>
        public void MatchCell(Cell cell)
        {
            cell.PropertyChanged += this.CellPropertyChanged;
            if (this.Variables.ContainsKey(cell.Index))
            {
                if (double.TryParse(cell.Value, out double val))
                {
                    this.Variables[cell.Index] = val;
                }
                else
                {
                    this.Variables[cell.Index] = 0;
                }
            }
        }

        /// <summary>
        /// Unmatches variables from their correct cells in the spreadsheet.
        /// </summary>
        /// <param name="cell"> The correct cell. </param>
        public void UnmatchCell(Cell cell)
        {
            cell.PropertyChanged -= this.CellPropertyChanged;
        }

        /// <summary>
        /// Will trigger when a cell property has changed.
        /// </summary>
        /// <param name="sender"> SpreadsheetCell. </param>
        /// <param name="e"> arguments. </param>
        public void CellPropertyChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(SpreadsheetCell))
            {
                SpreadsheetCell cell = sender as SpreadsheetCell;
                if (this.Variables.ContainsKey(cell.Index))
                {
                    if (double.TryParse(cell.Value, out double val))
                    {
                        this.Variables[cell.Index] = val;
                    }
                    else
                    {
                        this.Variables[cell.Index] = 0;
                    }

                    this.ExpressionCell.Value = this.Evaluate().ToString();
                }
            }
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <returns> the evaluated expression. </returns>
        public double Evaluate()
        {
            return this.Evaluate(this.rootNode);
        }

        private double Evaluate(Node node)
        {
            if (node != null)
            {
                if (node is OperatorNode)
                {
                    OperatorNode opNode = node as OperatorNode;
                    double evaluation = opNode.Evaluate();
                    return evaluation;
                }

                if (node is ConstantNode)
                {
                    ConstantNode constNode = node as ConstantNode;
                    return constNode.Value;
                }

                if (node is VariableNode)
                {
                    VariableNode varNode = node as VariableNode;
                    return this.Variables[varNode.Name];
                }
            }

            return 0;
        }

        /// <summary>
        /// Edsger Dijkstra's Shunting Yard algorithm to convert an infix expression into postfix expression.
        /// </summary>
        /// <param name="infixExpression"> Infix expression. </param>
        /// <returns> Postfix expression. </returns>
        private Queue<Node> ShuntingYardAlgorithm(string infixExpression)
        {
            Queue<Node> postfixExpression = new Queue<Node>();
            Stack<OperatorNode> operatorStack = new Stack<OperatorNode>();
            for (int index = 0; index < infixExpression.Length;)
            {
                // Need to create substring until the first operator in the expression is reached.
                string substring = string.Empty;
                int substringIndex;
                for (substringIndex = index; substringIndex < infixExpression.Length && !ExpressionTreeFactory.MatchesCharacter(infixExpression[substringIndex]); substringIndex++)
                {
                    substring += infixExpression[substringIndex];
                }

                // If the substring is a number then turn that substring into one.
                if (double.TryParse(substring, out double numSubstring))
                {
                    postfixExpression.Enqueue(new ConstantNode() { Value = numSubstring });
                    index = substringIndex;
                }
                else
                {
                    if (ExpressionTreeFactory.MatchesOperator(infixExpression[index]))
                    {
                        OperatorNode op = ExpressionTreeFactory.CreateOperatorNode(infixExpression[index]);
                        try
                        {
                            // If there are operators on the operatorStack with higher precedence than op then they need to be removed from the stack and added to the postfixExpression first.
                            while (operatorStack.Peek().Precedence >= op.Precedence && operatorStack.Peek().Operator != '(')
                            {
                                postfixExpression.Enqueue(operatorStack.Pop());
                            }
                        }
                        catch
                        {
                            // Possible for there to be an empty operatorStack. In that case, nothing happens.
                        }

                        operatorStack.Push(op);
                    }
                    else if (infixExpression[index] == '(')
                    {
                        operatorStack.Push(new OperatorNode('(') { Precedence = 4 });
                    }
                    else if (infixExpression[index] == ')')
                    {
                        try
                        {
                            while (operatorStack.Peek().Operator != '(')
                            {
                                postfixExpression.Enqueue(operatorStack.Pop());
                            }
                        }
                        catch
                        {
                            throw new ArgumentException("Missing a left parenthesis");
                        }

                        operatorStack.Pop();
                    }
                    else
                    {
                        // If the substring cannot be parsed as a double (constant) or parentheses then it's a variable.
                        postfixExpression.Enqueue(new VariableNode() { Name = substring });
                        if (!this.Variables.ContainsKey(substring))
                        {
                            this.SetVariable(substring, 0);
                        }

                        index = substringIndex - 1;
                    }

                    index++;
                }
            }

            // End of the expression, popping all operators onto the queue
            while (operatorStack.Count != 0)
            {
                if (operatorStack.Peek().Operator != '(')
                {
                    postfixExpression.Enqueue(operatorStack.Pop());
                }
                else
                {
                    throw new ArgumentException("Missing a right parenthesis");
                }
            }

            return postfixExpression;
        }

        /// <summary>
        /// Takes the expression stack and creates the expression tree.
        /// </summary>
        /// <returns> The root of the expression tree. </returns>
        private Node CreateTree()
        {
            Stack<Node> expressionStack = new Stack<Node>();
            Stack<Node> opStack = new Stack<Node>();
            Queue<Node> treeQueueCopy = new Queue<Node>();

            // Making sure nothing is in the tree before it is made.
            this.rootNode = null;

            while (this.treeQueue.Count > 0)
            {
                expressionStack.Push(this.treeQueue.Peek());
                treeQueueCopy.Enqueue(this.treeQueue.Dequeue());
            }

            if (this.rootNode == null)
            {
                try
                {
                    if (expressionStack.Peek() is OperatorNode opNode)
                    {
                        opNode.Left = null;
                        opNode.Right = null;
                        this.rootNode = opNode;
                    }
                    else
                    {
                        ConstantNode constNode = new ConstantNode();

                        if (expressionStack.Peek() is VariableNode varNode)
                        {
                            constNode = new ConstantNode() { Value = this.Variables[varNode.Name] };
                        }

                        this.rootNode = constNode;
                    }

                    expressionStack.Pop();
                }
                catch
                {
                    throw new ArgumentException("No expression");
                }
            }

            while (expressionStack.Count > 0)
            {
                this.TraverseTree(expressionStack.Pop(), this.rootNode);
            }

            while (treeQueueCopy.Count > 0)
            {
                this.treeQueue.Enqueue(treeQueueCopy.Dequeue());
            }

            return this.rootNode;
        }

        /// <summary>
        /// Finds where to insert a new node into the expression tree and calls InsertNode().
        /// </summary>
        /// <param name="newNode"> The node being inserted. </param>
        /// <param name="currentNode"> The current position in the expression tree. </param>
        /// <returns> True or false depending on if the node could be inserted. Helps with recursion of the tree. </returns>
        private bool TraverseTree(Node newNode, Node currentNode)
        {
            OperatorNode positionNode = currentNode as OperatorNode;
            OperatorNode rightNode = positionNode.Right as OperatorNode;

            // Right first to match the order of the expression stack.
            if (positionNode.Right != null && rightNode != null)
            {
                if (!this.TraverseTree(newNode, rightNode))
                {
                    if (positionNode.Left is OperatorNode leftNode)
                    {
                        if (this.TraverseTree(newNode, leftNode))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return this.InsertNode(newNode, currentNode);
                    }
                }
            }
            else if (positionNode.Left != null)
            {
                if (positionNode.Left is OperatorNode leftNode)
                {
                    if (this.TraverseTree(newNode, leftNode))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return this.InsertNode(newNode, currentNode);
                }
            }

            return this.InsertNode(newNode, currentNode);
        }

        /// <summary>
        /// Places a new node into the expression tree from the currentNode in either the left or right branches.
        /// </summary>
        /// <param name="newNode"> The node being inserted. </param>
        /// <param name="currentNode"> The current position in the expression tree. </param>
        /// <returns> True of false depending on if the node could be inserted. Helps TraverseTree() with recursion. </returns>
        private bool InsertNode(Node newNode, Node currentNode)
        {
            OperatorNode positionNode = currentNode as OperatorNode;
            ConstantNode constNode = newNode as ConstantNode;
            OperatorNode opNode = newNode as OperatorNode;
            VariableNode varNode = newNode as VariableNode;

            // Right first to match the order of the expression stack.
            if (positionNode.Right == null)
            {
                if (opNode != null)
                {
                    // Making sure nothing is in the new branches as the tree is being made.
                    opNode.Left = null;
                    opNode.Right = null;
                    positionNode.Right = opNode;
                }
                else
                {
                    if (varNode != null)
                    {
                        constNode = new ConstantNode() { Value = this.Variables[varNode.Name] };
                    }

                    positionNode.Right = constNode;
                }

                return true;
            }
            else if (positionNode.Left == null)
            {
                if (opNode != null)
                {
                    // Making sure nothing is in the new branches as the tree is being made.
                    opNode.Left = null;
                    opNode.Right = null;
                    positionNode.Left = opNode;
                }
                else
                {
                    if (varNode != null)
                    {
                        constNode = new ConstantNode() { Value = this.Variables[varNode.Name] };
                    }

                    positionNode.Left = constNode;
                }

                return true;
            }

            return false;
        }
    }
}
