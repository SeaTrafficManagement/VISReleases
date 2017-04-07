using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.MapLayers
{
    public class EniroTileSource : Microsoft.Maps.MapControl.WPF.TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            var tileY = Math.Pow(2, zoomLevel) - 1 - y;

            return new Uri(UriFormat.
                           Replace("{x}", x.ToString()).
                           Replace("{y}", tileY.ToString()).
                           Replace("{z}", zoomLevel.ToString()));
        }
    }
}
