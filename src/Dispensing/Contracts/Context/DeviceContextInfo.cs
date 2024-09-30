using System;
using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
	public sealed class DeviceContextInfo
	{
		#region Constructors

		public DeviceContextInfo()
		{
		}

        public DeviceContextInfo(Guid dispensingDeviceKey)
        {
            Key = dispensingDeviceKey;
        }

		#endregion

		#region Operator Overloads

		public static explicit operator Guid?(DeviceContextInfo deviceContextInfo)
		{
			return deviceContextInfo != null ? deviceContextInfo.Key : default(Guid?);
		}

		#endregion

		#region Public Properties (WCF - Specific)

        [DataMember]
        public Guid Key { get; set; }

        [DataMember]
        public Guid SnapshotKey { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid FacilityKey { get; set; }

        [DataMember]
        public Guid FacilitySnapshotKey { get; set; }

        [DataMember]
        public string FacilityName { get; set; }

        [DataMember]
        public Guid PharmacyInformationSystemKey { get; set; }

        [DataMember]
        public Guid ControlledSubstanceLicenseKey { get; set; }

        [DataMember]
        public Guid ControlledSubstanceLicenseSnapshotKey { get; set; }
        #endregion

        #region Public Properties

        // Facility setting
        public short UserAuthenticationRequestDuration2Amount { get; set; }

        public AuthenticationMethodInternalCode FailoverAuthenticationMethod { get; set; }

        public AuthenticationMethodInternalCode FailoverCardPinAuthenticationMethod { get; set; }

        public AuthenticationMethodInternalCode FailoverRfidAuthenticationMethod { get; set; }

        public bool CardPinAbsentFlag { get; set; }

        public bool PreventCardPINAuthenticationFailoverFlag { get; set; }

        public string ServerAddress { get; set; }

        public bool IsOutOfService { get; set; }

        public bool AutomaticallyPrintMedEmptyReturnBin { get; set; }

        public bool AutomaticallyPrintMedReturn { get; set; }

        public bool AutomaticallyPrintMedRemove { get; set; }

        public bool AutomaticallyPrintMedDispose { get; set; }

        public bool IsProfileMode { get; set; }

        public short OpenDrawerTimeOutDuration { get; set; }

        public short? OpenBinTimeoutDurationAmount { get; set; }

        public short EmptyReturnBinTimeOutDuration { get; set; }

        public short MenuTimeOutDuration { get; set; }

		public short AnesthesiaTimeOutDuration { get; set; }

        public short GCSMDestructionBinTimeOutDurationAmount { get; set; }

        public GCSMCompareReportStandardRangeInternalCode? GCSMCompareReportStandardRange { get; set; }

        public short? AnesthesiaSwitchUserDurationAmount { get; set; }

        public bool AnesthesiaSwitchUserFlag { get; set; }

        public bool IsCriticalOverride { get; set; }

        public bool IsTemporaryNonProfileMode { get; set; }

        public bool OutdateTracking { get; set; }

        public bool RxCheck { get; set; }

        public short InventoryDrawerMapTimeOutDuration { get; set; }

        public AuthenticationMethodInternalCode DeviceAuthenticationMode { get; set; }

        public AuthenticationMethodInternalCode FingerPrintExemptAuthenticationMode { get; set; }

        public AuthenticationMethodInternalCode CardPinExemptAuthenticationMode { get; set; }

        public bool ManualUpgradeRequired { get; set; }

        public UserScanModeInternalCode UserIdScanMode { get; set; }

        public bool MatchUserScanCodeByScanCode { get; set; }

        public string UserIDScanCodePreText { get; set; }

        public string UserIDScanCodePostText { get; set; }

        public string SyncServerAddress { get; set; }

        public DispensingDeviceTypeInternalCode DispensingDeviceType { get; set; }

        public byte[] LastUploadTickValue { get; set; }

        public bool IsSyncConnected { get; set; }

        public bool IsFullDownloadRequired { get; set; }

        public bool IsGrabScan { get; set; }

	    public byte MedSearchStringLength { get; set; }

	    public bool MedSearchStringFlag { get; set; }

        public bool DisplayPatientPreferredName { get; set; }

        public bool DisablePyxisBarcodeScanOnLoadRefillFlag { get; set; }

        // Sync 2.0 
        public string DispensingDeviceSyncStateStatusInternalCode { get; set; }

        public Guid InventoryPharmacyInformationSystemKey { get; set; }

        #endregion
    }
}
