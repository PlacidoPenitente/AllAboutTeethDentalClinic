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
    /// Interaction logic for AddPatientView.xaml
    /// </summary>
    public partial class AddPatientView : UserControl
    {
        public AddPatientView()
        {
            InitializeComponent();
        }

        private void addpatient_Click(object sender, RoutedEventArgs e)
        {
            ((AddPatientViewModel)DataContext).savePatient();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddPatientViewModel)DataContext).resetForm();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddPatientViewModel)DataContext).MenuViewModel.gotoPatients(((AddPatientViewModel)DataContext).ActiveUser);
        }

        private void treatment_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked != true) ((AddPatientViewModel)DataContext).ConditionBeingTreated = "";
        }

        private void operation_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked != true) ((AddPatientViewModel)DataContext).IllnessOrOperation = "";
        }

        private void hospitalized_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked != true) ((AddPatientViewModel)DataContext).Hospitalization = "";
        }

        private void medication_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked != true) ((AddPatientViewModel)DataContext).MedicationTaken = "";
        }

        private void allergy_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked != true) ((AddPatientViewModel)DataContext).Allergies = "";
        }

        private void disease_Click(object sender, RoutedEventArgs e)
        {
            if(((CheckBox)sender).IsChecked!=true) ((AddPatientViewModel)DataContext).Diseases = "";
        }
    }
}
