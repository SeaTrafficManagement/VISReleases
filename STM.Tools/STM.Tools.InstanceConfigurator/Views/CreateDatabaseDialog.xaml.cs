﻿using STM.Tools.InstanceConfigurator.ViewModels;
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
using System.Windows.Shapes;

namespace STM.Tools.InstanceConfigurator.Views
{
    /// <summary>
    /// Interaction logic for DeleteSubscriptionDialog.xaml
    /// </summary>
    public partial class CreateDatabaseDialog : Window
    {
        public CreateDatabaseDialog()
        {
            InitializeComponent();
            DataContext = new CreateDatabaseDialogViewModel();
        }

        public CreateDatabaseDialogViewModel ViewModel
        {
            get
            {
                return DataContext as CreateDatabaseDialogViewModel;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
