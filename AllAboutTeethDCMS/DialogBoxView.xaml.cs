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

namespace AllAboutTeethDCMS
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class DialogBoxView : UserControl
    {
        public DialogBoxView()
        {
            InitializeComponent();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            ((DialogBoxViewModel)DataContext).Answer = "Yes";
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            ((DialogBoxViewModel)DataContext).Answer = "No";
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            ((DialogBoxViewModel)DataContext).Answer = "OK";
        }
    }
}