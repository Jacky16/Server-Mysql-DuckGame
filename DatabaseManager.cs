using MySql.Data.MySqlClient;


class DatabaseManager
{
    const string databaseName = "bestunitydb";
    const int port = 3306;
    const string uID = "kimjong";
    const string password = "enti2022";
    string conncectionString = $"Server=db4free.net;Port={port};database={databaseName};Uid={uID};password={password};SSL Mode=None; connect timeout=3600; default command timeout = 3600;";
    MySqlConnection connection;

    public DatabaseManager()
    {
        ShowAllPlayer();
    }
    public void StartService()
    {
        connection = new MySqlConnection(conncectionString);

        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    public void Register(string nick, string password, string email)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Insert Into Player(username,password,email) Values('{nick}','{password}','{email}');";
        command.CommandText = query;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        connection.Close();
        ShowAllPlayer();
    }

    public bool LogInUser(string email, string password, ref Client client)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Player where email='{email}' and password='{password}';";
        Console.WriteLine(query);
        command.CommandText = query;
        MySqlDataReader reader;
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                client.SetEmail(reader["email"].ToString());
                client.SetNick(reader["username"].ToString());
                
                Console.WriteLine("Log in correcto " + reader["username"].ToString());
                return reader["email"].ToString() == email &&
                    reader["password"].ToString() == password;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return false;
    }

    void ShowAllPlayer()
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Player;";
        command.CommandText = query;
        MySqlDataReader reader;
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["username"].ToString() + " " + reader["password"].ToString() + " " + reader["email"].ToString());
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();

    }
}    

