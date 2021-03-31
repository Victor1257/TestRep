using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using ClientFileStorage;
using System.IO;
using System.Text.Json;
using System.Timers;
using System.Threading.Tasks;


namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter1 = null;
        private DataSet dataSet = null;
        public string Link;
        public string IdUser;
        private Timer timer;

        class Config
        {
            public string Link { get; set; }
            public string IdUser { get; set; }
        }
        public Service1()
        {
            InitializeComponent();
            Load();
            LoadData();
        }

        protected override async void OnStart(string[] args)
        {
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\meschaninov\Desktop\TestRep - копия\ClientFileStorage\Database1.mdf; Integrated Security = True");
            //sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath("Database1.mdf") + ";Integrated Security=True");
            sqlConnection.Open();
        }


        private async void Load()
        {
            using (FileStream fs = new FileStream("C:/Users/meschaninov/Desktop/TestRep - копия/WindowsService1/config.json", FileMode.OpenOrCreate))
            {
                Config restoredPerson = await JsonSerializer.DeserializeAsync<Config>(fs);
                Link = restoredPerson.Link;
                IdUser = restoredPerson.IdUser;
            }
        }

        protected override void OnStop()
        {
        }

        private async void Kake(object sender, EventArgs e)
        {
            sqlDataAdapter1.Update(dataSet, "Task");
            TaskWorker makeTasks = new TaskWorker(Link, IdUser);
            await System.Threading.Tasks.Task.Run(() => makeTasks.MakeTask());
        }

        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT *,'Delete' AS [Delete] FROM Task", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter1);
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();
                dataSet = new DataSet();
                sqlDataAdapter1.Fill(dataSet, "Task");
                Cheker cheker = new Cheker(Link, IdUser, dataSet);
                cheker.Check();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
