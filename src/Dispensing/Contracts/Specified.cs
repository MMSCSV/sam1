using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    [DataContract(Name = "{0}Specified", Namespace = ContractConstants.ContractsNamespaceV1)]
    public struct Specified<T>
    {
        private static readonly Func<T, bool> isNull;

        [DataMember(Name = "IsSpecified")]
        private readonly bool _isSpecified;
        [DataMember(Name = "Value")]
        private T _value;

        #region Constructors

        static Specified()
        {
            if (typeof(T).IsValueType)
            {
                isNull = val => false;
            }
            else
            {
                isNull = val => (EqualityComparer<T>.Default.Equals(val, default(T)));
            }
        }

        public Specified(T value)
        {
            _value = value;
            _isSpecified = true;
        }

        public static implicit operator Specified<T>(T value)
        {
            return new Specified<T>(value);
        }

        public static explicit operator T(Specified<T> value)
        {
            return value.Value;
        }

        #endregion

        #region Public Properties

        public bool IsSpecified
        {
            get { return _isSpecified; }
        }

        public T Value
        {
            get
            {
                if (!_isSpecified)
                    throw new InvalidOperationException("Attempted to get a value when not specified.");

                return _value;
            }
        }

        #endregion

        #region Public Members

        public T GetValueOrDefault()
        {
            return _value;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            if (!IsSpecified)
            {
                return defaultValue;
            }
            
            return _value;
        }

        public override bool Equals(object other)
        {
            if (!IsSpecified || isNull(_value))
            {
                if (other is Specified<T>)
                    return (!IsSpecified && !((Specified<T>)other).IsSpecified) ||
                        (((Specified<T>)other).IsSpecified && isNull(((Specified<T>)other)._value));

                return IsSpecified && (other == null);
            }

            if (other == null)
                return false;

            if (other is Specified<T>)
                return ((Specified<T>) other).IsSpecified && _value.Equals(((Specified<T>) other)._value);

            return _value.Equals(other);
        }

        public override int GetHashCode()
        {
            if (!IsSpecified || isNull(_value))
            {
                return 0;
            }

            return _value.GetHashCode();
        }

        public override string ToString()
        {
            if (!IsSpecified)
                return DispensingResources.Specified_Mixed;
            if(isNull(_value))
                return string.Empty;

            return _value.ToString();
        }

        #endregion
    }
}
