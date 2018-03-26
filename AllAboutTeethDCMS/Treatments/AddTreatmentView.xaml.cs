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

namespace AllAboutTeethDCMS.Treatments
{
    /// <summary>
    /// Interaction logic for AddTreatmentView.xaml
    /// </summary>
    public partial class AddTreatmentView : UserControl
    {
        public AddTreatmentView()
        {
            InitializeComponent();
        }

        private void addTreatment_Click(object sender, RoutedEventArgs e)
        {
            ((AddTreatmentViewModel)DataContext).saveTreatment();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddTreatmentViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddTreatmentViewModel)DataContext).MenuViewModel.gotoTreatments();
        }
    }
}
