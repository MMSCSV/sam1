using System;
using System.Linq;
using Pyxis.Core.Data.Schema.Core;
using Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Encryption
{
    internal class DatabaseKeyProvider : IKeyProvider
    {
        public EncryptionKey GetEncryptionKey(
            string keyContext,
            string algorithm)
        {
            // get encryption value from repository
            var repository = new EncryptionKeyEntryRepository();
            var entries = repository
                .GetEncryptionKeyEntries()
                .Where(x =>
                    x.EncryptionKeyContextText == keyContext &&
                    x.EncryptionAlgorithmInternalCode == algorithm);

            // if none, return null
            if (!entries.Any())
                return null;

            // should always only be one
            if (entries.Count() > 1)
                throw new Exception($"Multiple database entries for keyContext: {keyContext} and algorithm: {algorithm}.");

            // get key value
            var entry = entries.First();
            var keyValue = GetKey(entry.EncryptionKeyValue);
            return new EncryptionKey(keyValue);
        }

        public void SaveEncryptionKey(
            string keyContext,
            string algorithm,
            EncryptionKey key)
        {
            var repository = new EncryptionKeyEntryRepository();
            // save to repo
            repository.InsertEncryptionKeyEntry(
                new EncryptionKeyEntry
                {
                    EncryptionKeyValue = GetKeyValue(key.Value),
                    EncryptionKeyContextText = keyContext,
                    EncryptionAlgorithmInternalCode = algorithm,
                    CreatedDateTime = DateTimeOffset.Now
                });
        }

        private string GetKeyValue(byte[] key)
        {
            return Convert.ToBase64String(key);
        }

        private byte[] GetKey(string value)
        {
            return Convert.FromBase64String(value);
        }
    }
}
