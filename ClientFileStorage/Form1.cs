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
    public partial class Form1 : Form
    {
        public string IDUSER;
        SqlConnection sqlConnection;
        public FileStorage _cp;
        public void SetCP(FileStorage cp)
        {
            _cp = cp;
        }


        public Form1(string Link, string IdUser)
        {
            InitializeComponent();
            IDUSER = IdUser;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy hh:mm";
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+Application.StartupPath+ @"\Database1.mdf" + ";Integrated Security=True");
            await sqlConnection.OpenAsync();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            try
            {
                sqlReader = await command.ExecuteReaderAsync();
                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(Convert.ToString(sqlReader["Id"]) + " " + Convert.ToString(sqlReader["IdUser"]) + " " + Convert.ToString(sqlReader["FileName"]) + " " + Convert.ToString(sqlReader["Time"]));
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
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
            {
                SqlCommand command1 = new SqlCommand("INSERT INTO [Task] (IdUser,FileName,Time)VALUES (@IdUser,@FileName,@Time)", sqlConnection);
                command1.Parameters.AddWithValue("IdUser", IDUSER);
                command1.Parameters.AddWithValue("FileName", textBox1.Text);
                command1.Parameters.AddWithValue("Time", dateTimePicker1.Value);
                await command1.ExecuteNonQueryAsync();
            }
            else
            {
                label1.Visible = true;
                label1.Text = "Данные не введены";
            }
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Task]", sqlConnection);
            try
            {
                sqlReader = await command.ExecuteReaderAsync();
                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(Convert.ToString(sqlReader["Id"]) + " " + Convert.ToString(sqlReader["IdUser"]) + " " + Convert.ToString(sqlReader["FileName"]) + " " + Convert.ToString(sqlReader["Time"]));
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
    }
}
