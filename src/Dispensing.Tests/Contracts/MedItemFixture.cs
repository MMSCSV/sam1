using System;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using NUnit.Framework;

namespace CareFusion.Dispensing.Test.Contracts
{
    [TestFixture]
    public class MedItemFixture
    {
        #region Test Data

        private UnitOfMeasure _miligramUom = new UnitOfMeasure
        {
            Key = Guid.Parse("BCF1389D-7AA1-DF11-BA1C-0024818F4B0A"),
            DisplayCode = "mg",
            InternalCode = "mg",
            Description = "Miligram",
            UseDosageForm = false,
            SortOrder = 50,
            IsActive = true,
            BaseUnitOfMeasureKey = null,
            Conversion = null
        };

        private UnitOfMeasure _mililiterUom = new UnitOfMeasure
        {
            Key = Guid.Parse("C2F1389D-7AA1-DF11-BA1C-0024818F4B0A"),
            DisplayCode = "mL",
            InternalCode = "mL",
            Description = "Mililiter",
            UseDosageForm = false,
            SortOrder = 140,
            IsActive = true,
            BaseUnitOfMeasureKey = null,
            Conversion = null
        };

        private UnitOfMeasure _literUom = new UnitOfMeasure
        {
            Key = Guid.Parse("BAF1389D-7AA1-DF11-BA1C-0024818F4B0A"),
            DisplayCode = "L",
            InternalCode = "L",
            Description = "Liter",
            UseDosageForm = false,
            SortOrder = 140,
            IsActive = true,
            BaseUnitOfMeasureKey = Guid.Parse("C2F1389D-7AA1-DF11-BA1C-0024818F4B0A"), // mlililiter is the base
            Conversion = 1000
        };

        #endregion

        #region Strength Ratio Tests

        [Test]
        public void StrengthRatioStrengthOnlyTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500, // Strength
                null, // ConcentrationVolume
                null, // ConcentrationVolumUnitOfMeasure
                null, // TotalVolumeAmount
                null); // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(500, ratio);
        }

        [Test]
        public void StrengthRatioAllFieldsTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500,            // Strength
                2,              // ConcentrationVolume
                _mililiterUom,  // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                _literUom);     // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(750000, ratio);
        }

        [Test]
        public void StrengthRatioStrengthAndConcentrationVolumeOnlyTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500,            // Strength
                2,              // ConcentrationVolume
                _mililiterUom,  // ConcentrationVolumUnitOfMeasure
                null,           // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(500, ratio);
        }

        [Test]
        public void StrengthRatioStrengthAndTotalVolumeOnlyTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500,            // Strength
                null,           // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                _literUom);     // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(500, ratio);
        }

        [Test]
        public void StrengthRatioStrengthAndMissingUnitOfMeasuresTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500,            // Strength
                2,              // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(500, ratio);
        }

        [Test]
        public void StrengthRatioStrengthOnlyAndMissingUnitOfMeasuresTest()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                500,            // Strength
                null,           // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                null,           // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(500, ratio);
        }

        [Test]
        public void StrengthRatioAllFieldsMissing()
        {
            decimal? ratio = MedItem.GetStrengthAmount(
                null,           // Strength
                null,           // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                null,           // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        #endregion

        #region Volume Ratio Tests

        [Test]
        public void VolumeRatioAllFieldsMissingTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                null, // ConcentrationVolume
                null, // ConcentrationVolumUnitOfMeasure
                null, // TotalVolumeAmount
                null); // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        [Test]
        public void VolumeRatioAllFieldsTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                2,              // ConcentrationVolume
                _mililiterUom,  // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                _literUom);     // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(3000, ratio);
        }

        [Test]
        public void VolumeRatioConcentrationVolumeOnlyTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                2,              // ConcentrationVolume
                _mililiterUom,  // ConcentrationVolumUnitOfMeasure
                null,           // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(2, ratio);
        }

        [Test]
        public void VolumeRatioTotalVolumeOnlyTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                null,           // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                _literUom);     // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(3, ratio);
        }

        [Test]
        public void VolumeRatioMissingUnitOfMeasuresTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                2,              // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        [Test]
        public void VolumeRatioTotalVolumeAndMissingUnitOfMeasuresTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                null,              // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        [Test]
        public void VolumeRatioTotalVolumeUnitOfMeasureOnlyTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                null,              // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                null,              // TotalVolumeAmount
                _literUom);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        [Test]
        public void VolumeRatioConcentrationVolumeAndMissingUnitOfMeasureTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                2,              // ConcentrationVolume
                null,           // ConcentrationVolumUnitOfMeasure
                null,              // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        [Test]
        public void VolumeRatioConcentrationVolumeAndTotalVolumeAndMissingUnitOfMeasureTest()
        {
            decimal? ratio = MedItem.GetVolumeAmount(
                2,              // ConcentrationVolume
                _mililiterUom,           // ConcentrationVolumUnitOfMeasure
                3,              // TotalVolumeAmount
                null);          // TotalVolumeAmountUnitOfMeasure

            Assert.AreEqual(null, ratio);
        }

        #endregion
    }
}
