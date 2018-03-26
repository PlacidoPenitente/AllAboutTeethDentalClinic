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
            menuViewModel.gotoAppointments(menuViewModel.ActiveUser);
        }

        private void patients_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoPatients();
        }

        private void services_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoTreatments(menuViewModel.ActiveUser);
        }

        private void supplies_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoMedicines(menuViewModel.ActiveUser);
        }

        private void transactions_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoOperations(menuViewModel.ActiveUser);
        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
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
            menuViewModel.gotoProviders(menuViewModel.ActiveUser);
        }

        private void dashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void activityLog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dentist_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
