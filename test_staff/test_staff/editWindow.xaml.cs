using System.Data;
using System.Diagnostics;
using System.Windows;

namespace test_staff
{
    /// <summary>
    /// Interaction logic for editWindow.xaml
    /// </summary>
    public partial class editWindow : Window
    {
        private int? staffId;
        public editWindow(Staff staff)
        {
            InitializeComponent();

            LoadData(staff);
        }

        private void LoadData(Staff staff)
        {
            getPositions();

            getStaffInfo(staff);
        }

        private void getPositions()
        {
            bool isError = false;

            DataTable positionsTable;
            positionsTable = App.sqlServer.executeQuery("EXEC POSITIONS_SELECT", out isError);

            if (isError)
            {
                return;
            }

            cbPositions.ItemsSource = positionsTable.DefaultView;
        }

        private void getStaffInfo(Staff staff)
        {
            this.staffId = staff.id;

            labelId.Content = staff.id.ToString();
            textPIB.Text = staff.PIB;
            cbPositions.SelectedValue = staff.position;
            textSalary.Text = staff.salary.ToString();
            dateBirthDate.SelectedDate = staff.birthDate;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string validationMessage = "";

            bool isStaffValidated = false;

            try
            {
                if (string.IsNullOrEmpty(textPIB.Text) ||
                    cbPositions.SelectedValue == null ||
                    string.IsNullOrEmpty(textSalary.Text) ||
                    dateBirthDate.SelectedDate == null)
                {
                    throw new Exception("Не всі поля заповнені!");
                }

                string? PIB = textPIB.Text;
                int? position = (int)cbPositions.SelectedValue;
                float? salary = float.Parse(textSalary.Text);

                DateTime? birthDate = dateBirthDate.SelectedDate;

                Staff staff = new Staff(PIB, position, salary, birthDate, this.staffId);

                isStaffValidated = StaffValidator.validate(staff, out validationMessage);

                if (isStaffValidated)
                {
                    editStaff(staff);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(validationMessage, "Невалідні дані", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }  
        }

        private void editStaff(Staff staff)
        {
            bool isError = false;



            string query = "DECLARE @DATEVAR DATE = CONVERT(DATE, '" + ((DateTime)(staff.birthDate)).ToString("dd.MM.yyyy") + "', 104);"
               +"EXEC STAFF_EDIT @PIB = N'"
               + staff.PIB.Replace("'", "''") + "'"
               + ",@BIRTHDATE = @DATEVAR"
               + ",@POSITIONID = " + staff.position
               + ",@SALARY = " + staff.salary
               + ",@STAFFID = " + staff.id;

            Debug.WriteLine(query);

            App.sqlServer.executeQuery(query, out isError);

            if(!isError)
            {
                MessageBox.Show("Дані співробітника змінено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                MessageBox.Show("Дані співробітника НЕ змінено", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
