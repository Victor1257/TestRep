using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Web.Script.Serialization;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Drawing;
<<<<<<< HEAD
using System.Net.Http;
using System.Text.Json;
using System.Text;
=======
>>>>>>> 0059e1e51aec3b06275ed2f338eab13cd0b8bd6c

namespace ClientFileStorage
{
    public partial class FileStorage : Form
    {
        public string Link;
        public string IdUser;
        private ListViewColumnSorter lvwColumnSorter;
        public ListViewItem listView;
        public string price;
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        public HttpClient client;
        public FileStorage(string LINK, string IDUser, HttpClient httpClient)
        {
            InitializeComponent();
<<<<<<< HEAD
            client = httpClient;
=======
            try
            {
                //_connection = new HubConnectionBuilder()
                //    .WithUrl(LINK)
                //    .AddMessagePackProtocol()
                //    .WithAutomaticReconnect()
                //    .Build();
                //_connection.KeepAliveInterval = TimeSpan.FromSeconds(1);
                //_connection.ServerTimeout = TimeSpan.FromMinutes(10);
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                return;
            }
>>>>>>> 0059e1e51aec3b06275ed2f338eab13cd0b8bd6c
            Opacity = 0;
            Timer timer = new Timer();
            timer.Tick += new EventHandler((sender, e) =>
            {
                if ((Opacity += 0.3d) == 1) timer.Stop();
            });
            timer.Interval = 100;
            timer.Start();
            this.textBox1.Text = LINK;
            Link = LINK;
            IdUser = IDUser;
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            tabPage1.Text = "Файлы";
            tabPage2.Text = "Задачи";
        }

        public DataGridView GetDataGridView()
        {
            return dataGridView1;
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
            try
            {
                listView1.Items.Clear();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                List<Movie> movies = (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
                ImageList imageList = new ImageList();
                Image imageData = Image.FromFile("C:/Users/meschaninov/Desktop/3/ClientFileStorage/Image/DataBase.jpg");
                Image imageFile = Image.FromFile("C:/Users/meschaninov/Desktop/3/ClientFileStorage/Image/File.png");
                imageList.Images.Add(imageData);
                imageList.Images.Add(imageFile);
                listView1.SmallImageList = imageList;
                int i = 0;
                foreach (Movie mo in movies)
                {
                    listView = new ListViewItem(mo.IDUser);
                    listView.SubItems.Add(mo.Title);
                    listView.SubItems.Add(mo.ReleaseDate.ToString());
                    listView.SubItems.Add(mo.Name);
                    listView1.Items.Add(listView);
                    if (Path.GetExtension(mo.Name.ToString()) == ".gz")
                    {
                        listView1.Items[i].ImageIndex = 0;
                    }
                    else
                        listView1.Items[i].ImageIndex = 1;
                    i++;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void OnSendAll(string movie1)
        {
            listView1.Items.Clear();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<Movie> movies = (List<Movie>)json_serializer.Deserialize(movie1, typeof(List<Movie>));
            ImageList imageList = new ImageList();
            Image imageData = Image.FromFile("C:/Users/meschaninov/Desktop/3/ClientFileStorage/Image/DataBase.jpg");
            Image imageFile = Image.FromFile("C:/Users/meschaninov/Desktop/3/ClientFileStorage/Image/File.png");
            imageList.Images.Add(imageData);
            imageList.Images.Add(imageFile);
            listView1.SmallImageList = imageList;
            int i = 0;
            foreach (Movie mo in movies)
            {
                listView = new ListViewItem(mo.IDUser);
                listView.SubItems.Add(mo.Title);
                listView.SubItems.Add(mo.ReleaseDate.ToString());
                listView.SubItems.Add(mo.Name);
                listView1.Items.Add(listView);
                if (Path.GetExtension(mo.Name.ToString()) == ".gz")
                {
                    listView1.Items[i].ImageIndex = 0;
                }
                else
                    listView1.Items[i].ImageIndex = 1;
                i++;
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
                    dataGridView1[19, i] = linkCell;
                }
                //Check();
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
                    DataGridViewLinkCell linkCell1 = new DataGridViewLinkCell();
                    dataGridView1[19, i] = linkCell;
                    dataGridView1[0, i] = linkCell1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private  void Form2_Load(object sender, EventArgs e)
        {

           // this.taskTableAdapter.Fill(this.modelDataSet.Task);
<<<<<<< HEAD
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) + "\\Database1.mdf; Integrated Security = True");
           // sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
          //  sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\meschaninov\Desktop\3\ClientFileStorage\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();
            listView1.View = View.Details;
            Console.WriteLine("State: {0}", sqlConnection.State);
          
=======
           // sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) + "\\Database1.mdf; Integrated Security = True");
           // sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\meschaninov\Desktop\3\ClientFileStorage\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();
            listView1.View = View.Details;
            Console.WriteLine("State: {0}", sqlConnection.State);
            _connection.On<string>("Receive", (s1) => OnSend(s1));
            _connection.On<byte[], string, long, long, long>("doStuff", (s1, s2, s3, s4, s5) => DoStuff(s1, s2, s3, s4, s5));
            _connection.On<string>("FileDelete", (s1) => DeleteFile(s1));
            _connection.On<string>("ReceiveAll", (s1) => OnSendAll(s1));
>>>>>>> 0059e1e51aec3b06275ed2f338eab13cd0b8bd6c

            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void получитьИлиОбновитьСписокФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync("api/user/getlist");
                responseMessage.EnsureSuccessStatusCode();
                var emp = await responseMessage.Content.ReadAsStringAsync();
                OnSend(emp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

<<<<<<< HEAD
        //private async void Check()
        //{
        //    for (int i = 0; i < dataSet.Tables["Task"].Rows.Count; i++)
        //    {
        //        DateTime Time = DateTime.Now;
        //        if (Time.DayOfWeek == DayOfWeek.Monday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Понедельник;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Понедельник;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Tuesday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Вторник;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Вторник;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Wednesday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Среда;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Среда;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Thursday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Четверг;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Четверг;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Friday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Пятница;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Пятница;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Saturday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Суббота;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Суббота;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Time.DayOfWeek == DayOfWeek.Sunday)
        //        {
        //            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
        //            {
        //                if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Воскресенье;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
        //                            {
        //                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
        //                                {
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
        //                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                    {
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
        //                                        while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
        //                                        {
        //                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
        //                                            sqlDataAdapter.Update(dataSet, "Task");
        //                                            string[] stringTask = new string[19];
        //                                            for (int j = 0; j < 19; j++)
        //                                            {
        //                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
        //                                            }
        //                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Воскресенье;"))
        //                    {
        //                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
        //                        {
        //                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
        //                            {
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
        //                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
        //                                sqlDataAdapter.Update(dataSet, "Task");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    sqlDataAdapter.Update(dataSet, "Task");
        //}
=======
        private async void Check()
        {
            for (int i = 0; i < dataSet.Tables["Task"].Rows.Count; i++)
            {
                DateTime Time = DateTime.Now;
                if (Time.DayOfWeek == DayOfWeek.Monday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Понедельник;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Понедельник;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Tuesday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Вторник;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Вторник;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Wednesday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Среда;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Среда;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Четверг;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Четверг;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Friday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Пятница;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Пятница;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Суббота;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Суббота;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
                if (Time.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]))
                    {
                        if (!Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["AOneTimeJob"]))
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Воскресенье;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay != Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date != Time.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString("dd.MM.yyyy"), Time.Date.ToString("dd.MM.yyyy"));
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay.ToString());
                                                while (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay <= Time.TimeOfDay)
                                                {
                                                    dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                    sqlDataAdapter.Update(dataSet, "Task");
                                                    string[] stringTask = new string[19];
                                                    for (int j = 0; j < 19; j++)
                                                    {
                                                        stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                    }
                                                    await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToString(dataSet.Tables["Task"].Rows[i]["DayOfTheWeek"]).Contains("Воскресенье;"))
                            {
                                if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartDate"]).Date <= Time.Date)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay > Time.TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Date.ToString(), Time.Date.ToString());
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString());
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            sqlDataAdapter.Update(dataSet, "Task");
        }
>>>>>>> 0059e1e51aec3b06275ed2f338eab13cd0b8bd6c
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
                //progressBar1.Minimum = 0;
                //progressBar1.Maximum = (int)Lenght;
                //progressBar1.Value = (int)Position;
                //if (progressBar1.Value >= (int)Lenght - 1048576)
                //{
                //    progressBar1.Value = 0;
                //    MessageBox.Show("Файл загружен!");
                //}

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
                byte[] READBUFFER_SIZE = new byte[1024];
                using (var httpResponseMessage = await client.GetStreamAsync("api/user7/Download"))
                {

                    httpResponseMessage.Read(READBUFFER_SIZE, 0, 1024);
                    File.WriteAllBytes("./Uploads/5345.txt", READBUFFER_SIZE);
                    using (FileStream FS = new FileStream("./Uploads/5345.txt", FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        long n = FS.Length;
                        httpResponseMessage.CopyTo(FS);
                        FS.Write(READBUFFER_SIZE, 0, 1024);
                    }
                }
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
                    HttpResponseMessage responseMessage = await client.DeleteAsync("api/user/" + price);
                    responseMessage.EnsureSuccessStatusCode();
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
                HttpResponseMessage responseMessage = await client.GetAsync("api/user1/GetAllList");
                responseMessage.EnsureSuccessStatusCode();
                var emp = await responseMessage.Content.ReadAsStringAsync();
                OnSendAll(emp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void переподключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2_Load(sender, e);
        }

        //private async void загрузитьФайлToolStripMenuItem_Click1(string FileNamePath)
        //{
        //    var fileName = new DirectoryInfo(FileNamePath).Name;
        //    long READBUFFER_SIZE = 1048576;
        //    int offset = 0;
        //    long Pos = 0;
        //    try
        //    {
        //        using (FileStream FS = new FileStream(FileNamePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        {
        //            long n = FS.Length;
        //            while (FS.Position < FS.Length)
        //            {
        //                if (n <= READBUFFER_SIZE)
        //                {
        //                    byte[] FSBuffer = new byte[n];
        //                    FS.Read(FSBuffer, offset, (int)n);
        //                    await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, n, IdUser);
        //                }
        //                else
        //                {
        //                    byte[] FSBuffer = new byte[READBUFFER_SIZE];
        //                    FS.Read(FSBuffer, offset, (int)READBUFFER_SIZE);
        //                    await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, READBUFFER_SIZE, IdUser);
        //                    n -= READBUFFER_SIZE;
        //                }
        //                Pos = FS.Position;
        //                GC.Collect();
        //            }
        //            await _connection.InvokeAsync("WriteToDataBase", fileName.Remove(fileName.Length - 7, 3), IdUser);

        //        }
        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}


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
                if (e.ColumnIndex == 19)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[19].Value.ToString();
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить задачу?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            string[] stringTask = new string[19];
                            for (int i = 0; i < 19; i++)
                            {
                                stringTask[i] = dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString();
                            }
                            HttpResponseMessage responseMessage = await client.DeleteAsync("api/user2/DeleteTaskFromDataBase?id="+stringTask[0]+"&IsFile="+stringTask[2]);
                            responseMessage.EnsureSuccessStatusCode();
                            int rowIndex = e.RowIndex;
                            int id1 = (int)dataSet.Tables["Task"].Rows[rowIndex]["Id"];
                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[rowIndex]["IsFile"]) == true)
                            {
                                string sql = "DELETE FROM [File] " + "WHERE Idfile = @IdFile";
                                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                                command1.Parameters.AddWithValue("@IdFile", id1);
                                command1.ExecuteNonQuery();
                            }
                            else
                            {
                                string sql = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                                command1.Parameters.AddWithValue("@IdSYBD", id1);
                                command1.ExecuteNonQuery();
                            }
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["Task"].Rows[rowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "Task");
                        }
                    }
                }
                if (e.ColumnIndex == 0)
                {
                    List<string> list = new List<string>();
                    string task = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    SqlDataReader sqlReader = null;
                    string b = "";
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[e.RowIndex]["IsFile"]) == true)
                    {
                        string sql = "SELECT * FROM [File] " + "WHERE Idfile = @IdFile";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdFile", Convert.ToInt32(task));
                        sqlReader = await command1.ExecuteReaderAsync();

                        while (await sqlReader.ReadAsync())
                        {
                            b = Convert.ToString(sqlReader["FileName"]);
                        }
                        sqlReader.Close();

                        MessageBox.Show(b, "Файл", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string sql = "SELECT * FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdSYBD", Convert.ToInt32(task));
                        sqlReader = await command1.ExecuteReaderAsync();
                        while (await sqlReader.ReadAsync())
                        {
                            b = "Поставщик СУБД: " + Convert.ToString(sqlReader["The_Supplier"]) + "\r\n";
                            b = b + "Адрес сервера: " + Convert.ToString(sqlReader["Adres_Server"]) + "\r\n";
                            b = b + "Порт сервера: " + Convert.ToString(sqlReader["Port_Server"]) + "\r\n";
                            b = b + "Экземпляр сервера: " + Convert.ToString(sqlReader["Instance_Server"]) + "\r\n";
                            b = b + "Логин СУБД: " + Convert.ToString(sqlReader["Login_SYBD"]) + "\r\n";
                            b = b + "Пароль СУБД: " + Convert.ToString(sqlReader["Password_SYBD"]) + "\r\n";
                            b = b + "Путь копирования: " + Convert.ToString(sqlReader["Way"]) + "\r\n";
                            b = b + "Имя СУБД: " + Convert.ToString(sqlReader["Name_SYBD"]) + "\r\n";
                            b = b + "Integrated_Security: " + Convert.ToString(sqlReader["Integrated_Security"]) + "\r\n";
                        }
                        sqlReader.Close();

                        MessageBox.Show(b, "СУБД", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (e.ColumnIndex == 0)
                {
                    List<string> list = new List<string>();
                    string task = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    SqlDataReader sqlReader = null;
                    string b="";
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[e.RowIndex]["IsFile"]) == true)
                    {
                        string sql = "SELECT * FROM [File] " + "WHERE Idfile = @IdFile";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdFile", Convert.ToInt32(task));
                        sqlReader = await command1.ExecuteReaderAsync();

                        while (await sqlReader.ReadAsync())
                        {
                            b = Convert.ToString(sqlReader["FileName"]);
                        }
                        sqlReader.Close();

                        MessageBox.Show(b, "Файл", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string sql = "SELECT * FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdSYBD", Convert.ToInt32(task));
                        sqlReader = await command1.ExecuteReaderAsync();
                        while (await sqlReader.ReadAsync())
                        {
                            b="Поставщик СУБД: "+ Convert.ToString(sqlReader["The_Supplier"])+ "\r\n";
                            b =b+ "Адрес сервера: " + Convert.ToString(sqlReader["Adres_Server"])+ "\r\n";
                            b =b + "Порт сервера: " + Convert.ToString(sqlReader["Port_Server"])+ "\r\n";
                            b = b + "Экземпляр сервера: " + Convert.ToString(sqlReader["Instance_Server"])+ "\r\n";
                            b = b + "Логин СУБД: " + Convert.ToString(sqlReader["Login_SYBD"])+ "\r\n";
                            b = b + "Пароль СУБД: " + Convert.ToString(sqlReader["Password_SYBD"])+ "\r\n";
                            b = b + "Путь копирования: " + Convert.ToString(sqlReader["Way"])+ "\r\n";
                            b = b + "Имя СУБД: " + Convert.ToString(sqlReader["Name_SYBD"])+ "\r\n";
                            b = b + "Integrated_Security: " + Convert.ToString(sqlReader["Integrated_Security"])+ "\r\n";
                        }
                        sqlReader.Close();

                        MessageBox.Show(b,"СУБД",MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            sqlDataAdapter.Update(dataSet, "Task");
            MakeTasks makeTasks = new MakeTasks(Link, IdUser,client);
            await System.Threading.Tasks.Task.Run(() => makeTasks.MakeTask());
            ReloadData();
        }


        private string get_formatted_time()
        {
            return DateTime.Now.Day.ToString() + "d-"
                + DateTime.Now.Month.ToString() + "m-"
                + DateTime.Now.Year.ToString() + "y "
                + DateTime.Now.Hour.ToString() + "h-"
                + DateTime.Now.Minute.ToString() + "m";

        }



        public async System.Threading.Tasks.Task WriteTaskToServer(string[] Data)
        {
            var js = JsonSerializer.Serialize(Data);
            HttpContent c = new StringContent(js, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.PostAsync("api/user3/WriteTaskToDataBase", c);
            responseMessage.EnsureSuccessStatusCode();
        }
        public async System.Threading.Tasks.Task WriteFileToServer(string[] Data)
        {
            var js = JsonSerializer.Serialize(Data);
            HttpContent c = new StringContent(js, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.PostAsync("api/user5/WriteFileToDataBase", c);
            responseMessage.EnsureSuccessStatusCode();
        }
        public async System.Threading.Tasks.Task WriteSYBDToServer(string[] Data)
        {
            var js = JsonSerializer.Serialize(Data);
            HttpContent c = new StringContent(js, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.PostAsync("api/user4/WriteSYBDToDataBase", c);
            responseMessage.EnsureSuccessStatusCode();
        }
        public async System.Threading.Tasks.Task WriteSYBDToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteSYBDToDataBase", Data, IdUser);
        }

        private void FileStorage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            sqlConnection.Close();
        }



    }
}

