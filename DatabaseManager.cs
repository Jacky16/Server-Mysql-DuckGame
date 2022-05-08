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
        GetAllClases();
    }

    #region Classes Functions
    public void AddClassToUser(string idUser, string avatarID)
    {     
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Insert INTO Avatar(IDPlayerOwner,IDClass) Values('{idUser}','{avatarID}');";
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
    }

    #endregion
    
    #region User Functions
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
    
    #endregion
    
    #region Classes getters
    public List<String> GetAllClases()
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Class;";
        command.CommandText = query;
        MySqlDataReader reader;
        List<String> classes = new List<String>();
        int i = 0;
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (i < 4)
                {
                    string data = reader.GetString("name") + "/" + reader.GetString("speed") + "/"
                        + reader.GetString("fire_rate") + "/" + reader.GetString("life");
                    classes.Add(data);
                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return classes;
    }

    public string GetClassById(string idClass)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Class where id='{idClass}';";
        command.CommandText = query;
        MySqlDataReader reader;
        string className = "";
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                className = reader.GetString("name") + "/" + reader.GetString("speed") + "/" + reader.GetString("fire_rate") + "/" + reader.GetString("life");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return className;
    }

    public string GetClassIdByUserId(int idUser)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Avatar where IDPlayerOwner='{idUser}';";
        command.CommandText = query;
        MySqlDataReader reader;
        string id = "";
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetString("IDClass");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return id;
    }

    public string GetClassIdByNameClass(string nameClass)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Class where name='{nameClass}';";
        command.CommandText = query;
        MySqlDataReader reader;
        string id = "";
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetString("id");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return id;
    }
    #endregion

    #region User Getters
    public bool CheckIfUserHasClass(int idUser)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select * from Avatar where IDPlayerOwner='{idUser}';";
        command.CommandText = query;
        MySqlDataReader reader;
        bool hasClass = false;
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                hasClass = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return hasClass;
    }
    public string GetUserIDByEmail(string email)
    {
        StartService();
        MySqlCommand command = connection.CreateCommand();
        string query = $"Select id from Player where email='{email}';";
        command.CommandText = query;
        MySqlDataReader reader;
        string id = "";
        try
        {
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetString("id");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
        return id;
    }

    #endregion

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
}