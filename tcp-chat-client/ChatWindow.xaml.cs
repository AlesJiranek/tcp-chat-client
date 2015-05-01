using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tcp_chat_client
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private Connection connection;
        private List<Message> messages;

        public ChatWindow(Connection client)
        {
            this.connection = client;

            InitializeComponent();

            this.MessageTextBox.Focus();
            this.ReceivedMessagesList.ItemsSource = this.messages = new List<Message>();

            Task receiveTask = new Task(this.receiveMessages);
            receiveTask.Start();

        }

        private void showConnectedUsersList()
        {
            this.Dispatcher.Invoke((Action)(() => { 
                Message users = this.connection.receiveMessage();
                List<String> usersList = (List<String>)users.Content;
                this.ConnectedUsersList.ItemsSource = usersList;
                this.ConnectedUsersList.Items.Refresh();
            }));
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.sendMessage();
        }

        private void sendMessage()
        {
            if (this.MessageTextBox.Text.Length <= 0)
                return;

            this.connection.SendMessage(MessageTextBox.Text);
            this.MessageTextBox.Text = "";
        }

        private void MessageTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (MessageTextBox.Text.Length > 0)
            {
                SendMessageButton.IsEnabled = true;

                if (e.Key == Key.Enter)
                    this.sendMessage();
            }
            else
            {
                SendMessageButton.IsEnabled = false;
            }
        }


        private void receiveMessages()
        {
            Message message;

            try
            {
                while (true)
                {
                    message = this.connection.receiveMessage();

                    if (message.Type == Message.MessageType.system)
                    {
                        this.handleSystemMessage(message);
                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            this.messages.Add(message);
                            this.ReceivedMessagesList.Items.MoveCurrentToLast();
                            this.ReceivedMessagesList.Items.Refresh();
                            this.ReceievedMessagesScrollView.ScrollToBottom();
                        }));
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Server was shut down");
                return;
            }
        }


        private void handleSystemMessage(Message message)
        {
            switch (message.Content.ToString())
            {
                case "Connected Users":
                    this.showConnectedUsersList(); 
                    break;
                default: return;
            }
        }

    }
}
