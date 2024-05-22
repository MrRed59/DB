using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

namespace BD_6_semester
{
    class DataBase
    {
        //SqlConnection sqlconnect = new SqlConnection("server=localhost;port=5589;username=root;password=;database=Export");
        SqlConnection sqlconnect = new SqlConnection(@"server=DESKTOP-V6Q9ITN\SQLEXPRESS;Initial Catalog=Export;Integrated Security=True");
        
        public void openConnection()
        {
            if (sqlconnect.State == System.Data.ConnectionState.Closed)
                sqlconnect.Open();
        }

        public void closeConnection()
        {
            if (sqlconnect.State == System.Data.ConnectionState.Open)
                sqlconnect.Close();
        }

        public SqlConnection getConnection()
        {
            return sqlconnect;
        }

        public string getState()
        {
            return sqlconnect.State.ToString();
        }
    }
}
