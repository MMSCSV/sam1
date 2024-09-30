using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ComboComponentAdminRouteResult
    {
        public Guid ComboComponentAdminRouteKey {get;set;}
        public Guid ComboComponentKey {get;set;}
        public Guid AdminRouteKey { get; set; }
        public string AdminRouteDisplayCode { get; set; }
        public string AdminRouteDescription { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
