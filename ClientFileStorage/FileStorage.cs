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
        public string IdUser;
        private HubConnection _connection;
        private ListViewColumnSorter lvwColumnSorter;

        public FileStorage(string LINK,string IDUser)
        {
            InitializeComponent();
            this.textBox1.Text = LINK;
            Link = LINK;
            IdUser = IDUser;
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
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
            listView1.Items.Clear();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<Movie> movies= (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
            foreach (Movie mo in movies)
            {
                ListViewItem listView = new ListViewItem(mo.IDUser);
                listView.SubItems.Add(mo.Title);
                listView.SubItems.Add(mo.ReleaseDate.ToString());
                listView.SubItems.Add(mo.Name);
                listView1.Items.Add(listView);
            }

        }


            private async void Form2_Load(object sender, EventArgs e)
        {

            // Ensure that the view is set to show details.
            listView1.View = View.Details;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
           
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void получитьИлиОбновитьСписокФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMovie",IdUser);
            }
            catch (Exception ex)
            {

            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void скачатьВыбранныйФаилToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
