﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tcp_chat_client
{
    public class Connection
    {
        private TcpClient serverSocket;
        private bool isConnected = false;
        private String username;


        /**
         * Descructor
         */
        ~Connection()
        {
            this.Disconnect();
        }

        /**
         * Connects to server
         */ 
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


        /**
         * Disconnects from server
         */ 
        public void Disconnect()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.serverSocket.Close();
            this.isConnected = false;
        }


        /**
         * Sets username
         */ 
        public void SetUsername(String username)
        {
            this.username = username;
        }


        /**
         * Wraps message text to message object and sends it to server
         */ 
        public void SendMessage(String content)
        {
            Message message = new Message();
            message.Type = Message.MessageType.normal;
            message.Username = this.username;
            message.Content = content;
            message.Timestamp = DateTime.Now;

            this.SendMessage(message);
        }


        /**
         * Wraps content to system message object and sends it to server
         */ 
        public void SendSystemMessage(String content)
        {
            Message message = new Message();
            message.Type = Message.MessageType.system;
            message.Username = "System";
            message.Content = content;

            this.SendMessage(message);
        }


        /**
         * Serializes and sends message to server
         */ 
        private void SendMessage(Message message)
        {
            if (!this.isConnected)
            {
                this.Connect();
            }

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new DeserializationBinder();
            formatter.Serialize(this.serverSocket.GetStream(), message);
        }


        /**
         * Receives and deserializes message from server
         */ 
        public Message receiveMessage()
        {
            NetworkStream stream = serverSocket.GetStream();

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new DeserializationBinder();

            Message message = (Message)formatter.Deserialize(stream);

            return message;
        }
    }
}
