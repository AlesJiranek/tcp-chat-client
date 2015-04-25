using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tcp_chat_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatClientWindow : Window
    {
        private TcpClient serverSocket;
        private bool isConnected = false;


        public ChatClientWindow()
        {
            Connect();
            if (!this.isConnected)
            {
                MessageBox.Show("Unable to connect chat server! Server is not turned on probably.");
                Environment.Exit(1);
            }

            InitializeComponent();

            this.showChatroomsList();
        }

        ~ChatClientWindow()
        {
            this.Disconnect();
        }

        protected void Connect()
        {
            String ip = "127.0.0.1";
            int port = 7777;

            if (this.isConnected)
            {
                return;
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
        }

        protected void Disconnect()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.serverSocket.Close();
            this.isConnected = false;
        }

        protected void SendMessage(String message)
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

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.SendMessage("[" + DateTime.Now.ToString() + "]: Ahoj");
        }

        private String receiveMessage()
        {
            NetworkStream stream = serverSocket.GetStream();
            StreamReader reader = new StreamReader(stream);
            String message = reader.ReadLine();

            return message;
        }

        private void showChatroomsList()
        {
            String roomsString = receiveMessage();
            List<String> chatRooms = roomsString.Split(';').ToList();
            this.ChatRoomsListBox.ItemsSource = chatRooms;
        }

        private void ChatRoomsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InputDialog dialog = new InputDialog("Enter your name:");

            if(dialog.ShowDialog() == true)
                Console.WriteLine(dialog.getAnswer());
        }
    }
}
