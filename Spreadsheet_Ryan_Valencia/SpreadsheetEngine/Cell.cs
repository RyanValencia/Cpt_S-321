// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CptS321
{
    /// <summary>
    /// Abstract Cell class that represents one cell in the worksheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged, IXmlSerializable
    {
        /// <summary>
        /// Variables contained in the cell's expression.
        /// </summary>
        public Dictionary<string, double> VarDict = new Dictionary<string, double>();

        /// <summary>
        /// This string contains the text within the cell.
        /// </summary>
        protected string text;

        /// <summary>
        /// This string contains the value within the cell.
        /// </summary>
        protected string value;

        /// <summary>
        /// The expression of the cell.
        /// </summary>
        protected ExpressionTree tree;

        /// <summary>
        /// uint matching the color of the cell.
        /// </summary>
        protected uint color;

        /// <summary>
        /// Need to create an index to know the respective column letter of the cell.
        /// </summary>
        private readonly Dictionary<int, string> letterIndex = new Dictionary<int, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex">The row of the cell.</param>
        /// <param name="colIndex">The column of the cell.</param>
        public Cell(int rowIndex = 0, int colIndex = 0)
        {
            this.RowIndex = rowIndex;
            this.ColIndex = colIndex;
            this.Text = string.Empty;
            this.Value = string.Empty;
            this.color = 0xFFFFFFFF;

            // When cell is created, fill letterIndex dictionary.
            int dictIndex = 0;
            for (int alphabeticNumVal = 65; alphabeticNumVal < 91; alphabeticNumVal++)
            {
                this.letterIndex.Add(dictIndex, ((char)alphabeticNumVal).ToString());
                dictIndex++;
            }
        }

        /// <summary>
        /// Property Changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Gets the row of the cell.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the column of the cell.
        /// </summary>
        public int ColIndex { get; }

        /// <summary>
        /// Gets the Index which is the letter-number representation of the cell.
        /// </summary>
        public string Index
        {
            get
            {
                return this.letterIndex[this.ColIndex].ToString() + (this.RowIndex + 1).ToString();
            }
        }

        /// <summary>
        /// Gets or sets the text within the cell.
        /// </summary>
        public string Text
        {
            get => this.text;

            set
            {
               if (value == this.text)
                {
                    return;
                }
               else
                {
                    this.text = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        /// <summary>
        /// Gets or sets value variable. Property gets set by Spreadsheet.cs.
        /// </summary>
        public string Value
        {
            get => this.value;

            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// Gets or sets the uint color variable.
        /// </summary>
        public uint BGColor
        {
            get => this.color;

            set
            {
                if (value == this.color)
                {
                    return;
                }

                this.color = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
            }
        }

        /// <summary>
        /// Needed this to remove error.
        /// </summary>
        /// <returns> returns nothing. </returns>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Needed this to remove error.
        /// </summary>
        /// <param name="reader"> reader. </param>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Format cell in XML.
        /// </summary>
        /// <param name="writer"> writer. </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("cell");
            writer.WriteElementString("value", this.Value);
            writer.WriteElementString("text", this.text);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Matches expression tree to a specific cell.
        /// </summary>
        /// <param name="cell"> Cell specified. </param>
        public void MatchTreeToCell(Cell cell)
        {
            this.tree.MatchCell(cell);
        }

        /// <summary>
        /// Unmatches expression tree to a specific cell.
        /// </summary>
        /// <param name="cell"> Cell specified. </param>
        public void UnmatchTreeToCell(Cell cell)
        {
            this.tree.UnmatchCell(cell);
        }

        /// <summary>
        /// Builds the cell's expression tree.
        /// </summary>
        /// <param name="expression"> Expression being made into a tree. </param>
        public void BuildTree(string expression)
        {
            this.tree = new ExpressionTree(expression);
            this.VarDict = this.tree.Variables;
            this.tree.ExpressionCell = this;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <returns> Returns the evaluated expression as a string. </returns>
        public string EvaluateExpression()
        {
            return this.tree.Evaluate().ToString();
        }
    }
}
