using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FingerprintServices
{
    public class Attendance
    {
        public static event Action<string> MessageReceived;
        public static event Action<string> SpeakerReceived;
        DataAccessServices dataAccess = new DataAccessServices();

        internal static void Broadcast(string message, bool voice)
        {
            if (MessageReceived != null)
            {
                MessageReceived(message);
            }

            if (voice)
            {
                if (SpeakerReceived != null)
                {
                    SpeakerReceived(message);
                }
            }

        }

        internal void MessageDisplayer(string meaasage, int status)
        {
            if (status == 0)
            {
                Broadcast(meaasage,false);
            }
            else if (status == 1)
            {
                Broadcast(meaasage,true);
            }
        }

        internal void EmployeeSignedInSignedOut(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            Employee employee = dataAccess.getEmployeebyEmployeeID(employeeID);
            bool signedIn = dataAccess.isEmployeeAlreadySignedInForTheDay(employeeID, Year, Month, Day);

            if (signedIn)// employee signed in  for the day and ....
            {
                bool signedOut = dataAccess.isEmployeeAlreadySignedOutForTheDay(employeeID, Year, Month, Day);

                if (signedOut) // employee was away from office and come again in to the office 
                {
                   TimeSpan timeSpentAwayFromOffice = dataAccess.calculateAwayTimeFromOffice(employeeID, Year, Month, Day, Hour, Minute, Second);
                   dataAccess.updateSignedOutTimeAndTimeSpentAwayFromOffice(employeeID, Year, Month, Day, timeSpentAwayFromOffice);
                   MessageDisplayer(employee.FirstName + " is back to office", 1);
                }
                else // employee is signing out for the day
                {
                    dataAccess.signedOffEmployeeForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
                    MessageDisplayer(employee.FirstName + " signed out", 1);
                }       
            }
            else // employee gonna signed in for the day
            {
                dataAccess.signedInEmployeeForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
                MessageDisplayer(employee.FirstName + " signed in", 1);
            }
        }

        internal bool SyncSignedInSignedOut(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
        {
            try
            {
                Employee employee = dataAccess.getEmployeebyEmployeeID(employeeID);
                bool signedIn = dataAccess.isEmployeeAlreadySignedInForTheDay(employeeID, Year, Month, Day);

                if (signedIn)// employee signed in  for the day and ....
                {
                    bool signedOut = dataAccess.isEmployeeAlreadySignedOutForTheDay(employeeID, Year, Month, Day);

                    if (signedOut) // employee was away from office and come again in to the office 
                    {
                        //TimeSpan timeSpentAwayFromOffice = dataAccess.calculateAwayTimeFromOffice(employeeID, Year, Month, Day, Hour, Minute, Second);

                        //dataAccess.updateSignedOutTimeAndTimeSpentAwayFromOffice(employeeID, Year, Month, Day, timeSpentAwayFromOffice.Duration());
                        dataAccess.signedInEmployeeForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
                    }
                    else // employee is signing out for the day
                    {
                        dataAccess.signedOffEmployeeForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);

                    }
                }
                else // employee gonna signed in for the day
                {
                    dataAccess.signedInEmployeeForTheDay(employeeID, Year, Month, Day, Hour, Minute, Second);
                }
                return true;
            }
            catch
            {
                MessageDisplayer("Sync failed", 1);
                return false;
            }
        }
    }
}
