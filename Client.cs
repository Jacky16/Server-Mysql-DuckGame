using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    private TcpClient tcp;
    private string nick;

    public Client(TcpClient tcp)
    {
        this.tcp = tcp;
        this.nick = "Anonymous";
    }
}
