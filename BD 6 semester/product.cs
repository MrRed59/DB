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
    public partial class product : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public product()
        {
            InitializeComponent();
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("product_name", "Название");
            dataGridView1.Columns.Add("article_number", "Артикул");
            dataGridView1.Columns.Add("date_of_manufacture", "Дата изготовления");
            dataGridView1.Columns.Add("expiration_date", "Срок годности");
            dataGridView1.Columns.Add("cost_price", "Себестоимость");
            dataGridView1.Columns.Add("factory_id", "id завода");

            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            try
            {
                dgw.Rows.Add(record.GetInt32(0),
                                record.GetString(1),
                                record.GetString(2),
                                record.GetValue(3).ToString().Split(' ')[0],
                                record.GetInt32(4),
                                record.GetValue(5),
                                record.GetInt32(6),
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

            string query = $"SELECT * FROM product";

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
                textBoxTradeDuty.Text = row.Cells[2].Value.ToString();
                dateTimePicker1.Text = row.Cells[3].Value.ToString();
                textBoxExp.Text = row.Cells[4].Value.ToString();
                textBoxCostPrice.Text = row.Cells[5].Value.ToString();
                textBoxFactoryName.Text = row.Cells[6].Value.ToString();
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

            var productName = textBoxName.Text;
            var articleNum = textBoxTradeDuty.Text;
            string dateManufacture = dateTimePicker1.Value.ToString();
            int expDate;
            float costPrice;
            int factoryId;

            if (int.TryParse(textBoxExp.Text, out expDate) &&
                float.TryParse(textBoxCostPrice.Text, out costPrice) &&
                int.TryParse(textBoxFactoryName.Text, out factoryId))
            {
                var query = $"INSERT INTO product (product_name, article_number, date_of_manufacture, expiration_date, cost_price, factory_id) " +
                            $"VALUES ('{productName}', '{articleNum}', '{dateManufacture.Split(' ')[0]}', '{expDate}', '{costPrice}', {factoryId});";
                var command = new SqlCommand(query, dataBase.GetConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись добавлена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Запись не была добавлена. \"Целевая прибыль\" должна иметь числовой формат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RefreshDataGrid(dataGridView1);
        }

        //поиск элемента в БД
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            var query = $"SELECT * FROM product WHERE CONCAT (id, product_name, article_number, date_of_manufacture, expiration_date, cost_price, factory_id) " +
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
                    var query = $"DELETE FROM product WHERE id={id}";

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

            var productName = textBoxName.Text;
            var articleNum = textBoxTradeDuty.Text;
            string dateManufacture = dateTimePicker1.Value.ToString();
            int expDate;
            float costPrice;
            int factoryId;            

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxExp.Text, out expDate) &&
                float.TryParse(textBoxCostPrice.Text, out costPrice) &&
                int.TryParse(textBoxFactoryName.Text, out factoryId))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(productName, articleNum, dateManufacture, expDate, costPrice, factoryId);

                    string query = $"UPDATE product SET " +
                                    $"article_number='{articleNum}', date_of_manufacture='{dateManufacture.Split(' ')[0]}', expiration_date={expDate}," +
                                    $"cost_price={costPrice}, factory_id={factoryId}" +
                                    $"WHERE product_name='{productName}';";
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


        /// <summary>
        /// ///
        /// </summary>
        private void product_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }
    }
}
