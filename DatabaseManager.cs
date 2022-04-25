using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DatabaseManager
{
    static void Main(string[] args)
    {
        const string databaseName = "bestunitydb";
        const int port = 3306;
        const string uID = "kimjong";
        const string password = "enti2022";
        string conncectionString = $"Server=db4free.net;Port={port};database={databaseName};Uid={uID};password={password};SSL Mode=None; connect timeout=3600; default command timeout = 3600;";

        MySqlConnection connection = new MySqlConnection(conncectionString);

        try
        {
            connection.Open();
        } catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        //SelectExample(connection);
        InsertExample(connection);
        connection.Close();

        void SelectExample(MySqlConnection connection)
        {
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "Select * from Avatar";

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["Nick"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        void InsertExample(MySqlConnection connection)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Insert Into Player Values('Testing',12345,'testing@testing.com',2000-11-22,0);";
            try
            {
                command.ExecuteNonQuery();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}

