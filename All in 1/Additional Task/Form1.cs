using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\Lab4\ScheduleDB.mdf;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            string query = "SELECT * FROM [Table]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        dataTable.Load(reader);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DisplayDataInGrid("SELECT Day, Subject FROM [Table]");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DisplayDataInGrid("SELECT * FROM [Table] WHERE Day = N'Понеділок'");
        }

        private void DisplayDataInGrid(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            dataGridView1.DataSource = dataTable;
                            dataGridView1.Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка: " + ex.Message);
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = "SELECT * FROM [Table]";
            string whereClause = "";

            if (checkBox1.Checked)
            {
                whereClause += " Time >= '12:00'"; // Умова для checkBox1 після 12:00
            }

            if (checkBox2.Checked)
            {
                if (whereClause.Length > 0)
                {
                    whereClause += " OR ";
                }
                whereClause += " Day IN (N'Четвер', N'П''ятниця')"; // Умова для checkBox2 після середи
            }

            if (whereClause.Length > 0)
            {
                query += $" WHERE {whereClause}";
            }

            DisplayDataInGrid(query);
        }

        private DataTable ReadCSV(string filePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(',');
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            return dataTable;
        }

        private void ImportCSV()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select CSV file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;
                        DataTable dataTable = ReadCSV(filePath);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand clearCmd = new SqlCommand("DELETE FROM [Table]", connection))
                            {
                                clearCmd.ExecuteNonQuery();
                            }

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                            {
                                bulkCopy.DestinationTableName = "[Table]";
                                bulkCopy.ColumnMappings.Add("Id", "Id");
                                bulkCopy.ColumnMappings.Add("Day", "Day");
                                bulkCopy.ColumnMappings.Add("Subject", "Subject");
                                bulkCopy.ColumnMappings.Add("Time", "Time");
                                bulkCopy.ColumnMappings.Add("Classroom", "Classroom");


                                bulkCopy.WriteToServer(dataTable);
                            }
                        }

                        Form1_Load(this, EventArgs.Empty);

                        MessageBox.Show("CSV file imported successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error importing CSV file: " + ex.Message);
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ImportCSV();
        }
    }
}