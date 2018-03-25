using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class UserPreviewViewModel : ViewModelBase
    {
        private User user;

        public User User { get => user; set { user = value; OnPropertyChanged(); } }
    }
}
