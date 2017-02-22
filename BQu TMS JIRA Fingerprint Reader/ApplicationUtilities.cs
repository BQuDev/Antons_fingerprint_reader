using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Net.Sockets;

namespace BQu_TMS_JIRA_Fingerprint_Reader
{
    public class ApplicationUtilities
    {
        ///translator::::: www2.research.att.com/~ttsweb/tts/demo.php
        public void playSound(string id,string status)
        {
            MediaPlayer mediaplayer = new MediaPlayer();

            Uri reading;
            if(status =="in")
            {
                reading = new Uri(GetApplicationDataPath() + "\\Sound\\signIn\\" + id + ".wav");
            }
            else
            {
                reading = new Uri(GetApplicationDataPath() + "\\Sound\\signOut\\" + id + ".wav");
            }

            mediaplayer.Open(reading);
            mediaplayer.Volume = 100;
            mediaplayer.Play();
        }

        public string GetApplicationDataPath()
        {
            try
            {
                string dir = string.Empty;
                dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dir = Path.Combine(dir, "TMS Fingerprint");
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show("Application data folder not found ,  Reinstall the application", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                return dir;

            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred while finding application data folder, \n " + e.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return string.Empty;
            }

        }

        internal static bool CheckConnection()
        {
            try
            {
                //////////////check connection
                TcpClient clnt = new TcpClient("213.171.200.70", 3306);
                clnt.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
