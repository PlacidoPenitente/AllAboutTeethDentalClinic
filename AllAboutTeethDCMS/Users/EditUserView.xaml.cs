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

namespace AllAboutTeethDCMS.Users
{
    /// <summary>
    /// Interaction logic for EditUserView.xaml
    /// </summary>
    public partial class EditUserView : UserControl
    {
        public EditUserView()
        {
            InitializeComponent();
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            ((EditUserViewModel)DataContext).saveUser();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((EditUserViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((EditUserViewModel)DataContext).MenuViewModel.gotoUsers();
        }
    }
}
