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

namespace AllAboutTeethDCMS.Operations
{
    /// <summary>
    /// Interaction logic for OperationView.xaml
    /// </summary>
    public partial class OperationView : UserControl
    {
        public OperationView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((OperationViewModel)DataContext).loadOperations();
        }

        private void add_treatment_Click(object sender, RoutedEventArgs e)
        {
            //((TreatmentViewModel)DataContext).MenuViewModel.gotoAddTreatmentView(((TreatmentViewModel)DataContext).ActiveUser);
        }

        private void edit_treatment_Click(object sender, RoutedEventArgs e)
        {
            //((TreatmentViewModel)DataContext).MenuViewModel.gotoEditTreatmentView(((TreatmentViewModel)DataContext).ActiveUser, (Treatment)((TreatmentViewModel)DataContext).Treatment.Clone());
        }

        private void delete_treatment_Click(object sender, RoutedEventArgs e)
        {
            ((OperationViewModel)DataContext).deleteOperation();
        }
    }
}
