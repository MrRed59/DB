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
    enum RowState
    {
        Existed,
        New,
        ModifiedNew,
        Deleted
    }

    public partial class main : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public main()
        {
            InitializeComponent();
            label3.Text = dataBase.getState();
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
            dgw.Rows.Add(   record.GetInt32(0), 
                            record.GetString(1), 
                            record.GetString(2),
                            record.GetInt32(3),
                            record.GetInt32(4),
                            record.GetDecimal(5),
                            RowState.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            //string query = $"SELECT * from factory";
            string query = $"SELECT * FROM factory LEFT JOIN target_point on factory.id = target_point.factory_id";
            

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                ReadSingleRow(dgw, reader);

            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet.target_point". При необходимости она может быть перемещена или удалена.
            this.target_pointTableAdapter.Fill(this.exportDataSet.target_point);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet.target_point". При необходимости она может быть перемещена или удалена.
            this.target_pointTableAdapter.Fill(this.exportDataSet.target_point);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet1.export". При необходимости она может быть перемещена или удалена.
            this.exportTableAdapter.Fill(this.exportDataSet1.export);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet.target_point". При необходимости она может быть перемещена или удалена.
            this.target_pointTableAdapter.Fill(this.exportDataSet.target_point);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet.factory". При необходимости она может быть перемещена или удалена.
            this.factoryTableAdapter.Fill(this.exportDataSet.factory);
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        /*private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; // приводим отправителя к элементу типа CheckBox

            if (checkBox.Checked == true)
                dataBase.openConnection();
            else
                dataBase.closeConnection();

            label1.Text = dataBase.getState();
        }*/

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxFactoryName.Text = row.Cells[0].Value.ToString();
                textBoxFactoryAddress.Text = row.Cells[0].Value.ToString();
                textBoxTargetProfit.Text = row.Cells[0].Value.ToString();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void buttonFactoryAdd_Click(object sender, EventArgs e)
        {
            var factoryName = textBoxFactoryName.Text;
            var factoryAddress = textBoxFactoryAddress.Text;
            decimal targetProfit;

            if (decimal.TryParse(textBoxTargetProfit.Text, out targetProfit))
            {
                var query = $"INSERT INTO factory (name_of_factory, address) VALUES ('{factoryName}', '{factoryAddress}')";
                var command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteNonQuery();

                query = $"DECLARE @id_factory int;" +
                        $"SET @id_factory = (SELECT id FROM factory WHERE name_of_factory='{factoryName}');" +
                        $"INSERT INTO target_point (factory_id, title) VALUES (@id_factory, {targetProfit})";
                command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись добавлена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Запись не была добавлена. \"Целевая прибыль\" должна иметь числовой формат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //dataBase.closeConnection();
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.factoryTableAdapter.FillBy(this.exportDataSet.factory);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            var query = $"SELECT * FROM factory WHERE CONCAT (id, name_of_factory, address) LIKE '%" + textBoxSearch.Text + "%'";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader read = command.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }
    }
}
