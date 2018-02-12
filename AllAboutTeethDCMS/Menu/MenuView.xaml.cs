using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            menuViewModel.gotoUsers();
        }

        private void maintenance_Click(object sender, RoutedEventArgs e)
        {
            MenuViewModel menuViewModel = (MenuViewModel)DataContext;
            menuViewModel.gotoMaintenance();
        }
    }
}
