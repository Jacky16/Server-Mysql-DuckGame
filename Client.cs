using System.Net.Sockets;

//Clase para almacenar los datos necesarios de cada cliente
class Client
{

    private TcpClient tcp; //Almacenamos los datos de conexion del cliente
    public string nick; //Almacenamos el nick del cliente (en este caso hardcodeado)
    public string password;
    public string email;
    public int idDatabase;
    private bool waitingPing; //Almacenamos si estamos a la espera que responda un ping de conexion

    //Constructor de la clase
    public Client(TcpClient tcp)
    {
        this.tcp = tcp;
        this.nick = "Anonymous";
        this.waitingPing = false;
    }
   
    //-------------------------------- Getters ----------------------
    public TcpClient GetTcpClient()
    {
        return this.tcp;
    }

    public string GetNick()
    {
        return this.nick;
    }

    public string GetPassword()
    {
        return this.password;
    }
    public string GetEmail()
    {
        return this.email;
    }

    public bool GetWaitingPing()
    {
        return this.waitingPing;
    }

    //-------------------------------- Setters ----------------------
    public void SetWaitingPing(bool waitingPing)
    {
        this.waitingPing = waitingPing;
    }
    public void SetNick(string nick)
    {
        this.nick = nick;
    }
    public void SetPassword(string password)
    {
        this.password = password;
    }
    public void SetEmail(string email)
    {
        this.email = email;
    }
    public void SetIdDatabase(int idDatabase)
    {
        this.idDatabase = idDatabase;
    }
}