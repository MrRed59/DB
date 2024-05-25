using System;
using System.Windows.Forms;

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

        private void button5_Click(object sender, EventArgs e)
        {
            keeping form1 = new keeping();
            form1.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Export form1 = new Export();
            form1.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            calculation form1 = new calculation();
            form1.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            price form1 = new price();
            form1.Show();
        }
    }
}
