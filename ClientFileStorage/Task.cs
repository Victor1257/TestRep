using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFileStorage
{
    public partial class Task : Form
    {
        public string IDUSER;
        public string s1;
        public int n1;
        SqlConnection sqlConnection;
        public FileStorage _cp;
        public void SetCP(FileStorage cp)
        {
            _cp = cp;
        }

        private Dictionary<int, int> periodicity = new Dictionary<int, int>(6);

        public Task(string Link, string IdUser)
        {
            InitializeComponent();
            IDUSER = IdUser;
            //dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker3.Format = DateTimePickerFormat.Time;
            dateTimePicker4.Format = DateTimePickerFormat.Time;
            //dateTimePicker5.CustomFormat = "dd/MM/yyyy";
            dateTimePicker6.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
            periodicity.Add(0, 60);
            periodicity.Add(1, 360);
            periodicity.Add(2, 720);
            periodicity.Add(3, 1440);
            periodicity.Add(4, 10080);
            periodicity.Add(5, 43200);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string databasefilename = "Database1.mdf";
            //строка для удобства отладки: чтобы если поменял в условии, то не надо было менять в else название БД
            if (!System.IO.File.Exists(Application.StartupPath + @"\" + databasefilename))
            {
                MessageBox.Show("Подключение невозможно");
                Application.Exit();
            }
            else
            {
                //sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=False");
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Repositories\TestRep\ClientFileStorage\Database1.mdf;Integrated Security=True");
                //sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\merli\Source\Repos\TestRep\ClientFileStorage\Database1.mdf;Integrated Security=True;" + "MultipleActiveResultSets=True");
                await sqlConnection.OpenAsync();
                mysqlcommand();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            this.Close();
            _cp.ReloadData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            _cp.ReloadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult rslt = folderBrowserDialog1.ShowDialog();
            var FileNamePath = folderBrowserDialog1.SelectedPath;
            textBox1.Text = FileNamePath;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string error = " задача не добавлена.";
            bool ok = true;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Не указан источник копирования (папка)" + error);
                ok = false;
            }
                
            if ( ! ( radioButton3.Checked || radioButton4.Checked) )
            {
                MessageBox.Show("Не выбран переключатель периодичности копирования" + error);
                ok = false;
            }

            if (ok)
            {//#okSTART
                listBox1.Items.Clear();
                if (label1.Visible)
                {
                    label1.Visible = false;
                }

                    SqlCommand command1 = new SqlCommand("INSERT INTO [Task] (IdUser,FileName,IsPeriodic,Frequency_InProgress,Frequency_RepeatedEvery,DayOfTheWeek,AOneTimeJob,AOneTimeJobValue,RunsEvery,StartsAt,EndsIn,StartDate,EndDate,EndDateValue,DateNoPeriodic,TimeNoPeriodic,MustBeExecuted) VALUES (@IdUser,@FileName,@IsPeriodic,@Frequency_InProgress,@Frequency_RepeatedEvery,@DayOfTheWeek,@AOneTimeJob,@AOneTimeJobValue,@RunsEvery,@StartsAt,@EndsIn,@StartDate,@EndDate,@EndDateValue,@DateNoPeriodic,@TimeNoPeriodic,@MustBeExecuted)", sqlConnection);
                    command1.Parameters.AddWithValue("IdUser", IDUSER);
                    command1.Parameters.AddWithValue("FileName", textBox1.Text);

                    if (radioButton3.Checked)
                    {
                        command1.Parameters.AddWithValue("IsPeriodic", true);
                        command1.Parameters.AddWithValue("Frequency_InProgress", comboBox1.SelectedItem.ToString());
                        command1.Parameters.AddWithValue("Frequency_RepeatedEvery", n1);
                        command1.Parameters.AddWithValue("DayOfTheWeek", s1);
                        command1.Parameters.AddWithValue("DateNoPeriodic", "");
                        command1.Parameters.AddWithValue("TimeNoPeriodic", "");
                    if (radioButton1.Checked)
                        {
                            command1.Parameters.AddWithValue("AOneTimeJob", true);
                            command1.Parameters.AddWithValue("AOneTimeJobValue", dateTimePicker2.Value);
                            command1.Parameters.AddWithValue("RunsEvery", 0);
                            command1.Parameters.AddWithValue("StartsAt", "");
                            command1.Parameters.AddWithValue("EndsIn", "");
                            command1.Parameters.AddWithValue("MustBeExecuted", dateTimePicker1.Value);
                    }
                        else
                        {
                        string s2 =Convert.ToDateTime(dateTimePicker3.Value).Date.ToString();
                        string s3 = Convert.ToDateTime(dateTimePicker3.Value).TimeOfDay.ToString();
                        s2 = s2.Replace(Convert.ToDateTime(dateTimePicker3.Value).Date.ToString(), Convert.ToDateTime(dateTimePicker1.Value).Date.ToString());
                        s2 = s2.Replace("0:00:00", s3);
                        command1.Parameters.AddWithValue("AOneTimeJob", false);
                        command1.Parameters.AddWithValue("AOneTimeJobValue", "");
                        command1.Parameters.AddWithValue("RunsEvery", numericUpDown1.Value);
                        command1.Parameters.AddWithValue("StartsAt", dateTimePicker3.Value);
                        command1.Parameters.AddWithValue("EndsIn", dateTimePicker4.Value);
                        command1.Parameters.AddWithValue("MustBeExecuted", s2);
                    }
                        command1.Parameters.AddWithValue("StartDate", dateTimePicker1.Value);
                    if (radioButton5.Checked)
                    {
                        command1.Parameters.AddWithValue("EndDateValue", dateTimePicker7.Value);
                        command1.Parameters.AddWithValue("EndDate", true);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("EndDate", false);
                        command1.Parameters.AddWithValue("EndDateValue", "");
                    }
                }
                    else
                    {
                    command1.Parameters.AddWithValue("IsPeriodic", false);
                    command1.Parameters.AddWithValue("DateNoPeriodic", dateTimePicker5.Value);
                    command1.Parameters.AddWithValue("TimeNoPeriodic", dateTimePicker6.Value);
                    command1.Parameters.AddWithValue("Frequency_InProgress", "");
                    command1.Parameters.AddWithValue("Frequency_RepeatedEvery",0);
                    command1.Parameters.AddWithValue("DayOfTheWeek", "");
                    command1.Parameters.AddWithValue("AOneTimeJob", false);
                    command1.Parameters.AddWithValue("AOneTimeJobValue", "");
                    command1.Parameters.AddWithValue("RunsEvery", 0);
                    command1.Parameters.AddWithValue("StartsAt", "");
                    command1.Parameters.AddWithValue("EndsIn", "");
                    command1.Parameters.AddWithValue("StartDate", "");
                    command1.Parameters.AddWithValue("EndDateValue","" );
                    command1.Parameters.AddWithValue("EndDate", false);
                    command1.Parameters.AddWithValue("MustBeExecuted", "");

                }
                 await command1.ExecuteNonQueryAsync();


                //}
                //    else if (radioButtonTrue.Checked)
                //        command1.Parameters.AddWithValue("IsPeriodic", true);
                //    if (radioButtonTrue.Checked)
                //    {
                //        if (comboBox1.SelectedIndex < 5)
                //            command1.Parameters.AddWithValue("Period", periodicity[comboBox1.SelectedIndex]);
                //        else command1.Parameters.AddWithValue("Period", Convert.ToInt32(textBox2.Text));
                //    }

                //    else
                //        command1.Parameters.AddWithValue("Period", 0);
                //    await command1.ExecuteNonQueryAsync();
                //}
                //else
                //{
                //    label1.Visible = true;
                //    label1.Text = "Данные не введены";
                //}
                mysqlcommand();
            }//#okEND


        }

        private async void mysqlcommand()
        {
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            try
            {
                string[] stringTask = new string[18];
                sqlReader = await command.ExecuteReaderAsync();
                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(
                        Convert.ToString(sqlReader["Id"]) + " " + Convert.ToString(sqlReader["IdUser"])
                        + " " + Convert.ToString(sqlReader["FileName"]) + " "+
                        Convert.ToString(sqlReader["IsPeriodic"]) + " " + Convert.ToString(sqlReader["Frequency_InProgress"]) + " " + Convert.ToString(sqlReader["Frequency_RepeatedEvery"]) + " " +
                        Convert.ToString(sqlReader["DayOfTheWeek"]) + " " + Convert.ToString(sqlReader["AOneTimeJob"]) + " " + Convert.ToString(sqlReader["AOneTimeJobValue"]) + " " +
                        Convert.ToString(sqlReader["RunsEvery"]) + " " + Convert.ToString(sqlReader["StartsAt"]) + " " + Convert.ToString(sqlReader["EndsIn"]) + " " +
                        Convert.ToString(sqlReader["StartDate"]) + " " + Convert.ToString(sqlReader["EndDate"]) + " " + Convert.ToString(sqlReader["EndDateValue"]) + " " +
                        Convert.ToString(sqlReader["DateNoPeriodic"]) + " " + Convert.ToString(sqlReader["TimeNoPeriodic"]) + " " + Convert.ToString(sqlReader["MustBeExecuted"])
                        );
                    stringTask[0] = Convert.ToString(sqlReader["Id"]);
                    stringTask[1] = Convert.ToString(sqlReader["IdUser"]);
                    stringTask[2] = Convert.ToString(sqlReader["FileName"]);// + Convert.ToString(sqlReader["LastUploadDate"]).Replace(":", "-");
                    stringTask[3] = Convert.ToString(sqlReader["IsPeriodic"]);
                    stringTask[4] = Convert.ToString(sqlReader["Frequency_InProgress"]);
                    stringTask[5] = Convert.ToString(sqlReader["Frequency_RepeatedEvery"]);
                    stringTask[6] = Convert.ToString(sqlReader["DayOfTheWeek"]);
                    stringTask[7] = Convert.ToString(sqlReader["AOneTimeJob"]);
                    stringTask[8] = Convert.ToString(sqlReader["AOneTimeJobValue"]);
                    stringTask[9] = Convert.ToString(sqlReader["RunsEvery"]);
                    stringTask[10] = Convert.ToString(sqlReader["StartsAt"]);
                    stringTask[11] = Convert.ToString(sqlReader["EndsIn"]);
                    stringTask[12] = Convert.ToString(sqlReader["StartDate"]);
                    stringTask[13] = Convert.ToString(sqlReader["EndDate"]);
                    stringTask[14] = Convert.ToString(sqlReader["EndDateValue"]);
                    stringTask[15] = Convert.ToString(sqlReader["DateNoPeriodic"]);
                    stringTask[16] = Convert.ToString(sqlReader["TimeNoPeriodic"]);
                    stringTask[17] = Convert.ToString(sqlReader["MustBeExecuted"]);
                    //MessageBox.Show("BEFORE");
                    //await _cp.WriteTaskToServer(stringTask);
                    //MessageBox.Show("AFTER");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }
        }

        //private string get_formatted_time()
        //{
        //    return DateTime.Now.Day.ToString() + "d-"
        //        + DateTime.Now.Month.ToString() + "m-"
        //        + DateTime.Now.Year.ToString() + "y "
        //        + DateTime.Now.Hour.ToString() + "h-"
        //        + DateTime.Now.Minute.ToString() + "m";

        //}

        //private void radioButtonTrue_CheckedChanged(object sender, EventArgs e)
        //{
        //    comboBox1.Enabled = true;
        //    checkBox1.Enabled = true;
        //    checkBox2.Enabled = true;
        //    checkBox3.Enabled = true;
        //    checkBox4.Enabled = true;
        //    checkBox5.Enabled = true;
        //    checkBox6.Enabled = true;
        //    checkBox7.Enabled = true;
        //}

        //private void radioButtonFalse_CheckedChanged(object sender, EventArgs e)
        //{
        //    comboBox1.Enabled = false;
        //    checkBox1.Enabled = false;
        //    checkBox2.Enabled = false;
        //    checkBox3.Enabled = false;
        //    checkBox4.Enabled = false;
        //    checkBox5.Enabled = false;
        //    checkBox6.Enabled = false;
        //    checkBox7.Enabled = false;
        //}

        //private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    if (comboBox1.Text == "ввести своё")
        //        { textBox2.Enabled = true; }
        //    else textBox2.Enabled = false;
        //}

        //private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
        //    {
        //        e.Handled = true;
        //    }
        //}



        private void radioButton3_CheckedChanged(object sender, EventArgs e)
            {

                groupBox4.Enabled = false;

                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
            }

            private void radioButton4_CheckedChanged(object sender, EventArgs e)
            {

                groupBox4.Enabled = true;

                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
            }

            private void radioButton1_CheckedChanged(object sender, EventArgs e)
            {
                dateTimePicker2.Enabled = true;
                numericUpDown1.Enabled = false;
                textBox5.Enabled = false;
                dateTimePicker3.Enabled = false;
                dateTimePicker4.Enabled = false;
            }

            private void radioButton2_CheckedChanged(object sender, EventArgs e)
            {
                dateTimePicker2.Enabled = false;
                numericUpDown1.Enabled = true;
                textBox5.Enabled = true;
                dateTimePicker3.Enabled = true;
                dateTimePicker4.Enabled = true;
            }

            private void radioButton6_CheckedChanged(object sender, EventArgs e)
            {
                dateTimePicker7.Enabled = false;
            }

            private void radioButton5_CheckedChanged(object sender, EventArgs e)
            {
                dateTimePicker7.Enabled = true;
            }

            private void checkBox1_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox1.Checked)
                {
                    s1 = s1 + "Понедельник; ";
                }
                else s1 = s1.Replace("Понедельник; ", "");
            }

            private void checkBox2_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox2.Checked)
                {
                    s1 = s1 + "Вторник; ";
                }
                else s1 = s1.Replace("Вторник; ", "");
            }

            private void checkBox3_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox3.Checked)
                {
                    s1 = s1 + "Среда; ";
                }
                else s1 = s1.Replace("Среда; ", "");
            }

            private void checkBox4_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox4.Checked)
                {
                    s1 = s1 + "Четверг; ";
                }
                else s1 = s1.Replace("Четверг; ", "");
            }

            private void checkBox5_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox5.Checked)
                {
                    s1 = s1 + "Пятница; ";
                }
                else s1 = s1.Replace("Пятница; ", "");
            }

            private void checkBox6_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox6.Checked)
                {
                    s1 = s1 + "Суббота; ";
                }
                else s1 = s1.Replace("Суббота; ", "");
            }

            private void checkBox7_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBox7.Checked)
                {
                    s1 = s1 + "Воскресенье; ";
                }
                else s1 = s1.Replace("Воскресенье; ", "");
            }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                label7.Text = "день";
                n1 =  1440;
                numericUpDown2.Value = 1;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                label7.Text = "нед.";
                n1 = 10080;
                numericUpDown2.Value = 1;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                label7.Text = "месяц";
                n1 = 43200;
                numericUpDown2.Value = 1;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (label7.Text == "день")
            {
                n1 = (int)(numericUpDown2.Value * 1440);
            }
            if (label7.Text == "нед.")
            {
                n1 = (int)(numericUpDown2.Value * 10080);
            }
            if (label7.Text == "месяц")
            {
                n1 = (int)(numericUpDown2.Value * 43200);
            }
        }
    }
    }

