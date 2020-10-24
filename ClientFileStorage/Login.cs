using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;


namespace ClientFileStorage
{
    
    public partial class Login : Form
    {
        private HubConnection _connection;
        public string IDUSER;
        public string LINK;
        public Login()
        {
            InitializeComponent();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            addressTextBox.Focus();
        }

        private void addressTextBox_Enter(object sender, EventArgs e)
        {
            AcceptButton = connectButton;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            UpdateState(connected: false);
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(addressTextBox.Text)
                    .Build();
                LINK = addressTextBox.Text;
            }
            catch (Exception ex)
            {
                Log(Color.Red, ex.ToString());
                return;
            }
            _connection.On<string,string,string>("broadcastMessage", (s1, s2,s3) => OnSend(s1, s2,s3));

            Log(Color.Gray, "Starting connection...");
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Log(Color.Red, ex.ToString());
                return;
            }

            Log(Color.Gray, "Connection established.");

            UpdateState(connected: true);

            textBox1.Focus();
            textBox2.Focus();
        }

        private async void disconnectButton_Click(object sender, EventArgs e)
        {
            Log(Color.Gray, "Stopping connection...");
            try
            {
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {
                Log(Color.Red, ex.ToString());
            }

            Log(Color.Gray, "Connection terminated.");

            UpdateState(connected: false);
        }

        private void messageTextBox_Enter(object sender, EventArgs e)
        {
            AcceptButton = sendButton;
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("Send", "WinFormsApp", textBox1.Text,textBox2.Text);
            }
            catch (Exception ex)
            {
                Log(Color.Red, ex.ToString());
            }
        }

        private void UpdateState(bool connected)
        {
            disconnectButton.Enabled = connected;
            connectButton.Enabled = !connected;
            addressTextBox.Enabled = !connected;

            textBox1.Enabled = connected;
            textBox2.Enabled = connected;
            sendButton.Enabled = connected;
        }

        private void OnSend(string name, string message,string UserId)
        {
            Log(Color.Black, message);
            if (message=="true")
            {
                IDUSER = UserId;
                FileStorage newForm = new FileStorage(this.LINK);
                newForm.Show();
                this.Hide();
 
            }    
        }

        private void Log(Color color, string message)
        {
            Action callback = () =>
            {
               textBox3.Text= message;
            };

            Invoke(callback);
        }

        private class LogMessage
        {
            public Color MessageColor { get; }

            public string Content { get; }

            public LogMessage(Color messageColor, string content)
            {
                MessageColor = messageColor;
                Content = content;
            }
        }

    }
}
