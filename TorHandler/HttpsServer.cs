using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TorHandler
{
    class HttpsServer
    {
        Utils utils = new Utils();
        public int httpsport;
        TcpListener listener;
        TorConnectionHandler torConnectionHandler;

        public HttpsServer(TorConnectionHandler torConnectionHandler, int httpsport)
        {
            this.httpsport = httpsport;//= utils.get_random_available_ports(1)[0];
            this.startserver();
            this.torConnectionHandler = torConnectionHandler;
        }
        private void startserver()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            this.listener = new TcpListener(IPAddress.Any, this.httpsport);
            this.listener.Start();
            new Thread(handleloop).Start();
        }
        private void handleloop()
        {
            while(true)
            {
                handle();
            }
        }
        private void handle()
        {
            Socket AcceptedConnection = this.listener.AcceptSocket();
            byte[] buffer = new byte[AcceptedConnection.ReceiveBufferSize];
            int read = 0;
            StringBuilder request = new StringBuilder();

            do
            {
                read = AcceptedConnection.Receive(buffer);
                request.Append(Encoding.UTF8.GetString(buffer, 0, read));
            } while (AcceptedConnection.Available > 0);

            string req = request.ToString();

            Socket socket;
            if (req.StartsWith("CONNECT"))//https
            {
                string connect = req.Split(' ')[1];
                IPHostEntry hostEntry = Dns.GetHostEntry(connect.Split(':')[0]);

                IPAddress connectto = hostEntry.AddressList[0];
                int port = Int32.Parse(connect.Split(':')[1]);

                AcceptedConnection.Send(Encoding.ASCII.GetBytes(string.Format("HTTP/1.1 200 OK\r\n\r\n", req[2])));
                socket = torConnectionHandler.CreateTorConnection(connectto, port);
                do
                {
                    if(socket.Available > 0)
                    {
                        read = socket.Receive(buffer);
                        AcceptedConnection.Send(buffer, read, SocketFlags.None);
                    }

                    if(AcceptedConnection.Available > 0)
                    {
                        read = AcceptedConnection.Receive(buffer);
                        socket.Send(buffer, read, SocketFlags.None);
                    }
                } while (socket.Connected && AcceptedConnection.Connected);    
            }
            else if(req.StartsWith("GET"))
            {
                string url = req.Split(' ')[1];
                Uri uri = new Uri(url);

                IPHostEntry hostEntry = Dns.GetHostEntry(uri.Host);
                IPAddress connectto;
                if (hostEntry.AddressList.Length > 0)
                {
                    connectto = hostEntry.AddressList[0];
                }
                else
                {
                    connectto = IPAddress.Parse(uri.Host);
                }
                int port = uri.Port;
                socket = torConnectionHandler.CreateTorConnection(connectto, port);

                Headers headers = new Headers(req);
                socket.Send(Encoding.ASCII.GetBytes(headers.get_headers()));
                do
                {
                    do
                    {
                        read = socket.Receive(buffer);
                        AcceptedConnection.Send(buffer, read, SocketFlags.None);

                    } while (socket.Available > 0);

                } while (socket.Connected && AcceptedConnection.Connected);
            }
            else
            {
                socket = null;
                return;
            }
        }
    }
}
