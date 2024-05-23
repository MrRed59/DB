﻿using System;
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
    public partial class trade_duty : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public trade_duty()
        {
            InitializeComponent();
        }

        private void trade_duty_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("country_name", "Название страны");
            dataGridView1.Columns.Add("title", "Пошлина");
            dataGridView1.Columns.Add("product_name", "Название товара");
            dataGridView1.Columns.Add("article_number", "Артикул");
            dataGridView1.Columns.Add("cost_price", "Себестоимость товара");

            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            try
            {
                dgw.Rows.Add(record.GetInt32(0),
                                record.GetString(1),
                                record.GetInt32(2),
                                record.GetString(3),
                                record.GetString(4),
                                record.GetValue(5),
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

            string query = $"select trade_duty.id, country.country_name, trade_duty.title, product.product_name, product.article_number, product.cost_price " +
                            $"from trade_duty LEFT JOIN country on country.id = trade_duty.country_id left join product on product.id = trade_duty.product_id; ";

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
                textBoxNameProduct.Text = row.Cells[3].Value.ToString();
            }
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
            }

            dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        //добавить элемент в таблицу
        private void buttonFactoryAdd_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var countryName = textBoxName.Text;
            int tradeDuty;

            if (int.TryParse(textBoxTradeDuty.Text, out tradeDuty))
            {
                var query = $"INSERT INTO country (country_name, continent, square) VALUES ('{countryName}', {tradeDuty});";
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

            var query = $"SELECT * FROM country WHERE CONCAT (id, country_name, continent, square) LIKE '%" + textBoxSearch.Text + "%'";

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
                var rowState = (RowState)dataGridView1.Rows[index].Cells[4].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var query = $"DELETE FROM country WHERE id={id}";

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

            var countryName = textBoxName.Text;
            int tradeDuty;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxTradeDuty.Text, out tradeDuty))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(countryName, tradeDuty);

                    /*string query = $"UPDATE country SET continent='{continent}', square='{square}' WHERE country_name='{countryName}';";
                    var command = new SqlCommand(query, dataBase.GetConnection());
                    command.ExecuteNonQuery();*/

                    MessageBox.Show("Запись изменена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Запись не была изменена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}