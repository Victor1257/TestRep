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
                    dataGridView1[6, i] = linkCell;
                }    
                
            }
            catch(Exception ex)
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
                    dataGridView1[6, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void connect_to_sql_database(string dbfilename)
        {
            //строка для удобства отладки: чтобы если поменял в условии, то не надо было менять в else название БД
            if (!System.IO.File.Exists(Application.StartupPath + @"\" + dbfilename))
            {
                MessageBox.Show("Подключение невозможно");
                Application.Exit();
            }
            else
            {

                /*
                 * sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + @"\Database1.mdf" + ";Integrated Security=True");
                    sqlConnection.Open();
                 */
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + @"\" + dbfilename + ";Integrated Security=True");
                sqlConnection.Open();
            }
        }



        private async void Form2_Load(object sender, EventArgs e)
        {

            //sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\merli\Source\Repos\TestRep\ClientFileStorage\Database1.mdf;Integrated Security=True;"+ "MultipleActiveResultSets=True");
            /*sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + @"\Database1.mdf" + ";Integrated Security=True");
            sqlConnection.Open();*/

            connect_to_sql_database("Database1.mdf");

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
            _connection.On<byte[], string, long,long,long>("doStuff", (s1, s2,s3,s4,s5) => DoStuff(s1, s2,s3,s4,s5));
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
     
       

        private  void DoStuff(byte[] data, string name, long Position,long Lenght,long READBUFFER_SIZE)
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
                if (progressBar1.Value >= (int)Lenght-1048576)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("Файл загружен!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            //try
            //{
            //    File.WriteAllBytes("./Uploads/" + name, data);
            //    GC.Collect();
            //    MessageBox.Show("Файл успешно заружен");
            //}

            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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
                await _connection.InvokeAsync("GetMovie", price,IdUser);
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
                await _connection.InvokeAsync("DeleteFile", price,IdUser);
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
                            //await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, n,IdUser);
                        }
                        else
                        {
                            byte[] FSBuffer = new byte[READBUFFER_SIZE];
                            FS.Read(FSBuffer, offset, (int)READBUFFER_SIZE);
                            //await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, READBUFFER_SIZE,IdUser);
                            n -= READBUFFER_SIZE;
                        }
                        Pos = FS.Position;
                        GC.Collect();
                    }
                    //await _connection.InvokeAsync("WriteToDataBase", fileName, IdUser);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void создатьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 newForm = new Form1(Link, IdUser);
            newForm.SetCP(this);
            newForm.Show();
        }

        public void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex==6)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить задачу?","Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
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
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                bool isPeriodic = Convert.ToBoolean(Convert.ToInt32(dataGridView1[4, i].Value));

                //MessageBox.Show( Convert.ToString(isPeriodic) );

                if (isPeriodic)
                {
                    await Task.Run(() => MakeTask(true));
                }
                else
                {
                    await Task.Run(() => MakeTask(false));
                }
            }
        }

        /*private async void MakeTaskNoPeriod()
        {
            var Time = DateTime.Now;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {

                if (Convert.ToDateTime(dataGridView1[3, i].Value) >= Time && Convert.ToDateTime(dataGridView1[3, i].Value) <= Time.AddMinutes(1) && Convert.ToInt32(dataGridView1[4, i].Value) == 0)
                {
                    string path = (string)dataGridView1[2, i].Value;
                    string dirName = new DirectoryInfo(path).Name;
                    string time = get_formatted_time();
                    string archivePath = "./ToSend/";
                    string archivename = dirName + time + ".zip";

                    //string destinationpath = @"./filesfolder/archive " + time + ".zip";
                    string destinationpath = archivePath + archivename;
                    ZipFile.CreateFromDirectory(path, destinationpath);
                    if (System.IO.File.Exists(destinationpath))
                    { MessageBox.Show("Архив успешно создан"); }
                    //ZipFile.CreateFromDirectory(path, archivePath + dirName + ".zip");
                   await Task.Run(()=> загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                    dataSet.Tables["Task"].Rows[i].Delete();
                    sqlDataAdapter.Update(dataSet, "Task");
                    ReloadData();
                }
            }
        }*/

        private async void MakeTask(bool isPeriodic)
        {
            var Time = DateTime.Now;
            
            for (int i = 0; i < dataGridView1.Rows.Count && dataGridView1[0, i].Value != null; i++)
            {
                var Time2 = Convert.ToDateTime(dataGridView1[3, i].Value);
                //if (Convert.ToDateTime(dataGridView1[3, i].Value) >= Time && Convert.ToDateTime(dataGridView1[3, i].Value) <= Time.AddMinutes(1) && Convert.ToInt32(dataGridView1[4, i].Value) == 1)
                if (Convert.ToDateTime(dataGridView1[3, i].Value)/*.AddMinutes( Convert.ToInt32( dataGridView1[5, i].Value ) )*/ <= Time)
                {
                    string path = (string)dataGridView1[2, i].Value;
                    string dirName = new DirectoryInfo(path).Name;
                    string time = get_formatted_time();
                    string archivePath = "./ToSend/";
                    string archivename = dirName + time + ".zip";

                    //string destinationpath = @"./filesfolder/archive " + time + ".zip";
                    string destinationpath = archivePath + archivename;
                    ZipFile.CreateFromDirectory(path, destinationpath);
                    if (System.IO.File.Exists(destinationpath))
                    { MessageBox.Show("Архив успешно создан"); }
                    //ZipFile.CreateFromDirectory(path, archivePath + dirName + ".zip");
                    await Task.Run(() => загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                    if (isPeriodic)
                        {
                            sqlDataAdapter.Update(dataSet, "Task");
                        //var Time343 = Time2;
                            //MessageBox.Show("*Time2 " + Convert.ToString(Time2) );
                            Time2 = Time2.AddMinutes( Convert.ToDouble (dataGridView1[5, i].Value) );
                            //MessageBox.Show(Convert.ToString(Time343) + "\n" + Convert.ToString(Time2) );
                            SqlCommand command = new SqlCommand("UPDATE [Task] SET [LastUploadDate]=@LastUploadDate WHERE [id]=@id", sqlConnection);
                            command.Parameters.AddWithValue("id", Convert.ToInt32( dataGridView1[0, i].Value) );
                            command.Parameters.AddWithValue("LastUploadDate", Time2);
                            await command.ExecuteNonQueryAsync();
                        }
                    else if (!isPeriodic)
                        {
                            dataSet.Tables["Task"].Rows[i].Delete();
                            sqlDataAdapter.Update(dataSet, "Task");
                            ReloadData();
                        }
                }
            
            }
        }
            private string get_formatted_time()
        {
            return DateTime.Now.Day.ToString() + "d-"
                + DateTime.Now.Month.ToString() + "m-"
                + DateTime.Now.Year.ToString() + "y "
                + DateTime.Now.Hour.ToString() + "h-"
                + DateTime.Now.Minute.ToString() + "m-"
                + DateTime.Now.Second.ToString() + "s";
        }

        private void Zip(string directoryPath)
        {
            // путь к архиву
            string archivePath = "./ToSend/";

            // вызов метода, который заархивирует указанную папку
            ZipFile.CreateFromDirectory(directoryPath, archivePath+directoryPath.FirstOrDefault());
        }
    }
}

