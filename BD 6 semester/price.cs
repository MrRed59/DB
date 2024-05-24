using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BD_6_semester
{
    public partial class price : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;
        public price()
        {
            InitializeComponent();
        }

        private void price_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("name_of_factory", "Название завода");
            dataGridView1.Columns.Add("product_name", "Название товара");
            dataGridView1.Columns.Add("cost_price", "Цена");
            dataGridView1.Columns.Add("margin", "Наценка");
            dataGridView1.Columns.Add("country_name", "Страна");
            dataGridView1.Columns.Add("TD", "Пошлина");
            dataGridView1.Columns.Add("TP", "Целевая прибыль");

            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            try
            {
                dgw.Rows.Add(record.GetInt32(0),
                                record.GetString(1),
                                record.GetString(2),
                                record.GetValue(3),
                                record.GetValue(4),
                                record.GetString(5),
                                record.GetValue(6),
                                record.GetValue(7),
                                RowState.ModifiedNew);
                dgw.Columns[0].Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = $"SELECT * FROM min_trade_duty_v";

            SqlCommand command = new SqlCommand(query, dataBase.GetConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                ReadSingleRow(dgw, reader);

            reader.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                float TP;
                float.TryParse(row.Cells[7].Value.ToString(), out TP);
                float cost_price;
                float.TryParse(row.Cells[3].Value.ToString(), out cost_price);
                float margin;
                float.TryParse(row.Cells[4].Value.ToString(), out margin);

                int sum = (int)(TP / margin);
                if (sum < 1)
                    sum = 1;
                label2.Text = sum.ToString();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        //поиск элемента в БД
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            var query = $"SELECT * FROM min_trade_duty_v WHERE CONCAT (name_of_factory, product_name, cost_price, margin, country_name, TD, TP) " +
                        $"LIKE '%" + textBoxSearch.Text + "%'";

            SqlCommand command = new SqlCommand(query, dataBase.GetConnection());

            dataBase.OpenConnection();

            SqlDataReader read = command.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        //поле поиска
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }
    }
}
