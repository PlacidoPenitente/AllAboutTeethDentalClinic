using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllAboutTeethDCMS.DentalCharts
{
    /// <summary>
    /// Interaction logic for ToothViewRotated.xaml
    /// </summary>
    public partial class ToothViewRotated : UserControl
    {
        public ToothViewRotated()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!((ToothViewModel)DataContext).Teeth.Contains(((ToothViewModel)DataContext)))
            {
                ((ToothViewModel)DataContext).Teeth.Add((ToothViewModel)DataContext);
                ((ToothViewModel)DataContext).Load();
            }
            else
            {
                ((ToothViewModel)DataContext).Teeth.Remove((ToothViewModel)DataContext);
            }
        }
    }
}
