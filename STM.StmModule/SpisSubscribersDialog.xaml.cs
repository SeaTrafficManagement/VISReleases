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
    /// Interaction logic for NewVoyageplanDialog.xaml
    /// </summary>
    public partial class SpisSubscribersDialog : Window
    {
        public SpisSubscribersDialog()
        {
            InitializeComponent();
            DataContext = new SpisSubscriptionViewModel();
        }

        public SpisSubscriptionViewModel ViewModel
        {
            get
            {
                return DataContext as SpisSubscriptionViewModel;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
