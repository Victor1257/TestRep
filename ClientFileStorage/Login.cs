using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientFileStorage
{
    
    public partial class Login : Form
    {
        public static HubConnection _connection;
        public string IDUSER;
        public string LINK;
        class Config
        {
            public string Link { get; set; }
            public string IdUser { get; set; }
        }
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
                _connection.ServerTimeout = TimeSpan.FromMinutes(60);
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

        private async void OnSend(string name, string message,string UserId)
        {
            Log(Color.Black, message);
            if (message=="true")
            {
                IDUSER = UserId;
                FileStorage newForm = new FileStorage(this.LINK,this.IDUSER);
                /*MessageBox.Show( Application.StartupPath );
                MessageBox.Show( Path.GetDirectoryName(Application.StartupPath) );
                MessageBox.Show( Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) );*/
                //using (FileStream fs = new FileStream("C:/Users/meschaninov/Desktop/TestRep - копия/WindowsService1/config.json", FileMode.OpenOrCreate))
                using (FileStream fs = new FileStream(Path.GetDirectoryName (Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath))) + "/WindowsService1/config.json", FileMode.OpenOrCreate))
                {
                    Config tom = new Config() { Link = LINK, IdUser = IDUSER };
                    await JsonSerializer.SerializeAsync<Config>(fs, tom);
                    Console.WriteLine("Data has been saved to file");
                }

                this.Hide();
                newForm.Show();
 
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox2.Focus();
        }

        private async void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    await _connection.InvokeAsync("Send", "WinFormsApp", textBox1.Text, textBox2.Text);
                }
                catch (Exception ex)
                {
                    Log(Color.Red, ex.ToString());
                }
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == true)
                textBox2.UseSystemPasswordChar = false;
            else textBox2.UseSystemPasswordChar = true;
        }

    }
}
