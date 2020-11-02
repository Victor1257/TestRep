using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFileStorage
{
    public partial class TaskForm : Form
    {
        public TaskForm()
        {
            InitializeComponent();
        }

        public string selectedpath;

        private void forced_archivate_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog OPF = new FolderBrowserDialog();

            if (OPF.ShowDialog() == DialogResult.OK)
            {
                string time = get_formatted_time();
                //string destinationpath = "D:/Repositories/Client/ClientFileStorage/filesfolder/archive " + time + ".zip";
                //string destinationpath = AppDomain.CurrentDomain.BaseDirectory. + "/filesfolder/archive " + time + ".zip";
                string destinationpath = @"./filesfolder/archive " + time + ".zip";
                selectedpath = OPF.SelectedPath;
                
                ZipFile.CreateFromDirectory(OPF.SelectedPath, destinationpath);
                if (System.IO.File.Exists(destinationpath))
                {
                    MessageBox.Show("Архив успешно создан");
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

        private void actionbutton_Click(object sender, EventArgs e)
        {
            string time = get_formatted_time();
            string destinationpath = @"./filesfolder/archive " + time + ".zip";
            ZipFile.CreateFromDirectory(selectedpath, destinationpath);
                if (System.IO.File.Exists(destinationpath))
                {
                    MessageBox.Show("Архив успешно создан");
                }
        }
    }
}
