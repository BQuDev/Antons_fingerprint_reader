using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BQu_TMS_JIRA_Fingerprint_Reader
{
    public class ImageConverter : IValueConverter
    {
        ApplicationUtilities utilities =  new ApplicationUtilities();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return utilities.GetApplicationDataPath()+"\\users\\"+value.ToString() + ".png";
            //return "./Images/" + value.ToString() + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Item
    {
        ApplicationUtilities utilities = new ApplicationUtilities();

        public int PictureID { get; set; }
        public string Name { get; set; }
        public string date { get; set; }
        public string SignInTime { get; set; }
        public string SignOutTime { get; set; }
        public string PictureString
        {
            get { return utilities.GetApplicationDataPath() + "\\users\\" + PictureID.ToString() + ".png"; }
        }
    }
}
