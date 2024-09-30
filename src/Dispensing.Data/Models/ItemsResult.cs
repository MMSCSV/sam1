using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemsResult
    {
        public Guid ItemKey { get; set; }

        public Guid ItemSnapshotKey { get; set; }

        public string BusinessDomainInternalCode { get; set; }

        public Guid? ExternalSystemKey { get; set; }

        public string ExternalSystemName { get; set; }

        public bool? PISProvidesMedClassFlag { get; set; }

        public bool? PISProvidesPureGenericNameFlag { get; set; }

        public bool? PISProvidesTherapeuticClassFlag { get; set; }

        public Guid? FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string ItemID { get; set; }

        public string AlternateItemID { get; set; }

        public string ItemName { get; set; }

        public string DescriptionText { get; set; }

        public string ItemTypeInternalCode { get; set; }

        public string ItemSubTypeInternalCode { get; set; }

        public bool MedItemFlag { get; set; }

        public DateTime? ExternalSystemDeleteUTCDateTime { get; set; }

        public DateTime? ExternalSystemDeleteLocalDateTime { get; set; }

        public string CustomField1Text { get; set; }

        public string CustomField2Text { get; set; }

        public string CustomField3Text { get; set; }

        public string EnterpriseItemID { get; set; }

        public bool DeleteFlag { get; set; }

        public Binary LastModifiedBinaryValue { get; set; }

        public Guid MedItemKey { get; set; }

        public Guid MedItemSnapshotKey { get; set; }

        public string MedDisplayName { get; set; }

        public string GenericName { get; set; }

        public string SearchGenericName { get; set; }

        public string PureGenericName { get; set; }

        public string BrandName { get; set; }

        public string SearchBrandName { get; set; }

        public string StrengthText { get; set; }

        public decimal? StrengthAmount { get; set; }

        public Guid? StrengthUOMKey { get; set; }

        public Guid? StrengthBaseUOMKey { get; set; }

        public string StrengthUOMDisplayCode { get; set; }

        public string StrengthUOMDescriptionText { get; set; }

        public bool? StrengthUOMUseDosageForm { get; set; }

        public decimal? StrengthUOMConversionAmount { get; set; }

        public Guid? StrengthExternalUOMKey { get; set; }

        public decimal? ConcentrationVolumeAmount { get; set; }

        public Guid? ConcentrationVolumeUOMKey { get; set; }

        public Guid? ConcentrationVolumeBaseUOMKey { get; set; }

        public string ConcentrationVolumeUOMDisplayCode { get; set; }

        public string ConcentrationVolumeUOMDescriptionText { get; set; }

        public bool? ConcentrationVolumeUOMUseDosageForm { get; set; }

        public decimal? ConcentrationVolumeUOMConversionAmount { get; set; }

        public Guid? ConcentrationVolumeExternalUOMKey { get; set; }

        public decimal? TotalVolumeAmount { get; set; }

        public Guid? TotalVolumeUOMKey { get; set; }

        public Guid? TotalVolumeBaseUOMKey { get; set; }

        public string TotalVolumeUOMDisplayCode { get; set; }

        public string TotalVolumeUOMDescriptionText { get; set; }

        public bool? TotalVolumeUOMUseDosageForm { get; set; }

        public decimal? TotalVolumeUOMConversionAmount { get; set; }

        public Guid? TotalVolumeExternalUOMKey { get; set; }

        public Guid? DosageFormKey { get; set; }

        public string DosageFormCode { get; set; }

        public string DosageFormDescriptionText { get; set; }

        public Guid? MedClassKey { get; set; }

        public string MedClassCode { get; set; }

        public bool? MedClassControlledFlag { get; set; }

        public string MedItemTypeInternalCode { get; set; }

        public decimal? MinimumDoseAmount { get; set; }

        public decimal? MaximumDoseAmount { get; set; }

        public Guid? DoseUOMKey { get; set; }

        public Guid? DoseBaseUOMKey { get; set; }

        public string DoseUOMDisplayCode { get; set; }

        public string DoseUOMDescriptionText { get; set; }

        public bool? DoseUOMUseDosageForm { get; set; }

        public decimal? DoseUOMConversionAmount { get; set; }
    }
}
