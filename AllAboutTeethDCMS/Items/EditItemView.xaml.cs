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
    /// Interaction logic for EditItemView.xaml
    /// </summary>
    public partial class EditItemView : UserControl
    {
        public EditItemView()
        {
            InitializeComponent();
        }

        private void editSupplier_Click(object sender, RoutedEventArgs e)
        {
            ((EditItemViewModel)DataContext).saveSupplier();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((EditItemViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((EditItemViewModel)DataContext).MenuViewModel.gotoItems(((EditItemViewModel)DataContext).ActiveUser);
        }
    }
}
