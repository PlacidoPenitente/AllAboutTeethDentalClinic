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
    /// Interaction logic for AddItemView.xaml
    /// </summary>
    public partial class AddItemView : UserControl
    {
        public AddItemView()
        {
            InitializeComponent();
        }

        private void addSupplier_Click(object sender, RoutedEventArgs e)
        {
            ((AddItemViewModel)DataContext).saveSupplier();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddItemViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddItemViewModel)DataContext).MenuViewModel.gotoItems(((AddItemViewModel)DataContext).ActiveUser);
        }
    }
}
