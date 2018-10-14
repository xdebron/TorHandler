using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorHandler
{
    class Headers
    {
        Dictionary<string, string> HeaderDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string QueryLine;

        public Headers(string data)
        {
            parse_string(data);
        }

        private void override_headers()
        {

            HeaderDict["Connection"] = "Close";//HeaderDict["Proxy-Connection"];
            HeaderDict.Remove("Proxy-Connection");
        }

        public void parse_string(string data)
        {
            string[] lines = data.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
            bool first = true;
            foreach(string line in lines)
            {
                if(first)//qline
                {
                    QueryLine = line;
                    first = false;
                }
                else if(line.Contains(": "))
                {

                    string[] linedata = line.Split(new string[] { ": " }, StringSplitOptions.None);
                    
                    HeaderDict.Add(linedata[0], linedata[1]);
                }
                
            }

            override_headers();
        }

        public void set_header(string key, string value)
        {
            HeaderDict[key] = value;
        }

        public bool delete_header(string key)
        {
            return HeaderDict.Remove(key);
        }

        public string get_header(string key)
        {
            return HeaderDict[key];
        }

        public string get_headers()
        {
            List<string> lines = new List<string>();
            lines.Add(QueryLine);
            foreach (KeyValuePair<string, string> Header in HeaderDict)
            {
                lines.Add(string.Join(": ", Header.Key, Header.Value));
            }

            return string.Join("\r\n", lines) + "\r\n\r\n";
        }

    }
}
