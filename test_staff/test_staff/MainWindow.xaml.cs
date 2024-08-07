using System.Windows;
using System.Windows.Controls;
using System.Data;
using ClosedXML.Excel;
using Microsoft.Win32;
namespace test_staff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setEditAvailability(false);
            LoadData();
        }

        private void LoadData() //Оновити дані в гріді та статистику
        {
            refreshStaffGrid();

            refreshStatistics();
        }

        private void refreshStaffGrid(string? queryPIB = null) //Оновити дані в гріді
        {
            string query = "EXEC STAFF_SELECT";

            if (!string.IsNullOrEmpty(queryPIB))
            {
                query += " @PIB = N'" + queryPIB.Replace("'", "''''") + "'";
            }

            bool isError = false;
            DataTable staffTable;
            staffTable = App.sqlServer.executeQuery(query, out isError);

            if (isError)
            {
                return;
            }

            staffGrid.ItemsSource = staffTable.DefaultView;
        }

        private void refreshStatistics() //Оновити дані статистики
        {
            bool isError = false;
            string amount = "";
            string average = "";

            DataTable statisticsTable;
            statisticsTable = App.sqlServer.executeQuery("EXEC STAFF_STATISTICS", out isError);

            if (isError)
            {
                return;
            }

            amount = statisticsTable.Rows[0].Field<String>(0);
            average = statisticsTable.Rows[1].Field<String>(0);

            labelAmount.Content = amount;
            labelAverage.Content = average;
        }

        private void btnFire_Click(object sender, RoutedEventArgs e) //Звільнити виділеного співробітника
        {
            bool isError = false;

            DataRowView selectedRow = (DataRowView)staffGrid.SelectedItem;
            string staffID = Convert.ToString(selectedRow["staffID"]);

            App.sqlServer.executeQuery("EXEC STAFF_FIRE @ID = " + staffID, out isError);

            if (isError)
            {
                return;
            }

            LoadData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) //Додати нового співробітника
        {
           addWindow addWindow = new addWindow();
           addWindow.ShowDialog();

           LoadData();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) //Редагувати виділеного співробітника
        {
            DataRowView selectedRow = (DataRowView)staffGrid.SelectedItem;
            int staffID = Convert.ToInt32(selectedRow["staffID"]);
            string PIB = Convert.ToString(selectedRow["PIB"]);
            float salary = (float)Convert.ToDouble(selectedRow["SALARY"]);
            DateTime birthDate = Convert.ToDateTime(selectedRow["BIRTHDATE"]);
            int position = Convert.ToInt32(selectedRow["positionID"]); ;

            Staff staff = new Staff(PIB, position, salary, birthDate, staffID);

            editWindow editWindow = new editWindow(staff);
            editWindow.ShowDialog();

            LoadData();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e) //Оновити дані в гріді та статистику 
        {
            LoadData();
        }

        private void staffGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //Доступність кнопок редагування та звільнення
        {
            if (staffGrid.SelectedItem != null)
            {
                var selectedRow = staffGrid.ItemContainerGenerator.ContainerFromItem(staffGrid.SelectedItem) as DataGridRow;
                if (selectedRow != null)
                {
                    setEditAvailability(true);
                }
            }
        }

        private void setEditAvailability(bool isAvailable)
        {
            btnEdit.IsEnabled = isAvailable;
            btnFire.IsEnabled = isAvailable;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e) //Пошук по ПІБ
        {
            string queryPIB = textFind.Text;

            if (!string.IsNullOrEmpty(queryPIB))
            {
                refreshStaffGrid(queryPIB);
            }
            else
            {
                refreshStaffGrid();
                MessageBox.Show("Введіть дані для пошуку по ПІБ", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            refreshStatistics();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e) //Експорт
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Save an Excel File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                DataTable dt = ((DataView)staffGrid.ItemsSource).ToTable();

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "DataGridExport");
                    wb.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Дані експортовано в Excel!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e) //Імпорт
        {
            bool isError = false;
            string query = "";

            List<Staff> staffList = new List<Staff>(); //Співробітники, що будуть імпортовані
            string staffValues = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx";
            openFileDialog.Title = "Open an Excel File";
            if (openFileDialog.ShowDialog() == true)
            {
                DataTable dt = new DataTable();

                using (var workbook = new XLWorkbook(openFileDialog.FileName))
                {
                    var worksheet = workbook.Worksheet(1); // Робочий лист, з якого будуть імпортуватися дані
                    bool firstRow = true;
                    foreach (var row in worksheet.RowsUsed())
                    {
                        if (firstRow) //в першому рядку назви стовпців
                        {
                            foreach (var cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (var cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    string validationMessage = "";

                    string PIB = Convert.ToString(row[1]);
                    DateTime birthDate = Convert.ToDateTime(row[2]);
                    float salary = (float)Convert.ToDouble(row[3]);
                    int position = Convert.ToInt32(row[5]);

                    Staff staff = new Staff(PIB, position, salary, birthDate);
                    if (StaffValidator.validate(staff, out validationMessage))
                    {
                        staffList.Add(staff);
                    }
                    else
                    {
                        MessageBox.Show("Невалідні дані: " + validationMessage, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                foreach (Staff staff in staffList)
                {
                    staffValues += "(N'" + staff.PIB.Replace("'", "''")
                    + "', " + "CONVERT(DATE, '" + ((DateTime)(staff.birthDate)).ToString("dd.MM.yyyy") + " ', 104),"
                    + staff.position + ","
                    + staff.salary + ")";
                }

                staffValues = staffValues.Replace(")(", "),\n(");
                staffValues = staffValues.Replace("'", "''");
                query = "EXEC STAFF_IMPORT @VALUES = N'" + staffValues + "'";

                App.sqlServer.executeQuery(query, out isError);

                if (isError)
                {
                    MessageBox.Show("Дані не імпортовано", "Помилка зі сторони бази", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Дані імпортовані з Excel!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            LoadData();
        }
    }
}