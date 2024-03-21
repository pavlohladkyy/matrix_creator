using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace lab2
{
#region Form
    public partial class Form1 : Form
    {
        CharacteristicMatrix matrix;
        public Form1()
        {
            InitializeComponent();

            InitializeTooltips();





        }
        private void InitializeTooltips()
        {
            
            toolTip2.SetToolTip(buttonSort, "сортування за сумою модулів відємних непарних елеменів переставляючи стовпці матриці у їх відповідності з ростом характеристик");
            toolTip3.SetToolTip(CountSum, "Обчислення суми елементів у тих стовпцях, які містять хоча б один від’ємний\r\nелемент.");
        }


        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxRow.Text, out var row))
            {
                MessageBox.Show("invalid row");
                return;
            }
            if (!int.TryParse(textBoxCol.Text, out var col))
            {
                MessageBox.Show("invalid col");
                return;
            }
            
            matrix = new CharacteristicMatrix(row, col);

            if (!int.TryParse(textBoxMin.Text, out var min))
            {
                MessageBox.Show("invalid min");
                return;
            }
            if (!int.TryParse(textBoxMax.Text, out var max))
            {
                MessageBox.Show("invalid max");
                return;
            }

            if (min > max)
            {
                MessageBox.Show("мінімальне та максимальне значення введено не в правильному порядку ");
                return;
            }

            matrix.FillElementRandom(min, max);
            Print(matrix);
        }


        private void buttonSort_Click(object sender, EventArgs e)
        {
            if (matrix == null)
            {
                MessageBox.Show("Please create a matrix first.");
                return;
            }

            // Calculate characteristic for each column
            List<int> characteristics = new List<int>();
            for (int j = 0; j < matrix.ColCount; j++)
            {
                int sum = 0;
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    int value = matrix[i, j];
                    if (value < 0 && value % 2 != 0)
                    {
                        sum += Math.Abs(value);
                    }
                }
                characteristics.Add(sum);
            }

            // Sort columns based on characteristics
            var sortedColumns = characteristics.Select((x, i) => new KeyValuePair<int, int>(i, x))
                                                .OrderBy(x => x.Value)
                                                .Select(x => x.Key)
                                                .ToArray();

            // Rearrange matrix columns
            CharacteristicMatrix sortedMatrix = new CharacteristicMatrix(matrix.RowCount, matrix.ColCount);
            for (int j = 0; j < sortedColumns.Length; j++)
            {
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    sortedMatrix[i, j] = matrix[i, sortedColumns[j]];
                }
            }
            if (!int.TryParse(textBoxRow.Text, out var row))
            {
                MessageBox.Show("invalid row");
                return;
            }
            if (!int.TryParse(textBoxCol.Text, out var col))
            {
                MessageBox.Show("invalid col");
                return;
            }

            matrix = new CharacteristicMatrix(row, col);

            if (!int.TryParse(textBoxMin.Text, out var min))
            {
                MessageBox.Show("invalid min");
                return;
            }
            if (!int.TryParse(textBoxMax.Text, out var max))
            {
                MessageBox.Show("invalid max");
                return;
            }

            if (min > max)
            {
                MessageBox.Show("мінімальне та максимальне значення введено не в правильному порядку ");
                return;
            }
            // Print sorted matrix
            Print(sortedMatrix);

            
        }

        private void Print(CharacteristicMatrix matrix)
        {
            DataTable dt = new DataTable();
            int columns = matrix.ColCount;
            int rows = matrix.RowCount;

            // Add columns to the DataTable
            for (int i = 0; i < columns; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(double));
            }

            // Add rows to the DataTable
            for (int row = 0; row < rows; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < columns; col++)
                {
                    dr[col] = matrix[row, col];
                }
                dt.Rows.Add(dr);
            }

            dataGrid.DataSource = dt; // Set the DataSource property directly
            dataGrid.ColumnHeadersVisible = false; // Hide column headers
            dataGrid.RowHeadersVisible = false; // Hide row headers
            dataGrid.AllowUserToAddRows = false; // Prevent users from adding rows
        }

        private void CountSum_Click(object sender, EventArgs e)
        {
            if (matrix == null)
            {
                MessageBox.Show("Please create a matrix first.");
                return;
            }

            int sumOfNegativeColumns = 0;
            for (int j = 0; j < matrix.ColCount; j++)
            {
                bool hasNegative = false;
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    if (matrix[i, j] < 0)
                    {
                        hasNegative = true;
                        break;
                    }
                }
                if (hasNegative)
                {
                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        sumOfNegativeColumns += matrix[i, j];
                    }
                }
            }
            MessageBox.Show($"Сума елементів у стовпцях, які містять хоча б один негативний елемент: {sumOfNegativeColumns}");
        }

        
    }
    #endregion

    #region class
    class CharacteristicMatrix
    {
        #region поля
        int[,] matrix;
        #endregion

        #region властивості та індексатори
        public int RowCount { get; }
        public int ColCount { get; }

        public int this[int i, int j]
        {
            get
            {
                if (i < RowCount && i >= 0 && j < ColCount && j >= 0)
                    return matrix[i, j];
                else throw new IndexOutOfRangeException("Індекс виходить за межі масиву!");
            }
            set
            {
                if (i < RowCount && i >= 0 && j < ColCount && j >= 0)
                    matrix[i, j] = value;
                else throw new IndexOutOfRangeException("Індекс виходить за межі масиву!");
            }
        }
        #endregion

        #region Конструктор
        public CharacteristicMatrix(int row, int col)
        {
            if(row<0 || col < 0)
            {
                MessageBox.Show("введено відємний елемент");
                return;
            }
            if (row > 100 || col > 100)
            {
                MessageBox.Show("введено не можливу кількість");
                return;
            }

            matrix = new int[row, col];
            RowCount = row;
            ColCount = col;
        }
        #endregion

        #region Методи
        public void FillElementRandom(int min, int max)
        {
            Random rand = new Random();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    matrix[i, j] = rand.Next(min, max + 1);
                }
            }
        }
        #endregion
    }
    #endregion
}