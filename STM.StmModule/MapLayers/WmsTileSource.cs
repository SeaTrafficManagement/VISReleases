using MightyLittleGeodesy.Positions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.MapLayers
{
    public class WmsTileSource : Microsoft.Maps.MapControl.WPF.TileSource
    {
        public WmsTileSource()
        {
            UseSweref99 = false;
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            var result = new Uri(String.Format(this.UriFormat, XYZoomToBBox(x, y, zoomLevel)));
            return result;
        }

        public bool UseSweref99 { get; set; }

        public string XYZoomToBBox(int x, int y, int zoom)
        {
            var tileSize = 256;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
           
            // From the grid position and zoom, work out the min and max Latitude / Longitude values of this tile
            double w = ((float)(x * tileSize) * 360 / (float)(tileSize * Math.Pow(2, zoom)) - 180);
            double n = ((float)Math.Asin((Math.Exp((0.5 - (y * tileSize) / (tileSize) / Math.Pow(2, zoom)) * 4 * Math.PI) - 1) / (Math.Exp((0.5 - (y * tileSize) / 256 / Math.Pow(2, zoom)) * 4 * Math.PI) + 1)) * 180 / (float)Math.PI);
            double e = ((float)((x + 1) * tileSize) * 360 / (float)(tileSize * Math.Pow(2, zoom)) - 180);
            double s = ((float)Math.Asin((Math.Exp((0.5 - ((y + 1) * tileSize) / (tileSize) / Math.Pow(2, zoom)) * 4 * Math.PI) - 1) / (Math.Exp((0.5 - ((y + 1) * tileSize) / 256 / Math.Pow(2, zoom)) * 4 * Math.PI) + 1)) * 180 / (float)Math.PI);

            if (UseSweref99)
            {
                WGS84Position wgstopLeft = new WGS84Position(n, w);
                WGS84Position wgsBottomRight = new WGS84Position(s, e);

                SWEREF99Position swerefTopLeft = new SWEREF99Position(wgstopLeft, SWEREF99Position.SWEREFProjection.sweref_99_tm);
                SWEREF99Position swerefBottomRight = new SWEREF99Position(wgsBottomRight, SWEREF99Position.SWEREFProjection.sweref_99_tm);

                string[] bounds = new string[] { swerefTopLeft.Longitude.ToString(nfi), swerefBottomRight.Latitude.ToString(nfi), swerefBottomRight.Longitude.ToString(nfi), swerefTopLeft.Latitude.ToString(nfi) };

                return string.Join(",", bounds);
            }
            else
            {
                string[] bounds = new string[] { w.ToString(nfi), s.ToString(nfi), e.ToString(nfi), n.ToString(nfi) };

                return string.Join(",", bounds);
            }
        }
    }
}