using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing.Contracts
{
    public enum SearchOperator
    {
        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_StartsWith")]
        StartsWith,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_Contains")]
        Contains,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_EndsWith")]
        EndsWith,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_NotContains")]
        NotContains,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_Equals")]
        Equals,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_NotEquals")]
        NotEquals,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_LessThan")]
        LessThan,

        [LocalizableDisplayName(typeof(DispensingResources), "SearchOperator_GreaterThan")]
        GreaterThan
    }
}
