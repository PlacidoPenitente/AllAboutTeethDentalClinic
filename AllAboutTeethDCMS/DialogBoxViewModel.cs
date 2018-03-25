using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS
{
    public class DialogBoxViewModel : ViewModelBase
    {
        private string title = "";
        private string message = "";
        private string answer = "";
        private string yesVisibility = "Collpased";
        private string noVisibility = "Collpased";
        private string okVisibility = "Collpased";
        private string mode = "Information";

        public string Title { get => title; set { title = value; OnPropertyChanged(); } }
        public string Message { get => message; set { message = value; OnPropertyChanged(); } }
        public string Answer { get => answer; set { answer = value; OnPropertyChanged(); } }

        public string YesVisibility { get => yesVisibility; set { yesVisibility = value; OnPropertyChanged(); } }
        public string NoVisibility { get => noVisibility; set { noVisibility = value; OnPropertyChanged(); } }
        public string OkVisibility { get => okVisibility; set { okVisibility = value; OnPropertyChanged(); } }

        public string Mode { get => mode; set { mode = value; OnPropertyChanged();
                if (value.Equals("Information"))
                {
                    OkVisibility = "Visible";
                    YesVisibility = "Collapsed";
                    NoVisibility = "Collapsed";
                }
                else
                {
                    OkVisibility = "Collapsed";
                    YesVisibility = "Visible";
                    NoVisibility = "Visible";
                }
            } }
    }
}
