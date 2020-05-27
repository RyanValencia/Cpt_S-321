// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CptS321
{
    /// <summary>
    /// Spreadsheet class that inherits from the Cell class.
    /// </summary>
    public class Spreadsheet
    {
        private readonly Cell[,] spreadsheetArray;
        private readonly Dictionary<string, int> letterIndex = new Dictionary<string, int>();
        private readonly int rowCount;
        private readonly int colCount;

        // Stacks that keep track of previous states.
        private readonly Stack<InterfaceCommands> undo = new Stack<InterfaceCommands>();
        private readonly Stack<InterfaceCommands> redo = new Stack<InterfaceCommands>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rowIndex"> The number of rows in the spreadsheet.</param>
        /// <param name="colIndex"> The number of columns in the spreadsheet.</param>
        public Spreadsheet(int rowIndex, int colIndex)
        {
            this.spreadsheetArray = new Cell[rowIndex, colIndex];
            this.rowCount = rowIndex;
            this.colCount = colIndex;
            for (int rowNum = 0; rowNum < rowIndex; rowNum++)
            {
                for (int colNum = 0; colNum < colIndex; colNum++)
                {
                    this.spreadsheetArray[rowNum, colNum] = new SpreadsheetCell(rowNum, colNum);
                    this.spreadsheetArray[rowNum, colNum].PropertyChanged += this.OnCellPropertyChanged;
                }
            }

            int dictIndex = 0;
            for (int alphabeticNumVal = 65; alphabeticNumVal < 91; alphabeticNumVal++)
            {
                this.letterIndex.Add(((char)alphabeticNumVal).ToString(), dictIndex);
                dictIndex++;
            }
        }

        /// <summary>
        /// Event that states when a cell property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler CellPropertyChanged = (sender, e) => { };

        /// <summary>
        /// Gets the cell specified by the parameters.
        /// </summary>
        /// <param name="rowIndex">row the cell is at.</param>
        /// <param name="colIndex">column the cell is at.</param>
        /// <returns>Cell matching row and col.</returns>
        public Cell GetCell(int rowIndex, int colIndex)
        {
            if (rowIndex > this.spreadsheetArray.GetLength(0) || colIndex > this.spreadsheetArray.GetLength(1))
            {
                return null;
            }
            else
            {
                return this.spreadsheetArray[rowIndex, colIndex];
            }
        }

        /// <summary>
        /// Similar to GetCellText, but used for getting the cell from a string in an XML file.
        /// </summary>
        /// <param name="cellName"> The name/ location of the cell. </param>
        /// <returns> the desired cell. </returns>
        public Cell GetCellString(string cellName)
        {
            int row = Convert.ToInt32(cellName.Substring(1)) - 1;
            int col = this.letterIndex[cellName[0].ToString()];
            return this.GetCell(row, col);
        }

        /// <summary>
        /// When a cell has the property of another cell, we want to get where that original cell is.
        /// This will be called when a cell's Text starts with '='.
        /// </summary>
        /// <param name="cellText"> Text of cell assigned to another cell. </param>
        /// <returns> The original cell. </returns>
        public Cell GetCellText(string cellText)
        {
            int row = Convert.ToInt32(cellText.Substring(1)) - 1;
            int col = this.letterIndex[cellText[0].ToString()];
            return this.GetCell(row, col);
        }

        /// <summary>
        /// Adds command to undo stack.
        /// </summary>
        /// <param name="command"> command being added.</param>
        public void AddUndo(InterfaceCommands command)
        {
            this.undo.Push(command);
            this.CellPropertyChanged(command, new PropertyChangedEventArgs("UndosExist"));
        }

        /// <summary>
        /// Properly manipulates undo and redo stacks for undo command.
        /// </summary>
        public void UndoCommand()
        {
            this.undo.Peek().UndoChange();
            this.redo.Push(this.undo.Pop());
            this.CellPropertyChanged(this.redo.Peek(), new PropertyChangedEventArgs("RedosExist"));

            if (this.undo.Count > 0)
            {
                this.CellPropertyChanged(this.redo.Peek(), new PropertyChangedEventArgs("UndosExist"));
            }
            else
            {
                this.CellPropertyChanged(this.redo.Peek(), new PropertyChangedEventArgs("NoUndos"));
            }
        }

        /// <summary>
        /// Properly manipulates undo and redo stacks for redo command.
        /// </summary>
        public void RedoCommand()
        {
            this.redo.Peek().Change();
            this.undo.Push(this.redo.Pop());
            this.CellPropertyChanged(this.undo.Peek(), new PropertyChangedEventArgs("UndosExist"));

            if (this.redo.Count > 0)
            {
                this.CellPropertyChanged(this.undo.Peek(), new PropertyChangedEventArgs("RedosExist"));
            }
            else
            {
                this.CellPropertyChanged(this.undo.Peek(), new PropertyChangedEventArgs("NoRedos"));
            }
        }

        /// <summary>
        /// Saves the sheet in xml format.
        /// </summary>
        /// <param name="outfile"> outstream for the xml file. </param>
        public void SaveFile(Stream outfile)
        {
            // Create writer.
            XmlWriter xmlWriter = XmlWriter.Create(outfile);

            // Start xml file.
            xmlWriter.WriteStartDocument();

            // Write spreadsheet element.
            xmlWriter.WriteStartElement("spreadsheet");

            for (int row = 0; row < this.rowCount; row++)
            {
                for (int col = 0; col < this.colCount; col++)
                {
                    Cell cell = this.GetCell(row, col);
                    if (cell.Text != string.Empty || cell.BGColor != 0xFFFFFFFF)
                    {
                        // Write cell element.
                        xmlWriter.WriteStartElement("cell");

                        // Write name element then close it.
                        xmlWriter.WriteStartElement("name");
                        xmlWriter.WriteString(cell.Index);
                        xmlWriter.WriteEndElement();

                        // Write bgcolor element then close it.
                        xmlWriter.WriteStartElement("bgcolor");
                        xmlWriter.WriteString(cell.BGColor.ToString());
                        xmlWriter.WriteEndElement();

                        // Write text element then close it.
                        xmlWriter.WriteStartElement("text");
                        xmlWriter.WriteString(cell.Text);
                        xmlWriter.WriteEndElement();

                        // Close cell element.
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            // Close spreadsheet element.
            xmlWriter.WriteEndElement();

            // End xml file.
            xmlWriter.WriteEndDocument();

            // Close writer.
            xmlWriter.Close();
        }

        /// <summary>
        /// Loads a sheet from xml format.
        /// </summary>
        /// <param name="infile"> instream for xml file. </param>
        public void LoadFile(Stream infile)
        {
            var settings = new XmlReaderSettings()
            {
                IgnoreWhitespace = true,
            };

            XmlReader reader = XmlReader.Create(infile, settings);

            string cellContent;
            Cell cell;

            // open reader.
            reader.ReadStartElement("spreadsheet");

            // Read the cells from the file and set their properties to the corresponding cells in the spreadsheet.
            while (reader.Name == "cell")
            {
                // Beginning of cell.
                reader.ReadStartElement("cell");

                // Read the cell name and get that cell.
                reader.ReadStartElement("name");
                cellContent = reader.ReadContentAsString();
                cell = this.GetCellText(cellContent);
                reader.ReadEndElement();

                // Read the cell color and set the cell color in the spreadsheet to that color.
                reader.ReadStartElement("bgcolor");
                cellContent = reader.ReadContentAsString();
                uint.TryParse(cellContent, out uint colorText);
                cell.BGColor = colorText;
                reader.ReadEndElement();

                // Read the cell text and set the cell text in the spreadsheet to that text.
                reader.ReadStartElement("text");
                cellContent = reader.ReadContentAsString();
                cell.Text = cellContent;
                reader.ReadEndElement();

                // End of cell.
                reader.ReadEndElement();
            }

            // close reader.
            reader.ReadEndElement();
        }

        /// <summary>
        /// method that invokes the property changed event.
        /// </summary>
        /// <param name="sender">object being changed.</param>
        /// <param name="e">property being changed.</param>
        protected void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SpreadsheetCell sheetCell = (SpreadsheetCell)sender;

            if (e.PropertyName == "Color")
            {
                this.CellPropertyChanged("Color", new PropertyChangedEventArgs(sheetCell.RowIndex.ToString() + ","
                    + sheetCell.ColIndex.ToString() + "," + sheetCell.BGColor));
                return;
            }

            if (e.PropertyName == "Value")
            {
                this.CellPropertyChanged(this, new PropertyChangedEventArgs(sheetCell.RowIndex.ToString() + ","
                    + sheetCell.ColIndex.ToString() + "," + sheetCell.Value));
                return;
            }

            if (sheetCell.Text.Length == 0 || sheetCell.Text[0] != '=')
            {
                sheetCell.Value = sheetCell.Text;
                if (sheetCell.VarDict.Count > 0)
                {
                    foreach (KeyValuePair<string, double> index in sheetCell.VarDict.ToList())
                    {
                        sheetCell.UnmatchTreeToCell(this.GetCellText(index.Key));
                    }
                }

                this.CellPropertyChanged(sheetCell, new PropertyChangedEventArgs("Text"));
            }
            else
            {
                sheetCell.BuildTree(sheetCell.Text.Substring(1));
                if (this.CheckReference(sheetCell))
                {
                    foreach (KeyValuePair<string, double> index in sheetCell.VarDict.ToList())
                    {
                        sheetCell.MatchTreeToCell(this.GetCellText(index.Key));
                    }

                    sheetCell.Value = sheetCell.EvaluateExpression();
                }

                this.CellPropertyChanged(this, new PropertyChangedEventArgs(sheetCell.RowIndex.ToString() + ","
                    + sheetCell.ColIndex.ToString() + "," + sheetCell.Value));
            }
        }

        /// <summary>
        /// For checking to see if a cell references a cell within the range of the spreadsheet.
        /// </summary>
        /// <param name="name"> cell name. </param>
        /// <returns> If the cell is within range or not. </returns>
        private bool CheckCellRange(string name)
        {
            char col = name[0];

            int.TryParse(name.Substring(1), out int row);
            int colNum = Convert.ToInt32(col) - 65;

            if (row < 0 || row > this.rowCount || colNum < 0 || colNum > this.colCount)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checking if a cell has a circular reference.
        /// </summary>
        /// <param name="name"> cell Name. </param>
        /// <param name="refCell"> cell that is being referenced. </param>
        /// <returns> True if circular. False if not. </returns>
        private bool CheckCircular(string name, string refCell)
        {
            Cell cell = this.GetCellString(name);

            foreach (KeyValuePair<string, double> cellName in cell.VarDict.ToList())
            {
                if (cellName.Key != refCell)
                {
                    return true;
                }

                return this.CheckCircular(cellName.Key, refCell);
            }

            return false;
        }

        /// <summary>
        /// Changes the value of the cell if it has any bad references.
        /// </summary>
        /// <param name="cell"> cell being checked. </param>
        /// <returns> True if references are good; False otherwise. </returns>
        private bool CheckReference(Cell cell)
        {
            foreach (KeyValuePair<string, double> cellName in cell.VarDict.ToList())
            {
                if (cellName.Key == cell.Index)
                {
                    cell.Value = "!(self reference)";
                    return false;
                }
                else if (this.CheckCircular(cellName.Key, cell.Index))
                {
                    cell.Value = "!(circular reference)";
                    return false;
                }
                else if (this.CheckCellRange(cellName.Key))
                {
                    cell.Value = "!(bad reference)";
                    return false;
                }
            }

            return true;
        }
    }
}
