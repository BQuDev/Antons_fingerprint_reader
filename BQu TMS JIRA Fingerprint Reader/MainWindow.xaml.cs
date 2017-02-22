using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FingerprintServices;
using System.Windows.Threading;
using System.Data;

namespace BQu_TMS_JIRA_Fingerprint_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FingerprintConfiguration fingerConfig = FingerprintConfiguration.Instance();
        DataAccessServices daServices = new DataAccessServices();
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer datetimer = new DispatcherTimer();
        Speaker speaker;
        public List<Item> dataList { get; set; }
        public static DataTable attendance;

        public MainWindow()
        {
            InitializeComponent();
            DisplayClock();
            DisplayDay();
            identifyAttendance();
        }

        private void identifyAttendance()
        {
            speaker = new Speaker();
            Attendance.MessageReceived += messagetext =>
            {
                 status_lable.Content = messagetext;
            };

            Attendance.SpeakerReceived += speach =>
            {
                speaker.speachThis(speach);
            };
            //192.168.1.62 wifi
            //124.43.24.147
            //192.168.0.14 wired
            bool connect = fingerConfig.connectViaIP("192.168.0.16", 4370);
            if (connect)
            {
                uploadLogRecordsToDatbase();
                fingerConfig.stardIdentifying();
            }
        }

        private void uploadLogRecordsToDatbase()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            fingerConfig.syncAttendance();
            Mouse.OverrideCursor = null;
        }

        private void DisplayClock()
        {
            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Start();
            timer.Tick += new EventHandler(delegate(object s, EventArgs a)
            {

                if (DateTime.Now.ToString("hh:mm:ss tt") == "06:30:00 AM")
                {
                    //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                   // Application.Current.Shutdown();
                    //Environment.Exit(0);
                    DisplayDay();
                    LoadAttendedEmployeeList();
                }
                else
                {
                    Time_Lable.Content = DateTime.Now.ToString("hh:mm:ss tt");
                }
                

            });
        }

        private void DisplayDay()
        {           
            Date_lable.Content = DateTime.Now.ToString("dddd dd/MM/yyyy");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            //this.WindowState = WindowState.;
        }
        

        #region system tray icon

        private WindowState lastWindowState;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.lastWindowState = WindowState;
            //this.Hide();
        }

        private void OnNotifyIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Show();
                this.WindowState = this.lastWindowState;
            }
        }
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = this.lastWindowState;
        }


        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            try
            {
                fingerConfig.disconnect();
            }

            catch (Exception)
            {
            }

            Environment.Exit(0);
        }
        #endregion

        private void hideAllGrids()
        {
            Home_Grid.Visibility = Visibility.Hidden;
            DateTime_Grid.Visibility = Visibility.Hidden;
            TopNavigator_Attendence_Grid.Visibility = Visibility.Hidden;
            TopNavigator_Registration_Grid.Visibility = Visibility.Hidden;
            Student_Registration_Grid.Visibility = Visibility.Hidden;
            TopNavigator_Registration_And_Attendence_Grid.Visibility = Visibility.Hidden;
            Attendance_management_Grid.Visibility = Visibility.Hidden;
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hideAllGrids();
            Home_Grid.Visibility = Visibility.Visible;
            DateTime_Grid.Visibility = Visibility.Visible;
            status_lable.Content = "";
        }

        #region home section eliments

        private void Attendence_Management_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hideAllGrids();
            TopNavigator_Registration_Grid.Visibility = Visibility.Visible;
            Attendance_management_Grid.Visibility = Visibility.Visible;
            LoadAttendedEmployeeList();
        }

        private void LoadAttendedEmployeeList()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
               // ListBoxConverter.ItemsSource = "";
                //Dictionary<string, string> todayAttendedEmployees = daServices.todayAttendedEmployees(DateTime.Now);
                attendance = daServices.todayAttendedEmployees(DateTime.Now);

                dataList = new List<Item>();
                Item newItem;
                if (attendance.Rows.Count > 0)
                {
                    for (int i = 0; i < attendance.Rows.Count; i++)
                    {
                        DataRow row = attendance.Select()[i];

                        Employee employee = daServices.getEmployeebyEmployeeID(row["employee_id"].ToString());
                        newItem = new Item();
                        newItem.Name = employee.FirstName;
                        newItem.date = DateTime.Now.ToShortDateString();
                        newItem.SignInTime = row["in_time"].ToString();
                        newItem.SignOutTime = row["out_time"].ToString();
                        dataList.Add(newItem);
                    }
                }

                attendance_listview.ItemsSource = dataList;
               // ListBoxConverter.ItemsSource = dataList;
               // this.ListBoxConverter.DataContext = this;
                Mouse.OverrideCursor = null;
            }
            catch(ArgumentException)
            {
                Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show("Two or more records for same employee for today.", "Graph error", MessageBoxButton.OK, MessageBoxImage.Error);
                hideAllGrids();
                Home_Grid.Visibility = Visibility.Visible;
                DateTime_Grid.Visibility = Visibility.Visible;
            }
        }

        private void Student_Registration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (ApplicationUtilities.CheckConnection())
            {
                showStudentRegistrationGrid();
                status_lable.Content = "";
            }
            else
            {
                System.Windows.MessageBox.Show("Cannot connect to the network, Check your network connection", "Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

        private void Tools_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion


        #region registration section eliments

        private void Retreive_Student_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((student_id.Text != String.Empty) || (first_name.Text != String.Empty) || (last_name.Text != String.Empty))
            {
                SearchResults searchresults = new SearchResults(student_id.Text, first_name.Text, last_name.Text);
                Mouse.OverrideCursor = null;
                searchresults.ShowDialog();
            }
            else
            {
                MessageBox.Show("Empty employee names or employee id", "Empty fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                Mouse.OverrideCursor = null;
            }

        }

        private void showStudentRegistrationGrid()
        {
            hideAllGrids();
            Student_Registration_Grid.Visibility = Visibility.Visible;
            TopNavigator_Attendence_Grid.Visibility = Visibility.Visible;
        }

        #endregion

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    this.Visibility = Visibility.Hidden;
                    break;
            }

        }

    }
}
