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
    public partial class Form1 : Form
    {
        DataBase dataBase = new DataBase();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; // приводим отправителя к элементу типа CheckBox
            if (checkBox.Checked == true)
            {
                dataBase.openConnection();
                textBox1.Text = "Подключено";
            }
            else
            {
                dataBase.closeConnection();
                textBox1.Text = "Отключено";
            }
        }

        private void selectAddItem_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
