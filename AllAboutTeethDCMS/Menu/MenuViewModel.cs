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

namespace AllAboutTeethDCMS.Menu
{
    public class MenuViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;

        private User activeUser;

        private OperationView operationView;
        private AddOperationView addOperationView;

        private AppointmentView appointmentView;
        private AddAppointmentView addAppointmentView;

        #region Users
        private UserView userView;
        private AddUserView addUserView;
        private EditUserView editUserView;

        public UserView UserView { get => userView; set => userView = value; }
        public AddUserView AddUserView { get => addUserView; set => addUserView = value; }
        public EditUserView EditUserView { get => editUserView; set => editUserView = value; }

        public void gotoUsers()
        {
            if(UserView==null)
            {
                UserView = new UserView();
            }
            MainWindowViewModel.ActivePage = UserView;
            ((UserViewModel)UserView.DataContext).ActiveUser = ActiveUser;
            ((UserViewModel)UserView.DataContext).MenuViewModel = this;
        }

        public void gotoAddUserView()
        {
            if(AddUserView==null)
            {
                AddUserView = new AddUserView();
            }
            MainWindowViewModel.ActivePage = AddUserView;
            ((AddUserViewModel)AddUserView.DataContext).ActiveUser = ActiveUser;
            ((AddUserViewModel)AddUserView.DataContext).MenuViewModel = this;
        }

        public void gotoEditUserView(User selectedUser)
        {
            if(EditUserView==null)
            {
                EditUserView = new EditUserView();
            }
            MainWindowViewModel.ActivePage = EditUserView;
            ((EditUserViewModel)EditUserView.DataContext).ActiveUser = ActiveUser;
            ((EditUserViewModel)EditUserView.DataContext).User = selectedUser;
            ((EditUserViewModel)EditUserView.DataContext).CopyUser = (User)selectedUser.Clone();
            ((EditUserViewModel)EditUserView.DataContext).MenuViewModel = this;
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
            if(PatientView==null)
            {
                PatientView = new PatientView();
            }
            MainWindowViewModel.ActivePage = PatientView;
            ((PatientViewModel)PatientView.DataContext).ActiveUser = ActiveUser;
            ((PatientViewModel)PatientView.DataContext).MenuViewModel = this;
        }
        public void gotoAddPatientView()
        {
            if(AddPatientView==null)
            {
                AddPatientView = new AddPatientView();
            }
            MainWindowViewModel.ActivePage = AddPatientView;
            ((AddPatientViewModel)AddPatientView.DataContext).ActiveUser = ActiveUser;
            ((AddPatientViewModel)AddPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoEditPatientView(Patient selectedPatient)
        {
            if(EditPatientView==null)
            {
                EditPatientView = new EditPatientView();
            }
            MainWindowViewModel.ActivePage = EditPatientView;
            ((EditPatientViewModel)EditPatientView.DataContext).ActiveUser = ActiveUser;
            ((EditPatientViewModel)EditPatientView.DataContext).Patient = selectedPatient;
            ((EditPatientViewModel)EditPatientView.DataContext).CopyPatient = (Patient)selectedPatient.Clone();
            ((EditPatientViewModel)EditPatientView.DataContext).MenuViewModel = this;
        }
        #endregion


        private TreatmentView treatmentView;
        private AddTreatmentView addTreatmentView;
        private EditTreatmentView editTreatmentView;

        #region Suppliers
        private SupplierView supplierView;
        private AddSupplierView addSupplierView;
        private EditSupplierView editSupplierView;

        public SupplierView SupplierView { get => supplierView; set => supplierView = value; }
        public AddSupplierView AddSupplierView { get => addSupplierView; set => addSupplierView = value; }
        public EditSupplierView EditSupplierView { get => editSupplierView; set => editSupplierView = value; }

        public void gotoSuppliers()
        {
            if(SupplierView==null)
            {
                SupplierView = new SupplierView();
            }
            MainWindowViewModel.ActivePage = SupplierView;
            ((SupplierViewModel)SupplierView.DataContext).ActiveUser = ActiveUser;
            ((SupplierViewModel)SupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoAddSupplierView()
        {
            if(AddSupplierView==null)
            {
                AddSupplierView = new AddSupplierView();
            }
            MainWindowViewModel.ActivePage = AddSupplierView;
            ((AddSupplierViewModel)AddSupplierView.DataContext).ActiveUser = ActiveUser;
            ((AddSupplierViewModel)AddSupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoEditSupplierView(Supplier selectedSupplier)
        {
            if(EditSupplierView==null)
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
            if(ProviderView==null)
            {
                ProviderView = new ProviderView();
            }
            MainWindowViewModel.ActivePage = ProviderView;
            ((ProviderViewModel)ProviderView.DataContext).ActiveUser = ActiveUser;
            ((ProviderViewModel)ProviderView.DataContext).MenuViewModel = this;
        }

        public void gotoEditProviderView(Provider provider)
        {
            if(EditProviderView==null)
            {
                EditProviderView = new EditProviderView();
            }
            MainWindowViewModel.ActivePage = EditProviderView;
            ((EditProviderViewModel)EditProviderView.DataContext).ActiveUser = ActiveUser;
            ((EditProviderViewModel)EditProviderView.DataContext).Provider = provider;
            ((EditProviderViewModel)EditProviderView.DataContext).CopyProvider = (Provider)provider.Clone();
            ((EditProviderViewModel)EditProviderView.DataContext).MenuViewModel = this;
        }

        public void gotoAddProviderView()
        {
            if(AddAppointmentView==null)
            {
                AddProviderView = new AddProviderView();
            }
            MainWindowViewModel.ActivePage = AddProviderView;
            ((AddProviderViewModel)AddProviderView.DataContext).ActiveUser = ActiveUser;
            ((AddProviderViewModel)AddProviderView.DataContext).MenuViewModel = this;
        }
        #endregion

        private MedicineView itemView;
        private AddMedicineView addMedicineView;
        private EditMedicineView editMedicineView;

        private MaintenanceView maintenanceView;

        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }

        public AppointmentView AppointmentView { get => appointmentView; set => appointmentView = value; }
        public TreatmentView TreatmentView { get => TreatmentView1; set => TreatmentView1 = value; }
        public MaintenanceView MaintenanceView { get => maintenanceView; set => maintenanceView = value; }

        public User ActiveUser { get => activeUser; set => activeUser = value; }
        public TreatmentView TreatmentView1 { get => treatmentView; set => treatmentView = value; }
        public AddTreatmentView AddTreatmentView { get => addTreatmentView; set => addTreatmentView = value; }
        public EditTreatmentView EditTreatmentView { get => editTreatmentView; set => editTreatmentView = value; }
        public MedicineView MedicineView { get => ItemView; set => ItemView = value; }
        public MedicineView ItemView { get => itemView; set => itemView = value; }
        public AddMedicineView AddMedicineView { get => addMedicineView; set => addMedicineView = value; }
        public EditMedicineView EditMedicineView { get => editMedicineView; set => editMedicineView = value; }

        public AddAppointmentView AddAppointmentView { get => addAppointmentView; set => addAppointmentView = value; }
        public OperationView OperationView { get => operationView; set => operationView = value; }

        public AddOperationView AddOperationView { get => addOperationView; set => addOperationView = value; }

        public void gotoOperations(User activeUser)
        {
            OperationView = new OperationView();
            MainWindowViewModel.ActivePage = OperationView;
            ((OperationViewModel)OperationView.DataContext).ActiveUser = activeUser;
            ((OperationViewModel)OperationView.DataContext).MenuViewModel = this;
            ((OperationViewModel)OperationView.DataContext).loadOperations();
        }

        public void gotoAddOperationView(User activeUser, Appointment appointment)
        {
            AddOperationView = new AddOperationView();
            MainWindowViewModel.ActivePage = AddOperationView;
            ((AddOperationViewModel)AddOperationView.DataContext).ActiveUser = activeUser;
            ((AddOperationViewModel)AddOperationView.DataContext).Appointment = appointment;
            ((AddOperationViewModel)AddOperationView.DataContext).MenuViewModel = this;
        }

        public void gotoAppointments(User activeUser)
        {
            AppointmentView = new AppointmentView();
            MainWindowViewModel.ActivePage = AppointmentView;
            ((AppointmentViewModel)AppointmentView.DataContext).ActiveUser = activeUser;
            ((AppointmentViewModel)AppointmentView.DataContext).MenuViewModel = this;
            ((AppointmentViewModel)AppointmentView.DataContext).loadAppointments();
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
