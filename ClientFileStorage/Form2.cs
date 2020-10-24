using Carbon.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;
using MediaBrowser.Model.Serialization;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;

namespace ClientFileStorage
{
    public partial class FileStorage : Form
    {
        public string Link;
        private HubConnection _connection;
        
        public FileStorage(string LINK)
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
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<Movie> movies= (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
            //listView1.Columns.Add("IdUSer");
            //listView1.Columns.Add("Name");
            //listView1.Columns.Add("ReleaseDate");
            //listView1.Columns.Add("File Name ");
            foreach (Movie mo in movies)
            {
                ListViewItem listView = new ListViewItem(mo.IDUser);
                listView.SubItems.Add(mo.Title);
                listView.SubItems.Add(mo.ReleaseDate.ToString());
                listView.SubItems.Add(mo.Name);
                listView1.Items.Add(listView);
            }

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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
