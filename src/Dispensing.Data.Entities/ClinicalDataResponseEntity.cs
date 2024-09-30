using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ClinicalDataResponseEntity : IContractConvertible<ClinicalDataResponse>
    {
        #region Implementation of IContractConvertible<ClinicalDataResponse>

        public ClinicalDataResponse ToContract()
        {
            return new ClinicalDataResponse(Key)
                {
                    Assent =
                        ClinicalDataAssentInternalCode.FromNullableInternalCode<ClinicalDataAssentInternalCode>(),
                    Instruction = InstructionText,
                    Response = ResponseText,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
