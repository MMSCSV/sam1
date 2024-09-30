using System;
using CareFusion.Dispensing.Models;
using NUnit.Framework;

namespace CareFusion.Dispensing.Test.Contracts
{
    [TestFixture]
    public class UnitOfMeasureFixture
    {
        [Test]
        public void CanConvertFromMililiterToLiterTest()
        {
            UnitOfMeasure mililiterUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = null,
                Conversion = null
            };

            UnitOfMeasure literUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = mililiterUOM.Key,
                Conversion = 1000
            };

            Assert.IsTrue(UnitOfMeasure.CanConvert(mililiterUOM, literUOM));
        }

        [Test]
        public void CanConvertFromLiterToMililiterTest()
        {
            UnitOfMeasure mililiterUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = null,
                Conversion = null
            };

            UnitOfMeasure literUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 150,
                IsActive = true,
                BaseUnitOfMeasureKey = mililiterUOM.Key,
                Conversion = 1000
            };

            Assert.IsTrue(UnitOfMeasure.CanConvert(literUOM, mililiterUOM));
        }

        [Test]
        public void CannotConvertFromLiterToMiligramTest()
        {
            UnitOfMeasure miligramUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mg",
                InternalCode = "mg",
                Description = "Miligram",
                UseDosageForm = false,
                SortOrder = 50,
                IsActive = true,
                BaseUnitOfMeasureKey = null,
                Conversion = null
            };

            UnitOfMeasure literUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = Guid.NewGuid(), // Should be the Mililiter unit of measure as base
                Conversion = 1000
            };

            Assert.IsFalse(UnitOfMeasure.CanConvert(literUOM, miligramUOM));
        }

        [Test]
        public void ConvertFromLiterToMililiterTest()
        {
            UnitOfMeasure mililiterUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = null,
                Conversion = null
            };

            UnitOfMeasure literUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 150,
                IsActive = true,
                BaseUnitOfMeasureKey = mililiterUOM.Key,
                Conversion = 1000
            };

            // Convert 2 liters to mililiters
            decimal convertedValue = UnitOfMeasure.Convert(2, literUOM, mililiterUOM);
            Assert.AreEqual(2000, convertedValue);
        }

        [Test]
        public void ConvertFromMililiterToLiterTest()
        {
            UnitOfMeasure mililiterUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 140,
                IsActive = true,
                BaseUnitOfMeasureKey = null,
                Conversion = null
            };

            UnitOfMeasure literUOM = new UnitOfMeasure
            {
                Key = Guid.NewGuid(),
                DisplayCode = "mL",
                InternalCode = "mL",
                Description = "Mililiter",
                UseDosageForm = false,
                SortOrder = 150,
                IsActive = true,
                BaseUnitOfMeasureKey = mililiterUOM.Key,
                Conversion = 1000
            };

            // Convert 5000 mililiters to liters
            decimal convertedValue = UnitOfMeasure.Convert(5000, mililiterUOM, literUOM);
            Assert.AreEqual(5, convertedValue);
        }
    }
}
