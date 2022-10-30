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
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<DischargeDetails> Discharges
        {
            get; set;
        } = new();

        List<Counts> counters;

        public MainWindow()
        {
            this.InitializeComponent();
            DBconnect();
           
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
            Discharges.Clear();
            myButton.Content = "Refresh";
            table.Visibility = Visibility.Visible;
            SpotCount.Visibility = Visibility.Visible;
            OptimalCount.Visibility = Visibility.Visible;
            BadCount.Visibility = Visibility.Visible;
            List<DischargeDetails> discharges = Cook.Discharge();

            counters = Cook.Counts();

            foreach (var discharge in discharges)
                Discharges.Add(discharge);
            SpotCount.Text = "Spot Count : " + counters[0].SpotCount;
            BadCount.Text = "Bad Count  : " + counters[0].BadCount;
            OptimalCount.Text = "Optimal Count : " + counters[0].OptimalCount;

        }
    }
}
