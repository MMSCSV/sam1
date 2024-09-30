using Pyxis.Core.Data.Schema.Strg.Models;

namespace CareFusion.Dispensing.Data.Models
{
    public class StorageSpaceStateResult : StorageSpaceState
    {
        public string DescriptionText { get; set; }

        public int SortValue { get; set; }
    }
}
