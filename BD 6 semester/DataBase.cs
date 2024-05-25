﻿using System.Data.SqlClient;

namespace BD_6_semester
{
    class DataBase
    {
        SqlConnection sqlconnect = new SqlConnection(@"server=DESKTOP-V6Q9ITN\SQLEXPRESS;Initial Catalog=Export;Integrated Security=True");
        
        public void OpenConnection()
        {
            if (sqlconnect.State == System.Data.ConnectionState.Closed)
                sqlconnect.Open();
        }

        public void CloseConnection()
        {
            if (sqlconnect.State == System.Data.ConnectionState.Open)
                sqlconnect.Close();
        }

        public SqlConnection GetConnection()
        {
            return sqlconnect;
        }

        public string GetState()
        {
            return sqlconnect.State.ToString();
        }
    }
}
