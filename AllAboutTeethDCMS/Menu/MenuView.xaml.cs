using System.Windows;
using System.Windows.Controls;

namespace AllAboutTeethDCMS.Menu
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void appointments_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoAppointments();
        }

        private void patients_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoPatients();
        }

        private void services_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoTreatments();
        }

        private void supplies_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoMedicines();
        }

        private void transactions_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoOperations(menuViewModel.ActiveUser);
        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoTransactionReports();
        }

        private void accounts_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoUsers();
        }

        private void maintenance_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoMaintenance();
        }

        private void suppliers_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoSuppliers();
        }

        private void providers_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoProviders();
        }

        private void dashboard_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.GotoDashboard();
        }

        private void activityLog_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoActivityLogs();
        }

        private void dentist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void billing_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoBillings();
        }
    }
}
