using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FingerprintDatabase;

namespace FingerprintServices
{
    public class DataAccessServices
    {
        DatabaseAccess dbAccess = new DatabaseAccess();
        DataTable employeeTable;
        public DataAccessServices()
        {
            employeeTable = dbAccess.getAllEmployees();
        }

        public Employee getEmployeebyEmployeeID(string employeeID)
        {
            Employee Employee = new Employee();


            int numberOfRecords = employeeTable.Select("employee_id ='" + employeeID + "'").Length;
            if (numberOfRecords == 1)
            {
                foreach (DataRow row in employeeTable.Select("Employee_id ='" + employeeID + "'"))
                {
                    Employee.EmployeeId = (int)row["employee_id"];
                    Employee.EmployeeNumber = row["employee_number"].ToString();
                    Employee.FirstName = row["first_name"].ToString();
                    Employee.LastName = row["last_name"].ToString();
                    Employee.FingerprintID = row["fingerprint_id"].ToString();
                    Employee.FingerprintData = row["fingerprint_data"].ToString();
                }

                return Employee;
            }
            else
            {
                return null;
            }
        }

        internal bool isEmployeeAlreadySignedInForTheDay(string employeeID, string Year, string Month, string Day)
        {
            int numberOfRecords = dbAccess.getEmployeeNumberOfLogingRecordsForTheDay(employeeID, Year, Month, Day);
            if (numberOfRecords > 0)
            {
                return true;
            }
            return false;
        }


        internal bool isEmployeeAlreadySignedOutForTheDay(string employeeID, string Year, string Month, string Day)
        {
            int numberOfRecords = dbAccess.getEmployeeNotLogOutRecordsForTheDay(employeeID, Year, Month, Day);
            if (numberOfRecords > 0)
            {
                return false;
            }
            return true;
        }

        internal TimeSpan calculateAwayTimeFromOffice(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            DateTime nowTime = Convert.ToDateTime(Hour + ":" + Minute + ":" + Second);
            DateTime signedOutTime = dbAccess.getEmployeeSignedOutTimeForTheDay(employeeID, Year, Month, Day);

            TimeSpan span = nowTime.Subtract(signedOutTime);
            return span;
        }

        internal void updateSignedOutTimeAndTimeSpentAwayFromOffice(string employeeID, string Year, string Month, string Day, TimeSpan timeSpentAwayFromOffice)
        {
            //empty da signed out time and add the time spent time for existing time

            TimeSpan alreadyAwaytime = dbAccess.getAlreadybeenAwayTime(employeeID, Year, Month, Day);
            TimeSpan newAwayTime = alreadyAwaytime.Add(timeSpentAwayFromOffice);

            dbAccess.updateEmployeeSignedOutAndNewAwayTime(employeeID, newAwayTime, Year, Month, Day);
        }

        internal void signedOffEmployeeForTheDay(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            dbAccess.updateEmployeeSignedOutForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
        }

        internal void signedInEmployeeForTheDay(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            dbAccess.insertEmployeeSignedInForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
        }


        internal bool updateEmployee(Employee employee)
        {
            bool status = dbAccess.UpdateEmployee(employee.EmployeeId, employee.FingerprintData);
            return status;
        }

        public DataTable todayAttendedEmployees(DateTime dateTime)
        {
            DataTable employeeAttendTable = dbAccess.getAttendedEmployeeDetailsByDate(dateTime);
            //Dictionary<string, string> attendedEmployeeDetails = new Dictionary<string, string>();
            //foreach(DataRow row in employeeAttendTable.Rows)
            //{
            //    attendedEmployeeDetails.Add(row["employee_id"].ToString(), row["in_time"].ToString(), row["out_time"].ToString());
            //}

            return employeeAttendTable;

        }

        public List<Employee> searchStudents(string firstname, string employeeid, string lastname)
        {
            List<Employee> searchedEmployeesList = new List<Employee>();
            
            Employee employee;
            string condition;
            if (employeeid != "")
            {
                int numberOfRecords = employeeTable.AsEnumerable().Where(x => x["employee_id"].ToString() == employeeid).ToList().Count;
                if (numberOfRecords > 0)
                {
                    DataRow row = employeeTable.Select("employee_id ='" + employeeid + "'")[0];
                    employee = new Employee();
                    employee.EmployeeId = (int)row["employee_id"];
                    employee.EmployeeNumber = row["employee_number"].ToString();
                    employee.FirstName = row["first_name"].ToString();
                    employee.LastName = row["last_name"].ToString();
                    employee.FingerprintID = row["fingerprint_id"].ToString();
                    employee.FingerprintData = row["fingerprint_data"].ToString();
                    searchedEmployeesList.Add(employee);

                }
            }
            else
            {
                if (firstname != "")
                {
                    condition = "first_name LIKE '%" + firstname + "%'";
                }
                else
                {
                    condition = "last_name LIKE '%" + lastname + "%'";
                }
                int numberOfRecords = employeeTable.Select(condition).Length;
                if (numberOfRecords > 0)
                {
                    foreach (DataRow row in employeeTable.Select(condition))
                    {
                        employee = new Employee();
                        employee.EmployeeId = (int)row["employee_id"];
                        employee.EmployeeNumber = row["employee_number"].ToString();
                        employee.FirstName = row["first_name"].ToString();
                        employee.LastName = row["last_name"].ToString();
                        employee.FingerprintID = row["fingerprint_id"].ToString();
                        employee.FingerprintData = row["fingerprint_data"].ToString();

                        searchedEmployeesList.Add(employee);
                    }
                }

            }
            return searchedEmployeesList;
        }
    }
}
