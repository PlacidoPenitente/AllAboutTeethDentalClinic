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
    /// Interaction logic for MedicineView.xaml
    /// </summary>
    public partial class MedicineView : UserControl
    {
        public MedicineView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((MedicineViewModel)DataContext).loadMedicines();
        }

        private void add_treatment_Click(object sender, RoutedEventArgs e)
        {
            //((MedicineViewModel)DataContext).MenuViewModel.gotoAddTreatmentView(((MedicineViewModel)DataContext).ActiveUser);
        }

        private void edit_treatment_Click(object sender, RoutedEventArgs e)
        {
            //((MedicineViewModel)DataContext).MenuViewModel.gotoEditTreatmentView(((MedicineViewModel)DataContext).ActiveUser, (Medicine)((MedicineViewModel)DataContext).Treatment.Clone());
        }

        private void delete_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((MedicineViewModel)DataContext).deleteMedicine();
        }
    }
}
