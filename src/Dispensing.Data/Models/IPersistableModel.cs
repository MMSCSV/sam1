using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Models
{
    /// <summary>
    /// Represents persistable data object.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IPersistableModel<TKey> : IEntity<TKey> // Deriving from IEntity<TKey> is only for backward compatibility.
    {
    }
}
