using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FingerprintServices
{
    public class Employee
    {
        private int _Employee_Id;
        private string _first_Name;
        private string _last_Name;
        private string _Employee_Number;
        private string _fingerprintId;
        private string _fingerprintdata;

        public int EmployeeId
        {
            get { return _Employee_Id; }
            set { _Employee_Id = value; }
        }

        public string FirstName
        {
            get { return _first_Name; }
            set { _first_Name = value; }
        }

        public string LastName
        {
            get { return _last_Name; }
            set { _last_Name = value; }
        }

        public string EmployeeNumber
        {
            get { return _Employee_Number; }
            set { _Employee_Number = value; }
        }

        public string FingerprintID
        {
            get { return _fingerprintId; }
            set { _fingerprintId = value; }
        }

        public string FingerprintData
        {
            get { return _fingerprintdata; }
            set { _fingerprintdata = value; }
        }



    }
}
