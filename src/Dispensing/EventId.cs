namespace CareFusion.Dispensing
{
    public static class EventId
    {
        public const int UnexpectedError = 999;
        public const int ADError = 2000;
        public const int DataSyncRetryConfigChange = 5001;
        public const int MaintenanceServiceError = 6200;
        public const int NotificationServiceError = 6500;
        public const int StationPrinterError = 12000;
        public const int ServiceDependency = 13000;
        public const int PurgeDeletionError = 17101;
        //-----------------------------------------------------
        // Inbound Message Processing
        public const int InboundMessageProcessingError = 3100;
        //------------------------------------------------------

        //------------------------------------------------------
        // Outbound Message Processing
        public const int OutboundMessagePublishingError = 4000;
        public const int OutboundMessageMissingTxError = 4001;
        //------------------------------------------------------

        public const int SystemEventError = 7000;
        public const int InboundProblemEventError = 7100;
        public const int NewItemIdEventError = 7200;

        public const int MsgSenderError = 4100;
        public const int MsgSenderSubscriberDisconnectedError = 4101;
        public const int MsgSenderSubscriberDeactivatedError = 4102;

        public const int BulletinServiceError = 6000;
        public const int ArchiveServiceError = 6100;
        public const int ArchiveSessionItemTransactionQuantityMismatch = 6101;

        //------------------------------------------------------ 
        // Inventory Optimization 
        public const int InventoryOptimizationProcessingError = 4200;
        //------------------------------------------------------ 
    }
}
