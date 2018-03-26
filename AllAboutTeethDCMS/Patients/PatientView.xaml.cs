using AllAboutTeethDCMS.DentalCharts;
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

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).loadPatients();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).MenuViewModel.gotoAddPatientView();
        }

        private void view_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).PatientPreviewViewModel.Visibility = "Visible";
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).MenuViewModel.gotoEditPatientView((Patient)((PatientViewModel)DataContext).Patient.Clone());
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).deletePatient();
        }

        private void unarchive_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).unarchive();
        }

        private void archive_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).archive();
        }

        private void chart_Click(object sender, RoutedEventArgs e)
        {
            ((PatientViewModel)DataContext).DentalChartPreviewViewModel.Visibility = "Visible";
        }
    }
}
