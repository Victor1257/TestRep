using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientFileStorage
{
    
    public partial class Login : Form
    {
        public string IDUSER;
        public string LINK;
        public HttpClient client;
        public CookieContainer cookieContainer;
        public HttpClientHandler HttpClientHandler;
        private Uri baseAddress;
        private HttpClientHandler handler;

        class Config
        {
            public string Link { get; set; }
            public string IdUser { get; set; }
        }
        public Login()
        {
            InitializeComponent();
                Opacity = 0;
            Timer timer = new Timer();
            timer.Tick += new EventHandler((sender, e) =>
            {
                if ((Opacity += 0.3d) == 1) timer.Stop();
            });
            timer.Interval = 100;
            timer.Start();
            UpdateState(connected: false);
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
                baseAddress = new Uri(addressTextBox.Text);
                CookieContainer cookieContainer = new CookieContainer();
                handler = new HttpClientHandler();
                handler.CookieContainer = cookieContainer;
                client = new HttpClient(handler);
                client.BaseAddress = baseAddress;
                HttpResponseMessage response = await client.GetAsync(addressTextBox.Text);
                response.EnsureSuccessStatusCode();
                LINK = addressTextBox.Text;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Log(Color.Gray, "Connection established.");
                UpdateState(connected: true);
                textBox1.Focus();
            }
        }

        private async void disconnectButton_Click(object sender, EventArgs e)
        {
            Log(Color.Gray, "Stopping connection...");
            try
            {
                client = null;
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
               HttpResponseMessage responseMessage =await client.GetAsync("api/login/send?Email=" + textBox1.Text+ "&Password=" + textBox2.Text);
                responseMessage.EnsureSuccessStatusCode();
                var emp = await responseMessage.Content.ReadAsStringAsync();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                string[] Param = (string[])json_serializer.Deserialize(emp, typeof(string[]));
                if (Param[0]=="true")
                {
                    IDUSER = Param[2];
                    FileStorage newForm = new FileStorage(this.LINK, this.IDUSER);
                    this.Hide();
                    newForm.Show();
                }
                else
                {
                    textBox3.Text = "Неверный логин или пароль";
                }
                
            }
            catch (HttpRequestException ex)
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
                    HttpResponseMessage responseMessage = await client.GetAsync("api/user?Email=" + textBox1.Text + "&Password=" + textBox2.Text);
                    responseMessage.EnsureSuccessStatusCode();
                    var emp = await responseMessage.Content.ReadAsStringAsync();
                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    string[] Param = (string[])json_serializer.Deserialize(emp, typeof(string[]));
                    if (Param[0] == "true")
                    {
                        IDUSER = Param[2];
                        FileStorage newForm = new FileStorage(this.LINK, this.IDUSER);
                        this.Hide();
                        newForm.Show();
                    }
                    else
                    {
                        textBox3.Text = "Неверный логин или пароль";
                    }
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

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            client = null;
        }
    }
}
