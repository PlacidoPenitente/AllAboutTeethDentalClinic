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

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).loadUsers();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).MenuViewModel.gotoAddUserView();
        }

        private void view_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).UserPreviewViewModel.Visibility = "Visible";
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).MenuViewModel.gotoEditUserView((User)((UserViewModel)DataContext).User.Clone());
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).deleteUser();
        }

        private void unarchive_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).unarchive();
        }

        private void archive_Click(object sender, RoutedEventArgs e)
        {
            ((UserViewModel)DataContext).archive();
        }
    }
}
