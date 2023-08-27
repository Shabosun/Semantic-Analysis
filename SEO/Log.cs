using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO
{
    internal static class Log
    {

        static MySqlConnection mySqlConnection;

        static Log()
        {
            try
            {
                string connectionString = "server = localhost; uid = root; pwd = ; database = seodb; port = 3306; charset=utf8; ";
                mySqlConnection = new MySqlConnection(connectionString);
                mySqlConnection.Open();

                AppDomain.CurrentDomain.ProcessExit += StaticClass_Dtor;
            }
            catch (Exception)
            {

                throw;
            }
        }


        static void StaticClass_Dtor(object sender, EventArgs e)
        {
            if (mySqlConnection.State == ConnectionState.Open)
            {
                mySqlConnection.Close();
                mySqlConnection = null;
            }
        }

        public static void SendActionLog(string name)
        {
            string query =
                $"INSERT INTO actions " +
                $"(Name)  " +
                $"VALUES ('{name}')";

            new MySqlCommand(query, mySqlConnection) { CommandType = CommandType.Text }.ExecuteNonQuery();
        }
    }
}
