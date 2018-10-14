using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TorHandler;
using System.Text;

namespace TorHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            TorHandler th = new TorHandler(true, "Tor/tor.exe", 8080);
            th.start_httpserver();

            WebProxy proxy = new WebProxy("127.0.0.1", 8080);
            proxy.UseDefaultCredentials = true;

            WebClient webClient = new WebClient();
            webClient.Proxy = proxy;

            Console.WriteLine(webClient.DownloadString("https://www.youtube.com/watch?v=gMkFqfisOkk"));
        }
    }
}
