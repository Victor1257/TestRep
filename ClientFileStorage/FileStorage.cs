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
        public HubConnection _connection;
        private ListViewColumnSorter lvwColumnSorter;
        public ListViewItem listView;
        public string price;
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null, FileAdapter = null, SYBDAdapter = null;
        private DataSet dataSet = null;
        private DataGridView detailsDataGridView = new DataGridView();
        private BindingSource masterBindingSource = new BindingSource();
        private BindingSource detailsBindingSource = new BindingSource();

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
                    dataGridView1[19, i] = linkCell;
                }
                Check();
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
                    dataGridView1[19, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            this.taskTableAdapter.Fill(this.modelDataSet.Task);

            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\meschaninov\Desktop\TestRep - копия\ClientFileStorage\Database1.mdf; Integrated Security = True");
            //    sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
            sqlConnection.Open();
            listView1.View = View.Details;
            Console.WriteLine("State: {0}", sqlConnection.State);
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
                await _connection.InvokeAsync("SendMovie", IdUser);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["StartsAt"]).TimeOfDay <= Time.TimeOfDay && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["EndsIn"]).TimeOfDay >= Time.TimeOfDay)
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
                    await _connection.InvokeAsync("WriteToDataBase", fileName.Remove(fileName.Length - 7, 3), IdUser);

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
                            await _connection.InvokeAsync("DeleteTaskFromDataBase", stringTask, IdUser);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            sqlDataAdapter.Update(dataSet, "Task");
            MakeTasks makeTasks = new MakeTasks(Link, IdUser);
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

        private void Zip(string directoryPath)
        {
            // путь к архиву
            string archivePath = "./ToSend/";

            // вызов метода, который заархивирует указанную папку
            ZipFile.CreateFromDirectory(directoryPath, archivePath + directoryPath.FirstOrDefault());
        }

        public async System.Threading.Tasks.Task WriteTaskToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteTaskToDataBase", Data, IdUser);
        }
        public async System.Threading.Tasks.Task WriteFileToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteFileToDataBase", Data, IdUser);
        }

        private void FileStorage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            sqlConnection.Close();
        }



        //private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["IdUser"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["IdUser"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["FileName"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["FileName"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["IsPeriodic"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["IsPeriodic"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["Frequency_InProgress"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Frequency_InProgress"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["Frequency_RepeatedEvery"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Frequency_RepeatedEvery"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["DayOfTheWeek"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["DayOfTheWeek"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["AOneTimeJob"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["AOneTimeJob"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["AOneTimeJobValue"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["AOneTimeJobValue"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["RunsEvery"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["RunsEvery"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["StartsAt"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["StartsAt"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["EndsIn"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["EndsIn"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["StartDate"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["StartDate"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["EndDate"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["EndDate"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["EndDateValue"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["EndDateValue"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["DateNoPeriodic"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["DateNoPeriodic"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["TimeNoPeriodic"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["TimeNoPeriodic"].Value);
        //    dataSet.Tables["Task"].Rows[e.RowIndex]["MustBeExecuted"] = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["MustBeExecuted"].Value);
        //    sqlDataAdapter.Update(dataSet, "Task");
        //}
    }
}

