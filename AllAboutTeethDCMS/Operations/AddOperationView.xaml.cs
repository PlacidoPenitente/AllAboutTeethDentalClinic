﻿using System;
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
using AllAboutTeethDCMS.TreatmentRecords;

namespace AllAboutTeethDCMS.Operations
{
    /// <summary>
    /// Interaction logic for AddOperationView.xaml
    /// </summary>
    public partial class AddOperationView : UserControl
    {
        public AddOperationView()
        {
            InitializeComponent();
            var context = (AddOperationViewModel)DataContext;
            context.DentalChartViewModel.TreatmentRecordViewModel = (TreatmentRecordViewModel)records.DataContext;
        }
        
 
  
        private void addTreatment_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).updateList();
            //((AddOperationViewModel)DataContext).saveOperation();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).finishTreatment();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).MenuViewModel.gotoAppointments();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).updateList();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((AddOperationViewModel)DataContext).AmountCharge = "0";
        }
    }
}
