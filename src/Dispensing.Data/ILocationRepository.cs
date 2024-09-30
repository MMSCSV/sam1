using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;

namespace CareFusion.Dispensing.Data
{
    /// <summary>
    /// Represents the Location repository.
    /// </summary>
    public interface ILocationRepository : IRepository
    {
        #region Unit Members

        /// <summary>
        /// Loads the unit that matched the specified key others throws an exception.
        /// </summary>
        /// <param name="unitKey">The unit key.</param>
        /// <returns>A <see cref="Unit"/> object otherwise throws an <see cref="EntityNotFoundException"/> if not exist.</returns>
        Unit LoadUnit(Guid unitKey);

        /// <summary>
        /// Gets the unit that matches the specified key.
        /// </summary>
        /// <param name="unitKey">The unit key.</param>
        /// <returns>A <see cref="Unit"/> object.</returns>
        Unit GetUnit(Guid unitKey);

        /// <summary>
        /// Gets the unit that matches the facility key and unit name.
        /// </summary>
        /// <param name="facilityKey">The facility key.</param>
        /// <param name="unitName">The unit name.</param>
        /// <returns>A <see cref="Unit"/> object.</returns>
        Unit GetUnit(Guid facilityKey, string unitName);

        /// <summary>
        /// Persists the new unit to the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="unit">The unit to persist.</param>
        /// <returns>A unit key, which uniquely identifies the unit in the database.</returns>
        Guid InsertUnit(Context context, Unit unit);

        /// <summary>
        /// Updates an existing unit in the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="unit">The unit to update.</param>
        void UpdateUnit(Context context, Unit unit);

        /// <summary>
        /// Logically deletes an existing unit in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="unitKey">The unit to delete.</param>
        void DeleteUnit(Context context, Guid unitKey);

        /// <summary>
        /// Retrieve units that are associated with patient encounters
        /// </summary>
        /// <param name="selectedUnitKeys">The selected unit keys</param>
        /// <returns>Returns a list of AssociatedUnits</returns>
        IReadOnlyCollection<AssociatedUnit> GetUnitsAssociatedWithEncounters(IEnumerable<Guid> selectedUnitKeys) ;

        #endregion

        #region Room Members

        /// <summary>
        /// Gets the room that matches the specified key.
        /// </summary>
        /// <param name="roomKey">The room key.</param>
        /// <returns>A <see cref="Room"/> object.</returns>
        Room GetRoom(Guid roomKey);

        /// <summary>
        /// Gets the room that matches the unit key and room name.
        /// </summary>
        /// <param name="unitKey">The room key.</param>
        /// <param name="roomName"></param>
        /// <returns>A <see cref="Room"/> object.</returns>
        Room GetRoom(Guid unitKey, string roomName);


        /// <summary>
        /// Persists the new room to the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="room">The room to persist.</param>
        /// <returns>A room key, which uniquely identifies the room in the database.</returns>
        Guid InsertRoom(Context context, Room room);

        /// <summary>
        /// Updates an existing room in the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="room">The room to update.</param>
        void UpdateRoom(Context context, Room room);

        /// <summary>
        /// Logically deletes an existing room in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="roomKey">The room to delete.</param>
        void DeleteRoom(Context context, Guid roomKey);

        #endregion

        #region Area Members

        /// <summary>
        /// Retrieves a collection of <see cref="Area"/>.
        /// </summary>
        /// <param name="areaKeys">The collection of facility keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Area"/>.</returns>
        IEnumerable<Area> GetAreas(IEnumerable<Guid> areaKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets the area that matches the specified key.
        /// </summary>
        /// <param name="areaKey">The unit key.</param>
        /// <returns>A <see cref="Area"/> object.</returns>
        Area GetArea(Guid areaKey);

        /// <summary>
        /// Persists the new area to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="area">The area to persist.</param>
        /// <returns>A area key, which uniquely identifies the area in the database.</returns>
        Guid InsertArea(Context context, Area area);

        /// <summary>
        /// Updates an existing area in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="area">The area to update.</param>
        void UpdateArea(Context context, Area area);

        /// <summary>
        /// Logically deleted an existing area in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="areaKey">The area key.</param>
        void DeleteArea(Context context, Guid areaKey);

        #endregion
    }
}
