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

namespace AllAboutTeethDCMS.Providers
{
    /// <summary>
    /// Interaction logic for ProviderView.xaml
    /// </summary>
    public partial class ProviderView : UserControl
    {
        public ProviderView()
        {
            InitializeComponent();
        }

        private void search_account_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).loadProviders();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).MenuViewModel.gotoAddProviderView();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).MenuViewModel.gotoEditProviderView((Provider)((ProviderViewModel)DataContext).Provider.Clone());
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).deleteProvider();
        }

        private void unarchive_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).unarchive();
        }

        private void archive_Click(object sender, RoutedEventArgs e)
        {
            ((ProviderViewModel)DataContext).archive();
        }
    }
}
