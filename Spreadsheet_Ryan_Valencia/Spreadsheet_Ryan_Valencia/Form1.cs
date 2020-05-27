// Ryan Valencia - 11642323

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CptS321;

namespace Spreadsheet_Ryan_Valencia
{
    /// <summary>
    /// partial class premade by VS.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// loader.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> arguments. </param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.sheet = new Spreadsheet(50, 26);
            this.dataGridView1.RowCount = 50;

            for (int rowNum = 0; rowNum < 50; rowNum++)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[rowNum].HeaderCell.Value = (rowNum + 1).ToString();
            }

            this.dataGridView1.ColumnCount = 26;
            int count = 0;

            for (int index = 65; index < 91; index++)
            {
                this.dataGridView1.Columns[count].Name = ((char)index).ToString();
                count++;
            }

            this.sheet.CellPropertyChanged += this.UpdateCell;
        }

        /// <summary>
        /// When the cell is changed.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> Spreadsheet arguments. </param>
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                string cellValue = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (this.sheet.GetCell(e.RowIndex, e.ColumnIndex).Value != cellValue)
                {
                    Cell cell = this.sheet.GetCell(e.RowIndex, e.ColumnIndex);
                    TextChange textChange = new TextChange(cell.Text, cellValue, cell);
                    this.sheet.AddUndo(textChange);
                    this.sheet.GetCell(e.RowIndex, e.ColumnIndex).Text = cellValue;
                }
            }
        }

        /// <summary>
        /// Whenever a cell property is changed, this function will update it.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> Spreadsheet arguments. </param>
        private void UpdateCell(object sender, PropertyChangedEventArgs e)
        {
            if (sender.GetType() == typeof(Spreadsheet))
            {
                string[] properties = e.PropertyName.Split(',');
                this.dataGridView1.Rows[Convert.ToInt32(properties[0])].Cells[Convert.ToInt32(properties[1])].Value = properties[2];
            }

            // update color
            else if (sender.GetType() == typeof(string))
            {
                string[] cellProperties = e.PropertyName.Split(',');
                if (uint.TryParse(cellProperties[2], out uint num))
                {
                    this.dataGridView1.Rows[Convert.ToInt32(cellProperties[0])].Cells[Convert.ToInt32(cellProperties[1])].Style.BackColor =
                        Color.FromArgb((int)num);
                }
            }

            // enable buttons when undos/redos exist, disable otherwise
            else if (sender is InterfaceCommands)
            {
                if (e.PropertyName == "UndosExist")
                {
                    this.undoToolStripMenuItem.Enabled = true;
                }
                else if (e.PropertyName == "NoUndos")
                {
                    this.undoToolStripMenuItem.Enabled = false;
                }
                else if (e.PropertyName == "RedosExist")
                {
                    this.redoToolStripMenuItem.Enabled = true;
                }
                else if (e.PropertyName == "NoRedos")
                {
                    this.redoToolStripMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// When the cell is edited, shows text.
        /// </summary>
        /// <param name="sender"> sender.</param>
        /// <param name="e"> Spreadsheet arguments. </param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string cellText = this.sheet.GetCell(e.RowIndex, e.ColumnIndex).Text;

            if (cellText != null)
            {
                if (cellText.Length > 0 && cellText[0] == '=')
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellText;
                }
            }
        }

        /// <summary>
        /// When cell is done being edited, shows value.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> Spreadsheet arguments. </param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string cellValue = this.sheet.GetCell(e.RowIndex, e.ColumnIndex).Value;

            if (cellValue.Length > 0)
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellValue;
            }
        }

        /// <summary>
        /// Converts color to uint value.
        /// </summary>
        /// <param name="color"> color. </param>
        /// <returns> uint value of color. </returns>
        private uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }

        /// <summary>
        /// Changes background color.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> arguments. </param>
        private void ChangeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Cell> cells = new List<Cell>();
            List<uint> colors = new List<uint>();
            ColorDialog colorDialog = new ColorDialog
            {
                AllowFullOpen = true,
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Update colors for each cell.
                foreach (DataGridViewTextBoxCell cell in this.dataGridView1.SelectedCells)
                {
                    cells.Add(this.sheet.GetCell(cell.RowIndex, cell.ColumnIndex));
                    colors.Add(this.sheet.GetCell(cell.RowIndex, cell.ColumnIndex).BGColor);
                    this.sheet.GetCell(cell.RowIndex, cell.ColumnIndex).BGColor = this.ColorToUInt(colorDialog.Color);
                }

                ColorChange colorChange = new ColorChange(this.ColorToUInt(colorDialog.Color), colors, cells);
                this.sheet.AddUndo(colorChange);
            }
        }

        /// <summary>
        /// undo button.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> arguments. </param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.UndoCommand();
        }

        /// <summary>
        /// redo button.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> arguments. </param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.RedoCommand();
        }

        /// <summary>
        /// Clears spreadsheet for loading a new one.
        /// </summary>
        private void Clear()
        {
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Refresh();
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
            this.Form1_Load(this, new EventArgs());
        }

        /// <summary>
        /// load xml files.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> argument. </param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML File |*.xml",
                Title = "Open an XML File",
            };
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != string.Empty)
            {
                System.IO.FileStream fileStream = (System.IO.FileStream)openFileDialog.OpenFile();

                this.Clear();
                this.sheet.LoadFile(fileStream);

                fileStream.Close();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XML File |*.xml",
                Title = "Save an XML File",
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != string.Empty)
            {
                System.IO.FileStream fileStream = (System.IO.FileStream)saveFileDialog.OpenFile();
                this.sheet.SaveFile(fileStream);
                fileStream.Close();
            }
        }
    }
}
