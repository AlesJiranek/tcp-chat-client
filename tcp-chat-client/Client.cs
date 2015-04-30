using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_chat_client
{
    class Client
    {
        private TcpClient serverSocket;
        private bool isConnected = false;

        public bool Connect()
        {
            String ip = "127.0.0.1";
            int port = 7777;

            if (this.isConnected)
            {
                return true;
            }

            IPAddress ipAddress = IPAddress.Parse(ip);
            try
            {
                this.serverSocket = new TcpClient();
                this.serverSocket.Connect(ip, port);
                this.isConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                this.isConnected = false;
            }

            return this.isConnected;
        }

        public void Disconnect()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.serverSocket.Close();
            this.isConnected = false;
        }

        public void SendMessage(String message)
        {
            if (!this.isConnected)
            {
                return;
            }

            NetworkStream stream = this.serverSocket.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(message);
            writer.Flush();
        }

        public String receiveMessage()
        {
            NetworkStream stream = serverSocket.GetStream();
            StreamReader reader = new StreamReader(stream);
            String message = reader.ReadLine();

            return message;
        }

        public bool ValidateName(String name)
        {
            Regex reg = new Regex(@"^[a-zA-Zěščřžýáíéúů\s]+$");

            return reg.IsMatch(name);
        }
    }
}
