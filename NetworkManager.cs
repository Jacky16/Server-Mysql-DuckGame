using System.Net.Sockets;
using System.Net;
public class Network_Manager
{
    private List<Client> clients;
    private TcpListener serverListener;
    private Mutex clientListMutex;
    private int lastTimePing;
    private List<Client> disconnectClients;
    private DatabaseManager databaseManager;

    public Network_Manager()
    {
        //Lista de clientes que se van conectando al servidor
        this.clients = new List<Client>();

        //Instanciamos la clase de listener para que escuche cualquier IP por el puerto 6543
        this.serverListener = new TcpListener(IPAddress.Any, 6543);

        //Instanciamos el lister para evitar problemas de memoria compartida
        this.clientListMutex = new Mutex();

        //Creamos una variable a modo de temporizador para enviar pings
        this.lastTimePing = Environment.TickCount;

        //Lista de clientes pendientes de desconexion
        this.disconnectClients = new List<Client>();

        databaseManager = new DatabaseManager();

    }

    public void Start_Network_Service()
    {
        databaseManager.StartService();

        try
        {
            //Iniciamos el listener para prepararlo para la escucha de peticiones
            this.serverListener.Start();

            StartListening();
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void StartListening()
    {
        Console.WriteLine("Esperando nueva conexión");
        //Cada vez que nos llece una solicitud de conexion llamaremos de forma asincrona a la funcion de Callback AcceptConnection (automaticamente genera un thread)
        this.serverListener.BeginAcceptTcpClient(AcceptConnection, this.serverListener);
    }

    private void AcceptConnection(IAsyncResult ar)
    {
        Console.WriteLine("Recibo una conexion");

        //Almacenamos los datos de la peticion entrante
        TcpListener listener = (TcpListener)ar.AsyncState;

        //Bloqueo el mutex ya que la lista de clientes es usada en el thread principal del programa y en esta funcion que es llamada de forma asincrona cada vez que hay una peticion de conexion
        clientListMutex.WaitOne();

        //Añado la conexion en la lista
        this.clients.Add(new Client(listener.EndAcceptTcpClient(ar)));

        //Libero el mutex
        clientListMutex.ReleaseMutex();

        //Vuelvo a escuchar nuevas peticiones
        StartListening();
    }

    public void CheckMessage()
    {
        //Dado que voy a recorrer la lista de clientes bloqueo las peticiones
        clientListMutex.WaitOne();
        foreach (Client client in this.clients)
        {
            //Obtento el streaming de datos, es decir, el envio y recepcion de datos
            NetworkStream netStream = client.GetTcpClient().GetStream();

            //Comprobamos si la información esta lista para ser leida (puede ser enviada parcialmente, corrompida...)
            if (netStream.DataAvailable)
            {
                //Instancio el reader para leer los datos recibidos
                StreamReader reader = new StreamReader(netStream, true);

                //Leo una linea de datos, cada readline lee un writeline recibido
                string data = reader.ReadLine();

                //Solo leo datos si no son nulos
                if (data != null)
                {
                    ManageData(client, data);
                }
            }
        }
        //Libero el mutex
        clientListMutex.ReleaseMutex();
    }


    public void CheckConnection()
    {
        //Revisamos conexiones cada cierto tiempo, en este caso 5000 milisegundos
        if (Environment.TickCount - this.lastTimePing > 5000)
        {
            //Bloqueo el mutex para poder leer la lista de clientes conectados
            clientListMutex.WaitOne();

            //Recorremos los clientes conectados
            foreach (Client client in this.clients)
            {
                //Si le habiamos enviado un ping le añadimos a la lista de usuarios a desconectar (todavia no lo desconecto)
                if (client.GetWaitingPing() == true)
                {
                    disconnectClients.Add(client);
                }
                //Si no habiamos enviado ping le enviamos uno
                else
                {
                    //Enviamos ping para confirmar que sigue conectado
                    SendPing(client);
                }
            }

            //Almacenamos el tiempo actual del ultimo envio de ping
            this.lastTimePing = Environment.TickCount;

            //Liberamos mutex
            clientListMutex.ReleaseMutex();
        }
    }

    private void ManageData(Client client, string data)
    {
        //Separo los datos enviados, primero el comando y luego los parametros adicionales
        string[] parameters = data.Split('/');

        switch (parameters[0])
        {
            //Hemos definido que el 0 significa login y recibe dos parametros mas (usuario y contraseña)
            case "Login":
                
                client.SetEmail(parameters[1]);
                client.SetPassword(parameters[2]); 
                
                string idUser = databaseManager.GetUserIDByEmail(parameters[1]);
                if (idUser == "") return;
                client.SetIdDatabase(int.Parse(idUser));

                Login(client);

                break;
            case "Register":
                Register(parameters[1], parameters[2], parameters[3]);
                break;
                
            case "AddClassUser":
                string idClass = databaseManager.GetClassIdByNameClass(parameters[2]);
                databaseManager.AddClassToUser(parameters[1], idClass);
                break;
            case "GetClassByNickName":
                string classToSend = databaseManager.GetClassByNickName(parameters[1]);
                SendClass(client, classToSend);
                break;
                
        }
    }

    private void SendClass(Client client, string classToSend)
    {
        try
        {
            //Instanciamos el writer para enviar el mensaje de ping
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            writer.WriteLine("Class|" + classToSend);
            writer.Flush();
            
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        
    }

    bool Login(Client client)
    {
        //Leyenda codigos
        //2: Logeo pero sin clase asiganda
        //3: User no encontrado
        //4: Logeo con clase asignada
        int code;
        bool login = false;
        //Logeo correcto
        if (databaseManager.LogInUser(client.GetEmail(), client.GetPassword(), ref client))
        {
            login = true;
            code = 2;
            //Comprobar si el usuario tiene clase asignada
            if (databaseManager.CheckIfUserHasClass(client.idDatabase))
            {
                code = 4;
            }            
        }
        else
        {
            //User no encontrado
            code = 3;
        }
        SendInfoToUnityClient(client,code);
        return login;
    }

    public void Register(string nick, string password,string email)
    {
        databaseManager.Register(nick, password, email);
    }

    private void SendPing(Client client)
    {
        try
        {
            //Instanciamos el writer para enviar el mensaje de ping
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            //Enviamos el ping
            writer.WriteLine("ping");

            //Limpiamos el bufer de envio para evitar que siga acumulando datos
            writer.Flush();

            //Asignamos a true la variable de ping propia del cliente para identificar que le hemos enviado un ping para la siguiente iteracion
            client.SetWaitingPing(true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " with client" + client.GetNick());
        }
    }

    private void SendInfoToUnityClient(Client client,int code)
    {
        //Leyenda codigos
        //2: Logeo pero sin clase asiganda
        //3: User no encontrado
        //4: Logeo con clase asignada
        try
        {
            //Instanciamos el writer para enviar el mensaje de ping
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            string toSend = code.ToString() + "|";
            if (code == 2)
            {
                toSend += client.GetNick() + "/" + client.idDatabase;
                
            }
            else if(code == 3)
            {

            }
            else if (code == 4)
            {
                toSend += client.GetNick() + "/" + client.idDatabase + "/" + databaseManager.GetClassIdByUserId(client.idDatabase);
            }



            writer.WriteLine(toSend);

            //Limpiamos el bufer de envio para evitar que siga acumulando datos
            writer.Flush();
            
            if (code == 2 || code == 4)
                SendAllClassesToUnity(client);


            //Asignamos a true la variable de ping propia del cliente para identificar que le hemos enviado un ping para la siguiente iteracion
            client.SetWaitingPing(true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " with client" + client.GetNick());
        }
    }



    void SendAllClassesToUnity(Client client)
    {
        string toSend = "GetAllClasses|";

        toSend += databaseManager.GetAllClases();
        
        StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

        writer.WriteLine(toSend);

        writer.Flush();
    }
    public void DisconnectClients()
    {

        //Bloqueo el mutex para acceder a la lista de clientes (en el remove)
        clientListMutex.WaitOne();

        //Recorro la lista de usuarios a desconectar
        foreach (Client client in this.disconnectClients)
        {
            Console.WriteLine("Desconectando usuarios");

            //Cierro la conexion antes de eliminar el cliente de la lista de conectados
            client.GetTcpClient().Close();

            //Elimino el cliente de la lista de clientes a desconectar
            this.clients.Remove(client);
        }

        //Vacio la lista de usuarios a desconectar
        this.disconnectClients.Clear();

        //Libero el mutex
        clientListMutex.ReleaseMutex();
    }

}