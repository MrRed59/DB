using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_6_semester
{
    public partial class main : Form
    {
        DataBase dataBase = new DataBase();

        public main()
        {
            InitializeComponent();
            label1.Text = dataBase.getState();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; // приводим отправителя к элементу типа CheckBox

            if (checkBox.Checked == true)
                dataBase.openConnection();
            else
                dataBase.closeConnection();

            label1.Text = dataBase.getState();
        }

        private void selectAddItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = selectAddItem.SelectedItem.ToString();
            MessageBox.Show(selectedState);
        }

    }
}
