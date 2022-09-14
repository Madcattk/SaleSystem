using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;

namespace Sale
{
    public partial class Form1 : Form
    {
        Database.DatabaseProcess db = new Database.DatabaseProcess();
        public static string emp = "";
        public static string empName = "";
        public Form1()
        {
            InitializeComponent();
            this.Text = "SALE SYSTEM";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
            textBox1.MaxLength = 10;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void add_Click(object sender, EventArgs e)
        {
            
                
           
         
        }

        private void order_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = db.MySqQLconnect();
            MySqlDataReader reader = db.login(conn, "employee", textBox1.Text);
            if(reader.Read())
                {                  
                    label2.Text = "";
                    emp = textBox1.Text;
                    empName = reader["fname"] + " " + reader["lname"];
                    FrmSale f = new FrmSale();
                    f.ShowDialog();
                }
            else label2.Text = "ID is invalid";
            textBox1.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                order_Click(sender, e);
            }
        }
    }
}
