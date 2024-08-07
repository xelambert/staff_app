using System.Data;
using System.Diagnostics;
using System.Windows;

namespace test_staff
{
    /// <summary>
    /// Interaction logic for addWindow.xaml
    /// </summary>
    public partial class addWindow : Window
    {
        public addWindow()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            getPositions();
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string validationMessage = "";

            bool isStaffValidated = false;

            try
            {
                if (String.IsNullOrEmpty(textPIB.Text) ||
                    cbPositions.SelectedValue == null ||
                    String.IsNullOrEmpty(textSalary.Text) ||
                    dateBirthDate.SelectedDate == null)
                {
                    throw new Exception("Не всі поля заповнені!");
                }

                string? PIB = textPIB.Text;
                int? position = (int)cbPositions.SelectedValue;
                float? salary = float.Parse(textSalary.Text);

                DateTime? birthDate = dateBirthDate.SelectedDate;

                Staff staff = new Staff(PIB, position, salary, birthDate);

                isStaffValidated = StaffValidator.validate(staff, out validationMessage);

                if (isStaffValidated)
                {
                    addStaff(staff);
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

        private void addStaff(Staff staff)
        {
            bool isError = false;



            string query = "DECLARE @DATEVAR DATE = CONVERT(DATE, '" + ((DateTime)(staff.birthDate)).ToString("dd.MM.yyyy") + "', 104);"
               +"EXEC STAFF_ADD @PIB = N'"
               + staff.PIB.Replace("'", "''") + "'"
               + ",@BIRTHDATE = @DATEVAR"
               + ",@POSITIONID = " + staff.position
               + ",@SALARY = " + staff.salary;

            Debug.WriteLine(query);

            App.sqlServer.executeQuery(query, out isError);

            if(!isError)
            {
                MessageBox.Show("Співробітника додано", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                MessageBox.Show("Співробітник НЕ доданий", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
