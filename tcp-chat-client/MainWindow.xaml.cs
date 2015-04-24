using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        private TcpClient   clientSocket;
        private bool        isConnected = false;


        public MainWindow()
        {
            InitializeComponent();
            this.clientSocket = new TcpClient();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            this.Connect();
        }

        protected bool Connect()
        {
            String ip = "127.0.0.1";
            int port = 7777;

            IPAddress ipAddress = IPAddress.Parse(ip);
            try
            {
                this.clientSocket.Connect(ip, port);
                this.isConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to connect to chat server.");
                Console.WriteLine("Error: " + e.Message);
                this.isConnected = false;
            }

            return this.isConnected;
        }
    }
}
