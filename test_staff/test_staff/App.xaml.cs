using System.Windows;

namespace test_staff
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string connectionString = "Server=DESKTOP-I3EQDCI;Database=STAFF_APP;Trusted_Connection=True;";
        static public SQLServer sqlServer = new SQLServer(connectionString);
    }

}
