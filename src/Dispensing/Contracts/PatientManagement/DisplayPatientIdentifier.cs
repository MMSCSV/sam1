using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a display patient identifier for a given facility patient silo.
    /// </summary>
    public class DisplayPatientIdentifier
    {
        public static Guid EncounterIdIdentifierKey = Guid.Parse("CABD4D11-C236-45FE-966D-2EAA1318B388");
        public static Guid AlternateEncounterIdIdentifierKey = Guid.Parse("C65E6274-C696-4AAA-88CF-6D9E280499F9");
        public static Guid AccountIdIdentifierKey = Guid.Parse("AAD1A4BE-3407-4E9E-B32C-E27C6C7AFC7C");
        
        /// <summary>
        /// The key of the identifier.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// The display code of the identifier.
        /// </summary>
        public string DisplayCode { get; set; }


        #region contract rules

        public bool DisplayEncounterIdentifier
        {
            get { return Key == EncounterIdIdentifierKey; }
        }

        public bool DisplayAccountIdentifier
        {
            get { return Key == AccountIdIdentifierKey; }
        }

        public bool DisplayAlternateEncounterIdentifier
        {
            get { return Key == AlternateEncounterIdIdentifierKey; }
        }

        public Guid? PatientIdentifierTypeKey
        {
            get
            {
                return (DisplayAccountIdentifier || DisplayAlternateEncounterIdentifier || DisplayEncounterIdentifier || Key == Guid.Empty) ? default(Guid?) : Key;
            }
        }
        #endregion
    }
}
