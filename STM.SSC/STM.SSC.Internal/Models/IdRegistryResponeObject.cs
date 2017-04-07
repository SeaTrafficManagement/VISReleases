using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.SSC.Internal.Models
{
    class IdRegistryResponeObject
    {
        public List<Organization> content { get; set; }
        public bool last { get; set; }
        public int totalPages { get; set; }
        public int totalElements { get; set; }
        public int numberOfElements { get; set; }
        public bool first { get; set; }
        public object sort { get; set; }
        public int size { get; set; }
        public int number { get; set; }
    }
}
