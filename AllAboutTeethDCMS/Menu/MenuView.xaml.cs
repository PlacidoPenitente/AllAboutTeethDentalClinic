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
            menuViewModel.ActiveUser.No = 0;
            menuViewModel.gotoPatients(menuViewModel.ActiveUser);
        }

        private void services_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoTreatments(menuViewModel.ActiveUser);
        }

        private void dentists_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoDentists();
        }

        private void supplies_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoInventory();
        }

        private void transactions_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoTransactions();
        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
        }

        private void accounts_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.ActiveUser.No = 0;
            menuViewModel.gotoUsers(menuViewModel.ActiveUser);
        }

        private void maintenance_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoMaintenance();
        }

        private void suppliers_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.ActiveUser.No = 0;
            menuViewModel.gotoSuppliers(menuViewModel.ActiveUser);
        }
    }
}
