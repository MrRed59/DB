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
    public partial class Export : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public Export()
        {
            InitializeComponent();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("name_of_factory", "Название завода");
            dataGridView1.Columns.Add("product_name", "Название товара");
            dataGridView1.Columns.Add("sale_price", "Цена");
            dataGridView1.Columns.Add("country_name", "Название страны");
            dataGridView1.Columns.Add("date_of_transaction", "Дата сделки");
            dataGridView1.Columns.Add("delivery_date", "Дата поставки");

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
                                record.GetString(4),
                                record.GetValue(5).ToString().Split(' ')[0],
                                record.GetValue(6).ToString().Split(' ')[0],
                                RowState.ModifiedNew);
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

            string query = $"SELECT export.id, name_of_factory, product_name, sale_price, country_name, date_of_transaction, delivery_date FROM export " +
                $"left join factory on factory.id = export.factory_id " +
                $"left join product on product.id = export.product_id " +
                $"left join price on price.id = export.price_id " +
                $"left join country on country.id = export.country_id; ";

            SqlCommand command = new SqlCommand(query, dataBase.GetConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                ReadSingleRow(dgw, reader);

            reader.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxName.Text = row.Cells[1].Value.ToString();
                textBoxProduct.Text = row.Cells[2].Value.ToString();
                textBoxKeep.Text = row.Cells[3].Value.ToString();
                textBox1.Text = row.Cells[4].Value.ToString();
                dateTimePicker2.Text = row.Cells[5].Value.ToString();
                dateTimePicker3.Text = row.Cells[6].Value.ToString();

            }
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
            }

            dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        //добавить элемент в таблицу
        private void buttonFactoryAdd_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var factoryName = textBoxName.Text;
            var productName = textBoxProduct.Text;
            int kepping;
            var countryName = textBox1.Text;
            string dateTransaction = dateTimePicker2.Value.ToString();
            string dateDelivery = dateTimePicker3.Value.ToString();

            try
            {
                int.TryParse(textBoxKeep.Text, out kepping);
                var query = $"EXEC AddExport '{factoryName}', '{productName}', {kepping}, '{countryName}', '{dateTransaction}', '{dateDelivery}'";
                var command = new SqlCommand(query, dataBase.GetConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись добавлена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Запись не была добавлена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RefreshDataGrid(dataGridView1);
        }

        //поиск элемента в БД
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            var query = $"SELECT * FROM product WHERE CONCAT (id, product_name, article_number, date_of_manufacture, expiration_date, cost_price, factory_id, margin) " +
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

        //удалить из БД элемент
        private void UpdateDB()
        {
            dataBase.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var query = $"DELETE FROM export WHERE id={id}";

                    var command = new SqlCommand(query, dataBase.GetConnection());
                    command.ExecuteNonQuery();
                }
            }
            dataBase.CloseConnection();
        }

        //кнопка удалить
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        //кнопка сохранить
        private void buttonSave_Click(object sender, EventArgs e)
        {
            UpdateDB();
            RefreshDataGrid(dataGridView1);
        }

        //кнопка изменить
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();
            RefreshDataGrid(dataGridView1);
        }


        /// <summary>
        /// /////////////////////
        /// </summary>
        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

             var factoryName = textBoxName.Text;
            var productName = textBoxProduct.Text;
            int kepping;
            var countryName = textBox1.Text;
            string dateTransaction = dateTimePicker2.Value.ToString();
            string dateDelivery = dateTimePicker3.Value.ToString();

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxKeep.Text, out kepping))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(factoryName, productName, kepping, countryName, dateTransaction, dateDelivery);

                    string query = $"EXEC UpdateExport '{factoryName}', '{productName}','{kepping}', '{countryName}', '{dateTransaction}', '{dateDelivery}'";
                    var command = new SqlCommand(query, dataBase.GetConnection());
                    command.ExecuteNonQuery();

                    MessageBox.Show("Запись изменена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Запись не была изменена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void Export_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }
    }
}
