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

namespace AllAboutTeethDCMS.Operations
{
    /// <summary>
    /// Interaction logic for AddOperationView.xaml
    /// </summary>
    public partial class AddOperationView : UserControl
    {
        public AddOperationView()
        {
            InitializeComponent();
        }

        private void addTreatment_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).updateList();
            //((AddOperationViewModel)DataContext).saveOperation();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).finishTreatment();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).MenuViewModel.gotoOperations(((AddOperationViewModel)DataContext).ActiveUser);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).updateList();
        }
    }
}
