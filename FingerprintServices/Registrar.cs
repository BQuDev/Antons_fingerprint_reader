using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FingerprintServices
{
    public class Registrar
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
                Broadcast(meaasage, false);
            }
            else if (status == 1)
            {
                Broadcast(meaasage, true);
            }
        }

        internal bool registerEmployee(string employeeID, string fingerprintdata)
        {
            Employee employee = dataAccess.getEmployeebyEmployeeID(employeeID);
            employee.EmployeeNumber = employeeID;
            employee.FingerprintData = fingerprintdata;

            bool status = dataAccess.updateEmployee(employee);
            if (status)
            {
                MessageDisplayer(employee.FirstName + " successfully registered", 1);
                return true;
            }
            else
            {
                MessageDisplayer(employee.FirstName + " registration failed", 1);
                return false;
            }
        }
    }
}
