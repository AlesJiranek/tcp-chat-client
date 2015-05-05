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


        /**
         * Init main chat window
         */
        public ChatWindow(Connection client)
        {
            this.connection = client;

            InitializeComponent();

            this.MessageTextBox.Focus();
            this.ReceivedMessagesList.ItemsSource = this.messages = new List<Message>();

            Task receiveTask = new Task(this.ReceiveMessages);
            receiveTask.Start();

        }


        /**
         * Receives and displays list of users connected to chatroom
         */
        private void ShowConnectedUsersList()
        {
            // We need to invoke action when updating clients list from receive thread
            this.Dispatcher.Invoke((Action)(() =>
            {
                Message users = this.connection.ReceiveMessage();
                List<String> usersList = (List<String>)users.Content;
                this.ConnectedUsersList.ItemsSource = usersList;
                this.ConnectedUsersList.Items.Refresh();
            }));
        }


        /**
         * Handles send button click
         */
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.SendMessage();
        }


        /**
         * Gets text from text box and sends it to server
         */
        private void SendMessage()
        {
            if (this.MessageTextBox.Text.Length <= 0)
                return;

            this.connection.SendMessage(MessageTextBox.Text.Trim());
            this.MessageTextBox.Text = "";
        }


        /**
         * Handles user input to message text box
         */
        private void MessageTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (MessageTextBox.Text.Length > 0)
            {
                // Enable send button if message text is not empty
                SendMessageButton.IsEnabled = true;

                // If pressed key is enter, send message
                if (e.Key == Key.Enter)
                    this.SendMessage();
            }
            else
            {
                // Disable send button if message text is empty
                SendMessageButton.IsEnabled = false;
            }
        }


        /**
         * Method for receiving messages from server
         */
        private void ReceiveMessages()
        {
            Message message;

            try
            {
                while (true)
                {
                    message = this.connection.ReceiveMessage();

                    // Received message is system message
                    if (message.Type == Message.MessageType.system)
                    {
                        this.HandleSystemMessage(message);
                    }

                    // Received message is normal message
                    else
                    {
                        // Need to invoke action because we are updating messages list, which parent is in different thread
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            // Add message to messages list items control
                            this.messages.Add(message);
                            // Refresh messages list
                            this.ReceivedMessagesList.Items.Refresh();
                            // Scroll messages to bottom
                            this.ReceievedMessagesScrollView.ScrollToBottom();
                        }));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server was shut down");
                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.Close();
                }));

                return;
            }
        }


        /**
         * Handles system message
         */
        private void HandleSystemMessage(Message message)
        {
            switch (message.Content.ToString())
            {
                case "Connected Users":
                    this.ShowConnectedUsersList();
                    break;
                default: return;
            }
        }


        /**
         * Handles context menu on messages list
         */
        private void ShowTimesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Show messages times
            if (ShowTimesMenuItem.IsChecked)
            {
                Style style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.VisibilityProperty, Visibility.Hidden));
                style.Setters.Add(new Setter(TextBlock.WidthProperty, 0.0));
                Application.Current.Resources["MessageTimeStyle"] = style;
                ShowTimesMenuItem.IsChecked = false;
            }

            // Hide messages style
            else
            {
                Style style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.VisibilityProperty, Visibility.Visible));
                Application.Current.Resources["MessageTimeStyle"] = style;
                ShowTimesMenuItem.IsChecked = true;
            }
        }

    }
}
