using STM.StmModule.Simulator.ViewModels;
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

namespace STM.StmModule.Simulator.Views
{
    /// <summary>
    /// Interaction logic for PCMMessagesView.xaml
    /// </summary>
    public partial class PCMMessagesView : UserControl
    {
        public PCMMessagesView()
        {
            InitializeComponent();
            DataContext = new PCMMessagesViewModel();
        }

        public PCMMessagesViewModel ViewModel
        {
            get
            {
                return DataContext as PCMMessagesViewModel;
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel.SelectedMessage == null)
                return;
        }
    }
}
