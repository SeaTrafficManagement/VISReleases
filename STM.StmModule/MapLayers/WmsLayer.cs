using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.MapLayers
{
    public class WmsLayer : Microsoft.Maps.MapControl.WPF.MapTileLayer
    {
        public WmsLayer()
        {
            TileSource = new WmsTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }

        public bool UseSweref99
        {
            get { return ((WmsTileSource)TileSource).UseSweref99; }
            set { ((WmsTileSource)TileSource).UseSweref99 = value; }
        }
    }
}
