using System;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.Schema;

namespace CareFusion.Dispensing.Data
{
    public static class ContextExtensions
    {
        /// <summary>
        /// This method makes a best effort conversion of the <see cref="Context"/> objects action date/time
        /// to an <see cref="ActionContext"/> which is based on <see cref="DateTimeOffset"/>.
        /// </summary>
        public static ActionContext ToActionContext(this Context source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            DateTimeOffset dateTimeOffset;
            try
            {
                // Make sure ActionDateTime is Unspecified so that the offset is not compared to the
                // local system's current time zone. 
                var actionDateTime = DateTime.SpecifyKind(source.ActionDateTime, DateTimeKind.Unspecified);

                // Calulate the captured UTC offset based on the UTC and Local date/time's.
                var offset = actionDateTime - source.ActionUtcDateTime;
                dateTimeOffset = new DateTimeOffset(actionDateTime, offset);
            }
            catch
            {
                try
                {
                    // Best effort keeping the captured local date/time but using the local system's
                    // current time zone to calculate the UTC offset.
                    dateTimeOffset = new DateTimeOffset(source.ActionDateTime);
                }
                catch
                {
                    // Something is wrong with the ActionDateTime, falling back to capturing the local system's
                    // current time zone. The drawback to this fallback is that we will potentially lose milliseconds
                    // to seconds of accuracy of when the action was captured. A worse scenario is if the captured offset
                    // was made in a different time zone than this local system, but this fallback is a very edge case.
                    dateTimeOffset = DateTimeOffset.Now;
                }
            }

            return new ActionContext(
                    dateTimeOffset,
                    (Guid?)source.Actor,
                    (Guid?)source.User,
                    (Guid?)source.Device);
        }
    }
}
