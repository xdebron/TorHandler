using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace TorHandler
{
    class TorConnectionHandler
    {
        int socksport;
        byte[] handshake = new byte[] { 0x05, 0x01, 0x00 };

        public TorConnectionHandler(int socksport)
        {
            this.socksport = socksport;
        }

        public Socket CreateTorConnection(IPAddress ip, int port)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            sock.ReceiveTimeout = 30000;
            sock.SendTimeout = 30000;
            IPAddress localip = IPAddress.Parse("127.0.0.1");
            sock.Connect(localip, this.socksport);
            sock.Send(this.handshake);
            byte[] buffer = new byte[10];
            sock.Receive(buffer);
            if (buffer[1] == 0 && buffer[0] == 5)
            {
                byte[] ipbytes = ip.GetAddressBytes();
                byte[] portbytes = new byte[] { (byte)((port >> 8) & 0xff), (byte)(port & 0xff) };
                byte[] connect = new byte[] { 0x05, 0x01, 0x00, 0x01, ipbytes[0], ipbytes[1], ipbytes[2], ipbytes[3], portbytes[0], portbytes[1] };

                sock.Send(connect);
                sock.Receive(buffer);

                if (buffer[1] == 0)
                {
                    return sock;
                }
            }

            return null;


        }
        public Socket CreateTorConnection(string ip, int port)
        {
            return CreateTorConnection(IPAddress.Parse(ip), port);
        }

    }
}
