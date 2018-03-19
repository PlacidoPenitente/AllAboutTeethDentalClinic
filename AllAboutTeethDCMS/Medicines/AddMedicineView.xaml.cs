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

namespace AllAboutTeethDCMS.Medicines
{
    /// <summary>
    /// Interaction logic for AddMedicineView.xaml
    /// </summary>
    public partial class AddMedicineView : UserControl
    {
        public AddMedicineView()
        {
            InitializeComponent();
        }

        private void addTreatment_Click(object sender, RoutedEventArgs e)
        {
            ((AddMedicineViewModel)DataContext).saveMedicine();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddMedicineViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddMedicineViewModel)DataContext).MenuViewModel.gotoMedicines(((AddMedicineViewModel)DataContext).ActiveUser);
        }
    }
}
