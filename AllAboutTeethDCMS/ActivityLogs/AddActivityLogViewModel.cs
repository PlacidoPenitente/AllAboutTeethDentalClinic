using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.ActivityLogs
{
    public class AddActivityLogViewModel : CRUDPage<ActivityLog>
    {
        private ActivityLog activityLog;

        public ActivityLog ActivityLog { get => activityLog; set { activityLog = value; OnPropertyChanged(); } }

        protected override void afterCreate(bool isSuccessful)
        {
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<ActivityLog> list)
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            return true;
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        public void saveActivityLog()
        {
            startSaveToDatabase(ActivityLog, "allaboutteeth_activitylogs");
        }
    }
}
