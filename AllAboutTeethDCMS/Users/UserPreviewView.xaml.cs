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

namespace AllAboutTeethDCMS.Users
{
    /// <summary>
    /// Interaction logic for UserPreviewView.xaml
    /// </summary>
    public partial class UserPreviewView : UserControl
    {
        public UserPreviewView()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            ((UserPreviewViewModel)DataContext).Visibility = "Collapsed";
        }
    }
}
