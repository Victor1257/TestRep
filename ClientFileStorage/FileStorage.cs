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
using System.IO;
using System.Threading.Tasks;

namespace ClientFileStorage
{
    public partial class FileStorage : Form
    {
        public string Link;
        public string IdUser;
        private HubConnection _connection;
        private ListViewColumnSorter lvwColumnSorter;
        public ListViewItem listView;
        public string price;

        public FileStorage(string LINK, string IDUser)
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
            List<Movie> movies = (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
            foreach (Movie mo in movies)
            {
                listView = new ListViewItem(mo.IDUser);
                listView.SubItems.Add(mo.Title);
                listView.SubItems.Add(mo.ReleaseDate.ToString());
                listView.SubItems.Add(mo.Name);
                listView1.Items.Add(listView);
            }

        }

        private void OnSendAll(string movie1)
        {
            listView1.Items.Clear();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<Movie> movies = (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
            foreach (Movie mo in movies)
            {
                listView = new ListViewItem(mo.IDUser);
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
                    .WithAutomaticReconnect()
                    .Build();
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                return;
            }
            _connection.On<string>("Receive", (s1) => OnSend(s1));
            _connection.On<string, string>("doStuff", (s1, s2) => DoStuff(s1, s2));
            _connection.On<string>("FileDelete", (s1) => DeleteFile(s1));
            _connection.On<string>("ReceiveAll", (s1) => OnSendAll(s1));

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {

                return;
            }
        }

        private async void получитьИлиОбновитьСписокФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMovie", IdUser);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
     
        private async void скачатьВыбранныйФаилToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ListView.SelectedListViewItemCollection file =
        this.listView1.SelectedItems;
            foreach (ListViewItem item in file)
            {
                price = item.SubItems[3].Text;
            }
            textBox1.Text = price;
            try
            {
                await _connection.InvokeAsync("GetMovie", price);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void DoStuff(string data, string name)
        {
            try
            {
                byte[] newByte = Convert.FromBase64String(data);
                File.WriteAllBytes("C:/Users/merli/Desktop/Client/Client/ClientFileStorage/Uploads/" + name, newByte);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void DeleteFile(string name)
        {
            MessageBox.Show("Файл " + name + " удален.");
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListView tmp_SenderListView = sender as ListView;
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem tmp_SelectedItem = tmp_SenderListView.GetItemAt(e.X, e.Y);
                if (tmp_SelectedItem == null)
                {
                    contextMenuStrip1.Show(tmp_SenderListView, e.Location);
                }
                else
                {
                    tmp_SelectedItem.Selected = true;
                    contextMenuStrip1.Show(tmp_SenderListView, e.Location);
                }
            }
        }

        private async void скачатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection file =
                    this.listView1.SelectedItems;
            foreach (ListViewItem item in file)
            {
                price = item.SubItems[3].Text;
            }
            textBox1.Text = price;
            try
            {
                await _connection.InvokeAsync("GetMovie", price);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void удалитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection file =
                   this.listView1.SelectedItems;
            foreach (ListViewItem item in file)
            {
                price = item.SubItems[3].Text;
            }
            textBox1.Text = price;
            try
            {
                await _connection.InvokeAsync("DeleteFile", price);
                получитьИлиОбновитьСписокФайловToolStripMenuItem_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void списокВсехФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendAllMovie", IdUser);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

