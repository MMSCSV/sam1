using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using CareFusion.Dispensing.Resources;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.CDCat;
using Pyxis.Core.Data.Schema.Core;
using Pyxis.Core.Data.Schema.Location;
using Pyxis.Core.Data.Schema.TableTypes;
using LocationDAL = Pyxis.Core.Data.Schema.Location;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class LocationRepository : LinqBaseRepository, ILocationRepository
    {
        #region ILocationRepository Members

        Unit ILocationRepository.LoadUnit(Guid unitKey)
        {
            Unit unit = ((ILocationRepository) this).GetUnit(unitKey);
            if (unit == null)
                throw new EntityNotFoundException(DataResources.LoadFailed_UnitNotFound, unitKey);

            return unit;
        }

        Unit ILocationRepository.GetUnit(Guid unitKey)
        {
            Unit unit = null;

            try
            {
                var query = BuildGetUnitQuery("us.UnitKey = @UnitKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multiSetReader = connectionScope.QueryMultiple(query.ToString(),
                        new { UnitKey = unitKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text))
                    {
                        unit = GetUnitResult(multiSetReader);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return unit;
        }

        Unit ILocationRepository.GetUnit(Guid facilityKey, string unitName)
        {
            Unit unit = null;

            try
            {
                var condition = "us.FacilityKey = @FacilityKey AND us.UnitName = @UnitName";
                var query = BuildGetUnitQuery(condition);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multiSetReader = connectionScope.QueryMultiple(query.ToString(),
                        new
                        {
                            FacilityKey = facilityKey,
                            UnitName = unitName
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text))
                    {
                        unit = GetUnitResult(multiSetReader);
                    }
                }

            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return unit;
        }

        Guid ILocationRepository.InsertUnit(Context context, Unit unit)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(unit, "unit");
            Guid? unitKey = null;

            try
            {
                var unitRepository = new UnitRepository();

                using (ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    unitKey = unitRepository.InsertUnit(context.ToActionContext(),
                        new LocationDAL.Models.Unit
                        {
                            UnitKey = unit.Key,
                            FacilityKey = unit.FacilityKey,
                            UnitName = unit.Name,
                            DescriptionText = unit.Description,
                            AutoDischargeModeInternalCode = unit.AutoDischargeMode?.ToInternalCode(),
                            AutoDischargeDurationAmount = unit.AutoDischargeDuration,
                            AlternateAutoDischargeDurationAmount = unit.AlternateAutoDischargeDuration,
                            ShowPreadmissionFlag = unit.ShowPreadmission,
                            ShowRecurringAdmissionFlag = unit.ShowRecurringAdmission,
                            PreadmissionLeadDurationAmount = unit.PreadmissionLeadDuration,
                            PreadmissionProlongedInactivityDurationAmount = unit.PreadmissionProlongedInactivityDuration,
                            AdmissionProlongedInactivityDurationAmount = unit.AdmissionProlongedInactivityDuration,
                            DischargeDelayDurationAmount = unit.DischargeDelayDuration,
                            TransferDelayDurationAmount = unit.TransferDelayDuration,
                            OmnlNoticePrinterName = unit.OmnlNoticePrinterName,
                            LongTermCareFlag = unit.LongTermCare,
                            LastModifiedBinaryValue = unit.LastModified
                        });

                    if (unit.Areas != null)
                    {
                        InsertUnitAreas(
                            context,
                            unitKey.Value,
                            unit.Areas);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return unitKey.GetValueOrDefault();
        }

        void ILocationRepository.UpdateUnit(Context context, Unit unit)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(unit, "unit");

            try
            {
                var unitRepository = new UnitRepository();

                using (ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    unitRepository.UpdateUnit(context.ToActionContext(),
                        new LocationDAL.Models.Unit
                        {
                            UnitKey = unit.Key,
                            FacilityKey = unit.FacilityKey,
                            UnitName = unit.Name,
                            DescriptionText = unit.Description,
                            AutoDischargeModeInternalCode = unit.AutoDischargeMode?.ToInternalCode(),
                            AutoDischargeDurationAmount = unit.AutoDischargeDuration,
                            AlternateAutoDischargeDurationAmount = unit.AlternateAutoDischargeDuration,
                            ShowPreadmissionFlag = unit.ShowPreadmission,
                            ShowRecurringAdmissionFlag = unit.ShowRecurringAdmission,
                            PreadmissionLeadDurationAmount = unit.PreadmissionLeadDuration,
                            PreadmissionProlongedInactivityDurationAmount = unit.PreadmissionProlongedInactivityDuration,
                            AdmissionProlongedInactivityDurationAmount = unit.AdmissionProlongedInactivityDuration,
                            DischargeDelayDurationAmount = unit.DischargeDelayDuration,
                            TransferDelayDurationAmount = unit.TransferDelayDuration,
                            OmnlNoticePrinterName = unit.OmnlNoticePrinterName,
                            LongTermCareFlag = unit.LongTermCare,
                            LastModifiedBinaryValue = unit.LastModified
                        });

                    UpdateUnitAreas(
                        context,
                        unit.Key,
                        unit.Areas ?? new Guid[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ILocationRepository.DeleteUnit(Context context, Guid unitKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                var unitRepository = new UnitRepository();

                unitRepository.DeleteUnit(context.ToActionContext(), unitKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IReadOnlyCollection<AssociatedUnit> ILocationRepository.GetUnitsAssociatedWithEncounters(IEnumerable<Guid> selectedUnitKeys)
        {
            GuidKeyTable selectedKeys = new GuidKeyTable(selectedUnitKeys);
            var associatedUnits = new List<AssociatedUnit>();
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                    {
                        {"@unitKeys", selectedKeys.AsTableValuedParameter()}
                    });

                    associatedUnits = connectionScope.Query<AssociatedUnit>(
                        "Location.bsp_ListUnitsAssociatedWithEncounters",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return associatedUnits;
        }

        Room ILocationRepository.GetRoom(Guid roomKey)
        {
            Room room = null;

            try
            {
                var query = BuildGetRoomQuery("urs.UnitRoomKey = @UnitRoomKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    room = connectionScope.Query<Room>(query.ToString(),
                        new { UnitRoomKey = roomKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return room;
        }

        Room ILocationRepository.GetRoom(Guid unitKey, string roomName)
        {
            Room room = null;

            try
            {
                var condition = "urs.UnitKey = @UnitKey AND urs.RoomName = @RoomName";
                var query = BuildGetRoomQuery(condition);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    room = connectionScope.Query<Room>(query.ToString(),
                        new
                        {
                            UnitKey = unitKey,
                            RoomName = roomName
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return room;
        }

        Guid ILocationRepository.InsertRoom(Context context, Room room)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(room, "room");
            Guid? unitRoomKey = null;

            try
            {
                var unitRoomRepository = new UnitRoomRepository();

                unitRoomKey = unitRoomRepository.InsertUnitRoom(context.ToActionContext(),
                    new LocationDAL.Models.UnitRoom
                    {
                        UnitRoomKey = room.Key,
                        UnitKey = room.UnitKey,
                        RoomName = room.Name,
                        RoomDescriptionText = room.Description,
                        LastModifiedBinaryValue = room.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return unitRoomKey.GetValueOrDefault();
        }

        void ILocationRepository.UpdateRoom(Context context, Room room)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(room, "room");

            try
            {
                var unitRoomRepository = new UnitRoomRepository();

                unitRoomRepository.UpdateUnitRoom(context.ToActionContext(),
                    new LocationDAL.Models.UnitRoom
                    {
                        UnitRoomKey = room.Key,
                        UnitKey = room.UnitKey,
                        RoomName = room.Name,
                        RoomDescriptionText = room.Description,
                        LastModifiedBinaryValue = room.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ILocationRepository.DeleteRoom(Context context, Guid roomKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                var unitRoomRepository = new UnitRoomRepository();
                unitRoomRepository.DeleteUnitRoom(context.ToActionContext(), roomKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<Area> ILocationRepository.GetAreas(IEnumerable<Guid> areaKeys, bool? deleted, Guid? facilityKey)
        {
            List<Area> areas = new List<Area>();
            if (areaKeys != null && !areaKeys.Any())
                return areas; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (areaKeys != null)
                    selectedKeys = new GuidKeyTable(areaKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "Location.bsp_GetAreas",
                    new { SelectedKeys = selectedKeys.AsTableValuedParameter(), DeleteFlag = deleted, FacilityKey = facilityKey},
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var areaResults = multi.Read<AreaResult>()
                        .ToArray();
                    var userRoleResults = multi.Read()
                        .ToArray();
                    var clinicalDataSubjectResults = multi.Read()
                        .ToArray();

                    foreach (var areaResult in areaResults)
                    {
                        Area area = new Area(areaResult.AreaKey)
                            {
                                FacilityKey = areaResult.FacilityKey,
                                FacilityCode = areaResult.FacilityCode,
                                FacilityName = areaResult.FacilityName,
                                Name = areaResult.AreaName,
                                Description = areaResult.DescriptionText,
                                AllUserRoles = areaResult.AllUserRolesFlag,
                                IsDeleted = areaResult.DeleteFlag,
                                LastModified = areaResult.LastModifiedBinaryValue.ToArray()
                            };

                        if (userRoleResults.Any(ur =>
                                ur.AreaKey == areaResult.AreaKey))
                        {
                            area.AssociatedRoles = userRoleResults
                                .Where(ur => (Guid)ur.AreaKey == areaResult.AreaKey)
                                .Select(ur => (Guid)ur.UserRoleKey)
                                .ToArray();
                        }

                        if (clinicalDataSubjectResults.Any(cds =>
                                cds.AreaKey == areaResult.AreaKey))
                        {
                            area.ClinicalDataSubjects = clinicalDataSubjectResults
                                .Where(cds => (Guid)cds.AreaKey == areaResult.AreaKey)
                                .Select(cds => (Guid)cds.ClinicalDataSubjectKey)
                                .ToArray();
                        }

                        areas.Add(area);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return areas;
        }

        Area ILocationRepository.GetArea(Guid areaKey)
        {
            var areas =
                ((ILocationRepository)this).GetAreas(new[] { areaKey });

            return areas.FirstOrDefault();
        }

        Guid ILocationRepository.InsertArea(Context context, Area area)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(area, "area");
            Guid? areaKey = null;

            try
            {
                var areaRepository = new AreaRepository();

                using (ConnectionScopeFactory.Create())
                using (var tx = TransactionScopeFactory.Create())
                {
                    areaKey = areaRepository.InsertArea(context.ToActionContext(),
                        new LocationDAL.Models.Area
                        {
                            AreaKey = area.Key,
                            FacilityKey = area.FacilityKey,
                            AreaName = area.Name,
                            DescriptionText = area.Description,
                            AllUserRolesFlag = area.AllUserRoles,
                            LastModifiedBinaryValue = area.LastModified
                        });

                    if (area.AssociatedRoles != null)
                        InsertUserRoleAreas(
                            context,
                            areaKey.Value,
                            area.AssociatedRoles);

                    if (area.ClinicalDataSubjects != null)
                    {
                        InsertAreaClinicalDataSubjects(
                            context,
                            areaKey.Value,
                            area.ClinicalDataSubjects);
                    }

                    //Commit transaction
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return areaKey.GetValueOrDefault();
        }

        void ILocationRepository.UpdateArea(Context context, Area area)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(area, "area");

            try
            {
                var areaRepository = new AreaRepository();

                using (ConnectionScopeFactory.Create())
                using (var tx = TransactionScopeFactory.Create())
                {
                    areaRepository.UpdateArea(context.ToActionContext(),
                        new LocationDAL.Models.Area
                        {
                            AreaKey = area.Key,
                            FacilityKey = area.FacilityKey,
                            AreaName = area.Name,
                            DescriptionText = area.Description,
                            AllUserRolesFlag = area.AllUserRoles,
                            LastModifiedBinaryValue = area.LastModified
                        });

                    UpdateUserRoleAreas(
                            context,
                            area.Key,
                            area.AssociatedRoles ?? new Guid[0]);

                    UpdateAreaClinicalDataSubjects(
                        context,
                        area.Key,
                        area.ClinicalDataSubjects ?? new Guid[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ILocationRepository.DeleteArea(Context context, Guid areaKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                var areaRepository = new AreaRepository();
                areaRepository.DeleteArea(context.ToActionContext(), areaKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Private Members

        private void InsertUserRoleAreas(Context context, Guid areaKey, Guid[] userRoleKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userRoleKeys, "userRoleKeys");

            var userRoleRepository = new UserRoleRepository();
            foreach (var roleKey in userRoleKeys)
            {
                userRoleRepository.AssociateUserRoleArea(context.ToActionContext(), roleKey, areaKey);
            }
        }

        private void UpdateUserRoleAreas(Context context, Guid areaKey, Guid[] userRoleKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userRoleKeys, "userRoleKeys");

            Guid[] currentAreaRoleKeys;

            var currentAreaRoleKeysQuery = new SqlBuilder();
            currentAreaRoleKeysQuery.SELECT("ura.UserRoleKey")
                .FROM("Core.UserRoleArea ura")
                .WHERE("ura.AreaKey = @AreaKey")
                ._("ura.DisassociationUTCDateTime IS NULL");

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentAreaRoleKeys = connectionScope.Query<Guid>(
                         currentAreaRoleKeysQuery.ToString(),
                         new
                         {
                             AreaKey = areaKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text).ToArray();
            }

            var removedRoleKeys = currentAreaRoleKeys.Except(userRoleKeys);

            var userRoleRepository = new UserRoleRepository();
            foreach (var removedRoleKey in removedRoleKeys)
            {
                userRoleRepository.DisassociateUserRoleArea(context.ToActionContext(), removedRoleKey, areaKey);
            }

            var newRoleKeys = userRoleKeys.Except(currentAreaRoleKeys).ToArray();
            InsertUserRoleAreas(context, areaKey, newRoleKeys);
        }

        private void InsertAreaClinicalDataSubjects(Context context, Guid areaKey, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");

            var clinicalDataSubjectRepository = new ClinicalDataSubjectRepository();
            foreach (Guid clinicalDataSubjectKey in clinicalDataSubjectKeys)
            {
                clinicalDataSubjectRepository.AssociateClinicalDataSubjectArea(context.ToActionContext(), clinicalDataSubjectKey, areaKey);
            }
        }

        private void UpdateAreaClinicalDataSubjects(Context context, Guid areaKey, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");

            var clinicalDataSubjectRepository = new ClinicalDataSubjectRepository();
            var currentClinicalDataSubjectKeysQuery = new SqlBuilder();
            currentClinicalDataSubjectKeysQuery.SELECT("cdss.ClinicalDataSubjectKey")
                .FROM("CDCat.ClinicalDataSubjectArea cdsa")
                .INNER_JOIN("CDCat.ClinicalDataSubjectSnapshot cdss ON cdss.ClinicalDataSubjectKey = cdsa.ClinicalDataSubjectKey")
                .INNER_JOIN("Location.AreaSnapshot as ON as.AreaKey = cdsa.AreaKey")
                .WHERE("cdsa.AreaKey = @AreaKey")
                ._("cdsa.DisassociationUTCDateTime IS NULL")
                ._("cdss.DeleteFlag = 0")
                ._("cdss.EndUTCDateTime IS NULL")
                ._("as.DeleteFlag = 0")
                ._("as.EndUTCDateTime IS NULL");

            // Get the list of clinical data subject keys associated with this area.
            Guid[] currentClinicalDataSubjectKeys;

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentClinicalDataSubjectKeys = connectionScope.Query<Guid>(
                         currentClinicalDataSubjectKeysQuery.ToString(),
                         new
                         {
                             AreaKey = areaKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text).ToArray();
            }

            // Find the clinical data subject keys that were removed.
            IEnumerable<Guid> removedClinicalDataSubjectKeys = currentClinicalDataSubjectKeys.Except(clinicalDataSubjectKeys);

            // Remove override groups that are no longer associated with this area.
            foreach (Guid removedClinicalDataSubjectKey in removedClinicalDataSubjectKeys)
            {
                clinicalDataSubjectRepository.DisassociateClinicalDataSubjectArea(context.ToActionContext(), removedClinicalDataSubjectKey, areaKey);
            }

            // Find the clinical data subject keys that were added
            IEnumerable<Guid> addedClinicalDataSubjectKeys = clinicalDataSubjectKeys.Except(currentClinicalDataSubjectKeys);
            InsertAreaClinicalDataSubjects(context, areaKey, addedClinicalDataSubjectKeys);
        }

        private static void InsertUnitAreas(Context context, Guid unitKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");

            var areaRepository = new AreaRepository();

            foreach (Guid areaKey in areaKeys)
            {
                areaRepository.AssociateAreaUnit(
                    context.ToActionContext(),
                    areaKey,
                    unitKey);
            }
        }

        private static void UpdateUnitAreas(Context context, Guid unitKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");

            var areaRepository = new AreaRepository();
            
            // Get the list of area keys associated with this unit
            IEnumerable<Guid> currentAreaKeys = GetAssociatedAreasByUnit(unitKey);

            // Find the area keys that were removed.
            IEnumerable<Guid> removedAreaKeys = currentAreaKeys.Except(areaKeys);

            // Remove areas that are no longer associated with this unit
            foreach (Guid areaKey in removedAreaKeys)
            {
                areaRepository.DisassociateAreaUnit(
                    context.ToActionContext(),
                    areaKey,
                    unitKey);
            }

            // Find the area keys that were added
            IEnumerable<Guid> addedAreaKeys = areaKeys.Except(currentAreaKeys);
            InsertUnitAreas(context, unitKey, addedAreaKeys);
        }

        private static IReadOnlyCollection<Guid> GetAssociatedAreasByUnit(Guid unitKey)
        {
            IReadOnlyCollection<Guid> areaKeys;

            SqlBuilder query = new SqlBuilder();
            query.SELECT("au.AreaKey")
                    .FROM("Location.AreaUnit au")
                    .WHERE("au.DisassociationUTCDateTime IS NULL")
                    .WHERE("au.UnitKey = @UnitKey");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                areaKeys = connectionScope.Query(
                     query.ToString(),
                     new { UnitKey = unitKey },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text)
                     .Select(au => (Guid)au.AreaKey)
                     .ToList();
            }

            return areaKeys;
        }

        private Unit GetUnitResult(SqlMapper.GridReader multiSetReader)
        {
            var unit = multiSetReader.Read<Unit>().FirstOrDefault();
            var areaKeysResult = multiSetReader.Read<Guid>();
            var roomsResult = multiSetReader.Read<Guid>();

            unit.Areas = areaKeysResult.ToArray();
            unit.Rooms = roomsResult.ToArray();

            return unit;
        }

        private string BuildGetUnitQuery(string condition)
        {
            // Unit Query
            var unitQuery = new SqlBuilder();
            unitQuery.SELECT("us.UnitKey AS [Key]")
                ._("us.FacilityKey")
                ._("us.UnitName AS Name")
                ._("us.DescriptionText AS Description")
                ._("us.AutoDischargeModeInternalCode AS AutoDischargeMode")
                ._("us.AutoDischargeDurationAmount AS AutoDischargeDuration")
                ._("us.AlternateAutoDischargeDurationAmount AS AlternateAutoDischargeDuration")
                ._("us.ShowPreadmissionFlag AS ShowPreadmission")
                ._("us.ShowRecurringAdmissionFlag AS ShowRecurringAdmission")
                ._("us.PreadmissionLeadDurationAmount AS PreadmissionLeadDuration")
                ._("us.PreadmissionProlongedInactivityDurationAmount AS PreadmissionProlongedInactivityDuration")
                ._("us.AdmissionProlongedInactivityDurationAmount AS AdmissionProlongedInactivityDuration")
                ._("us.DischargeDelayDurationAmount AS DischargeDelayDuration")
                ._("us.TransferDelayDurationAmount AS TransferDelayDuration")
                ._("us.OmnlNoticePrinterName")
                ._("us.LongTermCareFlag AS LongTermCare")
                ._("us.LastModifiedBinaryValue AS LastModified")
                ._("fs.FacilityName")
                .FROM("Location.UnitSnapshot us")
                .INNER_JOIN("Location.FacilitySnapshot fs ON fs.FacilityKey = us.FacilityKey")
                .WHERE(condition)
                ._("us.DeleteFlag = 0")
                ._("us.EndUTCDateTime IS NULL")
                ._("fs.EndUTCDateTime IS NULL");

            // Areas
            var areaKeysQuery = new SqlBuilder();
            areaKeysQuery.SELECT("au.AreaKey")
                .FROM("Location.AreaUnit au")
                .INNER_JOIN("Location.AreaSnapshot ass ON ass.AreaKey = au.AreaKey")
                .INNER_JOIN("Location.UnitSnapshot us ON us.FacilityKey = ass.FacilityKey")
                .WHERE(condition)
                ._("au.DisassociationUTCDateTime IS NULL")
                ._("us.DeleteFlag = 0")
                ._("us.EndUTCDateTime IS NULL")
                ._("ass.DeleteFlag = 0")
                ._("ass.EndUTCDateTime IS NULL");

            // Rooms
            var roomsQuery = new SqlBuilder();
            roomsQuery.SELECT("urs.UnitRoomKey")
                .FROM("Location.UnitRoomSnapshot urs")
                .INNER_JOIN("Location.UnitSnapshot us ON us.UnitKey = urs.UnitKey")
                .WHERE(condition)
                ._("us.DeleteFlag = 0")
                ._("us.EndUTCDateTime IS NULL")
                ._("urs.DeleteFlag = 0")
                ._("urs.EndUTCDateTime IS NULL");

            var query = new StringBuilder();
            query.AppendLine(unitQuery.ToString());
            query.AppendLine(areaKeysQuery.ToString());
            query.AppendLine(roomsQuery.ToString());

            return query.ToString();
        }

        private string BuildGetRoomQuery(string condition)
        {
            var roomQuery = new SqlBuilder();
            roomQuery.SELECT()
                ._("urs.UnitRoomKey AS [Key]")
                ._("urs.UnitKey")
                ._("urs.RoomName AS Name")
                ._("urs.RoomDescriptionText AS Description")
                ._("urs.LastModifiedBinaryValue AS LastModified")
                ._("us.UnitName")
                .FROM("Location.UnitRoomSnapshot urs")
                .INNER_JOIN("Location.UnitSnapshot us ON us.UnitKey = urs.UnitKey")
                .WHERE(condition)
                ._("urs.DeleteFlag = 0")
                ._("urs.EndUTCDateTime IS NULL")
                ._("us.DeleteFlag = 0")
                ._("us.EndUTCDateTime IS NULL");

            var query = new StringBuilder();
            query.AppendLine(roomQuery.ToString());

            return query.ToString();
        }

        #endregion
    }
}
