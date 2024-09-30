using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class LastServerCommunicationTimeResult
    {
        public DateTime? LastServerCommunicationUTCDateTime { get; set; }
        public DateTime? LastServerCommunicationLocalDateTime { get; set; }
    }
}
