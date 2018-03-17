using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Dentists;
using AllAboutTeethDCMS.Items;
using AllAboutTeethDCMS.Maintenance;
using AllAboutTeethDCMS.Patients;
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
        private DentistView dentistView;
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
        
        private ItemView itemView;
        private AddItemView addItemView;
        private EditItemView editItemView;
        

        private MaintenanceView maintenanceView;

        public MenuViewModel()
        {
            AppointmentView = new AppointmentView();
            PatientView = new PatientView();
            TreatmentView = new TreatmentView();
            DentistView = new DentistView();
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
            ItemView = new ItemView();
            AddItemView = new AddItemView();
            EditItemView = new EditItemView();
        }

        public void gotoEditTreatmentView(User activeUser, Treatment treatment)
        {
            MainWindowViewModel.ActivePage = EditTreatmentView;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).ActiveUser = activeUser;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).Treatment = treatment;
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).CopyTreatment = (Treatment)treatment.Clone();
            ((EditTreatmentViewModel)EditTreatmentView.DataContext).MenuViewModel = this;
        }

        public void gotoAddTreatmentView(User activeUser)
        {
            MainWindowViewModel.ActivePage = AddTreatmentView;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).ActiveUser = activeUser;
            ((AddTreatmentViewModel)AddTreatmentView.DataContext).MenuViewModel = this;
        }

        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }

        public AppointmentView AppointmentView { get => appointmentView; set => appointmentView = value; }
        public PatientView PatientView { get => patientView; set => patientView = value; }
        public TreatmentView TreatmentView { get => TreatmentView1; set => TreatmentView1 = value; }
        public DentistView DentistView { get => dentistView; set => dentistView = value; }
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
        public ItemView ItemView { get => itemView; set => itemView = value; }
        public AddItemView AddItemView { get => addItemView; set => addItemView = value; }
        public EditItemView EditItemView { get => editItemView; set => editItemView = value; }

        public void gotoAppointments()
        {
            MainWindowViewModel.ActivePage = AppointmentView;
        }

        public void gotoPatients(User activeUser)
        {
            MainWindowViewModel.ActivePage = PatientView;
            ((PatientViewModel)PatientView.DataContext).ActiveUser = activeUser;
            ((PatientViewModel)PatientView.DataContext).MenuViewModel = this;
        }

        public void gotoTreatments(User activeUser)
        {
            MainWindowViewModel.ActivePage = TreatmentView;
            ((TreatmentViewModel)TreatmentView.DataContext).ActiveUser = activeUser;
            ((TreatmentViewModel)TreatmentView.DataContext).MenuViewModel = this;
        }

        public void gotoDentists()
        {
            MainWindowViewModel.ActivePage = DentistView;
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
            ((EditUserViewModel)EditUserView.DataContext).CopyUser = (User)selectedUser.Clone();
            ((EditUserViewModel)EditUserView.DataContext).MenuViewModel = this;
        }

        public void gotoSuppliers(User activeUser)
        {
            MainWindowViewModel.ActivePage = SupplierView;
            ((SupplierViewModel)SupplierView.DataContext).ActiveUser = activeUser;
            ((SupplierViewModel)SupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoAddSupplierView(User activeUser)
        {
            MainWindowViewModel.ActivePage = AddSupplierView;
            ((AddSupplierViewModel)AddSupplierView.DataContext).ActiveUser = activeUser;
            ((AddSupplierViewModel)AddSupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoEditSupplierView(User activeUser, Supplier selectedSupplier)
        {
            MainWindowViewModel.ActivePage = EditSupplierView;
            ((EditSupplierViewModel)EditSupplierView.DataContext).ActiveUser = activeUser;
            ((EditSupplierViewModel)EditSupplierView.DataContext).Supplier = selectedSupplier;
            ((EditSupplierViewModel)EditSupplierView.DataContext).CopySupplier = (Supplier)selectedSupplier.Clone();
            ((EditSupplierViewModel)EditSupplierView.DataContext).MenuViewModel = this;
        }

        public void gotoAddPatientView(User activeUser)
        {
            MainWindowViewModel.ActivePage = AddPatientView;
            ((AddPatientViewModel)AddPatientView.DataContext).ActiveUser = activeUser;
            ((AddPatientViewModel)AddPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoEditPatientView(User activeUser, Patient selectedPatient)
        {
            MainWindowViewModel.ActivePage = EditPatientView;
            ((EditPatientViewModel)EditPatientView.DataContext).ActiveUser = activeUser;
            ((EditPatientViewModel)EditPatientView.DataContext).Patient = selectedPatient;
            ((EditPatientViewModel)EditPatientView.DataContext).CopyPatient = (Patient)selectedPatient.Clone();
            ((EditPatientViewModel)EditPatientView.DataContext).MenuViewModel = this;
        }

        public void gotoItems(User activeUser)
        {
            MainWindowViewModel.ActivePage = ItemView;
            ((ItemViewModel)ItemView.DataContext).ActiveUser = activeUser;
            ((ItemViewModel)ItemView.DataContext).MenuViewModel = this;
        }

        public void gotoAddItemView(User activeUser)
        {
            MainWindowViewModel.ActivePage = AddItemView;
            ((AddItemViewModel)AddItemView.DataContext).ActiveUser = activeUser;
            ((AddItemViewModel)AddItemView.DataContext).MenuViewModel = this;
        }

        public void gotoEditItemView(User activeUser, Item selectedItem)
        {
            MainWindowViewModel.ActivePage = EditItemView;
            ((EditItemViewModel)EditItemView.DataContext).ActiveUser = activeUser;
            ((EditItemViewModel)EditItemView.DataContext).Item = selectedItem;
            ((EditItemViewModel)EditItemView.DataContext).CopyItem = (Item)selectedItem.Clone();
            ((EditItemViewModel)EditItemView.DataContext).MenuViewModel = this;
        }


        public void gotoMaintenance()
        {
            MainWindowViewModel.ActivePage = MaintenanceView;
        }
    }
}
