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

namespace STM.StmModule.Simulator
{
    /// <summary>
    /// Interaction logic for GetVPDialog.xaml
    /// </summary>
    public partial class GetVPDialog : Window
    {
        public GetVPDialog()
        {
            InitializeComponent();
            DataContext = new GetVPViewModel();
        }

        public GetVPViewModel ViewModel
        {
            get
            {
                return DataContext as GetVPViewModel;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
