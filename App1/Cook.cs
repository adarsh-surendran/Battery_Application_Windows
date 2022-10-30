using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace App1
{
    public class Cook
    {
        static SQLiteConnection con = new SQLiteConnection();
        static SQLiteCommand cmd = new SQLiteCommand();



      

        static string path = "C:\\Users\\asurendran\\OneDrive - SOTI Inc\\Desktop\\Windows\\Data\\data.db";

        static string cs = "Data Source=" + path + ";User Instance=True;";


        public static List<DischargeDetails> Discharge()
        {
            List<DischargeDetails> list = new List<DischargeDetails>();
            DateTime dateTime = new DateTime();
            DateTime dateNow = DateTime.Now;
            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "SELECT Timestamp from BatteryUsage where ID=1;";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            rdr.Read();


            if (rdr.HasRows) 
            {


                dateTime = rdr.GetDateTime(0);
                rdr.Close();


               
                string InitialDatetime = dateTime.ToString();
                string startingDatetime = DateTime.ParseExact(InitialDatetime, "MM/dd/yyyy h:mm:ss tt", null).ToString("yyyy-MM-dd HH:00:00.00");
                string endingDatetime = DateTime.ParseExact(InitialDatetime, "MM/dd/yyyy h:mm:ss tt", null).ToString("yyyy-MM-dd " + dateTime.AddHours(1).ToString("HH") + ":00:00.00");



                stm = "SELECT  * From  BatteryUsage WHERE Timestamp between @startTime  and @endTime ";
                cmd = new SQLiteCommand(stm, con);


                while (true)
                {
                    cmd.Parameters.AddWithValue("@startTime", startingDatetime);
                    cmd.Parameters.AddWithValue("@endTime", endingDatetime);


                    rdr = cmd.ExecuteReader();


                   

                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        float initialCharge = rdr.GetFloat(1);
                        DateTime initialTime = rdr.GetDateTime(3);
                        double charge = 0.00;
                        double time = 0.00;
                        TimeSpan dischargeTime = new TimeSpan();
                        while (rdr.Read())
                        {

                            float finalCharge = rdr.GetFloat(1);
                            string chargingStatus = rdr.GetString(2);
                            DateTime finalTime = rdr.GetDateTime(3);
                            


                            if (chargingStatus == "Offline" && initialCharge != finalCharge)
                            {
                                charge += initialCharge - finalCharge;
                                dischargeTime += finalTime - initialTime;
                                initialTime = finalTime;
                            }

                            if (chargingStatus == "Online")
                            {
                                initialTime = finalTime;
                            }
                            initialCharge = finalCharge;

                        }
                        time = dischargeTime.Minutes;
                        charge = charge * 100;
                        list.Add(new DischargeDetails { Time = Convert.ToDateTime(startingDatetime), DischargeLevel = charge, DischargeTime = time });
                    }
                    else
                    {
                       
                        list.Add(new DischargeDetails { Time = Convert.ToDateTime(startingDatetime), DischargeLevel = 0, DischargeTime = 0 });

                    }
                    rdr.Close();

                    if (dateTime.AddHours(1) >= dateNow)
                    {
                        break;
                    }

                    dateTime = Convert.ToDateTime(startingDatetime).AddHours(1);
                    InitialDatetime = dateTime.ToString();
                    startingDatetime = DateTime.ParseExact(InitialDatetime, "MM/dd/yyyy h:mm:ss tt", null).ToString("yyyy-MM-dd HH:00:00.00");
                    endingDatetime = DateTime.ParseExact(InitialDatetime, "MM/dd/yyyy h:mm:ss tt", null).ToString("yyyy-MM-dd " + dateTime.AddHours(1).ToString("HH") + ":00:00.00");
                }
            }

            rdr.Close();
            con.Close();
            return list;
        }



        public static List<Counts> Counts()
        {
            List<Counts> counter = new List<Counts> { new Counts { OptimalCount = 0, BadCount = 0, SpotCount = 0 } };

            using var con = new SQLiteConnection(cs);

            string stm = "SELECT * from BatteryUsage;";

            con.Open();
            var cmd = new SQLiteCommand(stm, con);

            SQLiteDataReader rdr = cmd.ExecuteReader();
            DateTime finalTime = new DateTime();
            DateTime initialTime = new DateTime();


            
            while (rdr.Read())
            {

                finalTime = new DateTime();
                string chargingStatus = rdr.GetString(2);
                int flag = 0;


                if (chargingStatus == "Online")
                {
                    flag = 1;
                    if (chargingStatus == "Online" && rdr.GetFloat(1) == 1.00)
                    {
                        flag = 2;
                        initialTime = rdr.GetDateTime(3);
                        finalTime = initialTime;
                    }


                    while (rdr.Read())
                    {
                        chargingStatus = rdr.GetString(2);
                        float charge = rdr.GetFloat(1);
                        if (chargingStatus == "Online" && flag == 1 && charge == 1.00)
                        {
                            initialTime = rdr.GetDateTime(3);
                            finalTime = rdr.GetDateTime(3);
                            flag = 2;
                        }
                        else if (flag == 2 && chargingStatus == "Online")
                        {
                            finalTime = rdr.GetDateTime(3);
                        }

                        if (chargingStatus == "Offline")
                            break;
                    }

                }

                if ((finalTime - initialTime).Minutes >= 30 && flag == 2)
                {
                    counter[0].BadCount++;
                }
                else if ((finalTime - initialTime).Minutes < 30 && flag == 2)
                {
                    counter[0].OptimalCount++;

                }
                else if (flag == 1)
                {
                    counter[0].SpotCount++;
                }
                flag = 0;

            }

            return counter;

        }
    }
}