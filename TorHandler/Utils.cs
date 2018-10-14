using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace TorHandler
{
    class Utils
    {
        public int[] get_random_available_ports(uint count = 1)
        {
            int[] ports = new int[count];
            Random rand = new Random();
            for(int i=0; i < count; i++)
            {
                while (true)
                {
                    int port = rand.Next(35000, 65535);
                    if (!check_port(port))
                    {
                        ports[i] = port;
                        break;
                    }
                }
            }

            return ports;
            
        }
        public bool check_port(int port)
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            try
            {
                TcpListener Listener = new TcpListener(ip, port);
                Listener.Start();
                Listener.Stop();
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
