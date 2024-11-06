using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
                whereClause += " Time >= '12:00'"; // Умова для checkBox1: після 12:00
            }

            if (checkBox2.Checked)
            {
                if (whereClause.Length > 0)
                {
                    whereClause += " OR ";
                }
                whereClause += " Day IN (N'Четвер', N'П''ятниця')"; // Умова для checkBox2: після середи
            }

            if (whereClause.Length > 0)
            {
                query += $" WHERE {whereClause}";
            }

            DisplayDataInGrid(query);
        }
    }
}