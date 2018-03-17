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

namespace AllAboutTeethDCMS.Items
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl
    {
        public ItemView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((ItemViewModel)DataContext).loadPatients();
        }

        private void add_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((ItemViewModel)DataContext).MenuViewModel.gotoAddItemView(((ItemViewModel)DataContext).ActiveUser);
        }

        private void edit_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((ItemViewModel)DataContext).MenuViewModel.gotoEditItemView(((ItemViewModel)DataContext).ActiveUser, ((ItemViewModel)DataContext).Item);
        }

        private void delete_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((ItemViewModel)DataContext).deleteTreatment();
        }
    }
}
