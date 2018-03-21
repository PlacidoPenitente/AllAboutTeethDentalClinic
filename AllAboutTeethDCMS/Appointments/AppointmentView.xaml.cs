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

namespace AllAboutTeethDCMS.Appointments
{
    /// <summary>
    /// Interaction logic for AppointmentView.xaml
    /// </summary>
    public partial class AppointmentView : UserControl
    {
        public AppointmentView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((AppointmentViewModel)DataContext).loadAppointments();
        }

        private void add_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((AppointmentViewModel)DataContext).MenuViewModel.gotoAddAppointmentView(((AppointmentViewModel)DataContext).ActiveUser);
        }

        private void edit_treatment_Click(object sender, RoutedEventArgs e)
        {
            //((AppointmentViewModel)DataContext).MenuViewModel.gotoAppointments(((AppointmentViewModel)DataContext).ActiveUser, (Appointment)((AppointmentViewModel)DataContext).Appointment.Clone());
        }

        private void delete_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((AppointmentViewModel)DataContext).deleteAppointment();
        }

        private void start_treatment_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
