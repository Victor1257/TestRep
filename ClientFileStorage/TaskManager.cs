using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFileStorage
{
    public partial class TaskManager : Form
    {
        public string IDUSER;
        SqlConnection sqlConnection;
        public FileStorage _cp;
        public void SetCP(FileStorage cp)
        {
            _cp = cp;
        }

        private Dictionary<int, int> periodicity = new Dictionary<int, int>(5);

        public TaskManager(string Link, string IdUser)
        {
            InitializeComponent();
            IDUSER = IdUser;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy hh:mm";

            periodicity.Add(0, 60);
            periodicity.Add(1, 360);
            periodicity.Add(2, 720);
            periodicity.Add(3, 1440);
            periodicity.Add(4, 10080);
        }

        private async void connect_to_sql_database(string dbfilename)
        {
            //строка для удобства отладки: чтобы если поменял в условии, то не надо было менять в else название БД
            if (!System.IO.File.Exists(Application.StartupPath + @"\" + dbfilename))
            {
                MessageBox.Show("Подключение невозможно");
                Application.Exit();
            }
            else
            {
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + @"\" + dbfilename + ";Integrated Security=True");
                await sqlConnection.OpenAsync();
            }
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
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + @"\" + databasefilename + ";Integrated Security=True");
                await sqlConnection.OpenAsync();
                
            }

            //connect_to_sql_database("Database1.mdf");

            mysqlcommand();
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
            listBox1.Items.Clear();
            if (label1.Visible)
            {
                label1.Visible = false;
            }
            if (!string.IsNullOrEmpty(textBox1.Text)/* && !string.IsNullOrWhiteSpace(textBox1.Text)*/)
            {
                SqlCommand command1 = new SqlCommand("INSERT INTO [Task] (IdUser,FileName,LastUploadDate,IsPeriodic,Period) VALUES (@IdUser,@FileName,@LastUploadDate,@IsPeriodic,@Period)", sqlConnection);
                command1.Parameters.AddWithValue("IdUser", IDUSER);
                command1.Parameters.AddWithValue("FileName", textBox1.Text);
                command1.Parameters.AddWithValue("LastUploadDate", dateTimePicker1.Value);
                if (radioButtonFalse.Checked)
                    command1.Parameters.AddWithValue("IsPeriodic", false);
                else if (radioButtonTrue.Checked)
                    command1.Parameters.AddWithValue("IsPeriodic", true);
                if (radioButtonTrue.Checked)
                {
                    if (comboBox1.SelectedIndex<5)
                        command1.Parameters.AddWithValue("Period", periodicity[comboBox1.SelectedIndex]);
                    else command1.Parameters.AddWithValue("Period", Convert.ToInt32(textBox2.Text) );
                }    
                
                else
                    command1.Parameters.AddWithValue("Period", 0);
                await command1.ExecuteNonQueryAsync();
            }
            else
            {
                label1.Visible = true;
                label1.Text = "Данные не введены";
            }
            mysqlcommand();
        }

        private async void mysqlcommand()
        {
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            try
            {
                sqlReader = await command.ExecuteReaderAsync();
                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(
                        Convert.ToString(sqlReader["Id"]) + " " + Convert.ToString(sqlReader["IdUser"])
                        + " " + Convert.ToString(sqlReader["FileName"]) + " " + Convert.ToString(sqlReader["LastUploadDate"])
                        + " " + Convert.ToString(sqlReader["IsPeriodic"]) + " " + Convert.ToString(sqlReader["Period"])
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }
        }

        private void radioButtonTrue_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
        }

        private void radioButtonFalse_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "ввести своё")
                { textBox2.Enabled = true; }
            else textBox2.Enabled = false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}
