using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace CareFusion.Dispensing.Data.Entities
{
    public class TemporaryTableEntity
    {
        private Guid _key;

        [Column(Name = "TableKey", Storage = "_key", DbType = "UniqueIdentifier NOT NULL")]
        public Guid Key
        {
            get { return _key; }
            set
            {
                if ((_key != value))
                {
                    _key = value;
                }
            }
        }
    }

    internal class BaseDataContext : DataContext
    {
        public BaseDataContext(IDbConnection connection)
            : base(connection)
        {
            
        }

        public BaseDataContext(IDbConnection connection, MappingSource mappingSource)
            : base(connection, mappingSource)
        {
                
        }

        [Function(Name = "Core.fn_GuidUniqueSplit", IsComposable = true)]
        public IQueryable<TemporaryTableEntity> CreateTemporaryTable([Parameter(Name = "KeyList", DbType = "NVarChar(MAX)")] string keyList, [Parameter(DbType = "Char(1)")] char? delimiter)
        {
            return CreateMethodCallQuery<TemporaryTableEntity>(this, ((MethodInfo)(MethodBase.GetCurrentMethod())), keyList, delimiter);
        }

        [Function(Name = "Item.fn_SimpleDisplayName", IsComposable = true)]
        [return: Parameter(DbType = "NVarChar(1000)")]
        public string ItemDisplayName([Parameter(Name = "ItemDisplayName", DbType = "NVarChar(500)")] string itemDisplayName, [Parameter(Name = "GenericName", DbType = "NVarChar(500)")] string genericName, [Parameter(Name = "ItemName", DbType = "NVarChar(100)")] string itemName,
            [Parameter(Name = "ItemTypeInternalCode", DbType = "VarChar(10)")] string itemTypeInternalCode, [Parameter(Name = "ItemSubTypeInternalCode", DbType = "VarChar(10)")] string itemSubTypeInternalCode, [Parameter(Name = "StrengthAmount", DbType = "Decimal(14,4)")] decimal? strengthAmount,
            [Parameter(Name = "StrengthDisplayCode", DbType = "NVarChar(50)")] string strengthDisplayCode, [Parameter(Name = "ConcentrationVolumeAmount", DbType = "Decimal(14,4)")] decimal? concentrationVolumeAmount,
            [Parameter(Name = "ConcentrationVolumeDisplayCode", DbType = "NVarChar(50)")] string concentrationVolumeDisplayCode, [Parameter(Name = "TotalVolumeAmount", DbType = "Decimal(14,4)")] decimal? totalVolumeAmount,
            [Parameter(Name = "TotalVolumeDisplayCode", DbType = "NVarChar(50)")] string totalVolumeDisplayCode, [Parameter(Name = "DosageFormCode", DbType = "NVarChar(20)")] string dosageFormCode)
        {
            return ((string)(this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                itemDisplayName,
                genericName,
                itemName,
                itemTypeInternalCode,
                itemSubTypeInternalCode,
                strengthAmount,
                strengthDisplayCode,
                concentrationVolumeAmount,
                concentrationVolumeDisplayCode,
                totalVolumeAmount,
                totalVolumeDisplayCode,
                dosageFormCode).ReturnValue));
        }
    }
}
