using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Server
{
    
    static void Main(string[] args)
    {
        bool serverOn = true;
        NetworkManager networkManager = new NetworkManager();
        StartServices();
        while (serverOn)
        {

        }
        void StartServices()
        {
            networkManager.StartNetworkService();
        }
    }
    

}
