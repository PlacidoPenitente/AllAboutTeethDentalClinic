using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AllAboutTeethDCMS
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AllAboutTeeth allAboutTeeth;

        public MainWindowViewModel()
        {
            AllAboutTeeth = new AllAboutTeeth();

            for(int i=21; i<29; i++)
            {
                Console.WriteLine("<CheckBox Style=\"{ StaticResource toothbox}\" root:AttachedProperties.ToothNo=\"{ Binding Tooth" + i + ".ToothNo}\" root:AttachedProperties.Icon=\"{ Binding Tooth" + i + ".ToothNo,Converter ={ StaticResource ToothNoToImageConverter}}\" root:AttachedProperties.Condition=\"{ Binding Tooth" + i+".Condition}\"/>");
            }
        }

        public AllAboutTeeth AllAboutTeeth { get => allAboutTeeth; set => allAboutTeeth = value; }
        public string Title { get => AllAboutTeeth.Title; set { if (AllAboutTeeth.Title != value) { AllAboutTeeth.Title = value; OnPropertyChanged("Title"); } } }
        public UserControl ActivePage { get => AllAboutTeeth.ActivePage; set { if (AllAboutTeeth.ActivePage != value) { AllAboutTeeth.ActivePage = value; OnPropertyChanged("ActivePage"); } } }
    }
}
