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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STM.StmModule.Simulator.Views
{
    /// <summary>
    /// Interaction logic for FindIdentitiesView.xaml
    /// </summary>
    public partial class CallServiceView : UserControl
    {
        public CallServiceView()
        {
            InitializeComponent();
            DataContext = new CallServiceViewModel();
        }

        public CallServiceViewModel ViewModel
        {
            get
            {
                return DataContext as CallServiceViewModel;
            }
        }
    }
}