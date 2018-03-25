using AllAboutTeethDCMS.Menu;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AllAboutTeethDCMS
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AllAboutTeeth allAboutTeeth;
        private LoginViewModel loginViewModel;
        private MenuViewModel menuViewModel;
        private User activeUser;
        private DateTime dateTime;
        private Thread timeThread;

        public MainWindowViewModel()
        {
            AllAboutTeeth = new AllAboutTeeth();
            LoginViewModel = new LoginViewModel();
            MenuViewModel = new MenuViewModel();

            LoginViewModel.MainWindowViewModel = this;
            MenuViewModel.MainWindowViewModel = this;

            timeThread = new Thread(setTime);
            timeThread.IsBackground = true;
            timeThread.Start();
        }

        public void setTime()
        {
            while(true)
            {
                DateTime = DateTime.Now;
                OnPropertyChanged("Time");
                Thread.Sleep(1000);
            }
        }

        public AllAboutTeeth AllAboutTeeth { get => allAboutTeeth; set => allAboutTeeth = value; }
        public string Title { get => AllAboutTeeth.Title; set { AllAboutTeeth.Title = value; OnPropertyChanged(); } }
        public UserControl ActivePage { get => AllAboutTeeth.ActivePage; set { AllAboutTeeth.ActivePage = value; OnPropertyChanged(); } }
        public LoginViewModel LoginViewModel { get => loginViewModel; set { loginViewModel = value; OnPropertyChanged(); } }
        public MenuViewModel MenuViewModel { get => menuViewModel; set { menuViewModel = value; OnPropertyChanged(); } }
        public User ActiveUser { get => activeUser; set { activeUser = value; OnPropertyChanged(); MenuViewModel.ActiveUser = value; } }
        public DateTime DateTime { get => dateTime; set { dateTime = value; OnPropertyChanged(); } }
        public string Time { get => DateTime.ToLongTimeString(); set {  } }
    }
}
