using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.ActivityLogs
{
    public class ActivityLogViewModel : CRUDPage<ActivityLog>
    {
        private ActivityLog activityLog;
        private List<ActivityLog> activityLogs;

        public ActivityLog ActivityLog { get => activityLog; set { activityLog = value; OnPropertyChanged(); } }
        public List<ActivityLog> ActivityLogs { get => activityLogs; set { activityLogs = value; OnPropertyChanged(); } }

        public DelegateCommand DeleteCommand { get => deleteCommand; set => deleteCommand = value; }
        public DelegateCommand LoadCommand { get => loadCommand; set => loadCommand = value; }

        private DelegateCommand loadCommand;
        private DelegateCommand deleteCommand;

        public ActivityLogViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(loadLogs));
            DeleteCommand = new DelegateCommand(new Action(DeleteActivity));
        }

        protected override bool beforeDelete()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Delete Activity Log";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this activity Log?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting activity log. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                loadLogs();
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

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<ActivityLog> list)
        {
            ActivityLogs = list;
            FilterResult = "";
            if (list.Count > 1)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "select * from allaboutteeth_activitylogs, allaboutteeth_users where activitylog_addedby=user_no AND (" +
                "concat(user_firstname, ' ', user_middlename, ' ', user_lastname) like @filter OR " +
                "concat(user_lastname, ' ', user_firstname, ' ', user_middlename) like @filter OR " +
                "user_username like @filter)";
            command.Parameters.AddWithValue("@filter", "%" + Filter.Replace(" ", "%") + "%");
            ActivityLogs = null;
            GC.Collect();
        }
        public void loadLogs()
        {
            startLoadFromDatabase("allaboutteeth_activitylogs",Filter);
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        public void DeleteActivity()
        {
            startDeleteFromDatabase(ActivityLog, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
    }
}
