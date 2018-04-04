using AllAboutTeethDCMS.Patients;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Payments
{
    public class PaymentViewModel : CRUDPage<Payment>
    {
        #region Fields
        private Payment payment;
        private List<Payment> payments;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public PaymentViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadPayments));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeletePayment));
            AddCommand = new DelegateCommand(new Action(GotoAddPayment));
            EditCommand = new DelegateCommand(new Action(GotoEditPayment));
            Payment = new Payment();
            Patient = new Patient();
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            //DialogBoxViewModel.Answer = "None";
            //DialogBoxViewModel.Mode = "Question";
            //if (Payment.Status.Equals("Active"))
            //{
            //    DialogBoxViewModel.Title = "Archive Payment";
            //    DialogBoxViewModel.Message = "Are you sure you want to archive this payment?";
            //}
            //else
            //{
            //    DialogBoxViewModel.Title = "Unarchive Payment";
            //    DialogBoxViewModel.Message = "Are you sure you want to activate this payment?";
            //}
            //while (DialogBoxViewModel.Answer.Equals("None"))
            //{
            //    Thread.Sleep(100);
            //}
            //if (DialogBoxViewModel.Answer.Equals("Yes"))
            //{
            //    if (Payment.Status.Equals("Active"))
            //    {
            //        Payment.Status = "Archived";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Archiving payment. Please wait.";
            //        DialogBoxViewModel.Answer = "None";
            //    }
            //    else
            //    {
            //        Payment.Status = "Active";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Activating payment. Please wait.";
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
                LoadPayments();
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
            DialogBoxViewModel.Title = "Delete Payment";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this payment?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting payment. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadPayments();
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

        private int billingNo = 0;
        private Patient patient;

        protected override void beforeLoad(MySqlCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "select * from allaboutteeth_payments where payment_billing='"+BillingNo+"'";
        }

        protected override void afterLoad(List<Payment> list)
        {
            Payments = list;
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

        public Payment Payment
        {
            get => payment;
            set
            {
                payment = value;
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
        public List<Payment> Payments { get => payments; set { payments = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public int BillingNo { get => billingNo; set => billingNo = value; }
        public Patient Patient { get => patient; set { patient = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddPayment()
        {
            //MenuViewModel.GotoAddPaymentView();
        }

        public void LoadPayments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditPayment()
        {
            //MenuViewModel.GotoEditPaymentView(Payment);
        }

        public void Archive()
        {
            startUpdateToDatabase(Payment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Payment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeletePayment()
        {
            startDeleteFromDatabase(Payment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
