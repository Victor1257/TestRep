using Carbon.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ClientFileStorage
{
    public partial class Form2 : Form
    {
        public string Link;
        private HubConnection _connection;
        
        public Form2(string LINK)
        {
            InitializeComponent();
            this.textBox1.Text = LINK;
            Link = LINK;
        }

        public class Movie
        {
            public int Id { get; set; }
            public string IDUser { get; set; }
            public string Title { get; set; }
            public DateTime ReleaseDate { get; set; }
            public string Name { get; set; }
        }
        public List<Movie> Movies { get; set; }
        private void OnSend(string movie1)
        {
            webBrowser1.Navigate(movie1);

        }


            private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMovie");
            }
            catch (Exception ex)
            {
                
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(Link)
                    .Build();
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                return;
            }
            _connection.On<string>("Receive", (s1) => OnSend(s1));


            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {

                return;
            }
        }
    }
}
