using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Resources;
using LinqKit;
using Mms.Logging;
using MsValidation = Microsoft.Practices.EnterpriseLibrary.Validation.Validation;

namespace CareFusion.Dispensing.Services.Business
{
    public class DispensingDeviceManager : IDispensingDeviceManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Contructors

        public DispensingDeviceManager()
        {
        }

        #endregion

        #region IDispensingDeviceManager Members

        public DispensingDevice Get(string computerName)
        {
            Guard.ArgumentNotNullOrEmptyString(computerName, "computerName");

            using (IStorageRepository repository = RepositoryFactory.Create<IStorageRepository>())
            {
                return repository.GetDispensingDevice(computerName);
            }
        }

        public DispensingDevice Get(Guid dispensingDeviceKey)
        {
            using (IStorageRepository repository = RepositoryFactory.Create<IStorageRepository>())
            {
                return repository.GetDispensingDevice(dispensingDeviceKey);
            }
        }

        public virtual Guid Add(Context context, DispensingDevice dispensingDevice)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dispensingDevice, "dispensingDevice");

            Guid dispensingDeviceKey;
            using (IStorageRepository repository = RepositoryFactory.Create<IStorageRepository>())
            {
                // Validate
                Validate(repository, dispensingDevice);

                dispensingDeviceKey = repository.InsertDispensingDevice(context, dispensingDevice);
            }

            return dispensingDeviceKey;
        }

        public virtual void Update(Context context, DispensingDevice dispensingDevice)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dispensingDevice, "dispensingDevice");

            using (IStorageRepository repository = RepositoryFactory.Create<IStorageRepository>())
            {
                // Validate
                Validate(repository, dispensingDevice);

                var currentDispensingDevice = (
                    from dd in repository.GetQueryableEntity<DispensingDeviceEntity>()
                    where dd.Key == dispensingDevice.Key
                    select new {dd.SyncUploadAllowSkipFlag, dd.SyncAllowDownloadOnUploadFailureFlag})
                    .FirstOrDefault();

                repository.UpdateDispensingDevice(context, dispensingDevice);

                if (currentDispensingDevice != null)
                {
                    // Log warnings if sync specific properties have changed.
                    if (currentDispensingDevice.SyncUploadAllowSkipFlag !=
                        dispensingDevice.SyncUploadAllowSkip)
                    {
                        Log.Warn(EventId.DataSyncRetryConfigChange,
                            string.Format(
                                "Sync configuration setting for '{0}' was changed:\r\n\tDevice Key: {1}\r\n\tDevice Name: {2}\r\n\tSigned In User Name: {3}\r\n\tSigned In User ID: {4}",
                                "Allow Skip",
                                dispensingDevice.Key,
                                dispensingDevice.Name,
                                context.User != null
                                    ? context.User.LastName + ", " + context.User.FirstName
                                    : "<unknown>",
                                context.User != null ? context.User.UserId : "<unknown>"));
                    }

                    if (currentDispensingDevice.SyncAllowDownloadOnUploadFailureFlag !=
                        dispensingDevice.SyncAllowDownloadOnUploadFailure)
                    {
                        Log.Warn(EventId.DataSyncRetryConfigChange,
                            string.Format(
                                "Sync configuration setting for '{0}' was changed:\r\n\tDevice Key: {1}\r\n\tDevice Name: {2}\r\n\tSigned In User Name: {3}\r\n\tSigned In User ID: {4}",
                                "Allow DownLoad On Upload Failure",
                                dispensingDevice.Key,
                                dispensingDevice.Name,
                                context.User != null
                                    ? context.User.LastName + ", " + context.User.FirstName
                                    : "<unknown>",
                                context.User != null ? context.User.UserId : "<unknown>"));
                    }
                }
            }
        }

        public string GetSerialId(Guid dispensingDeviceKey)
        {
            using (IStorageRepository repository = RepositoryFactory.Create<IStorageRepository>())
            {
                return repository.GetDispensingDeviceSerialId(dispensingDeviceKey);
            }
        }

        #endregion

        #region Private Members

        private static void Validate(IRepository repository, DispensingDevice dispensingDevice)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();

            // Validate contract
            var validationResults = MsValidation.Validate(dispensingDevice);
            if (!validationResults.IsValid)
                validationErrors.AddRange(validationResults.ToValidationErrorsArray());

            if (dispensingDevice.IsProfileMode == false &&
                dispensingDevice.IsTemporarilyNonProfileMode == true)
            {
                validationErrors.Add(ValidationError.CreateValidationError<DispensingDevice>(
                    dd=> dd.IsProfileMode, ValidationStrings.DispensingDeviceProfileModeIncorrectMessage));
            }

            // Name must be unique within a facility.
            Expression<Func<DispensingDeviceEntity, bool>> existsPredicate = (dd) =>
                    (dd.DispensingDeviceName == dispensingDevice.Name &&
                     dd.FacilityKey == dispensingDevice.FacilityKey);
            if (!dispensingDevice.IsTransient())
            {
                // Ignore self.
                existsPredicate = existsPredicate.And(dd => dd.Key != dispensingDevice.Key);
            }

            if (repository.Exists(existsPredicate))
            {
                validationErrors.Add(ValidationError.CreateValidationError<DispensingDevice>(
                    dd => dd.Name, ValidationStrings.DispensingDeviceNameNotUnique));
            }

            // Computer Name must be unique within a facility.
            existsPredicate = (dd) =>
                    (dd.ComputerName == dispensingDevice.ComputerName &&
                     dd.FacilityKey == dispensingDevice.FacilityKey);
            if (!dispensingDevice.IsTransient())
            {
                // Ignore self.
                existsPredicate = existsPredicate.And(dd => dd.Key != dispensingDevice.Key);
            }

            if (repository.Exists(existsPredicate))
            {
                validationErrors.Add(ValidationError.CreateValidationError<DispensingDevice>(
                    dd => dd.ComputerName, ValidationStrings.DispensingDeviceComputerNameNotUnique));
            }

            validationErrors.AddRange(ValidateCriticalOverrideSchedules(repository, dispensingDevice));

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(
                    string.Format(CultureInfo.CurrentCulture,
                        ServiceResources.Exception_Validation,
                        typeof(DispensingDevice).Name),
                    validationErrors);
            }
        }

        private static IEnumerable<ValidationError> ValidateCriticalOverrideSchedules(IRepository repository, DispensingDevice device)
        {
            List<ValidationError> result = new List<ValidationError>();

            var schedules = device.CriticalOverridePeriods == null ? Enumerable.Empty<CriticalOverridePeriod>() : device.CriticalOverridePeriods.ToList();
            var newSchedules = schedules.Where(c => c.IsTransient() || c.Key == Guid.Empty);

            foreach (CriticalOverridePeriod item in newSchedules)
            {
                CheckCriticalOverrideScheduleDuplicates(item, 
                    device.CriticalOverridePeriods.Where(c => c != item), result);
            }
            if (result.Count > 0)
            {
                return result;
            }
            //check the database
            IEnumerable<CriticalOverridePeriod> existingSchedules = null;
            using (IStorageRepository storagerepository = RepositoryFactory.Create<IStorageRepository>())
            {
                DispensingDevice current = storagerepository.GetDispensingDevice(device.Key);
                if (current != null && current.CriticalOverridePeriods != null)
                {
                    existingSchedules = current.CriticalOverridePeriods.Where(c => schedules.Contains(c.Key));
                }
            }
            if (existingSchedules != null)
            {
                foreach (CriticalOverridePeriod item in schedules)
                {
                    CheckCriticalOverrideScheduleDuplicates(item, existingSchedules.Where(c => c.Key != item.Key
                        ), result);
                }
            }
            return result;
        }

        private static void CheckCriticalOverrideScheduleDuplicates(CriticalOverridePeriod item, IEnumerable<CriticalOverridePeriod> schedules, List<ValidationError> result)
        {
            if (schedules.FirstOrDefault(s => string.Compare(s.Name, item.Name, true) == 0) != null)
            {
                result.Add(ValidationError.CreateValidationError<CriticalOverridePeriod>(
                    string.Format(ValidationStrings.CriticalOverridePeriodNameNotUnique, item.Name)));
            }
            if (schedules.FirstOrDefault(s =>
                s.StartTimeOfDay == item.StartTimeOfDay
                && s.EndTimeOfDay == item.EndTimeOfDay
                && s.Monday == item.Monday
                && s.Tuesday == item.Tuesday
                && s.Wednesday == item.Wednesday
                && s.Thursday == item.Thursday
                && s.Friday == item.Friday
                && s.Saturday == item.Saturday
                && s.Sunday == item.Sunday) != null)
            {
                result.Add(ValidationError.CreateValidationError<CriticalOverridePeriod>(
                    string.Format(ValidationStrings.CriticalOverridePeriodAttributesNotUnique, item.Name)));
            }
        }

        #endregion
    }
}
