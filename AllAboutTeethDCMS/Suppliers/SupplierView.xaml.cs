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

namespace AllAboutTeethDCMS.Suppliers
{
    /// <summary>
    /// Interaction logic for SupplierView.xaml
    /// </summary>
    public partial class SupplierView : UserControl
    {
        public SupplierView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).loadSuppliers();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).MenuViewModel.gotoAddSupplierView();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).MenuViewModel.gotoEditSupplierView((Supplier)((SupplierViewModel)DataContext).Supplier.Clone());
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).deleteSupplier();
        }

        private void unarchive_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).unarchive();
        }

        private void archive_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).archive();
        }
    }
}
