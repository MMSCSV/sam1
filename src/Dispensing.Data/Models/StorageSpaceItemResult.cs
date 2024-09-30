using Pyxis.Core.Data.Schema.Strg.Models;
using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class StorageSpaceItemResult : StorageSpaceItem
    {                
        public DateTime? EarliestNextExpirationDate { get; set; }

        public decimal? InventoryQuantity { get; set; }
    }
}
