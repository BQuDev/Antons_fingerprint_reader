using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Odbc;
using System.Windows.Forms;
using System.Configuration;

namespace FingerprintDatabase
{
    public class DBConnections
    {
        public static OdbcConnection MyConnection;

        public DBConnections()
        {
            InitializeConnnections();
        }

        /// <summary>
        /// Initializes the database connection 
        /// </summary>
        public void InitializeConnnections()
        {
            try
            {
                MyConnection = new OdbcConnection("DRIVER={MySQL ODBC 5.1 Driver};SERVER=" + "213.171.200.78" + ";PORT=" + "3306" + ";DATABASE=" + "anton" + ";UID=" + "anton123" + ";PASSWORD=" + "BQU@123456789" + ";OPTION=3");
            }
            catch (Exception)
            {
                MessageBox.Show("Please install MySQL ODBC 5.1 Driver", "Drivers Not Install", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        //create server connection
        public bool Connect()
        {
            try
            {
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    MyConnection.Open();
                    return true;
                }
            }
            catch (OdbcException MyOdbcException)
            {
                for (int i = 0; i < MyOdbcException.Errors.Count; i++)
                {
                    if (MyOdbcException.Errors[i].SQLState == "HY000")
                    {
                        MessageBox.Show("Unable to connect to the server.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    else
                    {
                        MessageBox.Show(MyOdbcException.Errors[i].Message + "\n" + " Please contact your system administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return false;
            }
        }
    }
}
