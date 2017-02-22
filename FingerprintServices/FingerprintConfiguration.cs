using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zkemkeeper;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Windows.Input;
using System.Data;

namespace FingerprintServices
{
    public class FingerprintConfiguration
    {
        CZKEMClass ZkFingerprint = new CZKEMClass();
        Dictionary<int, string> errorCodes = new Dictionary<int, string>();
        Attendance attendance = new Attendance();
        Registrar registrar = new Registrar();
        //string sCom = "";
        string previousUser = "";
        public bool IsConnect = false;
        private int iMachineNumber = 1;

        private static FingerprintConfiguration _instance;

        public static FingerprintConfiguration Instance()
        {
            if (_instance == null)
            {
                _instance = new FingerprintConfiguration();
            }
            return _instance;
        }
        public FingerprintConfiguration()
        {
            errorCodes.Add(-100, "Operation failed or data not exist");
            errorCodes.Add(-10, "Transmitted data length is incorrect");
            errorCodes.Add(-5, "Data already exists");
            errorCodes.Add(-4, "Space is not enough");
            errorCodes.Add(-3, "Error size");
            errorCodes.Add(-2, "Error in file read/write");
            errorCodes.Add(-1, "SDK is not initialized and needs to be reconnected");
            errorCodes.Add(0, "Data not found or data repeated");
            errorCodes.Add(1, "Operation is correct");
            errorCodes.Add(4, "Parameter is incorrect");
            errorCodes.Add(101, "Error in allocating buffer");
        }


        public void stardIdentifying()
        {
            this.ZkFingerprint.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(ZkFingerprint_OnVerify);
            this.ZkFingerprint.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(ZkFingerprint_OnAttTransactionEx);
        }


        public bool connectViaIP(string ipAddress, int port)
        {
            if (!IsConnect)
            {
                ZkFingerprint.Disconnect();
                int errorcode = 0;
                IsConnect = ZkFingerprint.Connect_Net(ipAddress, port);
                if (IsConnect == true)
                {
                   bool val = ZkFingerprint.RegEvent(iMachineNumber, 65535);
                    return true;
                }
                else
                {
                    ZkFingerprint.GetLastError(ref errorcode);
                    bool isReturnError = false;
                    foreach (KeyValuePair<int, string> error in errorCodes)
                    {
                        if (error.Key == errorcode)
                        {
                            attendance.MessageDisplayer("Unable to connect the device,\nDevice returns an error:" + error.Value.ToString(), 0);
                           // MessageBox.Show("Unable to connect the device,\nDevice returns an error:" + error.Value.ToString(), "Connection Failure", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            isReturnError = true;
                            break;
                        }
                    }

                    if (!isReturnError)
                    {
                        attendance.MessageDisplayer("Unable to connect the device, Device returns an error.\nPlease restart the device and try again", 0);
                    }

                    return false;
                }
            }
            else
            {
                return true;// already connected
            }
        }

        public void disconnect()
        {
            if (IsConnect == true)
            {
                ZkFingerprint.Disconnect();
                this.ZkFingerprint.OnEnrollFingerEx -= new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(ZkFingerprint_OnEnrollFinger);
                this.ZkFingerprint.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(ZkFingerprint_OnVerify);
                this.ZkFingerprint.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(ZkFingerprint_OnAttTransactionEx);
            }

        }

        //internal DataTable getLogData()
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("eid");
        //    dt.Columns.Add("name");
        //    dt.Columns.Add("log_date_time");
        //    if (IsConnect == true)
        //    {
        //        string Name = "";
        //        string pass = "";
        //        int privilig = 0;
        //        bool enabilyty = true;
        //        string sdwEnrollNumber = "";
        //        int VerifyMode = 0;
        //        int InOutMode = 0;
        //        int Year = 0;
        //        int Month = 0;
        //        int Day = 0;
        //        int Hour = 0;
        //        int Minute = 0;
        //        int Second = 0;
        //        int Workcode = 0;

        //        int idwErrorCode = 0;
        //        DataRow newLogRow;
        //        ZkFingerprint.EnableDevice(iMachineNumber, false);//disable the device
        //        if (ZkFingerprint.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
        //        {
        //            while (ZkFingerprint.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out VerifyMode,
        //                       out InOutMode, out Year, out Month, out Day, out Hour, out Minute, out Second, ref Workcode))//get records from the memory
        //            {
        //                //iMachineNumber, sdwEnrollNumber, Name, pass, privilig, enabilyty);
        //                ZkFingerprint.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out Name, out pass, out privilig, out enabilyty);
        //                newLogRow = dt.NewRow();
        //                newLogRow["eid"] = sdwEnrollNumber;
        //                newLogRow["name"] = Name;
        //                newLogRow["log_date_time"] = new DateTime(Year, Month, Day, Hour, Minute, Second);
        //                dt.Rows.Add(newLogRow);
        //            }
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);

        //            if (idwErrorCode != 0)
        //            {
        //                newLogRow = dt.NewRow();
        //                newLogRow["eid"] = "ERROR:Reading Fail";
        //                newLogRow["log_date_time"] = new DateTime();
        //                dt.Rows.Add(newLogRow);

        //            }
        //            else
        //            {
        //                newLogRow = dt.NewRow();
        //                newLogRow["eid"] = "ERROR:No Data";
        //                newLogRow["log_date_time"] = new DateTime();
        //                dt.Rows.Add(newLogRow);
        //            }
        //        }
        //        ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
        //    }
        //    return dt;
        //}

        //internal int getAvailableRecordCount()
        //{
        //    if (IsConnect == true)
        //    {
        //        int idwErrorCode = 0;
        //        int iValue = 0;

        //        ZkFingerprint.EnableDevice(iMachineNumber, false);//disable the device
        //        if (ZkFingerprint.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
        //        {
        //            ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
        //            return iValue;
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);
        //            ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
        //            bool isReturnError = false;
        //            foreach (KeyValuePair<int, string> error in errorCodes)
        //            {
        //                if (error.Key == idwErrorCode)
        //                {
        //                    MessageBox.Show("Unable to connect the device, \nDevice returns an error:" + error.Value.ToString(), "USB Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        //                    isReturnError = true;
        //                    break;
        //                }
        //            }

        //            if (!isReturnError)
        //            {
        //                MessageBox.Show("Unable to connect the device, Device returns an error.\nPlease restart the device and try again", "USB Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //            return -5;
        //        }
        //    }
        //    else
        //    {
        //        return -5;
        //    }

        //}


        //internal bool clearAttendanceLogdata()
        //{
        //    if (IsConnect == true)
        //    {
        //        int idwErrorCode = 0;
        //        ZkFingerprint.EnableDevice(iMachineNumber, false);//disable the device
        //        if (ZkFingerprint.ClearGLog(iMachineNumber))
        //        {
        //            ZkFingerprint.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //            ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
        //            return true;
        //            //MessageBox.Show("All att Logs have been cleared from terminal!", "Success");
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);
        //            ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
        //            return false;
        //            //MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //        }
        //    }
        //    else
        //    {
        //        return false;// machine not connected
        //    }
        //}

        //internal bool deleteAllUserData()
        //{
        //    if (IsConnect == true)
        //    {
        //        int iDataFlag = 5;// delete all user and his fingerprint data (5)
        //        int idwErrorCode = 0;
        //        if (ZkFingerprint.ClearData(iMachineNumber, iDataFlag))
        //        {
        //            ZkFingerprint.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //            return true;
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;// machine not connected
        //    }

        //}

        //internal string setConnectedDeviceSerialNumber()
        //{
        //    int idwErrorCode = 0;
        //    string sdwSerialNumber = "";

        //    if (ZkFingerprint.GetSerialNumber(iMachineNumber, out sdwSerialNumber))
        //    {
        //        return sdwSerialNumber;
        //    }
        //    else
        //    {
        //        ZkFingerprint.GetLastError(ref idwErrorCode);
        //        return idwErrorCode.ToString();
        //    }
        //}

        //internal bool enrollEmployeeByEmployeeID(string EmployeeID, string EmployeeFname)
        //{
        //    if (IsConnect == true)
        //    { 
        //        int idwErrorCode = 0;
        //        int iUserID = Convert.ToInt32(EmployeeID.Trim());
        //        int iFingerIndex = 0;
        //        int iFlag = 1;
        //        ZkFingerprint.CancelOperation();
        //        ZkFingerprint.SSR_DelUserTmpExt(iMachineNumber, EmployeeID, iFingerIndex);//If the specified index of user's templates has existed ,delete it first.(SSR_DelUserTmp is also available sometimes)
        //        ZkFingerprint.RefreshData(iMachineNumber);
        //        if (ZkFingerprint.SSR_SetUserInfo(iMachineNumber, EmployeeID, EmployeeFname, "", 0, true))//upload user information to the device
        //        {
        //            if (ZkFingerprint.StartEnrollEx(EmployeeID, iFingerIndex, iFlag))
        //            {
        //                ZkFingerprint.RegEvent(iMachineNumber, 8);
        //                this.ZkFingerprint.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(ZkFingerprint_OnEnrollFinger);
        //                //ZkFingerprint.StartIdentify();//After enrolling templates,you should let the device into the 1:N verification condition
        //                return true;
        //            }
        //            else
        //            {
        //                ZkFingerprint.GetLastError(ref idwErrorCode);
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //internal void reEnrollEmployeeByEmployeeID(string EmployeeID, string name)
        //{
        //    if (IsConnect == true)
        //    {
        //        int idwErrorCode = 0;
        //        int iUserID = Convert.ToInt32(EmployeeID.Trim());
        //        int iFingerIndex = 0;
        //        int iFlag = 1;
        //        ZkFingerprint.SSR_DelUserTmpExt(iMachineNumber, EmployeeID, iFingerIndex);
        //        ZkFingerprint.RefreshData(iMachineNumber);
        //        if (ZkFingerprint.StartEnrollEx(EmployeeID, iFingerIndex, iFlag))
        //        {
        //            ZkFingerprint.RegEvent(iMachineNumber, 8);
        //            this.ZkFingerprint.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(ZkFingerprint_OnEnrollFinger);
        //            //ZkFingerprint.StartIdentify();//After enrolling templates,you should let the device into the 1:N verification condition
        //        }
        //        else
        //        {
        //            ZkFingerprint.GetLastError(ref idwErrorCode);
        //        }
        //    }
        //}

        ////When you are enrolling your finger,this event will be triggered.

        public bool enrollEmployeeByEmployeeID(string employeeID, string employeeFname)
        {
            if (IsConnect == true)
            {
                int idwErrorCode = 0;
                int iUserID = Convert.ToInt32(employeeID.Trim());
                int iFingerIndex = 0;
                int iFlag = 1;
                ZkFingerprint.CancelOperation();
                ZkFingerprint.SSR_DelUserTmpExt(iMachineNumber, employeeID, iFingerIndex);//If the specified index of user's templates has existed ,delete it first.(SSR_DelUserTmp is also available sometimes)
                ZkFingerprint.RefreshData(iMachineNumber);
                if (ZkFingerprint.SSR_SetUserInfo(iMachineNumber, employeeID, employeeFname, "", 0, true))//upload user information to the device
                {
                    if (ZkFingerprint.StartEnrollEx(employeeID, iFingerIndex, iFlag))
                    {
                        ZkFingerprint.RegEvent(iMachineNumber, 8);
                        this.ZkFingerprint.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(ZkFingerprint_OnEnrollFinger);
                        //ZkFingerprint.StartIdentify();//After enrolling templates,you should let the device into the 1:N verification condition
                        return true;
                    }
                    else
                    {
                        ZkFingerprint.GetLastError(ref idwErrorCode);
                        return false;
                    }
                }
                else
                {
                    ZkFingerprint.GetLastError(ref idwErrorCode);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void ZkFingerprint_OnEnrollFinger(string iEnrollNumber, int iFingerIndex, int iActionResult, int iTemplateLength)
        {
            if (iActionResult == 0 && previousUser != iEnrollNumber)
            {
                string fingerprintdata = getEmployeeFingerprintDetails(iEnrollNumber);
                if (registrar.registerEmployee(iEnrollNumber, fingerprintdata))
                {
                    ZkFingerprint.WriteLCD(5, 5, " Registration successfully completed !");
                    ZkFingerprint.PlayVoiceByIndex(9);
                    previousUser = iEnrollNumber;
                    ZkFingerprint.EnableClock(1);
                }
                else
                {
                    ZkFingerprint.SSR_DelUserTmpExt(iMachineNumber, iEnrollNumber, iFingerIndex);
                    ZkFingerprint.RefreshData(iMachineNumber);
                    ZkFingerprint.WriteLCD(5, 5, "Registration not successful, Try again please");
                    ZkFingerprint.PlayVoiceByIndex(9);
                }
            }
            else if (iActionResult != 0)
            {
                ZkFingerprint.WriteLCD(5, 5, "Registration not successful, Try again please");
                ZkFingerprint.PlayVoiceByIndex(9);
            }

        }

        internal string getEmployeeFingerprintDetails(string EmployeeID)
        {
            string byTmpData = "";
            if (IsConnect == true)
            {
                int FingerIndex = 0;
                int iFlag = 0;
                int iTmpLength = 0;
                ZkFingerprint.EnableDevice(iMachineNumber, false);
                ZkFingerprint.ReadAllTemplate(iMachineNumber);
                ZkFingerprint.GetUserTmpExStr(iMachineNumber, EmployeeID, FingerIndex, out iFlag, out byTmpData, out iTmpLength);
                ZkFingerprint.RefreshData(iMachineNumber);//the data in the device should be refreshed
                ZkFingerprint.EnableDevice(iMachineNumber, true);
            }
            return byTmpData;
        }


        //internal bool uploadAllEmployeesDataToTheDevice()
        //{
        //    List<Employee> fingerprintAvailableEmployees = repositories.getFingerprintAvailbleEmployees();
        //    int iUpdateFlag = 1;//overwrite existing fingerprints
        //    int idwErrorCode = 0;//error code
        //    int iFingerIndex = 0;
        //    int iFlag = 0;
        //    ZkFingerprint.ClearData(iMachineNumber, 5);//delete all first
        //    ZkFingerprint.EnableDevice(iMachineNumber, false);
        //    if (ZkFingerprint.BeginBatchUpdate(iMachineNumber, iUpdateFlag))//create memory space for batching data
        //    {
        //        foreach (Employee emp in fingerprintAvailableEmployees)
        //        {
        //            if (ZkFingerprint.SSR_SetUserInfo(iMachineNumber, emp.EmployeeId.ToString(), emp.FirstName.ToString(), "", 0, true))//upload user information to the device
        //            {
        //                ZkFingerprint.SetUserTmpExStr(iMachineNumber, emp.EmployeeId.ToString(), iFingerIndex, iFlag, emp.FingerprintData.ToString());//upload templates information to the memory
        //            }
        //            else
        //            {
        //                ZkFingerprint.GetLastError(ref idwErrorCode);
        //                ZkFingerprint.EnableDevice(iMachineNumber, true);
        //                return false;
        //            }
        //        }
        //    }
        //    ZkFingerprint.BatchUpdate(iMachineNumber);//upload all the information in the memory
        //    ZkFingerprint.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //    ZkFingerprint.EnableDevice(iMachineNumber, true);
        //    return true;
        //}

        internal bool restartDevice()
        {
            if (IsConnect == true)
            {
                if (ZkFingerprint.RestartDevice(iMachineNumber))
                {
                    return true;
                }
                else
                {
                    //error occurred
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        internal bool powerOffDevice()
        {
            if (IsConnect == true)
            {
                if (ZkFingerprint.PowerOffDevice(iMachineNumber))
                {
                    return true;
                }
                else
                {
                    //error occurred
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal bool setDeviceTimeAsPCTime()
        {
            if (IsConnect == true)
            {
                if (ZkFingerprint.SetDeviceTime(iMachineNumber))
                {
                    ZkFingerprint.RefreshData(iMachineNumber);
                    return true;
                }
                else
                {
                    //error occurred
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal string getDeviceCurrentTime()
        {
            string currentTime = "";
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;

            if (ZkFingerprint.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))
            {
                currentTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
            }
            return currentTime;
        }

        private void ZkFingerprint_OnVerify(int iUserID)
        {
            if (iUserID != -1)
            {
                //if success do nothing it will handle by ZkFingerprint_OnAttTransactionEx
            }
            else
            {
                attendance.MessageDisplayer("Verified Failed, Try again",1);
            }
        }

        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void ZkFingerprint_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            attendance.EmployeeSignedInSignedOut(sEnrollNumber, iYear.ToString(), iMonth.ToString("00"), iDay.ToString("00"), iHour.ToString("00"), iMinute.ToString("00"), iSecond.ToString("00"));
        }

        public void syncAttendance()
        {
            DataTable dataTable = getLogData2();
            bool success = false;
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["Employee_number"].ToString() == "ERROR:Reading Fail")
                {
                    attendance.MessageDisplayer("Cannot read the data from the fingerprint terminal, Please try again", 0);
                    return;
                }

                if (row["Employee_number"].ToString() == "ERROR:No Data")
                {
                    attendance.MessageDisplayer("No log data in the fingerprint terminal", 0);
                    return;
                }

                success = attendance.SyncSignedInSignedOut(row["Employee_number"].ToString(), row["Year"].ToString(), row["Month"].ToString(), row["Day"].ToString(), row["Hour"].ToString(), row["Minute"].ToString(), row["Second"].ToString());
            }

            if (success)
            {
                clearAttendanceLogdata();
                attendance.MessageDisplayer("Sync success", 1);
            }
            else
            {
                attendance.MessageDisplayer("Sync Failed", 1);
            }

        }

        internal bool clearAttendanceLogdata()
        {
            if (IsConnect == true)
            {
                int idwErrorCode = 0;
                ZkFingerprint.EnableDevice(iMachineNumber, false);//disable the device
                if (ZkFingerprint.ClearGLog(iMachineNumber))
                {
                    ZkFingerprint.RefreshData(iMachineNumber);//the data in the device should be refreshed
                    ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
                    return true;
                    //MessageBox.Show("All att Logs have been cleared from terminal!", "Success");
                }
                else
                {
                    ZkFingerprint.GetLastError(ref idwErrorCode);
                    ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
                    return false;
                    //MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                }
            }
            else
            {
                return false;// machine not connected
            }
        }


        internal DataTable getLogData2()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Employee_number");
            dt.Columns.Add("Year");
            dt.Columns.Add("Month");
            dt.Columns.Add("Day");
            dt.Columns.Add("Hour");
            dt.Columns.Add("Minute");
            dt.Columns.Add("Second");
            if (IsConnect == true)
            {
                //string Name ="";
                //string pass = "";
                //int privilig = 0;
                //    bool enabilyty = true;
                int idwTMachineNumber = 0;
                int EnrollNumber = 0;
                int idwEMachineNumber = 0;
                string sdwEnrollNumber = "";
                int VerifyMode = 0;
                int InOutMode = 0;
                int Year = 0;
                int Month = 0;
                int Day = 0;
                int Hour = 0;
                int Minute = 0;
                int Second = 0;
                int Workcode = 0;

                int idwErrorCode = 0;
                DataRow newLogRow;
                ZkFingerprint.EnableDevice(iMachineNumber, false);//disable the device
                if (ZkFingerprint.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
                {
                    while (ZkFingerprint.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out VerifyMode,
                               out InOutMode, out Year, out Month, out Day, out Hour, out Minute, out Second, ref Workcode))//get records from the memory
                    {
                        //iMachineNumber, sdwEnrollNumber, Name, pass, privilig, enabilyty);
                        //ZkFingerprint.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out Name, out pass, out privilig, out enabilyty);
                        newLogRow = dt.NewRow();
                        newLogRow["Employee_number"] = sdwEnrollNumber.ToString();
                        newLogRow["Year"] = Year;
                        newLogRow["Month"] = Month.ToString("00");
                        newLogRow["Day"] = Day.ToString("00");
                        newLogRow["Hour"] = Hour.ToString("00");
                        newLogRow["Minute"] = Minute.ToString("00");
                        newLogRow["Second"] = Second.ToString("00");
                        dt.Rows.Add(newLogRow);
                    }
                }
                else
                {
                    ZkFingerprint.GetLastError(ref idwErrorCode);

                    if (idwErrorCode != 0)
                    {
                        newLogRow = dt.NewRow();
                        newLogRow["Employee_number"] = "ERROR:Reading Fail";
                        dt.Rows.Add(newLogRow);

                    }
                    else
                    {
                        newLogRow = dt.NewRow();
                        newLogRow["Employee_number"] = "ERROR:No Data";
                        dt.Rows.Add(newLogRow);
                    }
                }
                ZkFingerprint.EnableDevice(iMachineNumber, true);//enable the device
            }
            return dt;
        }
    }
}
