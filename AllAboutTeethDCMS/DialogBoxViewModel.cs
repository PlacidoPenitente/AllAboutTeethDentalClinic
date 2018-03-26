﻿using System;
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
        private string icon = "";

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
                    Icon = "/AllAboutTeethDCMS;component/Resources/icons8_Info_48px.png";
                }
                else if (value.Equals("Progress"))
                {
                    OkVisibility = "Collapsed";
                    YesVisibility = "Collapsed";
                    NoVisibility = "Collapsed";
                    Icon = "/AllAboutTeethDCMS;component/Resources/icons8_Spinner_Frame_8_48px.png";
                }
                else if (value.Equals("Error"))
                {
                    OkVisibility = "Visible";
                    YesVisibility = "Collapsed";
                    NoVisibility = "Collapsed";
                    Icon = "/AllAboutTeethDCMS;component/Resources/icons8_Error_48px.png";
                }
                else if (value.Equals("Success"))
                {
                    OkVisibility = "Visible";
                    YesVisibility = "Collapsed";
                    NoVisibility = "Collapsed";
                    Icon = "/AllAboutTeethDCMS;component/Resources/icons8_Ok_48px.png";
                }
                else
                {
                    OkVisibility = "Collapsed";
                    YesVisibility = "Visible";
                    NoVisibility = "Visible";
                    Icon = "/AllAboutTeethDCMS;component/Resources/icons8_Help_48px.png";
                }
            } }

        public string Icon { get => icon; set { icon = value; OnPropertyChanged(); } }
    }
}
