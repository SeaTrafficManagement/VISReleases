using Microsoft.Maps.MapControl.WPF;
using STM.StmModule.Simulator.MapLayers;
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
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
            var vm = new MapViewModel();
            DataContext = vm;

            MapRoutes.Map = RoutesLayer;
        }

        public MapView(MapViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void RemoveMapLayers()
        {
            var layersToRemove = new List<UIElement>();
            foreach (UIElement element in map.Children)
            {
                if (element.GetType() == typeof(EniroLayer)
                    || element.GetType() == typeof(OpenStreetmapLayer)
                    || element.GetType() == typeof(WmsLayer))
                {
                    layersToRemove.Add(element);
                }
            }

            foreach (var l in layersToRemove)
                map.Children.Remove(l);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RemoveMapLayers();
            switch (((MapViewModel)DataContext).BackgroundMap)
            {
                case BackgroundMapEnum.Eniro:
                    {
                        map.Mode = new MercatorMode();
                        map.Children.Insert(0, new EniroLayer
                        {
                            UriFormat = "http://map.eniro.com/geowebcache/service/tms1.0.0/nautical/{z}/{x}/{y}.png"
                        });
                        break;
                    }
                case BackgroundMapEnum.OpenStreetmap:
                    {
                        map.Mode = new MercatorMode();
                        map.Children.Insert(0, new OpenStreetmapLayer
                        {
                            UriFormat = "http://tile.openstreetmap.org/{z}/{x}/{y}.png"
                        });
                        break;
                    }
                case BackgroundMapEnum.Sfv:
                    {
                        map.Mode = new MercatorMode();
                        map.Children.Insert(0, new WmsLayer
                        {
                            UriFormat = "http://geodataint.sjofartsverket.se/MapService/wms.axd/SjkbasFastRasterViewKlippt?VERSION=1.1.1&REQUEST=GetMap&SERVICE=WMS&format=image/gif&Layers=SjkbasFastKlippt&CRS=EPSG:3006&HEIGHT=256&WIDTH=256&STYLES=&BBOX={0}",
                            UseSweref99 = true
                        });
                        break;
                    }
                case BackgroundMapEnum.BingAreal:
                    {
                        map.Mode = new RoadMode();
                        break;
                    }
                case BackgroundMapEnum.BingSatelite:
                    {
                        map.Mode = new AerialMode(true);
                        break;
                    }
            }
        }
    }
}
