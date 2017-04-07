using Microsoft.Maps.MapControl.WPF;
using STM.Common;
using STM.StmModule.Simulator.Contract.Rtz11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace STM.StmModule.Simulator.ViewModels
{
    public class MapRoutes
    {
        public static MapLayer Map { get; set; }

        public static void AddRoute(string routeXml, System.Windows.Media.Color color)
        {
            Map.Dispatcher.Invoke(() =>
            {
                Route rtz = null;

                try
                {
                    rtz = Serialization.Deserialize<Route>(routeXml);
                }
                catch (Exception)
                {
                }

                if (rtz != null)
                {
                    MapPolyline polyline = new MapPolyline();
                    polyline.Stroke = new SolidColorBrush(color);
                    polyline.StrokeThickness = 2;
                    polyline.Opacity = 0.7;

                    polyline.Locations = new LocationCollection();
                    foreach (var waypoint in rtz.waypoints.waypoint)
                    {
                        polyline.Locations.Add(new Location((double)waypoint.position.lat, (double)waypoint.position.lon));
                    }

                    Map.Children.Add(polyline);

                    var lable = new System.Windows.Controls.TextBlock();
                    lable.Text = rtz.routeInfo.routeName;
                    lable.Foreground = new SolidColorBrush(color);
                    lable.FontSize = 12;
                    lable.FontWeight = System.Windows.FontWeights.Bold;
                    Map.AddChild(lable, polyline.Locations[0]);
                }
            });
        }

        public static void CliearRoutes()
        {
            Map.Children.Clear();
        }
    }
}
