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
            ((SupplierViewModel)DataContext).loadPatients();
        }

        private void add_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).MenuViewModel.gotoAddSupplierView(((SupplierViewModel)DataContext).ActiveUser);
        }

        private void edit_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).MenuViewModel.gotoEditSupplierView(((SupplierViewModel)DataContext).ActiveUser, ((SupplierViewModel)DataContext).Supplier);
        }

        private void delete_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((SupplierViewModel)DataContext).deleteTreatment();
        }
    }
}
