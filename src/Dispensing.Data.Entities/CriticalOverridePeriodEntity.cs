using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class CriticalOverridePeriodEntity : IContractConvertible<CriticalOverridePeriod>
    {
        #region IContractConvertible<CriticalOverridePeriod> Members

        public CriticalOverridePeriod ToContract()
        {
            return new CriticalOverridePeriod(Key)
                {
                    Name = CriticalOverridePeriodName,
                    StartTimeOfDay = StartTimeOfDayValue,
                    EndTimeOfDay = EndTimeOfDayValue,
                    Monday = MondayFlag,
                    Tuesday = TuesdayFlag,
                    Wednesday = WednesdayFlag,
                    Thursday = ThursdayFlag,
                    Friday = FridayFlag,
                    Saturday = SaturdayFlag,
                    Sunday = SundayFlag,
                    CreatedActorKey = CreatedActorKey,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
