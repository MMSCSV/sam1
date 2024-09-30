using System;
using CoreDALModel = Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Data.Models
{
    public class PasswordCredentialResult : CoreDALModel.PasswordCredential
    {
        public DateTime StartUTCDateTime { get; set; }

        public DateTime StartLocalDateTime { get; set; }
    }
}
