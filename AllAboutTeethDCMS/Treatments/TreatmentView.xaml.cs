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

namespace AllAboutTeethDCMS.Treatments
{
    /// <summary>
    /// Interaction logic for TreatmentView.xaml
    /// </summary>
    public partial class TreatmentView : UserControl
    {
        public TreatmentView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).loadTreatments();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).MenuViewModel.gotoAddTreatmentView();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).MenuViewModel.gotoEditTreatmentView((Treatment)((TreatmentViewModel)DataContext).Treatment.Clone());
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).deleteTreatment();
        }

        private void unarchive_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).unarchive();
        }

        private void archive_Click(object sender, RoutedEventArgs e)
        {
            ((TreatmentViewModel)DataContext).archive();
        }
    }
}
