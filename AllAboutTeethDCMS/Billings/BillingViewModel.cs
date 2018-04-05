using AllAboutTeethDCMS.Payments;
using AllAboutTeethDCMS.Reports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS.Billings
{
    public class BillingViewModel : CRUDPage<Billing>
    {
        #region Fields
        private Billing billing;
        private List<Billing> billings;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string paymentVisibility = "Collapsed";

        private AddPaymentViewModel addPaymentViewModel;

        private DelegateCommand addPaymentCommand;

        public void AddPayment()
        {
            if(Double.Parse(AddPaymentViewModel.AmountPaid) >0)
            {
                if (MessageBox.Show("Are you sure you want to add this payment?", "Add Payment", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    AddPaymentViewModel.ActiveUser = ActiveUser;
                    AddPaymentViewModel.Billing = Billing;
                    AddPaymentViewModel.Balance = Billing.Balance - Double.Parse(AddPaymentViewModel.AmountPaid);
                    AddPaymentViewModel.savePayment();
                    AddPaymentViewModel.AmountPaid = "0";
                    MessageBox.Show("Payment successfully added", "Add Payment", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBillings();
                    MenuViewModel.gotoInvoices();
                    Invoice invoice = new Invoice();
                    MenuViewModel.InvoiceView.viewer.ViewerCore.ReportSource = invoice;
                    MenuViewModel.InvoiceView.viewer.ViewerCore.SelectionFormula = "{allaboutteeth_billings1.billing_no} = " + Billing.No;
                }
            }
        }

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";

        private PaymentViewModel paymentViewModel;
        #endregion

        public BillingViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadBillings));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteBilling));
            AddCommand = new DelegateCommand(new Action(GotoAddBilling));
            EditCommand = new DelegateCommand(new Action(GotoEditBilling));
            AddPaymentViewModel = new AddPaymentViewModel();
            AddPaymentCommand = new DelegateCommand(new Action(AddPayment));
            PaymentViewModel = new PaymentViewModel();
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            //DialogBoxViewModel.Answer = "None";
            //DialogBoxViewModel.Mode = "Question";
            //if (Billing.Status.Equals("Active"))
            //{
            //    DialogBoxViewModel.Title = "Archive Billing";
            //    DialogBoxViewModel.Message = "Are you sure you want to archive this billing?";
            //}
            //else
            //{
            //    DialogBoxViewModel.Title = "Unarchive Billing";
            //    DialogBoxViewModel.Message = "Are you sure you want to activate this billing?";
            //}
            //while (DialogBoxViewModel.Answer.Equals("None"))
            //{
            //    Thread.Sleep(100);
            //}
            //if (DialogBoxViewModel.Answer.Equals("Yes"))
            //{
            //    if (Billing.Status.Equals("Active"))
            //    {
            //        Billing.Status = "Archived";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Archiving billing. Please wait.";
            //        DialogBoxViewModel.Answer = "None";
            //    }
            //    else
            //    {
            //        Billing.Status = "Active";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Activating billing. Please wait.";
            //        DialogBoxViewModel.Answer = "None";
            //    }
            //    return true;
            //}
            return true;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                LoadBillings();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeDelete()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Delete Billing";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this billing?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting billing. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadBillings();
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeCreate()
        {
            return true;
        }

        protected override void afterCreate(bool isSuccessful)
        {
        }

        protected override void beforeLoad(MySqlCommand command)
        {
        }

        protected override void afterLoad(List<Billing> list)
        {
            Billings = list;
            foreach(Billing billing in Billings)
            {
                CreateConnection();
                MySqlCommand command = Connection.CreateCommand();
                command.CommandText = "select * from allaboutteeth_payments where payment_billing="+billing.No+" ORDER BY payment_dateadded DESC";
                MySqlDataReader reader = command.ExecuteReader();
                if(reader.Read())
                {
                    billing.Balance = reader.GetDouble("payment_balance");
                }
                reader.Close();
                Connection.Close();
                UpdateDatabase(billing, "allaboutteeth_billings");
            }
            FilterResult = "";
            if (list.Count > 1)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }
        #endregion

        #region Properties
        public DelegateCommand LoadCommand { get => loadCommand; set => loadCommand = value; }
        public DelegateCommand ArchiveCommand { get => archiveCommand; set => archiveCommand = value; }
        public DelegateCommand UnarchiveCommand { get => unarchiveCommand; set => unarchiveCommand = value; }
        public DelegateCommand DeleteCommand { get => deleteCommand; set => deleteCommand = value; }
        public DelegateCommand AddCommand { get => addCommand; set => addCommand = value; }
        public DelegateCommand EditCommand { get => editCommand; set => editCommand = value; }

        private double totalPaid = 0;

        public Billing Billing
        {
            get => billing;
            set
            {
                billing = value;
                AddPaymentViewModel.AmountPaid = "0";
                PaymentVisibility = "Collapsed";
                if (billing!=null)
                {
                    if(billing.Balance>0)
                    {
                        PaymentVisibility = "Visible";
                    }
                    else
                    {
                        PaymentVisibility = "Collapsed";
                    }
                    TotalPaid = Billing.AmountCharged - Billing.Balance;
                    AddPaymentViewModel.Billing = billing;
                    PaymentViewModel.Patient = billing.Appointment.Patient;
                    PaymentViewModel.BillingNo = billing.No;
                    PaymentViewModel.LoadPayments();
                }
                OnPropertyChanged();
                //ArchiveVisibility = "Collapsed";
                //UnarchiveVisibility = "Collapsed";
                //if (value != null)
                //{
                //    if (value.Status.Equals("Active"))
                //    {
                //        ArchiveVisibility = "Visible";
                //    }
                //    else
                //    {
                //        UnarchiveVisibility = "Visible";
                //    }
                //}
            }
        }
        public List<Billing> Billings { get => billings; set { billings = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public string PaymentVisibility { get => paymentVisibility; set { paymentVisibility = value; OnPropertyChanged(); } }

        public AddPaymentViewModel AddPaymentViewModel { get => addPaymentViewModel; set => addPaymentViewModel = value; }
        public DelegateCommand AddPaymentCommand { get => addPaymentCommand; set => addPaymentCommand = value; }
        public PaymentViewModel PaymentViewModel { get => paymentViewModel; set => paymentViewModel = value; }
        public double TotalPaid { get => totalPaid; set { totalPaid = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddBilling()
        {
            //MenuViewModel.GotoAddBillingView();
        }

        public void LoadBillings()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditBilling()
        {
            //MenuViewModel.GotoEditBillingView(Billing);
        }

        public void Archive()
        {
            startUpdateToDatabase(Billing, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Billing, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteBilling()
        {
            startDeleteFromDatabase(Billing, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
