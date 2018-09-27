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
using AllAboutTeethDCMS.Operations;
using AllAboutTeethDCMS.TreatmentRecords;
using AllAboutTeethDCMS.Reports;
using AllAboutTeethDCMS.Billings;
using AllAboutTeethDCMS.Dashboard;
using AllAboutTeethDCMS.ActivityLogs;
using System.Collections.ObjectModel;

namespace AllAboutTeethDCMS.Menu
{
    public class MenuViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;
        private User activeUser;

        private TreatmentRecordView operationView;
        private AddOperationView addOperationView;

        public MenuViewModel()
        {
            DashboardCommand = new DelegateCommand(new Action(GotoDashboard));
        }

        #region Users
        private UserView userView;
        private AddUserView addUserView;
        private EditUserView editUserView;

        public UserView UserView { get => userView; set => userView = value; }
        public AddUserView AddUserView { get => addUserView; set => addUserView = value; }
        public EditUserView EditUserView { get => editUserView; set => editUserView = value; }

        public void gotoUsers()
        {
            if (UserView == null)
            {
                UserView = new UserView();
            }
            MainWindowViewModel.ActivePage = UserView;
            ((UserViewModel)UserView.DataContext).ActiveUser = ActiveUser;
            if (ActiveUser.Type.Equals("Administrator"))
            {
                ((UserViewModel)UserView.DataContext).AddVisibility = "Visible";
            }
            ((UserViewModel)UserView.DataContext).MenuViewModel = this;
            ((UserViewModel)UserView.DataContext).LoadUsers();
        }

        public void GotoAddUserView()
        {
            if (AddUserView == null)
            {
                AddUserView = new AddUserView();
            }
            MainWindowViewModel.ActivePage = AddUserView;
            ((AddUserViewModel)AddUserView.DataContext).ActiveUser = ActiveUser;
            ((AddUserViewModel)AddUserView.DataContext).MenuViewModel = this;
            ((AddUserViewModel)AddUserView.DataContext).ImageCamera = AddUserView.camera.image;
        }

        public void GotoEditUserView(User selectedUser)
        {
            EditUserView = new EditUserView();
            MainWindowViewModel.ActivePage = EditUserView;
            ((EditUserViewModel)EditUserView.DataContext).ActiveUser = ActiveUser;
            ((EditUserViewModel)EditUserView.DataContext).User = selectedUser;
            ((EditUserViewModel)EditUserView.DataContext).CopyUser = (User)selectedUser.Clone();
            ((EditUserViewModel)EditUserView.DataContext).MenuViewModel = this;
            ((EditUserViewModel)EditUserView.DataContext).ImageCamera = EditUserView.camera.image;
        }
        #endregion

        #region Patients
        private PatientView patientView;
        private AddPatientView addPatientView;
        private EditPatientView editPatientView;

        public PatientView PatientView { get => patientView; set => patientView = value; }
        public AddPatientView AddPatientView { get => addPatientView; set => addPatientView = value; }
        public EditPatientView EditPatientView { get => editPatientView; set => editPatientView = value; }

        public void gotoPatients()
        {
            if (PatientView == null)
            {
                PatientView = new PatientView();
            }
            MainWindowViewModel.ActivePage = PatientView;
            ((PatientViewModel)PatientView.DataContext).ActiveUser = ActiveUser;
            ((PatientViewModel)PatientView.DataContext).MenuViewModel = this;
            ((PatientViewModel)PatientView.DataContext).LoadPatients();
        }
        public void GotoAddPatientView()
        {
            if (AddPatientView == null)
            {
                AddPatientView = new AddPatientView();
            }
            MainWindowViewModel.ActivePage = AddPatientView;
            ((AddPatientViewModel)AddPatientView.DataContext).ActiveUser = ActiveUser;
            ((AddPatientViewModel)AddPatientView.DataContext).MenuViewModel = this;
            ((AddPatientViewModel)AddPatientView.DataContext).ImageCamera = AddPatientView.camera.image;
        }

        public void GotoEditPatientView(Patient selectedPatient)
        {
            if (EditPatientView == null)
            {
                EditPatientView = new EditPatientView();
            }
            MainWindowViewModel.ActivePage = EditPatientView;
            ((EditPatientViewModel)EditPatientView.DataContext).ActiveUser = ActiveUser;
            ((EditPatientViewModel)EditPatientView.DataContext).Patient = selectedPatient;
            ((EditPatientViewModel)EditPatientView.DataContext).CopyPatient = (Patient)selectedPatient.Clone();
            ((EditPatientViewModel)EditPatientView.DataContext).MenuViewModel = this;
            ((EditPatientViewModel)EditPatientView.DataContext).ImageCamera = EditPatientView.camera.image;
        }
        #endregion

        #region Treatments
        private TreatmentView treatmentView;
        private AddTreatmentView addTreatmentView;
        private EditTreatmentView editTreatmentView;

        public TreatmentView TreatmentView { get => treatmentView; set => treatmentView = value; }
        public AddTreatmentView AddTreatmentView { get => addTreatmentView; set => addTreatmentView = value; }
        public EditTreatmentView EditTreatmentView { get => editTreatmentView; set => editTreatmentView = value; }

        public void gotoTreatments()
        {
            if (TreatmentView == null)
            {
                TreatmentView = new TreatmentView();
            }
            MainWindowViewModel.ActivePage = TreatmentView;
            ((TreatmentViewModel)TreatmentView.DataContext).ActiveUser = ActiveUser;
            ((TreatmentViewModel)TreatmentView.DataContext).MenuViewModel = this;
            ((TreatmentViewModel)TreatmentView.DataContext).LoadTreatments();
            ((TreatmentViewModel)TreatmentView.DataContext).AddVisibility = "Collapsed";
            if (ActiveUser.Type.Equals("Administrator", StringComparison.InvariantCultureIgnoreCase))
                ((TreatmentViewModel)TreatmentView.DataContext).AddVisibility = "Visible";
        }

        public void GotoEditTreatmentView(Treatment treatment)
        {
            if (EditTreatmentView == null)
            {
                EditTreatmentView = new EditTreatmentView();
            }
            MainWindowViewModel.ActivePage = EditTreatmentView;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).ActiveUser = ActiveUser;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).Treatment = treatment;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).CopyTreatment = (Treatment)treatment.Clone();
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).MenuViewModel = this;
        }

        public void GotoAddTreatmentView()
        {
            if (AddTreatmentView == null)
            {
                AddTreatmentView = new AddTreatmentView();
            }
            MainWindowViewModel.ActivePage = AddTreatmentView;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).ActiveUser = ActiveUser;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).MenuViewModel = this;
        }
        #endregion

        #region Suppliers
        private SupplierView supplierView;
        private AddSupplierView addSupplierView;
        private EditSupplierView editSupplierView;

        public SupplierView SupplierView { get => supplierView; set => supplierView = value; }
        public AddSupplierView AddSupplierView { get => addSupplierView; set => addSupplierView = value; }
        public EditSupplierView EditSupplierView { get => editSupplierView; set => editSupplierView = value; }

        public void gotoSuppliers()
        {
            if (SupplierView == null)
            {
                SupplierView = new SupplierView();
            }
            MainWindowViewModel.ActivePage = SupplierView;
            ((SupplierViewModel)SupplierView.DataContext).ActiveUser = ActiveUser;
            ((SupplierViewModel)SupplierView.DataContext).MenuViewModel = this;
            if (ActiveUser.Type.Equals("Administrator"))
            {
                ((SupplierViewModel)SupplierView.DataContext).AddVisibility = "Visible";
            }
            else
            {
                ((SupplierViewModel)SupplierView.DataContext).AddVisibility = "Collapsed";
            }
            ((SupplierViewModel)SupplierView.DataContext).LoadSuppliers();
        }

        public void GotoAddSupplierView()
        {
            if (AddSupplierView == null)
            {
                AddSupplierView = new AddSupplierView();
            }
            MainWindowViewModel.ActivePage = AddSupplierView;
            ((AddSupplierViewModel)AddSupplierView.DataContext).ActiveUser = ActiveUser;
            ((AddSupplierViewModel)AddSupplierView.DataContext).MenuViewModel = this;
        }

        public void GotoEditSupplierView(Supplier selectedSupplier)
        {
            if (EditSupplierView == null)
            {
                EditSupplierView = new EditSupplierView();
            }
            MainWindowViewModel.ActivePage = EditSupplierView;
            ((EditSupplierViewModel)EditSupplierView.DataContext).ActiveUser = ActiveUser;
            ((EditSupplierViewModel)EditSupplierView.DataContext).Supplier = selectedSupplier;
            ((EditSupplierViewModel)EditSupplierView.DataContext).CopySupplier = (Supplier)selectedSupplier.Clone();
            ((EditSupplierViewModel)EditSupplierView.DataContext).MenuViewModel = this;
        }
        #endregion

        #region Providers
        private ProviderView providerView;
        private AddProviderView addProviderView;
        private EditProviderView editProviderView;

        public ProviderView ProviderView { get => providerView; set => providerView = value; }
        public AddProviderView AddProviderView { get => addProviderView; set => addProviderView = value; }
        public EditProviderView EditProviderView { get => editProviderView; set => editProviderView = value; }

        public void gotoProviders()
        {
            if (ProviderView == null)
            {
                ProviderView = new ProviderView();
            }
            MainWindowViewModel.ActivePage = ProviderView;
            ((ProviderViewModel)ProviderView.DataContext).ActiveUser = ActiveUser;
            ((ProviderViewModel)ProviderView.DataContext).MenuViewModel = this;
            if (ActiveUser.Type.Equals("Administrator"))
            {
                ((ProviderViewModel)ProviderView.DataContext).AddVisibility = "Visible";
            }
            else
            {
                ((ProviderViewModel)ProviderView.DataContext).AddVisibility = "Collapsed";
            }
            ((ProviderViewModel)ProviderView.DataContext).LoadProviders();
        }

        public void GotoEditProviderView(Provider provider)
        {
            if (EditProviderView == null)
            {
                EditProviderView = new EditProviderView();
            }
            MainWindowViewModel.ActivePage = EditProviderView;
            ((EditProviderViewModel)EditProviderView.DataContext).ActiveUser = ActiveUser;
            ((EditProviderViewModel)EditProviderView.DataContext).Provider = provider;
            ((EditProviderViewModel)EditProviderView.DataContext).CopyProvider = (Provider)provider.Clone();
            ((EditProviderViewModel)EditProviderView.DataContext).MenuViewModel = this;
        }

        public void GotoAddProviderView()
        {
            if (AddProviderView == null)
            {
                AddProviderView = new AddProviderView();
            }
            MainWindowViewModel.ActivePage = AddProviderView;
            ((AddProviderViewModel)AddProviderView.DataContext).ActiveUser = ActiveUser;
            ((AddProviderViewModel)AddProviderView.DataContext).MenuViewModel = this;
        }
        #endregion

        #region Items
        private MedicineView itemView;
        private AddMedicineView addMedicineView;
        private EditMedicineView editMedicineView;

        public MedicineView MedicineView { get => itemView; set => itemView = value; }
        public AddMedicineView AddMedicineView { get => addMedicineView; set => addMedicineView = value; }
        public EditMedicineView EditMedicineView { get => editMedicineView; set => editMedicineView = value; }

        public void gotoMedicines()
        {
            if (MedicineView == null)
            {
                MedicineView = new MedicineView();
            }
            MainWindowViewModel.ActivePage = MedicineView;
            ((MedicineViewModel)MedicineView.DataContext).ActiveUser = ActiveUser;
            ((MedicineViewModel)MedicineView.DataContext).MenuViewModel = this;
            if (!ActiveUser.Type.Equals("Dentist"))
            {
                ((MedicineViewModel)MedicineView.DataContext).AddVisibility = "Visible";
            }
            else
            {
                ((MedicineViewModel)MedicineView.DataContext).AddVisibility = "Collapsed";
            }
            ((MedicineViewModel)MedicineView.DataContext).LoadMedicines();
        }

        public void GotoAddMedicineView()
        {
            if (AddMedicineView == null)
            {
                AddMedicineView = new AddMedicineView();
            }
            MainWindowViewModel.ActivePage = AddMedicineView;
            ((AddMedicineViewModel)AddMedicineView.DataContext).ActiveUser = ActiveUser;
            ((AddMedicineViewModel)AddMedicineView.DataContext).MenuViewModel = this;
            ((AddMedicineViewModel)AddMedicineView.DataContext).SupplierViewModel.Suppliers = null;
            ((AddMedicineViewModel)AddMedicineView.DataContext).startLoadThread();
            ((AddMedicineViewModel)AddMedicineView.DataContext).SupplierViewModel.LoadSuppliers();
        }

        public void GotoEditMedicineView(Medicine selectedMedicine)
        {
            if (EditMedicineView == null)
            {
                EditMedicineView = new EditMedicineView();
            }
            MainWindowViewModel.ActivePage = EditMedicineView;
            ((EditMedicineViewModel)EditMedicineView.DataContext).ActiveUser = ActiveUser;
            ((EditMedicineViewModel)EditMedicineView.DataContext).Medicine = selectedMedicine;
            ((EditMedicineViewModel)EditMedicineView.DataContext).CopyMedicine = (Medicine)selectedMedicine.Clone();
            ((EditMedicineViewModel)EditMedicineView.DataContext).MenuViewModel = this;
        }
        #endregion

        #region Appointments
        private AppointmentView appointmentView;
        private AddAppointmentView addAppointmentView;
        private EditAppointmentView editAppointmentView;

        public AppointmentView AppointmentView { get => appointmentView; set => appointmentView = value; }
        public AddAppointmentView AddAppointmentView { get => addAppointmentView; set => addAppointmentView = value; }
        public EditAppointmentView EditAppointmentView { get => editAppointmentView; set => editAppointmentView = value; }

        public void gotoAppointments()
        {
            if (AppointmentView == null)
            {
                AppointmentView = new AppointmentView();
            }
            MainWindowViewModel.ActivePage = AppointmentView;
            ((AppointmentViewModel)AppointmentView.DataContext).UniqueAppointment = new ObservableCollection<Appointment>();
            ((AppointmentViewModel)AppointmentView.DataContext).ActiveUser = ActiveUser;
            ((AppointmentViewModel)AppointmentView.DataContext).MenuViewModel = this;
            ((AppointmentViewModel)AppointmentView.DataContext).LoadAppointments();
        }

        public void GotoAddAppointmentView()
        {
            if (AddAppointmentView == null)
            {
                AddAppointmentView = new AddAppointmentView();
            }
            MainWindowViewModel.ActivePage = AddAppointmentView;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).ActiveUser = ActiveUser;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).MenuViewModel = this;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).PatientViewModel.Patients = null;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).TreatmentViewModel.Treatments = null;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).UserViewModel.Users = null;
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).startLoadPatientsThread();
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).startLoadTreatmentsThread();
            ((AddAppointmentViewModel)AddAppointmentView.DataContext).startLoadUsersThread();
        }

        public void GotoEditAppointmentView(Appointment selectedAppointment)
        {
            if (EditAppointmentView == null)
            {
                EditAppointmentView = new EditAppointmentView();
            }
            MainWindowViewModel.ActivePage = EditAppointmentView;
            ((EditAppointmentViewModel)EditAppointmentView.DataContext).ActiveUser = ActiveUser;
            ((EditAppointmentViewModel)EditAppointmentView.DataContext).Appointment = selectedAppointment;
            ((EditAppointmentViewModel)EditAppointmentView.DataContext).CopyAppointment = (Appointment)selectedAppointment.Clone();
            ((EditAppointmentViewModel)EditAppointmentView.DataContext).MenuViewModel = this;
        }
        #endregion

        public void gotoAddOperationView(Appointment appointment, List<Appointment> appointments)
        {
            AddOperationView = new AddOperationView();
            MainWindowViewModel.ActivePage = AddOperationView;
            ((AddOperationViewModel)AddOperationView.DataContext).ActiveUser = activeUser;
            ((AddOperationViewModel)AddOperationView.DataContext).Appointments = appointments;
            ((AddOperationViewModel)AddOperationView.DataContext).Appointment = appointment;
            ((AddOperationViewModel)AddOperationView.DataContext).MenuViewModel = this;
            ((AddOperationViewModel)AddOperationView.DataContext).initialize();
        }

        private MaintenanceView maintenanceView;
        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }


        public MaintenanceView MaintenanceView { get => maintenanceView; set => maintenanceView = value; }
        public User ActiveUser { get => activeUser; set => activeUser = value; }
        public TreatmentRecordView TreatmentRecordView { get => operationView; set => operationView = value; }
        public AddOperationView AddOperationView { get => addOperationView; set => addOperationView = value; }
        public BillingView BillingView { get => billingView; set => billingView = value; }
        public InvoiceView InvoiceView { get => invoiceView; set => invoiceView = value; }
        public TransactionReportView TransactionReportView { get => transactionReportView; set => transactionReportView = value; }
        public DashboardView DashboardView { get => dashboardView; set => dashboardView = value; }
        public ActivityLogView ActivityLogView { get => activityLogView; set => activityLogView = value; }

        public void gotoOperations(User activeUser)
        {
            TreatmentRecordView = new TreatmentRecordView();
            MainWindowViewModel.ActivePage = TreatmentRecordView;
            ((TreatmentRecordViewModel)TreatmentRecordView.DataContext).ActiveUser = activeUser;
            ((TreatmentRecordViewModel)TreatmentRecordView.DataContext).MenuViewModel = this;
            ((TreatmentRecordViewModel)TreatmentRecordView.DataContext).TreatmentRecords = null;
        }

        public void gotoMaintenance()
        {
            MainWindowViewModel.ActivePage = MaintenanceView;
        }

        private BillingView billingView;

        public void gotoBillings()
        {
            if (BillingView == null)
            {
                BillingView = new BillingView();
            }
            MainWindowViewModel.ActivePage = BillingView;
            ((BillingViewModel)BillingView.DataContext).ActiveUser = ActiveUser;
            ((BillingViewModel)BillingView.DataContext).MenuViewModel = this;
            ((BillingViewModel)BillingView.DataContext).Billings = null;
        }

        private InvoiceView invoiceView;

        public void gotoInvoices()
        {
            if (InvoiceView == null)
            {
                InvoiceView = new InvoiceView();
                ((InvoiceViewModel)InvoiceView.DataContext).MenuViewModel = this;
            }
            MainWindowViewModel.ActivePage = InvoiceView;
        }

        private TransactionReportView transactionReportView;

        public void gotoTransactionReports()
        {
            if (TransactionReportView == null)
            {
                TransactionReportView = new TransactionReportView();
            }
            MainWindowViewModel.ActivePage = TransactionReportView;
        }

        private bool _isInAppointments;

        public bool IsInAppointments
        {
            get { return _isInAppointments; }
            set { 
                _isInAppointments = value;
                OnPropertyChanged();
            }
        }


        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private DashboardView dashboardView;

        public void GotoDashboard()
        {
            if (DashboardView == null)
            {
                DashboardView = new DashboardView();
            }
            MainWindowViewModel.ActivePage = DashboardView;
            ((DashboardViewModel)DashboardView.DataContext).load();
            IsChecked = true;
        }

        private ActivityLogView activityLogView;

        public void gotoActivityLogs()
        {
            if (ActivityLogView == null)
            {
                ActivityLogView = new ActivityLogView();
            }
            MainWindowViewModel.ActivePage = ActivityLogView;
        }

        private DelegateCommand dashboardCommand;
        public DelegateCommand DashboardCommand { get => dashboardCommand; set => dashboardCommand = value; }

        public void Reset()
        {
            DashboardView = null;
            BillingView = null;
            InvoiceView = null;
            TransactionReportView = null;
            ActivityLogView = null;

            UserView = null;
            AddUserView = null;
            EditUserView = null;

            PatientView = null;
            AddPatientView = null;
            EditPatientView = null;

            SupplierView = null;
            AddSupplierView = null;
            EditSupplierView = null;

            TreatmentView = null;
            AddTreatmentView = null;
            EditTreatmentView = null;

            ProviderView = null;
            AddProviderView = null;
            EditProviderView = null;

            AppointmentView = null;
            AddAppointmentView = null;
            EditAppointmentView = null;

            MedicineView = null;
            AddMedicineView = null;
            EditMedicineView = null;
        }
    }
}
