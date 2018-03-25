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
    /// Interaction logic for AddUserView.xaml
    /// </summary>
    public partial class AddUserView : UserControl
    {
        public AddUserView()
        {
            InitializeComponent();
            birthdate.DisplayDateEnd = DateTime.Now;
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            ((AddUserViewModel)DataContext).saveUser();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            passwordMain.Password = "";
            passwordCopy.Password = "";
            ((AddUserViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddUserViewModel)DataContext).MenuViewModel.gotoUsers();
        }

        private void passwordMain_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((AddUserViewModel)DataContext).Password = passwordMain.Password;
        }

        private void passwordCopy_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((AddUserViewModel)DataContext).PasswordCopy = passwordCopy.Password;
        }
    }
}
