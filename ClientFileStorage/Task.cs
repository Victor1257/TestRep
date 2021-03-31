using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace ClientFileStorage
{
    public partial class Task : Form
    {
        public string IDUSER;
        public string s1;
        public int n1;
        public List<string> arrProjectList;
        public Dictionary<int, string> dataSource;
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
            radioButton7.Checked = true;
            radioButton9.Checked = false;
            radioButton10.Checked = true;
            radioButton9_CheckedChanged(sender,e);
            if (!System.IO.File.Exists(Application.StartupPath + @"\" + databasefilename))
            {
                MessageBox.Show("Подключение невозможно");
                Application.Exit();
            }
            else
            {
                //  sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=False");
                //   sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\meschaninov\Desktop\TestRep - копия\ClientFileStorage\Database1.mdf; Integrated Security = True");
                sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + System.IO.Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) + "\\Database1.mdf; Integrated Security = True");
                await sqlConnection.OpenAsync();
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
         //   _cp.ReloadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value == 1)
            {
                DialogResult rslt = folderBrowserDialog1.ShowDialog();
                var FileNamePath = folderBrowserDialog1.SelectedPath;
                textBox1.Text = FileNamePath;
            }
            if (trackBar1.Value == 2)
            {
                DialogResult rslt = openFileDialog1.ShowDialog();
                var FileNamePath = openFileDialog1.FileName;
                textBox1.Text = FileNamePath;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            int a = -1;
            if (label1.Visible)
            {
                label1.Visible = false;
            }



            SqlCommand command1 = new SqlCommand("INSERT INTO [Task] (IdUser,IsPeriodic,Frequency_InProgress,Frequency_RepeatedEvery,DayOfTheWeek,AOneTimeJob,AOneTimeJobValue,RunsEvery,StartsAt,EndsIn,StartDate,EndDate,EndDateValue,DateNoPeriodic,TimeNoPeriodic,MustBeExecuted,IsFile) VALUES (@IdUser,@IsPeriodic,@Frequency_InProgress,@Frequency_RepeatedEvery,@DayOfTheWeek,@AOneTimeJob,@AOneTimeJobValue,@RunsEvery,@StartsAt,@EndsIn,@StartDate,@EndDate,@EndDateValue,@DateNoPeriodic,@TimeNoPeriodic,@MustBeExecuted,@IsFile)", sqlConnection);
            command1.Parameters.AddWithValue("IdUser", IDUSER);

            // command1.Parameters.AddWithValue("FileName", textBox1.Text);
            if (radioButton7.Checked)
            {
                command1.Parameters.AddWithValue("IsFile", true);
            }
            else
            {
                command1.Parameters.AddWithValue("IsFile", false);
            }
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
                    string s2 = Convert.ToDateTime(dateTimePicker1.Value).Date.ToString();
                    string s3 = Convert.ToDateTime(dateTimePicker2.Value).TimeOfDay.ToString();
                    s2 = s2.Replace("0:00:00", s3);
                    command1.Parameters.AddWithValue("AOneTimeJob", true);
                        command1.Parameters.AddWithValue("AOneTimeJobValue", dateTimePicker2.Value);
                        command1.Parameters.AddWithValue("RunsEvery", 0);
                        command1.Parameters.AddWithValue("StartsAt", "");
                        command1.Parameters.AddWithValue("EndsIn", "");
                        command1.Parameters.AddWithValue("MustBeExecuted", s2);
                    }
                    else
                    {
                        string s2 = Convert.ToDateTime(dateTimePicker3.Value).Date.ToString();
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
                    string s2 = Convert.ToDateTime(dateTimePicker5.Value).Date.ToString();
                    string s3 = Convert.ToDateTime(dateTimePicker6.Value).TimeOfDay.ToString();
                    s2 = s2.Replace("0:00:00", s3);
                    command1.Parameters.AddWithValue("IsPeriodic", false);
                    command1.Parameters.AddWithValue("DateNoPeriodic", dateTimePicker5.Value);
                    command1.Parameters.AddWithValue("TimeNoPeriodic", dateTimePicker6.Value);
                    command1.Parameters.AddWithValue("Frequency_InProgress", "");
                    command1.Parameters.AddWithValue("Frequency_RepeatedEvery", 0);
                    command1.Parameters.AddWithValue("DayOfTheWeek", "");
                    command1.Parameters.AddWithValue("AOneTimeJob", false);
                    command1.Parameters.AddWithValue("AOneTimeJobValue", "");
                    command1.Parameters.AddWithValue("RunsEvery", 0);
                    command1.Parameters.AddWithValue("StartsAt", "");
                    command1.Parameters.AddWithValue("EndsIn", "");
                    command1.Parameters.AddWithValue("StartDate", "");
                    command1.Parameters.AddWithValue("EndDateValue", "");
                    command1.Parameters.AddWithValue("EndDate", false);
                    command1.Parameters.AddWithValue("MustBeExecuted", s2);
                }
                await command1.ExecuteNonQueryAsync();
                SqlDataReader sqlReader = null;
            SqlCommand command3 = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            sqlReader = await command3.ExecuteReaderAsync();
            try
            {
                while (await sqlReader.ReadAsync())
                {
                    if (Convert.ToInt32(sqlReader["id"]) > a)
                    {
                        a = Convert.ToInt32(sqlReader["id"]);
                    }
                }
            }
            finally
            {
                sqlReader.Close();
            }

            if (radioButton7.Checked)
            {
                SqlCommand command2 = new SqlCommand("INSERT INTO [File] (IdFile,FileName) VALUES (@IdFile,@FileName)",sqlConnection);
                command2.Parameters.AddWithValue("IdFile", a);
                command2.Parameters.AddWithValue("FileName", textBox1.Text);
                await command2.ExecuteNonQueryAsync();
            }

            if (radioButton8.Checked)
            {
                SqlCommand command2 = new SqlCommand("INSERT INTO [SYBD] (IdSYBD,The_Supplier,Adres_Server,Port_Server,Instance_Server,Login_SYBD,Password_SYBD,Way,Name_SYBD,Integrated_Security) VALUES (@IdSYBD,@The_Supplier,@Adres_Server,@Port_Server,@Instance_Server,@Login_SYBD,@Password_SYBD,@Way,@Name_SYBD,@Integrated_Security)", sqlConnection);
                command2.Parameters.AddWithValue("IdSYBD", a);
                command2.Parameters.AddWithValue("The_Supplier", comboBox2.Text);
                command2.Parameters.AddWithValue("Adres_Server", comboBox3.Text);
                command2.Parameters.AddWithValue("Port_Server", comboBox4.Text);
                command2.Parameters.AddWithValue("Instance_Server", comboBox5.Text);
                command2.Parameters.AddWithValue("Login_SYBD", comboBox6.Text);
                command2.Parameters.AddWithValue("Password_SYBD", textBox2.Text);
                command2.Parameters.AddWithValue("Way", comboBox7.Text);
                command2.Parameters.AddWithValue("Name_SYBD", comboBox8.Text);
                if (radioButton9.Checked)
                {
                    command2.Parameters.AddWithValue("Integrated_Security", true);
                }
                if (radioButton10.Checked)
                {
                    command2.Parameters.AddWithValue("Integrated_Security", false);
                }

                    await command2.ExecuteNonQueryAsync();
            }
             


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
            
        }

        private async void mysqlcommand()
        {
            SqlDataReader sqlReader = null;
            SqlDataReader sqlReader2 = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            SqlCommand command1 = new SqlCommand("SELECT * FROM [File]", sqlConnection);
            try
            {
                string[] stringTask = new string[19];
                sqlReader = await command.ExecuteReaderAsync();
                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(
                        Convert.ToString(sqlReader["Id"]) + " " + Convert.ToString(sqlReader["IdUser"])
                        + " " + Convert.ToString(sqlReader["IsFile"]) + " " +
                        Convert.ToString(sqlReader["IsPeriodic"]) + " " + Convert.ToString(sqlReader["Frequency_InProgress"]) + " " + Convert.ToString(sqlReader["Frequency_RepeatedEvery"]) + " " +
                        Convert.ToString(sqlReader["DayOfTheWeek"]) + " " + Convert.ToString(sqlReader["AOneTimeJob"]) + " " + Convert.ToString(sqlReader["AOneTimeJobValue"]) + " " +
                        Convert.ToString(sqlReader["RunsEvery"]) + " " + Convert.ToString(sqlReader["StartsAt"]) + " " + Convert.ToString(sqlReader["EndsIn"]) + " " +
                        Convert.ToString(sqlReader["StartDate"]) + " " + Convert.ToString(sqlReader["EndDate"]) + " " + Convert.ToString(sqlReader["EndDateValue"]) + " " +
                        Convert.ToString(sqlReader["DateNoPeriodic"]) + " " + Convert.ToString(sqlReader["TimeNoPeriodic"]) + " " + Convert.ToString(sqlReader["MustBeExecuted"]) + " " + Convert.ToString(sqlReader["GUID"])
                        );
                    stringTask[0] = Convert.ToString(sqlReader["Id"]);
                    stringTask[1] = Convert.ToString(sqlReader["IdUser"]);
                    stringTask[2] = Convert.ToString(sqlReader["IsFile"]);// + Convert.ToString(sqlReader["LastUploadDate"]).Replace(":", "-");
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
                    stringTask[18] = Convert.ToString(sqlReader["GUID"]);

                    await _cp.WriteTaskToServer(stringTask);
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
            try
            {
                if (radioButton7.Checked)
                {

                    string[] stringTask2 = new string[2];
                    sqlReader2 = await command1.ExecuteReaderAsync();
                    while (await sqlReader2.ReadAsync())
                    {
                        stringTask2[0] = Convert.ToString(sqlReader2["IdFile"]);
                        stringTask2[1] = Convert.ToString(sqlReader2["FileName"]);
                    }
                    await _cp.WriteFileToServer(stringTask2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (sqlReader2 != null)
                {
                    sqlReader2.Close();
                }
            }
            finally
            {
                if (sqlReader2 != null)
                {
                    sqlReader2.Close();
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
                dateTimePicker3.Enabled = false;
                dateTimePicker4.Enabled = false;
            }

            private void radioButton2_CheckedChanged(object sender, EventArgs e)
            {
                dateTimePicker2.Enabled = false;
                numericUpDown1.Enabled = true;
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

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                textBox1.Enabled = true;
                button2.Enabled = true;
                trackBar1.Enabled = true;
                groupBox6.Enabled = false;
                groupBox7.Enabled = false;
                groupBox8.Enabled = false;
                groupBox9.Enabled = false;
                groupBox10.Enabled = false;
                groupBox11.Enabled = false;
                groupBox12.Enabled = false;
                groupBox13.Enabled = false;


            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                radioButton10.Checked = true;
                textBox1.Enabled = false;
                button2.Enabled = false;
                trackBar1.Enabled = false;
                groupBox6.Enabled = true;
                groupBox7.Enabled = true;
                groupBox8.Enabled = true;
                groupBox9.Enabled = true;
                groupBox10.Enabled = true;
                groupBox11.Enabled = true;
                groupBox12.Enabled = true;
                groupBox13.Enabled = true;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            groupBox9.Enabled = false;
            groupBox10.Enabled = false;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            groupBox9.Enabled = true;
            groupBox10.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string con1;
            if (comboBox4.Text == "")
            {
                if (radioButton9.Checked)
                {
                    con1 = "Data Source = " + comboBox3.Text + "\\" + comboBox5.Text + "; Integrated Security = True";
                }
                else
                {
                    con1 = "Data Source = " + comboBox3.Text + "\\" + comboBox5.Text + "; User ID=" + comboBox6.Text + ";Password=" + textBox2.Text;
                }
            }
            else
            {
                if (radioButton9.Checked)
                {
                    con1 = "Data Source = " + comboBox3.Text + "," + comboBox4.Text + "; Integrated Security = True";
                }
                else
                {
                    con1 = "Data Source = " + comboBox3.Text + "," + comboBox4.Text + "; User ID=" + comboBox6.Text + ";Password=" + textBox2.Text;
                }
            }
            SqlConnection sqlConnection1 = new SqlConnection(@con1);
            sqlConnection1.Open();
            Console.WriteLine(sqlConnection1.State.ToString());
            if (sqlConnection1.State.ToString() == "Open")
            {
                if (comboBox4.Text == "")
                {
                    comboBox3.Items.Add(comboBox3.Text);
                    comboBox5.Items.Add(comboBox5.Text);
                }
                else
                    comboBox3.Items.Add(comboBox3.Text);
                comboBox4.Items.Add(comboBox4.Text);
            }
            SqlCommand command2 = new SqlCommand("SELECT name  FROM sys.databases", sqlConnection1);
            SqlDataReader reader = command2.ExecuteReader();
            while (reader.Read())
            {
                comboBox8.Items.Add(reader[0]);
            }

        }


    }
}

