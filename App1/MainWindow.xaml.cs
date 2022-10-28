using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using System.Data.SQLite;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        
      
        public MainWindow()
        {
            this.InitializeComponent();
        
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        static SQLiteConnection conn = new SQLiteConnection();
        static SQLiteCommand comm = new SQLiteCommand();
        public static void DBconnect()
        {
            string path = "C:\\Users\\asurendran\\OneDrive - SOTI Inc\\Desktop\\Windows\\Data\\data.db";
            string connectionString = "Data Source=" + path + ";User Instance=True";
           
            conn = new SQLiteConnection(connectionString);
            conn.Open();
            comm = new SQLiteCommand(conn);
         
        }
        public void myButton_Click(object sender, RoutedEventArgs e)
        {
            DBconnect();
            string stm = "Select * from BatteryUsage limit 5";
            comm = new SQLiteCommand(stm, conn);
            SQLiteDataReader reader = comm.ExecuteReader();

            myButton.Opacity=0;
            textblock1.Text = "";

            while (reader.Read())
            {
                textblock1.Text += reader.GetDouble(1).ToString() + "\n";
                textblock1.Text += reader.GetString(3)+"\n";
                textblock1.Text += reader.GetString(2)+"\n";

            }
           








        }
    }
}
