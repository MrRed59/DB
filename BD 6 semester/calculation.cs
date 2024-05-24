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
    public partial class calculation : Form
    {
        public calculation()
        {
            InitializeComponent();
        }





        private void prices_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet4.price". При необходимости она может быть перемещена или удалена.
            this.priceTableAdapter1.Fill(this.exportDataSet4.price);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "exportDataSet3.price". При необходимости она может быть перемещена или удалена.
            this.priceTableAdapter.Fill(this.exportDataSet3.price);
            /*CreateColumns();
            RefreshDataGrid(dataGridView1);*/
        }
    }
}
