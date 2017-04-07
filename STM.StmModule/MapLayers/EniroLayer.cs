using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.MapLayers
{
    public class EniroLayer : Microsoft.Maps.MapControl.WPF.MapTileLayer
    {
        public EniroLayer()
        {
            TileSource = new EniroTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
    }
}
