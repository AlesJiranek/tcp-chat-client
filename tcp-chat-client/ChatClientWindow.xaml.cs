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
        private const String CREATE_NEW_TEXTROOM_TEXT = "Create a new chatroom...";
        private Client client;


        public ChatClientWindow()
        {
            this.client = new Client();

            if (!this.client.Connect())
            {
                MessageBox.Show("Unable to connect chat server! Server is not turned on probably.");
                Environment.Exit(1);
            }

            InitializeComponent();

            this.showChatroomsList();
        }

        ~ChatClientWindow()
        {
            this.client.Disconnect();
        }

        private void showChatroomsList()
        {
            String roomsString = this.client.receiveMessage();
            List<String> chatRooms = roomsString.Split(';').ToList();
            chatRooms.Add(CREATE_NEW_TEXTROOM_TEXT);
            this.ChatRoomsListBox.ItemsSource = chatRooms;
        }


        private void ChatRoomsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock)e.OriginalSource;
            String chatroomName = tb.Text;

            if (chatroomName.Equals(CREATE_NEW_TEXTROOM_TEXT))
            {
                // Show dialog to enter new chatroom name
                InputDialog chatroomNameDialog = new InputDialog("Enter name of new chatroom:");
                chatroomNameDialog.Title = "Enter Chatroom Name";
                if (chatroomNameDialog.ShowDialog() == true)
                {
                    chatroomName = chatroomNameDialog.getAnswer();
                }
                else
                {
                    return;
                }

                // Check if chatroom with same name already exists
                // Items.Contains() doesn't support case insensitive comparsion
                if (this.ChatRoomsListBox.Items.OfType<string>().Contains(chatroomName,StringComparer.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Chatroom with name \"" + chatroomName + "\" already exists.");
                    return;
                }

                // Check if entered name contains only valid characters
                if (!this.client.ValidateName(chatroomName))
                {
                    MessageBox.Show("Invalid chatroom name!");
                    return;
                }
            }


            // Show dialog to enter username
            InputDialog userNameDialog = new InputDialog("Enter your name:");
            userNameDialog.Title = "Enter Name";
            if (userNameDialog.ShowDialog() == true)
            {
                String username = userNameDialog.getAnswer();

                // Check if entered name contains only valid characters
                if (!this.client.ValidateName(username))
                {
                    MessageBox.Show("Invalid name!");
                    return;
                }

                this.client.SendMessage(chatroomName);
                this.client.SendMessage(username);
            }
        }
    }
}
