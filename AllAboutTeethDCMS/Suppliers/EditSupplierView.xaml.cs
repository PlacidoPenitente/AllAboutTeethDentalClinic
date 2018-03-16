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
    /// Interaction logic for EditSupplierView.xaml
    /// </summary>
    public partial class EditSupplierView : UserControl
    {
        public EditSupplierView()
        {
            InitializeComponent();
        }

        private void editSupplier_Click(object sender, RoutedEventArgs e)
        {
            ((EditSupplierViewModel)DataContext).saveSupplier();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((EditSupplierViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((EditSupplierViewModel)DataContext).MenuViewModel.gotoSuppliers(((EditSupplierViewModel)DataContext).ActiveUser);
        }
    }
}
