using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ChatRoomsWindow.xaml
    /// </summary>
    public partial class ChatRoomsWindow : Window
    {
        private const String CREATE_NEW_TEXTROOM_TEXT = "Create a new chatroom...";
        private Connection connection;


        /**
         * Initializes chat rooms window
         */ 
        public ChatRoomsWindow()
        {
            this.connection = new Connection();

            // Connect to server
            if (!this.connection.Connect())
            {
                MessageBox.Show("Unable to connect chat server! Server is not turned on probably.");
                Environment.Exit(1);
            }
            InitializeComponent();

            // Show list of available chat rooms
            this.ShowChatroomsList();
        }


        /**
         * Shows list of available chat rooms
         */
        private void ShowChatroomsList()
        {
            Message rooms = this.connection.ReceiveMessage();
            List<String> chatRooms = (List<String>)rooms.Content;
            chatRooms.Add(CREATE_NEW_TEXTROOM_TEXT);
            this.ChatRoomsListBox.ItemsSource = chatRooms;
        }


        /**
         * Handle click on chatroom
         */ 
        private void ChatRoomsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock)e.OriginalSource;
            String chatroomName = tb.Text;

            /**
             * User wants to create new chatroom
             */ 
            if (chatroomName.Equals(CREATE_NEW_TEXTROOM_TEXT))
            {
                // Show dialog to enter new chatroom name
                InputDialog chatroomNameDialog = new InputDialog("Enter name of new chatroom:");
                chatroomNameDialog.Title = "Enter Chatroom Name";
                if (chatroomNameDialog.ShowDialog() == true)
                {
                    chatroomName = chatroomNameDialog.GetAnswer();

                    // User is joker and tries to break this client
                    if(chatroomName == CREATE_NEW_TEXTROOM_TEXT)
                    {
                        MessageBox.Show("Haha, very funny... Try it again");
                        return;
                    }
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
                if (!this.ValidateName(chatroomName))
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
                String username = userNameDialog.GetAnswer();

                // Check if entered name contains only valid characters
                if (!this.ValidateName(username))
                {
                    MessageBox.Show("Invalid name!");
                    return;
                }

                this.connection.SetUsername(username);

                this.connection.SendMessage(chatroomName);
                this.connection.SendMessage(username);

                Window chatWindow = new ChatWindow(this.connection);
                chatWindow.Title = chatroomName;
                chatWindow.Activate();
                chatWindow.Show();
                this.Close();
            }
        }


        /**
         * Validates given name
         */ 
        public bool ValidateName(String name)
        {
            Regex reg = new Regex(@"^[a-zA-ZěščřžýáíéúůĚŠČŘŽÝÁÍÉÚŮ\s]+$");

            return reg.IsMatch(name);
        }
    }
}
