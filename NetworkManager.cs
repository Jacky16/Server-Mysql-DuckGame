using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class NetworkManager
{
    TcpListener serverListener;
    List<Client> clients;
    public NetworkManager()
    {
        this.serverListener = new TcpListener(IPAddress.Any, 6543);
        clients = new List<Client>();
        serverListener = new TcpListener(IPAddress.Any, 6543);
    }
    public void StartNetworkService()
    {
        try
        {
            this.serverListener.Start();
            StartListening();
        }catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void StartListening()
    {
        Console.WriteLine("Esperando nueva conexion");
        this.serverListener.BeginAcceptTcpClient(AcceptConnection, this.serverListener);
    }

    void AcceptConnection(IAsyncResult ar)
    {
        Console.WriteLine("Recibo una conexion");

        TcpListener listener = (TcpListener)ar.AsyncState;  
        clients.Add(new Client(listener.EndAcceptTcpClient(ar)));

        StartListening();
    }
}
