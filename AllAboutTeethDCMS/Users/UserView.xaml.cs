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

namespace AllAboutTeethDCMS.Users {
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
        }

        private void add_account_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).MenuViewModel.gotoAddUserView(((UserViewModel)DataContext).ActiveUser);
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).loadUsers();
        }

        private void edit_account_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).MenuViewModel.gotoEditUserView(((UserViewModel)DataContext).ActiveUser, (User)((UserViewModel)DataContext).User.Clone());
        }

        private void delete_account_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).deleteUser();
        }
    }
}
