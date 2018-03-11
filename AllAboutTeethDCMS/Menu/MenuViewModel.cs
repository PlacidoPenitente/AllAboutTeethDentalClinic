using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Dentists;
using AllAboutTeethDCMS.Inventory;
using AllAboutTeethDCMS.Maintenance;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Transactions;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Menu
{
    public class MenuViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;

        private User activeUser = new User();

        private AppointmentView appointmentView;
        private PatientView patientView;
        private TreatmentView treatmentView;
        private DentistView dentistView;
        private InventoryView inventoryView;
        private TransactionView transactionView;

        private UserView userView;
        private AddUserView addUserView;
        private EditUserView editUserView;

        private MaintenanceView maintenanceView;

        public MenuViewModel()
        {
            AppointmentView = new AppointmentView();
            PatientView = new PatientView();
            TreatmentView = new TreatmentView();
            DentistView = new DentistView();
            InventoryView = new InventoryView();
            TransactionView = new TransactionView();
            UserView = new UserView();
            AddUserView = new AddUserView();
            MaintenanceView = new MaintenanceView();
            EditUserView = new EditUserView();
        }

        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }

        public AppointmentView AppointmentView { get => appointmentView; set => appointmentView = value; }
        public PatientView PatientView { get => patientView; set => patientView = value; }
        public TreatmentView TreatmentView { get => treatmentView; set => treatmentView = value; }
        public DentistView DentistView { get => dentistView; set => dentistView = value; }
        public InventoryView InventoryView { get => inventoryView; set => inventoryView = value; }
        public TransactionView TransactionView { get => transactionView; set => transactionView = value; }
        public UserView UserView { get => userView; set => userView = value; }
        public MaintenanceView MaintenanceView { get => maintenanceView; set => maintenanceView = value; }
        public AddUserView AddUserView { get => addUserView; set => addUserView = value; }
        public EditUserView EditUserView { get => editUserView; set => editUserView = value; }

        public User ActiveUser { get => activeUser; set => activeUser = value; }

        public void gotoAppointments()
        {
            MainWindowViewModel.ActivePage = AppointmentView;
        }

        public void gotoPatients()
        {
            MainWindowViewModel.ActivePage = PatientView;
        }

        public void gotoTreatments()
        {
            MainWindowViewModel.ActivePage = TreatmentView;
        }

        public void gotoDentists()
        {
            MainWindowViewModel.ActivePage = DentistView;
        }

        public void gotoInventory()
        {
            MainWindowViewModel.ActivePage = InventoryView;
        }

        public void gotoTransactions()
        {
            MainWindowViewModel.ActivePage = TransactionView;
        }

        public void gotoUsers(User activeUser)
        {
            MainWindowViewModel.ActivePage = UserView;
            ((UserViewModel)UserView.DataContext).ActiveUser = activeUser;
            ((UserViewModel)UserView.DataContext).MenuViewModel = this;
        }

        public void gotoAddUserView(User activeUser)
        {
            MainWindowViewModel.ActivePage = AddUserView;
            ((AddUserViewModel)AddUserView.DataContext).ActiveUser = activeUser;
            ((AddUserViewModel)AddUserView.DataContext).MenuViewModel = this;
        }

        public void gotoEditUserView(User activeUser, User selectedUser)
        {
            MainWindowViewModel.ActivePage = EditUserView;
            ((EditUserViewModel)EditUserView.DataContext).ActiveUser = activeUser;
            ((EditUserViewModel)EditUserView.DataContext).User = selectedUser;
            ((EditUserViewModel)EditUserView.DataContext).MenuViewModel = this;
        }

        public void gotoMaintenance()
        {
            MainWindowViewModel.ActivePage = MaintenanceView;
        }
    }
}
