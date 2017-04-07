using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.MapLayers
{
    public class OpenStreetmapTileSource : Microsoft.Maps.MapControl.WPF.TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri(UriFormat.
                           Replace("{x}", x.ToString()).
                           Replace("{y}", y.ToString()).
                           Replace("{z}", zoomLevel.ToString()));
        }
    }
}
