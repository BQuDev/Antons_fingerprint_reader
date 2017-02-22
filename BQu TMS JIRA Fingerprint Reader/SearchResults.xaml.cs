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
using System.Windows.Shapes;
using FingerprintServices;

namespace BQu_TMS_JIRA_Fingerprint_Reader
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Window
    {
        FingerprintConfiguration fingerConfig = FingerprintConfiguration.Instance();
        DataAccessServices daServices = new DataAccessServices();
        List<Employee> employeeList;
        Speaker speaker;

        public SearchResults(string id, string fname, string lname)
        {
            InitializeComponent();
            employeeList = daServices.searchStudents(fname, id, lname);
            speaker = new Speaker();
            Registrar.MessageReceived += messagetext =>
            {
                search_status_label.Content = messagetext;
            };

            Registrar.SpeakerReceived += speach =>
            {
                speaker.speachThis(speach);
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewResults();
        }

        private void viewResults()
        {
            if (employeeList.Count == 1)
            {
                single_student_grid.Visibility = Visibility.Visible;
                stid.Content = employeeList[0].EmployeeNumber.ToString();
                stfn.Content = employeeList[0].FirstName.ToString();
                stln.Content = employeeList[0].LastName.ToString();
                if (employeeList[0].FingerprintID.ToString() == "0")
                {
                    status.Content = "Unregistered";
                }
                else
                {
                    status.Content = "Registered";
                }

            }
            else if (employeeList.Count > 1)
            {
                student_list_grid.Visibility = Visibility.Visible;
                student_listview.ItemsSource = employeeList;
            }
            else
            {
                this.Close();
                MessageBox.Show("No result found", "Search", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        private void listRegisterButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (student_listview.SelectedItems.Count > 0)
            {
                var selectedStockObject = student_listview.SelectedItems[0] as Employee;
                if (selectedStockObject.FingerprintID.ToString() == "0")
                {
                    Register_Student(selectedStockObject.EmployeeId, selectedStockObject.FirstName);
                    //status.Content = "Registered";
                }
                else
                {
                    if (MessageBox.Show(selectedStockObject.FirstName.ToString() + " already registered.\nDo you need to reregister this student?", "Reregister ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Re_Register_Student(selectedStockObject.EmployeeId, selectedStockObject.FirstName);
                        //status.Content = "Registered";
                    }
                    //MessageBox.Show(selectedStockObject.FirstName.ToString() + " already registered", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Select a student", "Invalid selection", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void singleStudentRegisterButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (employeeList[0].FingerprintID.ToString() == "0")
            {
                Register_Student(employeeList[0].EmployeeId, employeeList[0].FirstName.ToString());//sid
                label5.Visibility = Visibility.Hidden;
                status.Visibility = Visibility.Hidden;
                singleStudentRegisterButton.Opacity = 0.2;
                singleStudentRegisterButton.IsEnabled = false;

            }
            else
            {
                if (MessageBox.Show(employeeList[0].FirstName.ToString() + " already registered.\nDo you need to reregister this student?", "Reregister ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Re_Register_Student(employeeList[0].EmployeeId, employeeList[0].FirstName.ToString());
                    label5.Visibility = Visibility.Hidden;
                    status.Visibility = Visibility.Hidden;
                    singleStudentRegisterButton.Opacity = 0.2;
                    singleStudentRegisterButton.IsEnabled = false;
                }
                //MessageBox.Show(studentList[0].FirstName.ToString() + " already registered", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Register_Student(int id, string name)
        {

            if (id == 0)
            {
                MessageBox.Show("Not found valid name in your system", "Invalid username", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;
                fingerConfig.enrollEmployeeByEmployeeID(id.ToString(), name);
                Mouse.OverrideCursor = null;
                //MessageBox.Show("Registration successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Re_Register_Student(int id, string name)
        {

            if (id == 0)
            {
                MessageBox.Show("Not found valid name in your system", "Invalid username", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;
                fingerConfig.enrollEmployeeByEmployeeID(id.ToString(), name);
                Mouse.OverrideCursor = null;
                //MessageBox.Show("Registration successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
