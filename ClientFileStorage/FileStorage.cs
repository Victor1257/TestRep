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
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;

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
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;

        public FileStorage(string LINK, string IDUser)
        {
            InitializeComponent();
            this.textBox1.Text = LINK;
            Link = LINK;
            IdUser = IDUser;
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            tabPage1.Text = "Файлы";
            tabPage2.Text = "Задачи";
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
        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT *,'Delete' AS [Delete] FROM Task", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();
                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Task");
                dataGridView1.DataSource = dataSet.Tables["Task"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[18, i] = linkCell;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ReloadData()
        {
            try
            {
                dataSet.Tables["Task"].Clear();
                sqlDataAdapter.Fill(dataSet, "Task");
                dataGridView1.DataSource = dataSet.Tables["Task"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[18, i] = linkCell;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "modelDataSet.Task". При необходимости она может быть перемещена или удалена.
            this.taskTableAdapter.Fill(this.modelDataSet.Task);

            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Repositories\TestRep\ClientFileStorage\Database1.mdf;Integrated Security=True");
            //sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
            sqlConnection.Open();
            LoadData();
            listView1.View = View.Details;
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(Link)
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .Build();
                _connection.ServerTimeout = TimeSpan.FromMinutes(10);
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                return;
            }
            _connection.On<string>("Receive", (s1) => OnSend(s1));
            _connection.On<byte[], string, long, long, long>("doStuff", (s1, s2, s3, s4, s5) => DoStuff(s1, s2, s3, s4, s5));
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
                if (lvwColumnSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }



        private void DoStuff(byte[] data, string name, long Position, long Lenght, long READBUFFER_SIZE)
        {

            try
            {
                if (File.Exists("./Uploads/" + name))
                {
                    using (FileStream FS = new FileStream(Path.Combine("./Uploads/", name), FileMode.Open, FileAccess.Write, FileShare.Write))
                    {
                        FS.Position = Position;
                        FS.Write(data, 0, (int)READBUFFER_SIZE);
                    }
                }
                else
                {
                    using (FileStream FS = new FileStream(Path.Combine("./Uploads/", name), FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        FS.Position = Position;
                        FS.Write(data, 0, (int)READBUFFER_SIZE);
                    }
                }
                progressBar1.Minimum = 0;
                progressBar1.Maximum = (int)Lenght;
                progressBar1.Value = (int)Position;
                if (progressBar1.Value >= (int)Lenght - 1048576)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("Файл загружен!");
                }

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
                await _connection.InvokeAsync("GetMovie", price, IdUser);
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
                if (MessageBox.Show("Удалить файл?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                           == DialogResult.Yes)
                {
                    await _connection.InvokeAsync("DeleteFile", price, IdUser);
                    получитьИлиОбновитьСписокФайловToolStripMenuItem_Click(sender, e);
                }
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

        private void задачиToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void переподключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2_Load(sender, e);
        }

        private async void загрузитьФайлToolStripMenuItem_Click1(string FileNamePath)
        {
            var fileName = new DirectoryInfo(FileNamePath).Name;
            long READBUFFER_SIZE = 1048576;
            int offset = 0;
            long Pos = 0;
            try
            {
                using (FileStream FS = new FileStream(FileNamePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long n = FS.Length;
                    while (FS.Position < FS.Length)
                    {
                        if (n <= READBUFFER_SIZE)
                        {
                            byte[] FSBuffer = new byte[n];
                            FS.Read(FSBuffer, offset, (int)n);
                            await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, n, IdUser);
                        }
                        else
                        {
                            byte[] FSBuffer = new byte[READBUFFER_SIZE];
                            FS.Read(FSBuffer, offset, (int)READBUFFER_SIZE);
                            await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, READBUFFER_SIZE, IdUser);
                            n -= READBUFFER_SIZE;
                        }
                        Pos = FS.Position;
                        GC.Collect();
                    }
                    await _connection.InvokeAsync("WriteToDataBase", fileName, IdUser);

                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void создатьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task newForm = new Task(Link, IdUser);
            newForm.SetCP(this);
            newForm.Show();
        }

        public void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 18)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[18].Value.ToString();
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить задачу?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            string[] stringTask = new string[18];
                            for (int i = 0; i < 18; i++)
                            {
                                if (i == 2)
                                {
                                    stringTask[i] = dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString() + dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString().Replace(":", "-");
                                }
                                else
                                stringTask[i] = dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString();
                            }
                            //MessageBox.Show("BEFORE");
                            //await _connection.InvokeAsync("DeleteTaskFromDataBase", stringTask, IdUser);
                            //MessageBox.Show("AFTER");
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["Task"].Rows[rowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "Task");
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() => MakeTask());
        }

        private async void MakeTask()
        {
            try
            {
                var Time = DateTime.Now;

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {

                    bool isPeriodic = Convert.ToBoolean(Convert.ToInt32(dataGridView1[3, i].Value));
/*                    var Time2 = Convert.ToDateTime(dataGridView1[3, i].Value)*/;

                    if (!isPeriodic)
                    {
                        if (Convert.ToDateTime(dataGridView1[15, i].Value).Date == Time.Date && Convert.ToDateTime(dataGridView1[16, i].Value).TimeOfDay.TotalMinutes>= Time.TimeOfDay.TotalMinutes && Convert.ToDateTime(dataGridView1[16, i].Value).TimeOfDay.TotalMinutes <= Time.AddMinutes(1).TimeOfDay.TotalMinutes)
                        {
                            string path = (string)dataGridView1[2, i].Value;
                            string dirName = new DirectoryInfo(path).Name;
                            string time = dataGridView1[16, i].Value.ToString();
                            string archivePath = "./ToSend/";
                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                            string destinationpath = archivePath + archivename;
                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                            await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                            dataSet.Tables["Task"].Rows[i].Delete();
                            sqlDataAdapter.Update(dataSet, "Task");
                            ReloadData();
                        }
                    }

                    if (isPeriodic)
                    {
                        if (Time.DayOfWeek == DayOfWeek.Monday)
                        {
                            if (Convert.ToString(dataGridView1[6, i].Value).Contains("Понедельник;"))
                            {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).Date == Time.Date)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }

                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[8, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes /*&& Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes*/ && Convert.ToDateTime(dataGridView1[17, i].Value).Date == Time.Date)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                //dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataGridView1.Rows[i].Cells["MustBeExecuted"].Value);
                                                //sqlDataAdapter.Update(dataSet,"Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = dataGridView1.Rows[i].Cells["MustBeExecuted"].Value;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                        if (Time.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                if (Convert.ToString(dataGridView1[6, i].Value).Contains("Вторник;"))
                                {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay >= Time.TimeOfDay && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay <= Time.AddMinutes(1).TimeOfDay)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная")
                                        {
                                            dataGridView1[12, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes >= Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        // dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[5, i].Value));
                                        dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataGridView1[17, i].Value = dataGridView1[12, i].Value;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                        if (Time.DayOfWeek == DayOfWeek.Wednesday)
                                {
                                    if (Convert.ToString(dataGridView1[6, i].Value).Contains("Среда;"))
                                    {
                                        bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                        if (AOneTimeJob)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay >= Time.TimeOfDay && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay <= Time.AddMinutes(1).TimeOfDay)
                                            {
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }
                                    }
                                        }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes >= Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60);
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                                }
                        if (Time.DayOfWeek == DayOfWeek.Thursday)
                        {
                            if (Convert.ToString(dataGridView1[6, i].Value).Contains("Четверг;"))
                            {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay >= Time.TimeOfDay && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay <= Time.AddMinutes(1).TimeOfDay)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная")
                                        {
                                            dataGridView1[12, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes >= Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        // dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[5, i].Value));
                                        dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataGridView1[17, i].Value = dataGridView1[12, i].Value;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                        if (Time.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (Convert.ToString(dataGridView1[6, i].Value).Contains("Пятница;"))
                            {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay >= Time.TimeOfDay && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay <= Time.AddMinutes(1).TimeOfDay)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная")
                                        {
                                            dataGridView1[12, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date == Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay )
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes >= Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes)
                                            {
                                                dataGridView1[17, i].Value =Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        // dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[5, i].Value));
                                        dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataGridView1[17, i].Value = dataGridView1[12, i].Value;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                        if (Time.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (Convert.ToString(dataGridView1[6, i].Value).Contains("Суббота;"))
                            {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).Date == Time.Date)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        
                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            ReloadData();
                                        }
                                        
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[8, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        //dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        //sqlDataAdapter.Update(dataSet, "Task");
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes /*&& Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes*/ && Convert.ToDateTime(dataGridView1[17, i].Value).Date==Time.Date)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        // dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[5, i].Value));
                                        //dataGridView1[12, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60); 
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                        if (Time.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (Convert.ToString(dataGridView1[6, i].Value).Contains("Воскресенье;"))
                            {
                                bool AOneTimeJob = Convert.ToBoolean(dataGridView1[7, i].Value);
                                if (AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[17, i].Value).Date == Time.Date)
                                    {
                                        string path = (string)dataGridView1[2, i].Value;
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet,"Task");
                                            ReloadData();
                                        }

                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[8, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[8, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataGridView1[12, i].Value).Date <= Time.Date)
                                    {  
                                        if (Convert.ToDateTime(dataGridView1[10, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes == Time.TimeOfDay.Minutes /*&& Convert.ToDateTime(dataGridView1[17, i].Value).TimeOfDay.Minutes <= Time.AddMinutes(1).TimeOfDay.Minutes*/ && Convert.ToDateTime(dataGridView1[17, i].Value).Date == Time.Date)
                                            {
                                                dataGridView1[17, i].Value = Convert.ToString(Convert.ToDateTime(dataGridView1[17, i].Value).AddMinutes(Convert.ToInt32(dataGridView1[9, i].Value) * 60));
                                                dataSet.Tables["Task"].Rows[i][17] = dataGridView1[17, i].Value;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string path = (string)dataGridView1[2, i].Value;
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataGridView1[13, i].Value) && Convert.ToDateTime(dataGridView1[12, i].Value).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    ReloadData();
                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataGridView1[4, i].Value) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataGridView1[11, i].Value).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataGridView1[11, i].Value).AddMinutes(1).TimeOfDay)
                                    {
                                        dataGridView1[17, i].Value = Convert.ToDateTime(dataGridView1[12, i].Value).AddDays(Convert.ToInt32(dataGridView1[5, i].Value) / 24 / 60);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = dataGridView1[17, i].Value;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                    //if (Convert.ToDateTime(dataGridView1[3, i].Value) >= Time && Convert.ToDateTime(dataGridView1[3, i].Value) <= Time.AddMinutes(1))
                    //{
                    //        string path = (string)dataGridView1[2, i].Value;
                    //        string dirName = new DirectoryInfo(path).Name;
                    //        string time = dataGridView1[3, i].Value.ToString();
                    //        string archivePath = "./ToSend/";
                    //        string archivename = dirName +time.Replace(":","-")+ ".zip";
                    //        string destinationpath = archivePath + archivename;
                    //        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal,true);
                    //        await System.Threading.Tasks.Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                    //    if (isPeriodic)
                    //    {
                    //        sqlDataAdapter.Update(dataSet, "Task");
                    //        Time2 = Time2.AddMinutes(Convert.ToDouble(dataGridView1[5, i].Value));
                    //        SqlCommand command = new SqlCommand("UPDATE [Task] SET [LastUploadDate]=@LastUploadDate WHERE [id]=@id", sqlConnection);
                    //        command.Parameters.AddWithValue("id", dataGridView1[0, i].Value);
                    //        command.Parameters.AddWithValue("LastUploadDate", Time2);
                    //        await command.ExecuteNonQueryAsync();
                    //        ReloadData();
                    //        string[] stringTask = new string[6];
                    //        for (int j = 0; j < 6; j++)
                    //        {
                    //            if (j==2)
                    //            {
                    //                stringTask[j] = dataGridView1.Rows[i].Cells[j].Value.ToString() + dataGridView1[3, i].Value.ToString().Replace(":","-") ;
                    //            }
                    //            else
                    //            stringTask[j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    //        }
                    //       await WriteTaskToServer(stringTask);
                    //    }
                    //    else if (!isPeriodic)
                    //    {
                    //        string[] stringTask = new string[6];
                    //        for (int j = 0; j < 6; j++)
                    //        {
                    //            stringTask[j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    //        }
                    //        await _connection.InvokeAsync("DeleteTaskFromDataBase", stringTask, IdUser);
                    //        dataSet.Tables["Task"].Rows[i].Delete();
                    //        sqlDataAdapter.Update(dataSet, "Task");
                    //        ReloadData();
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string get_formatted_time()
        {
            return DateTime.Now.Day.ToString() + "d-"
                + DateTime.Now.Month.ToString() + "m-"
                + DateTime.Now.Year.ToString() + "y "
                + DateTime.Now.Hour.ToString() + "h-"
                + DateTime.Now.Minute.ToString() + "m";

        }

        private void Zip(string directoryPath)
        {
            // путь к архиву
            string archivePath = "./ToSend/";

            // вызов метода, который заархивирует указанную папку
            ZipFile.CreateFromDirectory(directoryPath, archivePath + directoryPath.FirstOrDefault());
        }

        public async
        System.Threading.Tasks.Task
        WriteTaskToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteTaskToDataBase", Data, IdUser);
        }

       

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FileStorage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

