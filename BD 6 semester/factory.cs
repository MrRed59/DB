using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BD_6_semester
{
    enum RowState
    {
        Existed,
        New,
        ModifiedNew,
        Deleted
    }
    public partial class factory : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public factory()
        {
            InitializeComponent();
        }              

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("name_of_factory", "Название");
            dataGridView1.Columns.Add("address", "Адрес");
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("factory_id", "factory_id");
            dataGridView1.Columns.Add("target profit", "Целевая прибыль");

            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            try
            {
                dgw.Rows.Add(record.GetInt32(0),
                                            record.GetString(1),
                                            record.GetString(2),
                                            record.GetInt32(3),
                                            record.GetInt32(4),
                                            record.GetInt32(5),
                                            RowState.ModifiedNew);
                dgw.Columns[3].Visible = false;
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

            string query = $"SELECT * FROM factory LEFT JOIN target_point on factory.id = target_point.factory_id";

            SqlCommand command = new SqlCommand(query, dataBase.GetConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                ReadSingleRow(dgw, reader);

            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
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
                textBoxFactoryAddress.Text = row.Cells[2].Value.ToString();
                textBoxTargetProfit.Text = row.Cells[5].Value.ToString();
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
            var factoryName = textBoxName.Text;
            var factoryAddress = textBoxFactoryAddress.Text;
            int targetProfit;

            if (int.TryParse(textBoxTargetProfit.Text, out targetProfit))
            {
                var query = $"EXEC AddFactory '{factoryName}', '{factoryAddress}', '{targetProfit}';";
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

            var query = $"SELECT * FROM factory left join target_point on factory.id = target_point.factory_id WHERE CONCAT (factory.id, factory.name_of_factory, factory.address, target_point.title) LIKE '%" + textBoxSearch.Text + "%'";

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
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var query = $"DELETE FROM factory WHERE id={id}";

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

        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var factoryName = textBoxName.Text;
            var factoryAddress = textBoxFactoryAddress.Text;
            int targetProfit;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxTargetProfit.Text, out targetProfit))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(factoryName, factoryAddress, targetProfit);

                    string query = $"EXEC EditFactory '{factoryName}', '{factoryAddress}', {targetProfit};";
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
    }
}
