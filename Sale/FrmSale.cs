using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Sale
{
    public partial class FrmSale : Form
    {
        Database.DatabaseProcess db = new Database.DatabaseProcess();
        public string addp;
        public string price;
        public string lot;
        public string id;
        public string name;
        public string amountp = "Out Of Stock";
        public FrmSale()
        {
            InitializeComponent();
            this.Text = "EMPLOYEE: " + Form1.empName; 
            
        }

        private void FrmSale_Load(object sender, EventArgs e)
        {
            loadData(sender, e);
        }
        private void loadData(object sender, EventArgs e)
        {
            MySqlConnection conn = db.MySqQLconnect();         
            DataTable dataTable = new DataTable();
            dataTable.Load(db.QueryAll(conn,"product"));
            dataGridView1.DataSource = dataTable;
            dataGridView1.Columns[0].Visible = false;

            DataTable dataTable2 = new DataTable();
            dataTable2.Load(db.QueryAll(conn, "sale"));
            dataGridView2.DataSource = dataTable2;

            DataTable dataTable3 = new DataTable();
            dataTable3.Load(db.QueryAll(conn, "sale_detail"));
            dataGridView3.DataSource = dataTable3;


            DataTable dataTable4 = new DataTable();
            dataTable4.Load(db.QueryAll(conn, "product_lot"));
            dataGridView4.DataSource = dataTable4;
            dataGridView4.Columns[2].Visible = false;
            dataGridView4.Columns[3].Visible = false;

            DataTable dataTable6 = new DataTable();
            dataTable6.Load(db.QueryAll(conn, "employee"));
            dataGridView6.DataSource = dataTable6;
            dataGridView6.Columns[0].Visible = false;
            dataGridView6.Columns[3].Visible = false;
            dataGridView6.Columns[4].Visible = false;
            dataGridView6.Columns[5].Visible = false;
            dataGridView6.Columns[7].Visible = false;

            dataGridView5.ColumnCount = 5;
            dataGridView5.Columns[0].Name = "PRODUCT_LOT";
            dataGridView5.Columns[1].Name = "PRODUCT_ID";
            dataGridView5.Columns[2].Name = "PRODUCT_NAME";
            dataGridView5.Columns[3].Name = "AMOUNT";
            dataGridView5.Columns[4].Name = "PRICE";
            dataGridView5.Columns[0].Visible = false;
            dataGridView5.Columns[1].Visible = false;
            textBox1.ReadOnly = true;
            textBox1.BackColor = System.Drawing.SystemColors.Window;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    label6.Text = "";
                    dataGridView1.CurrentRow.Selected = true;
                    label1.Text = dataGridView1.Rows[e.RowIndex].Cells["PRODUCT_NAME"].FormattedValue.ToString();
                    price = dataGridView1.Rows[e.RowIndex].Cells["PRICE"].FormattedValue.ToString();
                    MySqlConnection conn = db.MySqQLconnect();
                    MySqlDataReader reader = db.GetProductLot(conn, dataGridView1.Rows[e.RowIndex].Cells["PRODUCT_ID"].FormattedValue.ToString());
                    int tempAmount = 0;
                    lot = "";
                    amount.Items.Clear();
                    if (reader.Read())
                    {
                        lot = reader["lot_no"].ToString();
                        tempAmount = Convert.ToInt32(reader["product_amount"]);
                        amount.Items.Remove("Out Of Stock");
                        for (int i = 1; i <= tempAmount; i++)
                        {
                            amount.Items.Add(i);
                        }
                        addp = "";
                    }
                    else
                    {
                        amount.Items.Add("Out Of Stock");
                    }
                    amount.SelectedIndex = 0;
                    id = dataGridView1.Rows[e.RowIndex].Cells["PRODUCT_ID"].FormattedValue.ToString();
                    name = dataGridView1.Rows[e.RowIndex].Cells["PRODUCT_NAME"].FormattedValue.ToString();
                }
            }
            catch (Exception)
            {

            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (amount.SelectedItem != null)
            {
                if (amount.SelectedItem.ToString() == "Out Of Stock") { }
                else
                {
                    addp = lot + ":" + id + ":" + name + ":" + amount.SelectedItem.ToString() + ":" + price;
                    textBox1.Text = "";
                    bool chk = true;
                    string[] adds = addp.Split(':');                   
                    foreach (DataGridViewRow row in dataGridView5.Rows)
                    {
                        if (adds[1] == row.Cells["PRODUCT_ID"].FormattedValue.ToString())
                        {
                            dataGridView5.Rows.Remove(row);
                            row.CreateCells(dataGridView5);
                            row.Cells[0].Value = adds[0];
                            row.Cells[1].Value = adds[1];
                            row.Cells[2].Value = adds[2];
                            row.Cells[3].Value = amount.SelectedItem.ToString();
                            row.Cells[4].Value = (Convert.ToInt32(adds[4]) * Convert.ToInt32(adds[3])).ToString();
                            dataGridView5.Rows.Add(row);
                            chk = false;
                        }
                    }
                    if (chk)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dataGridView5);
                        row.Cells[0].Value = adds[0];
                        row.Cells[1].Value = adds[1];
                        row.Cells[2].Value = adds[2];
                        row.Cells[3].Value = adds[3];
                        row.Cells[4].Value = (Convert.ToInt32(adds[4]) * Convert.ToInt32(adds[3])).ToString();
                        dataGridView5.Rows.Add(row);
                    }
                    calculateTotal();
                }
            }
        }

        private void order_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = db.MySqQLconnect();
            MySqlDataReader reader = db.GetCustomer(conn, cusid.Text);         
            if (dataGridView5.RowCount - 1 != 0 && reader.Read())
            {
                label3.Text = reader["fname"].ToString() + " " + reader["lname"].ToString();
                string sid = "s00000";
                conn.Close();
                conn.Open();
                reader = db.GetMaxSaleID(conn);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("sale_id"))) sid = reader["sale_id"].ToString();
                }
                
                string subsid = sid.Substring(1, 5);
                int sale = Convert.ToInt32(subsid) + 1;
                sid = "s" + sale.ToString("00000");

                reader.Dispose();
                reader.Close();
                conn.Close();
                conn.Open();
                string sql = $"insert into sale (sale_id, sale_date,customer_id,emp_id) values('{sid}', '{DateTime.Now.ToString("yyyy-MM-dd")}' ,'{cusid.Text}','{Form1.emp}')";
                db.insert(conn, sql);

                sql = $"update employee set sales = (select sales from employee where emp_id = '{Form1.emp}') + 1 where emp_id = '{Form1.emp}'";
                db.insert(conn, sql);

                foreach (DataGridViewRow row in dataGridView5.Rows)
                {
                    if (row.Cells["PRODUCT_LOT"].FormattedValue.ToString() != "")
                    {
                        sql = $"insert into sale_detail (lot_no,product_id,sale_id,sale_amount) values('{row.Cells["PRODUCT_LOT"].FormattedValue.ToString()}','{row.Cells["PRODUCT_ID"].FormattedValue.ToString()}','{sid}',{row.Cells["AMOUNT"].FormattedValue.ToString()})";
                        db.insert(conn, sql);
                    }
                }

                
                
                foreach (DataGridViewRow row in dataGridView5.Rows)
                {       
                    sql = $"update product_lot set product_amount =  product_amount-{Convert.ToInt32(row.Cells["AMOUNT"].Value)} where lot_no = '{row.Cells["PRODUCT_LOT"].FormattedValue.ToString()}' and product_id = '{row.Cells["PRODUCT_ID"].FormattedValue.ToString()}'";
                    db.insert(conn, sql);
                }

                label3.Text = "";
                label6.Text = "No: " + sid + " is added";
                textBox1.Text = "";
                cusid.Text = "";
                dataGridView5.Rows.Clear();
                dataGridView5.DataSource = null;
                label1.Text = "";
                amount.Text = "";
                amount.Items.Clear();
                //this.Controls.Clear();
                //this.InitializeComponent();
                loadData(sender, e);
            }
            else label3.Text = "Customer ID is invalid";
        }

        private void cusphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                MySqlConnection conn = db.MySqQLconnect();
                MySqlDataReader reader = db.GetCustomer(conn, cusid.Text);
                if (reader.Read())
                {
                    label3.Text = reader["fname"].ToString() + " " + reader["lname"].ToString();
                }
                else { label3.Text = ""; cusid.Text = ""; }
            }
          
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            foreach (DataGridViewRow row in dataGridView5.SelectedRows)
            {
                if (dataGridView5.CurrentRow.Index != dataGridView5.RowCount - 1 )
                {
                    string message = "Would you like to remove this product?";
                    string title = "Delete Confirmation";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);
                    if (result == DialogResult.Yes)
                    {
                        dataGridView5.Rows.RemoveAt(row.Index);
                        calculateTotal();

                    }
                }
                
            }
        }
        private void calculateTotal()
        {
            double total = 0;
            textBox1.Text = "";
            foreach (DataGridViewRow roww in dataGridView5.Rows)
            {
                total += Convert.ToInt32(roww.Cells["PRICE"].Value);
            }
            textBox1.Text += "Total: " + total.ToString();
        }
        private void cusid_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

