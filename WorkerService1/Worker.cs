using System.Data.SQLite;
using System.Management;
using System.Windows.Forms;
using System.Reflection;
namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        static DateTime dateTime;
        //
        static SQLiteConnection conn = new SQLiteConnection();
        static SQLiteCommand comm = new SQLiteCommand();
        public static void DBdesign()
        {
            string path = "C:\\Users\\asurendran\\OneDrive - SOTI Inc\\Desktop\\Windows\\Data\\data.db";
            string connectionString = "Data Source=" + path + ";User Instance=True";
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile("C:\\Users\\asurendran\\OneDrive - SOTI Inc\\Desktop\\Windows\\Data\\data.db");
            }
            conn = new SQLiteConnection(connectionString);
            conn.Open();
            comm = new SQLiteCommand(conn);
            try
            {
                comm.CommandText = @"Select * from BatteryUsage";
                comm.ExecuteNonQuery();

            }
            catch
            {
                comm.CommandText = @"Create Table BatteryUsage(ID INTEGER PRIMARY KEY AUTOINCREMENT,BatteryPercentage float,BatteryStatus string,Timestamp datetime)";
                comm.ExecuteNonQuery();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      
        {
            Worker.DBdesign();
            comm.CommandText = @"Insert into BatteryUsage(BatteryPercentage,BatteryStatus,TimeStamp) values(@batper,@batsts,@timestamp)";
            Type power = typeof(System.Windows.Forms.PowerStatus);
            PropertyInfo[] propinfo= power.GetProperties();
            while (!stoppingToken.IsCancellationRequested)
            {
                object batvalue = propinfo[3].GetValue(SystemInformation.PowerStatus,null);
                double batper = Convert.ToDouble(batvalue.ToString());
                string batstatus = (propinfo[0].GetValue(SystemInformation.PowerStatus, null)).ToString();
               
                dateTime = DateTime.Now;
                comm.Parameters.AddWithValue("@batper", batper);
                comm.Parameters.AddWithValue("@batsts", batstatus);
                comm.Parameters.AddWithValue("@timestamp", dateTime);
                comm.ExecuteNonQuery();

                _logger.LogInformation(batvalue.ToString());
                _logger.LogInformation(batstatus);
                await Task.Delay(30000, stoppingToken);
                    


                   
                    
                
              
            }
        }
    }
}