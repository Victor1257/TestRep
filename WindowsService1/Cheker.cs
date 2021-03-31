using Microsoft.AspNetCore.SignalR.Client;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace WindowsService1
{

    class Cheker
    {
        public string Link;
        public string IdUser;
        private HubConnection _connection;
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;

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
        public Cheker(string Link1, string IdUser1, DataSet dataSet1)
        {
            Link = Link1;
            IdUser = IdUser1;
            dataSet = dataSet1;
        }
        public async void Check()
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
    }
}
