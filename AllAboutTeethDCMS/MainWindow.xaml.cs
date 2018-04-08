using AllAboutTeethDCMS.Menu;
using AllAboutTeethDCMS.Users;
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

namespace AllAboutTeethDCMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((MenuViewModel)menuView.DataContext).MainWindowViewModel = (MainWindowViewModel)DataContext;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                ((LoginViewModel)login.DataContext).User = null;
            }
        }
    }
}
