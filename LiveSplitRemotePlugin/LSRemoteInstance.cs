using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiveSplit.RemotePlugin
{
    public class LSRemoteInstance
    {
        private readonly DateTime start;
        private TcpClient client;

        public string ServerStatus { get; set; }

        private readonly LSRemote remote;

        public LSRemoteInstance(LSRemote remote)
        {
            this.remote = remote;

            this.start = DateTime.Now;

            this.ConnectToServer();
        }

        public string sendCommandString(string commandString, bool hasReturn)
        {
            if (this.client.Connected)
            {
                StreamWriter sw = new StreamWriter(this.client.GetStream());
                StreamReader sr = new StreamReader(this.client.GetStream());
                sw.WriteLine(commandString);
                sw.Flush();
                if (hasReturn)
                {
                    return sr.ReadLine();
                }
            }

            return "";
        }

        public async Task ConnectToServer()
        {
            this.remote.settings.SetStatusText("Attempting connection...");

            try
            {
                this.client?.Close();
                this.client = new TcpClient();
                await this.client.ConnectAsync(this.remote.settings.ServerIP, this.remote.settings.Port);
                this.ServerStatus = "Connected";
            }
            catch
            {
                this.ServerStatus = "Connection Error\nPlease check settings and restart LiveSplit server";
            }

            this.remote.settings.SetStatusText(this.ServerStatus);
        }

        public void Disconnect()
        {

        }
    }
}
