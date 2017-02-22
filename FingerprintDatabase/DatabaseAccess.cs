using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Data.Odbc;

namespace FingerprintDatabase
{
    public class DatabaseAccess
    {
        DBConnections dbConnection = new DBConnections();
        OdbcCommand MyCommand;
        OdbcDataAdapter Adapter;

        public DataTable getAllEmployees()
        {
            dbConnection.Connect();
            DataTable employeeTable = new DataTable();
            string query = "SELECT * FROM tms_employee";
            MyCommand = new OdbcCommand(query, DBConnections.MyConnection);
            Adapter = new OdbcDataAdapter();
            Adapter.SelectCommand = MyCommand;
            Adapter.Fill(employeeTable);
            DBConnections.MyConnection.Close();
            return employeeTable;
        }

        public int getEmployeeNumberOfLogingRecordsForTheDay(string employeeID, string Year, string Month, string Day)
        {
            int RowCountServer = 0;
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "SELECT COUNT(employee_id) FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month +" AND date_day =" + Day ;
            MyCommand.CommandType = CommandType.Text;
            RowCountServer = Convert.ToInt32(MyCommand.ExecuteScalar());
            return RowCountServer;
        }

        public int getEmployeeNotLogOutRecordsForTheDay(string employeeID, string Year, string Month, string Day)
        {
            int RowCountServer = 0;
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "SELECT COUNT(employee_id) FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day + " AND (out_time = '' OR out_time IS NULL)";
            MyCommand.CommandType = CommandType.Text;
            RowCountServer = Convert.ToInt32(MyCommand.ExecuteScalar());
            return RowCountServer;
        }

        public DateTime getEmployeeSignedOutTimeForTheDay(string employeeID, string Year, string Month, string Day)
        {
            DateTime signedOffTime;
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "SELECT out_time FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
            MyCommand.CommandType = CommandType.Text;
            string times = (string)MyCommand.ExecuteScalar();
            if (times == "")
            {
                return new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), 0, 0, 0);
            }
            return signedOffTime= Convert.ToDateTime(times);
        }

        public TimeSpan getAlreadybeenAwayTime(string employeeID, string Year, string Month, string Day)
        {
            DateTime time;
            TimeSpan timeAway;
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "SELECT away_time FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
            MyCommand.CommandType = CommandType.Text;
            string times = (MyCommand.ExecuteScalar() == DBNull.Value) ? string.Empty : MyCommand.ExecuteScalar().ToString();
            if (times != "")
            {
                time = Convert.ToDateTime(times);
                timeAway = new TimeSpan(time.Hour, time.Minute, time.Second);
            }
            else
            {
                timeAway = new TimeSpan(0, 0, 0);
            }
            return timeAway;
        }

        public void deleteRecordForLateArrivalearlyDeparture(string employeeID, DateTime date, string recordType)
        {
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "DELETE FROM tms_lte_arr_erly_dep WHERE date='" + date.ToString("yyyy-MM-dd") + "' AND employee_id = " + employeeID + " AND type = '" + recordType + "'";
            MyCommand.CommandType = CommandType.Text;
            MyCommand.ExecuteNonQuery();
        }

        public void updateEmployeeSignedOutAndNewAwayTime(string employeeID, TimeSpan newAwayTime, string Year, string Month, string Day)
        {
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "update tms_logsheet set out_time='',away_time = '" + newAwayTime.Hours.ToString("00") + ":" + newAwayTime.Minutes.ToString("00") + "' where employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.ExecuteNonQuery();
            deleteRecordForLateArrivalearlyDeparture(employeeID, DateTime.Now, "Early departure");
        }

        public void updateEmployeeSignedOutForTheDay(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "update tms_logsheet set out_time='" + Hour + ":" + Minute + "' where employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
            MyCommand.CommandType = CommandType.Text;
            MyCommand.ExecuteNonQuery();
            LatearriArlyDep(employeeID,18, 0, Convert.ToInt32(Hour), Convert.ToInt32(Minute), "Early departure");
        }

        public void insertEmployeeSignedInForTheDay(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
           // if(! isEmployeeAlreaddySignedIn(employeeID,Year,Month,Day))
           // {
                dbConnection.Connect();
                string query = "INSERT INTO tms_logsheet (employee_id,date_year,date_month,date_day,in_time) VALUES (?,?,?,?,?)";
                MyCommand = DBConnections.MyConnection.CreateCommand();
                MyCommand.CommandText = query;
                MyCommand.Parameters.Add("?A", employeeID);
                MyCommand.Parameters.Add("?B", Year);
                MyCommand.Parameters.Add("?D", Month);
                MyCommand.Parameters.Add("?E", Day);
                MyCommand.Parameters.Add("?F", Hour+":"+Minute);
                MyCommand.ExecuteNonQuery();
                LatearriArlyDep(employeeID,Convert.ToInt32(Hour), Convert.ToInt32(Minute), 9, 0, "Late arrival");
            //}
        }

        public void LatearriArlyDep(string employeeID, int max_hh, int max_mm, int min_hh, int min_mm, string text)
        {
            //value ekak table 1ta yanna balapana eka max vidihata tharaganna.
            int defferance = 0;
            if (max_hh >= min_hh)
            {
                if (max_mm > min_mm)
                {
                    defferance = (max_mm - min_mm) + (max_hh - min_hh) * 60;
                }

                else if (max_mm < min_mm)
                {
                    defferance = ((max_mm + 60) - min_mm) + ((max_hh - 1) - min_hh) * 60;
                }

                if (defferance > 30 && defferance < 120)
                {
                    MyCommand = DBConnections.MyConnection.CreateCommand();
                    MyCommand.CommandText = "insert into tms_lte_arr_erly_dep(date,time,type,employee_id) values('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + defferance + "','" + text + "','" + employeeID + "')";
                    MyCommand.ExecuteNonQuery();
                }
            }
        }

        private bool isEmployeeAlreaddySignedIn(string employeeID, string Year, string Month, string Day)
        {
            int RowCountServer = 0;
            dbConnection.Connect();
            MyCommand = DBConnections.MyConnection.CreateCommand();
            MyCommand.CommandText = "SELECT COUNT(employee_id) FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
            MyCommand.CommandType = CommandType.Text;
            RowCountServer = Convert.ToInt32(MyCommand.ExecuteScalar());
            if (RowCountServer > 0)
            {
                return true;
            }

            return false;
        }
        
        public bool UpdateEmployee(int employee_id, string fingerprintdata)
        {
            try
            {
                dbConnection.Connect();
                string query = "Update tms_employee SET fingerprint_id ='" + employee_id + "' , fingerprint_data ='" + fingerprintdata + "' WHERE employee_id='" + employee_id + "' ";
                MyCommand = new OdbcCommand(query, DBConnections.MyConnection);
                MyCommand.ExecuteNonQuery();
                DBConnections.MyConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataTable getAttendedEmployeeDetailsByDate(DateTime dateTime)
        {
            dbConnection.Connect();
            DataTable employeeAttendedTable = new DataTable();
            string query = "SELECT employee_id,in_time,out_time FROM tms_logsheet where date_year =" + dateTime.Year + " AND date_month =" + dateTime.Month + " AND date_day =" + dateTime.Day;
            MyCommand = new OdbcCommand(query, DBConnections.MyConnection);
            Adapter = new OdbcDataAdapter();
            Adapter.SelectCommand = MyCommand;
            Adapter.Fill(employeeAttendedTable);
            DBConnections.MyConnection.Close();
            return employeeAttendedTable;
        }
    }
}
