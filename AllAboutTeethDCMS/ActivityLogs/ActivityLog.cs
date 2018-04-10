using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.ActivityLogs
{
    public class ActivityLog : ViewModelBase
    {
        private int no = -1;
        private User addedBy;
        private string activity = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;

        public int No { get => no; set => no = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
        public string Activity { get => activity; set => activity = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
    }
}
