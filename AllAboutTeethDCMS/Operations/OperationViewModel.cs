using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Operations;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Operations
{
    public class OperationViewModel : CRUDPage<Operation>
    {
        #region Fields
        private Operation medicine;
        private List<Operation> medicines;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public OperationViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadOperations));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteOperation));
            AddCommand = new DelegateCommand(new Action(GotoAddOperation));
            EditCommand = new DelegateCommand(new Action(GotoEditOperation));
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            //DialogBoxViewModel.Answer = "None";
            //DialogBoxViewModel.Mode = "Question";
            //if (Operation.Status.Equals("Active"))
            //{
            //    DialogBoxViewModel.Title = "Archive Item";
            //    DialogBoxViewModel.Message = "Are you sure you want to archive this item?";
            //}
            //else
            //{
            //    DialogBoxViewModel.Title = "Unarchive Item";
            //    DialogBoxViewModel.Message = "Are you sure you want to activate this item?";
            //}
            //while (DialogBoxViewModel.Answer.Equals("None"))
            //{
            //    Thread.Sleep(100);
            //}
            //if (DialogBoxViewModel.Answer.Equals("Yes"))
            //{
            //    if (Operation.Status.Equals("Active"))
            //    {
            //        Operation.Status = "Archived";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Archiving item. Please wait.";
            //        DialogBoxViewModel.Answer = "None";
            //    }
            //    else
            //    {
            //        Operation.Status = "Active";
            //        DialogBoxViewModel.Mode = "Progress";
            //        DialogBoxViewModel.Message = "Activating item. Please wait.";
            //        DialogBoxViewModel.Answer = "None";
            //    }
            //    return true;
            //}
            return false;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                LoadOperations();
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
            DialogBoxViewModel.Title = "Delete Item";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this item?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting item. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadOperations();
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

        protected override void afterLoad(List<Operation> list)
        {
            Operations = list;
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

        public Operation Operation
        {
            get => medicine;
            set
            {
                medicine = value;
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
        public List<Operation> Operations { get => medicines; set { medicines = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddOperation()
        {
            //MenuViewModel.gotoAddOperationView();
        }

        public void LoadOperations()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditOperation()
        {
            //MenuViewModel.GotoEditOperationView(Operation);
        }

        public void Archive()
        {
            startUpdateToDatabase(Operation, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Operation, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteOperation()
        {
            startDeleteFromDatabase(Operation, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
