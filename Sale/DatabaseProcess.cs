using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;

namespace Database
{
    class DatabaseProcess
    {
        string server = "localhost";
        string database = "grocery";
        string username = "root";
        string password = "";

        public MySqlConnection MySqQLconnect()
        {
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
            "UID=" + username + ";" + "PASSWORD=" + password + ";";
            MySqlConnection conn = new MySqlConnection(constring);
            conn.Open();
            return conn;
        }
        public MySqlDataReader login(MySqlConnection conn, string table, string id)
        {
            string query = "select * from " + table + " where emp_id = '" + id + "'";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public MySqlDataReader QueryAll(MySqlConnection conn, string table)
        {
            string query = "select * from " + table ;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public MySqlDataReader GetProductLot(MySqlConnection conn, string id)
        {
            string query = "use grocery; " +
                "select * from product_lot " +
                "where product_id = '" + id + "' and product_status = 'Alive' and expired_date = " +
                                                                        "(select min(expired_date) " +
                                                                         "from product_lot " +
                                                                         "where product_id = '" + id + "' and product_status = 'Alive'); ";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public MySqlDataReader GetCustomer(MySqlConnection conn, string id)
        {
            string query = "use grocery; select * from customer where customer_id = " + id +";";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public MySqlDataReader GetMaxSaleID(MySqlConnection conn)
        {
            string query = "use grocery; select MAX(sale_id) as sale_id from sale;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public MySqlDataReader CutLot(MySqlConnection conn,string lot_no, string product_id)
        {
            string query = "use grocery; select * from product_lot where lot_no = '" + lot_no + "' and product_id = '" + product_id + "';";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public void insert(MySqlConnection conn,string sql)
        {           
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }  
    }
}
