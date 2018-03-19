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

namespace AllAboutTeethDCMS.Providers
{
    /// <summary>
    /// Interaction logic for EditProviderView.xaml
    /// </summary>
    public partial class EditProviderView : UserControl
    {
        public EditProviderView()
        {
            InitializeComponent();
        }

        private void addTreatment_Click(object sender, RoutedEventArgs e)
        {
            ((EditProviderViewModel)DataContext).saveProvider();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((EditProviderViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((EditProviderViewModel)DataContext).MenuViewModel.gotoProviders(((EditProviderViewModel)DataContext).ActiveUser);
        }
    }
}
