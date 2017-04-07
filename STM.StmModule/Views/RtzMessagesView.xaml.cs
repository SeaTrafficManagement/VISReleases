using STM.StmModule.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace STM.StmModule.Simulator.Views
{
    /// <summary>
    /// Interaction logic for FindIdentitiesView.xaml
    /// </summary>
    public partial class RtzMessagesView : UserControl
    {
        public RtzMessagesView()
        {
            InitializeComponent();
            DataContext = new RtzMessagesViewModel();
        }

        public RtzMessagesViewModel ViewModel
        {
            get
            {
                return DataContext as RtzMessagesViewModel;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel.SelectedMessage == null)
                return;
        }
    }
}
