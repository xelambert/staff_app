using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace test_staff
{
    public class SQLServer
    {
        readonly string connectionString;

        public SQLServer(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable executeQuery(string query, out bool isError)
        {
            isError = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dataTable = new DataTable();
                try
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    dataAdapter.Fill(dataTable);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Помилка зі сторони бази", MessageBoxButton.OK, MessageBoxImage.Error);
                    isError = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    isError = true;
                }

                return dataTable;
            }
        }
    }
}
