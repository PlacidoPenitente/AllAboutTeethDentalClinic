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

namespace AllAboutTeethDCMS.Patients
{
    /// <summary>
    /// Interaction logic for PatientView.xaml
    /// </summary>
    public partial class PatientView : UserControl
    {
        public PatientView()
        {
            InitializeComponent();
        }

        private void delete_patient_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).deleteUser();
        }

        private void edit_patient_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).MenuViewModel.gotoEditPatientView(((PatientViewModel)DataContext).ActiveUser, (Patient)((PatientViewModel)DataContext).Patient.Clone());
        }

        private void add_patient_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).MenuViewModel.gotoAddPatientView(((PatientViewModel)DataContext).ActiveUser);
        }

        private void search_patient_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).loadPatients();
        }
    }
}
