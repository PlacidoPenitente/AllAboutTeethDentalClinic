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

namespace AllAboutTeethDCMS.Reports
{
    /// <summary>
    /// Interaction logic for TransactionReportView.xaml
    /// </summary>
    public partial class TransactionReportView : UserControl
    {
        public TransactionReportView()
        {
            InitializeComponent();
            from.SelectedDate = DateTime.Now;
            to.SelectedDate = DateTime.Now;
        }

        private TransactionReport transaction;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(transaction==null)
            {
                transaction = new TransactionReport();
            }
            viewer.ViewerCore.ReportSource = transaction;
            viewer.ViewerCore.SelectionFormula = "{allaboutteeth_billings1.billing_dateadded} in Date("+((DateTime)from.SelectedDate).Year +","+ ((DateTime)from.SelectedDate).Month + ","+ ((DateTime)from.SelectedDate).Day + ") TO Date("+ ((DateTime)to.SelectedDate).Year + ","+ ((DateTime)to.SelectedDate).Month + ","+ ((DateTime)to.SelectedDate).Day + ") and {allaboutteeth_users1.user_username} = \"" + dentist.Text + "\"";
        }
    }
}
