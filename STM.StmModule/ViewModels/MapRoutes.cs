using Microsoft.Maps.MapControl.WPF;
using STM.Common;
using STM.StmModule.Simulator.Contract.Rtz11;
using STM.StmModule.Simulator.Contract.TextMessage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace STM.StmModule.Simulator.ViewModels
{
    public class MapRoutes
    {
        public static MapLayer Map { get; set; }

        public static void AddRoute(string routeXml, Color color)
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

                    //Add pushpin for first waypoint
                    var pos = rtz.waypoints.waypoint.FirstOrDefault().position;
                    Pushpin pin = new Pushpin();
                    pin.Location = new Location((double)pos.lat, (double)pos.lon);
                    pin.ToolTip = pos.lat + " " + pos.lon + " " + rtz.waypoints.waypoint.FirstOrDefault().name; 
                    Map.Children.Add(pin);

                    //Add pushpin for last waypoint
                    pos = rtz.waypoints.waypoint.LastOrDefault().position;
                    pin = new Pushpin();
                    pin.Location = new Location((double)pos.lat, (double)pos.lon);
                    pin.ToolTip = pos.lat + " " + pos.lon + " " + rtz.waypoints.waypoint.LastOrDefault().name;
                    Map.Children.Add(pin);

                }
            });
        }

        public static void AddTextMessage(string txtXml, Color color)
        {
            Map.Dispatcher.Invoke(() =>
            {
                textMessage txt = null;

                try
                {
                    txt = Serialization.Deserialize<textMessage>(txtXml);
                }
                catch (Exception)
                {
                }

                if (txt != null)
                {
                    if (txt.position != null)
                    {
                        Pushpin pin = new Pushpin();
                        pin.Location = new Location((double)txt.position.lat, (double)txt.position.lon);
                        pin.ToolTip = txt.subject;
                        Map.Children.Add(pin);
                    }

                    if (txt.area != null && txt.area.Polygon != null)
                    {
                        MapPolyline polyline = new MapPolyline();
                        polyline.Stroke = new SolidColorBrush(color);
                        polyline.StrokeThickness = 2;
                        polyline.Opacity = 0.7;

                        polyline.Locations = new LocationCollection();
                        var points = txt.area.Polygon.posList.Split(' ');
                        for (int i = 0; i < points.Count(); i = i + 2)
                        {
                            polyline.Locations.Add(new Location(double.Parse(points[i], CultureInfo.InvariantCulture), double.Parse(points[i + 1], CultureInfo.InvariantCulture)));
                        }

                        Map.Children.Add(polyline);
                    }

                    if (txt.area != null && txt.area.Circle != null)
                    {
                        var Circle = new MapPolyline();
                        Circle.StrokeThickness = 2;
                        Circle.Stroke = new SolidColorBrush(color);
                        Circle.Locations = CalculateCircle((double)txt.area.Circle.position.lat, (double)txt.area.Circle.position.lon, txt.area.Circle.radius);
                        Map.Children.Add(Circle);
                    }
                }
            });
        }

        public static void CliearRoutes()
        {
            Map.Children.Clear();
        }


        // Constants and helper functions:

        const double earthRadius = 6371000D;
        const double Circumference = 2D * Math.PI * earthRadius;

        private static LocationCollection CalculateCircle(double lat, double lon, double Radius)
        {
            var GeoPositions = new LocationCollection();

            for (int i = (int)0; i <= 360; i++)
            {
                double Bearing = ToRad(i);
                double CircumferenceLatitudeCorrected = 2D * Math.PI * Math.Cos(ToRad(lat)) * earthRadius;
                double lat1 = Circumference / 360D * lat;
                double lon1 = CircumferenceLatitudeCorrected / 360D * lon;
                double lat2 = lat1 + Math.Sin(Bearing) * Radius;
                double lon2 = lon1 + Math.Cos(Bearing) * Radius;
                var location = new Location();
                location.Latitude = lat2 / (Circumference / 360D);
                location.Longitude = lon2 / (CircumferenceLatitudeCorrected / 360D);
                GeoPositions.Add(location);
            }
            return GeoPositions;
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180D);
        }
    }
}
