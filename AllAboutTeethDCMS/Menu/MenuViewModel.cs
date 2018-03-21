using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Medicines;
using AllAboutTeethDCMS.Maintenance;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Providers;
using AllAboutTeethDCMS.Suppliers;
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
        private AddAppointmentView addAppointmentView;

        private TransactionView transactionView;

        private UserView userView;
        private AddUserView addUserView;
        private EditUserView editUserView;

        private PatientView patientView;
        private AddPatientView addPatientView;
        private EditPatientView editPatientView;

        private TreatmentView treatmentView;
        private AddTreatmentView addTreatmentView;
        private EditTreatmentView editTreatmentView;

        private SupplierView supplierView;
        private AddSupplierView addSupplierView;
        private EditSupplierView editSupplierView;

        private ProviderView providerView;
        private AddProviderView addProviderView;
        private EditProviderView editProviderView;

        private MedicineView itemView;
        private AddMedicineView addMedicineView;
        private EditMedicineView editMedicineView;

        private MaintenanceView maintenanceView;

        private string patientTotal = "";

        public MenuViewModel()
        {
            AppointmentView = new AppointmentView();
            PatientView = new PatientView();
            TreatmentView = new TreatmentView();
            TransactionView = new TransactionView();
            UserView = new UserView();
            AddUserView = new AddUserView();
            MaintenanceView = new MaintenanceView();
            EditUserView = new EditUserView();
            AddPatientView = new AddPatientView();
            EditPatientView = new EditPatientView();
            AddTreatmentView = new AddTreatmentView();
            EditTreatmentView = new EditTreatmentView();
            SupplierView = new SupplierView();
            AddSupplierView = new AddSupplierView();
            EditSupplierView = new EditSupplierView();
            ProviderView = new ProviderView();
            AddProviderView = new AddProviderView();
            EditProviderView = new EditProviderView();
            MedicineView = new MedicineView();
            AddAppointmentView = new AddAppointmentView();
        }
        
        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }

        public AppointmentView AppointmentView { get => appointmentView; set => appointmentView = value; }
        public PatientView PatientView { get => patientView; set { patientView = value; OnPropertyChanged(); } }
        public TreatmentView TreatmentView { get => TreatmentView1; set => TreatmentView1 = value; }
        public TransactionView TransactionView { get => transactionView; set => transactionView = value; }
        public UserView UserView { get => userView; set => userView = value; }
        public MaintenanceView MaintenanceView { get => maintenanceView; set => maintenanceView = value; }
        public AddUserView AddUserView { get => addUserView; set => addUserView = value; }
        public EditUserView EditUserView { get => editUserView; set => editUserView = value; }

        public User ActiveUser { get => activeUser; set => activeUser = value; }
        public AddPatientView AddPatientView { get => addPatientView; set => addPatientView = value; }
        public EditPatientView EditPatientView { get => editPatientView; set => editPatientView = value; }
        public TreatmentView TreatmentView1 { get => treatmentView; set => treatmentView = value; }
        public AddTreatmentView AddTreatmentView { get => addTreatmentView; set => addTreatmentView = value; }
        public EditTreatmentView EditTreatmentView { get => editTreatmentView; set => editTreatmentView = value; }
        public SupplierView SupplierView { get => supplierView; set => supplierView = value; }
        public AddSupplierView AddSupplierView { get => addSupplierView; set => addSupplierView = value; }
        public EditSupplierView EditSupplierView { get => editSupplierView; set => editSupplierView = value; }
        public ProviderView ProviderView { get => providerView; set => providerView = value; }
        public AddProviderView AddProviderView { get => addProviderView; set => addProviderView = value; }
        public EditProviderView EditProviderView { get => editProviderView; set => editProviderView = value; }
        public MedicineView MedicineView { get => ItemView; set => ItemView = value; }
        public MedicineView ItemView { get => itemView; set => itemView = value; }
        public AddMedicineView AddMedicineView { get => addMedicineView; set => addMedicineView = value; }
        public EditMedicineView EditMedicineView { get => editMedicineView; set => editMedicineView = value; }
        public string PatientTotal { get => patientTotal; set { patientTotal = value; OnPropertyChanged(); } }

        public AddAppointmentView AddAppointmentView { get => addAppointmentView; set => addAppointmentView = value; }

        public void gotoAppointments(User activeUser)
        {
            AppointmentView = new AppointmentView();
            MainWindowViewModel.ActivePage = AppointmentView;
            ((AppointmentViewModel)AppointmentView.DataContext).ActiveUser = activeUser;
            ((AppointmentViewModel)AppointmentView.DataContext).MenuViewModel = this;
            ((AppointmentViewModel)AppointmentView.DataContext).loadAppointments();
        }

        public void gotoPatients(User activeUser)
        {
            PatientView = new PatientView();
            MainWindowViewModel.ActivePage = PatientView;
            ((PatientViewModel)PatientView.DataContext).ActiveUser = activeUser;
            ((PatientViewModel)PatientView.DataContext).MenuViewModel = this;
            ((PatientViewModel)PatientView.DataContext).loadPatients();
        }

        public void gotoTreatments(User activeUser)
        {
            TreatmentView = new TreatmentView();
            MainWindowViewModel.ActivePage = TreatmentView;
            ((TreatmentViewModel)TreatmentView.DataContext).ActiveUser = activeUser;
            ((TreatmentViewModel)TreatmentView.DataContext).MenuViewModel = this;
            ((TreatmentViewModel)TreatmentView.DataContext).loadTreatments();
        }

        public void gotoEditTreatmentView(User activeUser, Treatment treatment)
        {
            EditTreatmentView = new EditTreatmentView();
            MainWindowViewModel.ActivePage = EditTreatmentView;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).ActiveUser = activeUser;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).Treatment = treatment;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).CopyTreatment = (Treatment)treatment.Clone();
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).MenuViewModel = this;
        }

        public void gotoAddTreatmentView(User activeUser)
        {
            AddTreatmentView = new AddTreatmentView();
            MainWindowViewModel.ActivePage = AddTreatmentView;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).ActiveUser = activeUser;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).MenuViewModel = this;
        }

        public void gotoProviders(User activeUser)
        {
            ProviderView = new ProviderView();
            MainWindowViewModel.ActivePage = ProviderView;
            ((ProviderViewModel)ProviderView.DataContext).ActiveUser = activeUser;
            ((ProviderViewModel)ProviderView.DataContext).MenuViewModel = this;
            ((ProviderViewModel)ProviderView.DataContext).loadProviders();
        }

        public void gotoEditProviderView(User activeUser, Provider provider)
        {
            EditProviderView = new EditProviderView();
            MainWindowViewModel.ActivePage = EditProviderView;
            ((EditProviderViewModel)EditProviderView.DataContext).ActiveUser = activeUser;
            ((EditProviderViewModel)EditProviderView.DataContext).Provider = provider;
            ((EditProviderViewModel)EditProviderView.DataContext).CopyProvider = (Provider)provider.Clone();
            ((EditProviderViewModel)EditProviderView.DataContext).MenuViewModel = this;
        }

        public void gotoAddProviderView(User activeUser)
        {
            AddProviderView = new AddProviderView();
            MainWindowViewModel.ActivePage = AddProviderView;
            ((AddProviderViewModel)AddProviderView.DataContext).ActiveUser = activeUser;
            ((AddProviderViewModel)AddProviderView.DataContext).MenuViewModel = this;
        }

        public void gotoTransactions()
        {
            MainWindowViewModel.ActivePage = TransactionView;
        }

        public void gotoUsers(User activeUser)
        {
            UserView = new UserView();
            MainWindowViewModel.ActivePage = UserView;
            ((UserViewModel)UserView.DataContext).ActiveUser = activeUser;
            ((UserViewModel)UserView.DataContext).MenuViewModel = this;
            ((UserViewModel)UserView.DataContext).loadUsers();
        }

        public void gotoAddUserView(User activeUser)
        {
            AddUserView = new AddUserView();
            MainWindowViewModel.ActivePage = AddUserView;
            ((AddUserViewModel)AddUserView.DataContext).ActiveUser = activeUser;
            ((AddUserViewModel)AddUserView.DataContext).MenuViewModel = this;
        }

        public void gotoEditUserView(User activeUser, User selectedUser)
        {
            EditUserView = new EditUserView();
            MainWindowViewModel.ActivePage = EditUserView;
            ((EditUserViewModel)EditUserView.DataContext).ActiveUser = activeUser;
            ((EditUserViewModel)EditUserView.DataContext).User = selectedUser;
            ((EditUserViewModel)EditUserView.DataContext).CopyUser = (User)selectedUser.Clone();
            ((EditUserViewModel)EditUserView.DataContext).MenuViewModel = this;
        }

        public void gotoSuppliers(User activeUser)
        {
            SupplierView = new SupplierView();
            MainWindowViewModel.ActivePage = SupplierView;
            ((SupplierViewModel)SupplierView.DataContext).ActiveUser = activeUser;
            ((SupplierViewModel)SupplierView.DataContext).MenuViewModel = this;
            ((SupplierViewModel)SupplierView.DataContext).loadSuppliers();
        }

        public void gotoAddSupplierView(User activeUser)
        {
            AddSupplierView = new AddSupplierView();
            MainWindowViewModel.ActivePage = AddSupplierView;
            ((AddSupplierViewModel)AddSupplierView.DataContext).ActiveUser = activeUser;
            ((AddSupplierViewModel)AddSupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoEditSupplierView(User activeUser, Supplier selectedSupplier)
        {
            EditSupplierView = new EditSupplierView();
            MainWindowViewModel.ActivePage = EditSupplierView;
            ((EditSupplierViewModel)EditSupplierView.DataContext).ActiveUser = activeUser;
            ((EditSupplierViewModel)EditSupplierView.DataContext).Supplier = selectedSupplier;
            ((EditSupplierViewModel)EditSupplierView.DataContext).CopySupplier = (Supplier)selectedSupplier.Clone();
            ((EditSupplierViewModel)EditSupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoAddPatientView(User activeUser)
        {
            AddPatientView = new AddPatientView();
            MainWindowViewModel.ActivePage = AddPatientView;
            ((AddPatientViewModel)AddPatientView.DataContext).ActiveUser = activeUser;
            ((AddPatientViewModel)AddPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoEditPatientView(User activeUser, Patient selectedPatient)
        {
            EditPatientView = new EditPatientView();
            MainWindowViewModel.ActivePage = EditPatientView;
            ((EditPatientViewModel)EditPatientView.DataContext).ActiveUser = activeUser;
            ((EditPatientViewModel)EditPatientView.DataContext).Patient = selectedPatient;
            ((EditPatientViewModel)EditPatientView.DataContext).CopyPatient = (Patient)selectedPatient.Clone();
            ((EditPatientViewModel)EditPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoMedicines(User activeUser)
        {
            MedicineView = new MedicineView();
            MainWindowViewModel.ActivePage = MedicineView;
            ((MedicineViewModel)MedicineView.DataContext).ActiveUser = activeUser;
            ((MedicineViewModel)MedicineView.DataContext).MenuViewModel = this;
            ((MedicineViewModel)MedicineView.DataContext).loadMedicines();
        }

        public void gotoAddMedicineView(User activeUser)
        {
            AddMedicineView = new AddMedicineView();
            MainWindowViewModel.ActivePage = AddMedicineView;
            ((AddMedicineViewModel)AddMedicineView.DataContext).ActiveUser = activeUser;
            ((AddMedicineViewModel)AddMedicineView.DataContext).MenuViewModel = this;
        }

        public void gotoEditMedicineView(User activeUser, Medicine selectedMedicine)
        {
            EditMedicineView = new EditMedicineView();
            MainWindowViewModel.ActivePage = EditMedicineView;
            ((EditMedicineViewModel)EditMedicineView.DataContext).ActiveUser = activeUser;
            ((EditMedicineViewModel)EditMedicineView.DataContext).Medicine = selectedMedicine;
            ((EditMedicineViewModel)EditMedicineView.DataContext).CopyMedicine = (Medicine)selectedMedicine.Clone();
            ((EditMedicineViewModel)EditPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoMaintenance()
        {
            MainWindowViewModel.ActivePage = MaintenanceView;
        }

        public void gotoAddAppointmentView(User activeUser)
        {
            AddAppointmentView = new AddAppointmentView();
            MainWindowViewModel.ActivePage = AddAppointmentView;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).ActiveUser = activeUser;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).MenuViewModel = this;
        }
    }
}
