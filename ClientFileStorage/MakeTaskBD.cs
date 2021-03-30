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
    public class MakeTasksBD
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

        public MakeTasksBD(string Link1, string IdUser1)
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
                _connection.ServerTimeout = TimeSpan.FromMinutes(10);
            }
            catch (Exception ex)
            {
            }
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {

                return;
            }
        }

        public async void MakeTask()
        {
            try
            {
                SetCon();
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
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
                    bool isPeriodic = Convert.ToBoolean(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][3]));
                    if (!isPeriodic)
                    {
                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][15]).Date == Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes >= Time.TimeOfDay.TotalMinutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][16]).TimeOfDay.TotalMinutes <= Time.AddMinutes(1).TimeOfDay.TotalMinutes)
                        {
                            string path = (string)dataSet.Tables["Task"].Rows[i][2];
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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await _connection.InvokeAsync("WriteTaskToDataBase", stringTask, IdUser);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename.Remove(archivename.Length - 7, 3);
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
                                        {
                                            stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                        }
                                        await WriteTaskToServer(stringTask);

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
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Hours == Time.TimeOfDay.Hours && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                    {
                                        string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                        string dirName = new DirectoryInfo(path).Name;
                                        string time = DateTime.Now.ToString();
                                        string archivePath = "./ToSend/";
                                        string archivename = dirName + time.Replace(":", "-") + ".zip";
                                        string destinationpath = archivePath + archivename;
                                        ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                        await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));

                                        if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                        {
                                            dataSet.Tables["Task"].Rows[i].Delete();
                                            sqlDataAdapter.Update(dataSet, "Task");

                                        }

                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][8]).AddMinutes(1).TimeOfDay)
                                    {
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60);
                                        sqlDataAdapter.Update(dataSet, "Task");
                                    }
                                }
                                if (!AOneTimeJob)
                                {
                                    if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date <= Time.Date)
                                    {
                                        if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay >= Time.TimeOfDay)
                                        {
                                            if (Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.Minutes == Time.TimeOfDay.Minutes && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).Date == Time.Date)
                                            {
                                                dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).AddMinutes(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][9]) * 60);
                                                sqlDataAdapter.Update(dataSet, "Task");
                                                string[] stringTask = new string[18];
                                                for (int j = 0; j < 18; j++)
                                                {
                                                    stringTask[j] = dataSet.Tables["Task"].Rows[i][j].ToString();
                                                }
                                                await WriteTaskToServer(stringTask);
                                                string path = (string)dataSet.Tables["Task"].Rows[i][2];
                                                string dirName = new DirectoryInfo(path).Name;
                                                string time = DateTime.Now.ToString();
                                                string archivePath = "./ToSend/";
                                                string archivename = dirName + time.Replace(":", "-") + ".zip";
                                                string destinationpath = archivePath + archivename;
                                                ZipFile.CreateFromDirectory(path, destinationpath, CompressionLevel.Optimal, true);
                                                await System.Threading.Tasks.Task.Run(() => загрузитьфайл.загрузитьФайлToolStripMenuItem_Click1(destinationpath));
                                                if (Convert.ToBoolean(dataSet.Tables["Task"].Rows[i][13]) && Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).Date == DateTime.Now.Date)
                                                {
                                                    dataSet.Tables["Task"].Rows[i].Delete();
                                                    sqlDataAdapter.Update(dataSet, "Task");

                                                }
                                            }
                                        }
                                    }
                                    if (Convert.ToString(dataSet.Tables["Task"].Rows[i][4]) == "ежедневная" && Time.TimeOfDay >= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).TimeOfDay && Time.TimeOfDay <= Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][11]).AddMinutes(1).TimeOfDay)
                                    {
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string s1 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][10]).TimeOfDay.ToString();
                                        s1 = s1.Remove(s1.Length - 3, 3);
                                        string s2 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][12]).AddDays(Convert.ToInt32(dataSet.Tables["Task"].Rows[i][5]) / 24 / 60).ToString();
                                        string s3 = Convert.ToDateTime(dataSet.Tables["Task"].Rows[i][17]).TimeOfDay.ToString();
                                        s3 = s3.Remove(s3.Length - 3, 3);
                                        s2 = s2.Replace(s3, s1);
                                        dataSet.Tables["Task"].Rows[i]["MustBeExecuted"] = s2;
                                        sqlDataAdapter.Update(dataSet, "Task");
                                        string[] stringTask = new string[18];
                                        for (int j = 0; j < 18; j++)
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
                await _connection.StopAsync();
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async System.Threading.Tasks.Task WriteTaskToServer(string[] Data)
        {
            await _connection.InvokeAsync("WriteTaskToDataBase", Data, IdUser);
        }
    }
}
