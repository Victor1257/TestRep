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
    public class MakeTasks
    {
        public string Link;
        public string IdUser;
        public HubConnection _connection;
        public ListViewItem listView;
        public string price;
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        public string con1;
        public string The_Supplier, Adres_Server, Port_Server, Instance_Server, Login_SYBD, Password_SYBD, Way, Name_SYBD;
        public bool Integrated_Security;
        public MakeTasks(string Link1, string IdUser1)
        {
            Link = Link1;
            IdUser = IdUser1;
        }

        public async void SetCon()
        {
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(Link)
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .Build();
                _connection.ServerTimeout = TimeSpan.FromMinutes(60);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                await _connection.StartAsync();
                Console.WriteLine(_connection.State);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return;
            }
        }

        public async void MakeTask()
        {
            try
            {
                SetCon();
                Console.WriteLine(_connection.State);
                // sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
                sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\meschaninov\Desktop\TestRep - копия\ClientFileStorage\Database1.mdf; Integrated Security = True");
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter("SELECT *,'Delete' AS [Delete] FROM Task", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();
                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Task");
                Загрузитьфайл загрузитьфайл = new Загрузитьфайл(Link, IdUser);

                var Time = DateTime.Now;
                for (int i = 0; i < dataSet.Tables["Task"].Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i]["IsFile"]))
                    {
                        int id1 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                        string sql = "SELECT [FileName] FROM [File] " + "WHERE Idfile = @IdFile";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdFile", id1);
                        string a = command1.ExecuteScalar().ToString();
                        bool isPeriodic = Convert.ToBoolean(Convert.ToInt32(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]));
                        if (!isPeriodic)
                        {
                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][15]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes >= Time.TimeOfDay.TotalMinutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes <= Time.AddMinutes(1).TimeOfDay.TotalMinutes)
                            {
                                string path = a;
                                string dirName = new DirectoryInfo(path).Name;
                                string time = dataSet.Tables["Task"].Rows[i][16].ToString();
                                string archivePath = "./ToSend/";
                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                string destinationpath = archivePath + archivename;
                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                dataSet.Tables["Task"].Rows[i].Delete();
                                sqlDataAdapter.Update(dataSet, "Task");

                            }
                        }

                        if (isPeriodic)
                        {
                            if (Time.DayOfWeek == DayOfWeek.Monday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Понедельник;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Вторник;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Среда;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Thursday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Четверг;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Пятница;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Суббота;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Sunday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Воскресенье;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [File] " + "WHERE IdFile = @IdFile";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdFile", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[18];
                                            for (int j = 0; j < 18; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                            string path = a;
                                            string dirName = new DirectoryInfo(path).Name;
                                            string time = DateTime.Now.ToString();
                                            string archivePath = "./ToSend/";
                                            string archivename = dirName + time.Replace(":", "-") + ".zip";
                                            string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                            ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        int id1 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                        string sql = "SELECT [The_Supplier],[Adres_Server],[Port_Server],[Instance_Server],[Login_SYBD],[Password_SYBD],[Way],[Name_SYBD],[Integrated_Security] FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                        SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                        command1.Parameters.AddWithValue("@IdSYBD", id1);
                        SqlDataReader reader = command1.ExecuteReader();
                        while (reader.Read())
                        {
                            The_Supplier = (string)reader[0];
                            Adres_Server = (string)reader[1];
                            if (reader[2].ToString() != null)
                            {
                                Port_Server = (string)reader[2];
                            }
                            if (reader[3].ToString() != null)
                            {
                                Instance_Server = (string)reader[3];
                            }
                            if (reader[4].ToString() != null && reader[5].ToString() != null)
                            {
                                Login_SYBD = (string)reader[4];
                                Password_SYBD = (string)reader[5];
                            }
                            Way = (string)reader[6];
                            Name_SYBD = (string)reader[7];
                            Integrated_Security = (bool)reader[8];
                        }
                        reader.Close();


                        bool isPeriodic = Convert.ToBoolean(Convert.ToInt32(dataSet.Tables["Task"].Rows[i]["IsPeriodic"]));
                        if (!isPeriodic)
                        {
                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][15]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes >= Time.TimeOfDay.TotalMinutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes <= Time.AddMinutes(1).TimeOfDay.TotalMinutes)
                            {
                                con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                sqlConnection1.Open();
                                SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                command2.ExecuteNonQuery();
                                CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                command3.ExecuteNonQuery();
                                dataSet.Tables["Task"].Rows[i].Delete();
                                sqlDataAdapter.Update(dataSet, "Task");
                                sqlConnection1.Close();
                            }
                        }

                        if (isPeriodic)
                        {
                            if (Time.DayOfWeek == DayOfWeek.Monday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Понедельник;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);    
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Вторник;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Среда;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Thursday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Четверг;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Пятница;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Суббота;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Time.DayOfWeek == DayOfWeek.Sunday)
                            {
                                if (Convert.ToString(dataSet.Tables["Task"].Rows[i][6]).Contains("Воскресенье;"))
                                {
                                    bool AOneTimeJob = Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][7]);
                                    if (AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, NOINIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                        }
                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                            string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                            SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                            command3.Parameters.AddWithValue("@IdSYBD", id2);
                                            command3.ExecuteNonQuery();
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                        {
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToString(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).Replace(Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["MustBeExecuted"]).TimeOfDay.ToString(), (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i]["AOneTimeJobValue"]).TimeOfDay.ToString()));
                                            sqlDataAdapter.Update(dataSet, "Task");
                                        }
                                    }
                                    if (!AOneTimeJob)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Hour == Time.Hour && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Minute == Time.Minute)
                                        {
                                            con1 = ConSYBD(Integrated_Security, Adres_Server, Instance_Server, Name_SYBD, Login_SYBD, Password_SYBD, Time, Port_Server);
                                            dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                            sqlDataAdapter.Update(dataSet, "Task");
                                            string[] stringTask = new string[19];
                                            for (int j = 0; j < 19; j++)
                                            {
                                                stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                            }
                                            await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);

                                            SqlConnection sqlConnection1 = new SqlConnection(@con1);
                                            sqlConnection1.Open();
                                            SqlCommand command2 = new SqlCommand("BACKUP DATABASE[" + Name_SYBD + "] TO  DISK = N'" + Way + "\\" + Name_SYBD + ".bak" + "' WITH NOFORMAT, INIT, NAME = N'" + Name_SYBD + "-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD, STATS = 10", sqlConnection1);
                                            command2.CommandTimeout = 10000;
                                            command2.ExecuteNonQuery();
                                            CreatecompressSYBD(Adres_Server, Way, Name_SYBD);
                                            if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                            {
                                                int id2 = (int)dataSet.Tables["Task"].Rows[i]["Id"];
                                                string sql1 = "DELETE FROM [SYBD] " + "WHERE IdSYBD = @IdSYBD";
                                                SqlCommand command3 = new SqlCommand(sql1, sqlConnection);
                                                command3.Parameters.AddWithValue("@IdSYBD", id2);
                                                command3.ExecuteNonQuery();
                                                dataSet.Tables["Task"].Rows[i].Delete();
                                                sqlDataAdapter.Update(dataSet, "Task");

                                            }
                                        }

                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }

                                        }
                                        if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "еженедельная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                        {
                                            var m = EndDay(Convert.ToString(dataSet.Tables["Task"].Rows[i][6]));
                                            if (m == DateTime.Now.DayOfWeek)
                                            {
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                                s1 = s1.Remove(s1.Length - 3, 3);
                                                string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                                string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                                s3 = s3.Remove(s3.Length - 3, 3);
                                                s2 = s2.Replace(s3, s1);
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[19];
                                                for (int j = 0; j < 19; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _connection.StopAsync();
                sqlConnection.Close();
            }
        }
        public async System.Threading.Tasks.Task WriteTaskToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteTaskToDataBase", Data, IdUser);
        }

        public static void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }
        public DayOfWeek EndDay(string a)
        {
            string[] textMass;
            string text = a;
            textMass = text.Split(';');
            string[] max = new string[textMass.Count()];
            int[] b = new int[textMass.Count()];
            for (int i = 0; i < textMass.Count(); i++)
            {
                if (textMass[i] == "Понедельник")
                {
                    max[i] = "Понедельник";
                    b[i] = 1;
                }
                if (textMass[i] == "Вторник")
                {
                    max[i] = "Вторник";
                    b[i] = 2;
                }
                if (textMass[i] == "Среда")
                {
                    max[i] = "Среда";
                    b[i] = 3;
                }
                if (textMass[i] == "Четверг")
                {
                    max[i] = "Четверг";
                    b[i] = 4;
                }
                if (textMass[i] == "Пятница")
                {
                    max[i] = "Пятница";
                    b[i] = 5;
                }
                if (textMass[i] == "Суббота")
                {
                    max[i] = "Суббота";
                    b[i] = 6;
                }
                if (textMass[i] == "Воскресенье")
                {
                    max[i] = "Воскресенье";
                    b[i] = 7;
                }
            }
            int c = b.Max<int>();
            DayOfWeek dayOfWeek = (DayOfWeek)c;
            return dayOfWeek;
        }

        public async void CreatecompressSYBD(string Adres_Server, string Way, string Name_SYBD)
        {
            Загрузитьфайл загрузитьфайл = new Загрузитьфайл(Link, IdUser);
            string path = @"\\" + Adres_Server + Way.Remove(0, 2) + "\\" + Name_SYBD + ".bak";
            string dirName = new DirectoryInfo(path).Name;
            string time = DateTime.Now.ToString();
            string archivePath = "./ToSend/";
            string archivename = dirName.Remove(dirName.Length - 4, 4) + time.Replace(":", "-") + ".bak";
            string destinationpath = archivePath + archivename + ".gz";
            Compress(path, destinationpath);
            await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
        }
        public string ConSYBD(bool Integrated_Security, string Adres_Server, string Instance_Server, string Name_SYBD, string Login_SYBD, string Password_SYBD, DateTime Time, string Port_Server)
        {
            string con2;
            if (Port_Server == "")
            {
                if (Integrated_Security)
                {
                    con2 = "Data Source = " + Adres_Server + "\\" + Instance_Server + ";Initial Catalog=" + Name_SYBD + "; Integrated Security = True";
                }
                else
                {
                    con2 = "Data Source = " + Adres_Server + "\\" + Instance_Server + ";Initial Catalog=" + Name_SYBD + "; User ID=" + Login_SYBD + ";Password=" + Password_SYBD;
                }
            }
            else
            {
                if (Integrated_Security)
                {
                    con2 = "Data Source = " + Adres_Server + "," + Port_Server + ";Initial Catalog=" + Name_SYBD + "; Integrated Security = True";
                }
                else
                {
                    con2 = "Data Source = " + Adres_Server + "," + Port_Server + ";Initial Catalog=" + Name_SYBD + "; User ID=" + Login_SYBD + ";Password=" + Password_SYBD;
                }
            }
            return con2;
        }
    }
}
