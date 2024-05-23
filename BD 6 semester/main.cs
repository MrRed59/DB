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
    public partial class main : Form
    {
         public main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e){}

        private void button1_Click(object sender, EventArgs e)
        {
            factory form1 = new factory();
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            countries form1 = new countries();
            form1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trade_duty form1 = new trade_duty();
            form1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            product form1 = new product();
            form1.Show();
        }
    }
}
