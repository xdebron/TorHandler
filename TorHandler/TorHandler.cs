using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace TorHandler
{
    public class TorHandler
    {
        public Process torprocess = new Process();
        public int socksport;
        public int controlport;
        public int httpsport;
        public bool isready = false;

        private Utils utils = new Utils();
        private HttpsServer httpserver;
        private TorConnectionHandler torConnectionHandler;


        public TorHandler(bool block = false, string path="tor", int httpsport=8080)
        {
            this.httpsport = httpsport;
            this.torprocess.StartInfo.FileName = path;
            this.torprocess.StartInfo.UseShellExecute = false;
            this.torprocess.StartInfo.CreateNoWindow = true;
            this.torprocess.StartInfo.RedirectStandardOutput = true;

            int[] ports = utils.get_random_available_ports(2);
            this.socksport = ports[0];
            this.controlport = ports[1];

            this.torprocess.StartInfo.Arguments = string.Format("--socksport {0} --controlport {1}", this.socksport, this.controlport);
            this.torprocess.Start();

            if (!block)
            {
                new Thread(check_readiness).Start();
            }
            else
            {
                check_readiness();
            }

            this.torConnectionHandler = new TorConnectionHandler(this.socksport);
        }

        private void check_readiness()
        {
            while (!this.torprocess.StandardOutput.EndOfStream && !this.torprocess.HasExited)
            {
                string standard_output = this.torprocess.StandardOutput.ReadLine();
                Console.WriteLine(standard_output);
                if (standard_output.Contains("Bootstrapped 100"))
                {
                    this.isready = true;
                    break;
                }
            }
        }

        public void start_httpserver()
        {
            this.httpserver = new HttpsServer(this.torConnectionHandler, this.httpsport);
        }


    }
}
