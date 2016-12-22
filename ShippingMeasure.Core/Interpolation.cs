using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core
{
    public static class Interpolation
    {
        /// <summary>
        /// returns a density of standard value if the value exists in source list.
        /// if value not found, returns an interpolation value.
        /// </summary>
        /// <returns>a density of standard value in source list, or an interpolation one.</returns>
        public static decimal GetValue(this IEnumerable<StandardDensity> source, decimal temp, decimal densityMeasured)
        {
            var item = source.FirstOrDefault(d => d.Temp == temp && d.DensityMeasured == densityMeasured);
            if (item != null)
            {
                return item.DensityOfStandard;
            }

            #region record not found, interpolate a value

            var lowerTLowerDmItems = source.Where(d => d.Temp <= temp && d.DensityMeasured <= densityMeasured)
                .OrderByDescending(d => d.Temp).ThenByDescending(d => d.DensityMeasured).ToList();
            var lowerTUpperDmItems = source.Where(d => d.Temp <= temp && d.DensityMeasured >= densityMeasured)
                .OrderByDescending(d => d.Temp).ThenBy(d => d.DensityMeasured).ToList();
            var upperTLowerDmItems = source.Where(d => d.Temp >= temp && d.DensityMeasured <= densityMeasured)
                .OrderBy(d => d.Temp).ThenByDescending(d => d.DensityMeasured).ToList();
            var upperTUpperDmItems = source.Where(d => d.Temp >= temp && d.DensityMeasured >= densityMeasured)
                .OrderBy(d => d.Temp).ThenBy(d => d.DensityMeasured).ToList();

            StandardDensity lowerTLowerDm = null;
            StandardDensity lowerTUpperDm = null;
            StandardDensity upperTLowerDm = null;
            StandardDensity upperTUpperDm = null;

            if (lowerTLowerDmItems.Any() && !lowerTUpperDmItems.Any() && !upperTLowerDmItems.Any() && upperTUpperDmItems.Any())
            {
                lowerTLowerDm = lowerTLowerDmItems[0];
                upperTUpperDm = upperTUpperDmItems[0];

                decimal valueBetweenLowerAndUpper =  Interpolate(
                    new[]
                    {
                        new Tuple<decimal, decimal, decimal>(lowerTLowerDm.Temp, temp, upperTUpperDm.Temp),
                        new Tuple<decimal, decimal, decimal>(lowerTLowerDm.DensityMeasured, densityMeasured, upperTUpperDm.DensityMeasured)
                    },
                    lowerTLowerDm.DensityOfStandard, upperTUpperDm.DensityOfStandard);

                return valueBetweenLowerAndUpper;
            }

            for (int i = 0, count = lowerTLowerDmItems.Count; i < count; i++)
            {
                lowerTLowerDm = lowerTLowerDmItems[i];
                lowerTUpperDm = lowerTUpperDmItems.FirstOrDefault(d => d.Temp.Equals(lowerTLowerDm.Temp));
                upperTLowerDm = upperTLowerDmItems.FirstOrDefault(d => d.DensityMeasured.Equals(lowerTLowerDm.DensityMeasured));
                upperTUpperDm = upperTUpperDmItems.FirstOrDefault(d => d.Temp.Equals(upperTLowerDm.Temp) && d.DensityMeasured.Equals(lowerTUpperDm.DensityMeasured));

                if (lowerTUpperDm != null && upperTLowerDm != null && upperTUpperDm != null)
                {
                    break;
                }
            }

            if (lowerTLowerDm == null || lowerTUpperDm == null || upperTLowerDm == null || upperTUpperDm == null)
            {
                throw new Exception(String.Format("StandardDensity record not found. Temperature = {0}, DensityMeasured = {1}", temp, densityMeasured));
            }

            decimal inputTLowerDm = Interpolate(temp, lowerTLowerDm.Temp, upperTLowerDm.Temp, lowerTLowerDm.DensityOfStandard, upperTLowerDm.DensityOfStandard);
            decimal inputTUpperDm = Interpolate(temp, lowerTUpperDm.Temp, upperTUpperDm.Temp, lowerTUpperDm.DensityOfStandard, upperTUpperDm.DensityOfStandard);
            decimal inputTInputDm = Interpolate(densityMeasured, lowerTLowerDm.DensityMeasured, lowerTUpperDm.DensityMeasured, inputTLowerDm, inputTUpperDm);

            return inputTInputDm;

            #endregion
        }

        /// <summary>
        /// returns a VCF value if the value exists in source list.
        /// if value not found, returns an interpolation value.
        /// </summary>
        /// <returns>a VCF value in source list, or an interpolation one.</returns>
        public static decimal GetValue(this IEnumerable<VolumeCorrectionFactor> source, decimal temp, decimal densityOfStandard)
        {
            var item = source.FirstOrDefault(v => v.Temp == temp && v.DensityOfStandard == densityOfStandard);
            if (item != null)
            {
                return item.Factor;
            }

            #region record not found, interpolate a value

            var lowerTLowerDsItems = source
                .Where(f => f.Temp <= temp && f.DensityOfStandard <= densityOfStandard)
                .OrderByDescending(f => f.Temp)
                .ThenByDescending(f => f.DensityOfStandard)
                .ToList();
            var lowerTUpperDsItems = source
                .Where(f => f.Temp <= temp && f.DensityOfStandard >= densityOfStandard)
                .OrderByDescending(f => f.Temp)
                .ThenBy(f => f.DensityOfStandard)
                .ToList();
            var upperTLowerDsItems = source
                .Where(f => f.Temp >= temp && f.DensityOfStandard <= densityOfStandard)
                .OrderBy(f => f.Temp)
                .ThenByDescending(f => f.DensityOfStandard)
                .ToList();
            var upperTUpperDsItems = source
                .Where(f => f.Temp >= temp && f.DensityOfStandard >= densityOfStandard)
                .OrderBy(f => f.Temp)
                .ThenBy(f => f.DensityOfStandard)
                .ToList();

            VolumeCorrectionFactor lowerTLowerDs = null;
            VolumeCorrectionFactor lowerTUpperDs = null;
            VolumeCorrectionFactor upperTLowerDs = null;
            VolumeCorrectionFactor upperTUpperDs = null;

            if (lowerTLowerDsItems.Any() && !lowerTUpperDsItems.Any() && !upperTLowerDsItems.Any() && upperTUpperDsItems.Any())
            {
                lowerTLowerDs = lowerTLowerDsItems[0];
                upperTUpperDs = upperTUpperDsItems[0];

                decimal valueBetweenLowerAndUpper = Interpolate(
                    new[]
                    {
                        new Tuple<decimal, decimal, decimal>(lowerTLowerDs.Temp, temp, upperTUpperDs.Temp),
                        new Tuple<decimal, decimal, decimal>(lowerTLowerDs.DensityOfStandard, densityOfStandard, upperTUpperDs.DensityOfStandard)
                    },
                    lowerTLowerDs.Factor, upperTUpperDs.Factor);

                return valueBetweenLowerAndUpper;
            }

            for (int i = 0, count = lowerTLowerDsItems.Count; i < count; i++)
            {
                lowerTLowerDs = lowerTLowerDsItems[i];
                lowerTUpperDs = lowerTUpperDsItems.FirstOrDefault(f => f.Temp.Equals(lowerTLowerDs.Temp));
                upperTLowerDs = upperTLowerDsItems.FirstOrDefault(d => d.DensityOfStandard.Equals(lowerTLowerDs.DensityOfStandard));
                upperTUpperDs = upperTUpperDsItems.FirstOrDefault(d => d.Temp.Equals(upperTLowerDs.Temp) && d.DensityOfStandard.Equals(lowerTUpperDs.DensityOfStandard));

                if (lowerTLowerDs != null && lowerTUpperDs != null && upperTLowerDs != null && upperTUpperDs != null)
                {
                    break;
                }
            }

            if (lowerTLowerDs == null || lowerTUpperDs == null || upperTLowerDs == null || upperTUpperDs == null)
            {
                throw new Exception(String.Format("VCF record not found. Temperature = {0}, DensityOfStandard = {1}", temp, densityOfStandard));
            }

            decimal inputTLowerDs = Interpolate(temp, lowerTLowerDs.Temp, upperTLowerDs.Temp, lowerTLowerDs.Factor, upperTLowerDs.Factor);
            decimal inputTUpperDs = Interpolate(temp, lowerTUpperDs.Temp, upperTUpperDs.Temp, lowerTUpperDs.Factor, upperTUpperDs.Factor);
            decimal inputTInputDs = Interpolate(densityOfStandard, lowerTLowerDs.DensityOfStandard, lowerTUpperDs.DensityOfStandard, inputTLowerDs, inputTUpperDs);

            return inputTInputDs;

            #endregion
        }

        /// <summary>
        /// returns a volume value if the value exists in source list.
        /// if value not found, returns an interpolation value.
        /// </summary>
        /// <returns>a volume value in source list, or an interpolation one.</returns>
        public static decimal GetValue(this IEnumerable<OilVolume> source, string tankName, decimal hInclination, decimal vInclination, decimal height)
        {
            var item = source.FirstOrDefault(v => v.TankName.Equals(tankName)
                && v.HInclination == hInclination
                && v.VInclination == vInclination
                && v.Height == height);
            if (item != null)
            {
                return item.Volume;
            }

            #region record not found, interpolate a value

            var lowerHiLowerViLowerHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination <= hInclination && v.VInclination <= vInclination && v.Height <= height)
                .OrderByDescending(v => v.HInclination).ThenByDescending(v => v.VInclination).ThenByDescending(v => v.Height).Take(100).ToList();
            var lowerHiLowerViUpperHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination <= hInclination && v.VInclination <= vInclination && v.Height >= height)
                .OrderByDescending(v => v.HInclination).ThenByDescending(v => v.VInclination).ThenBy(v => v.Height).Take(100).ToList();
            var lowerHiUpperViLowerHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination <= hInclination && v.VInclination >= vInclination && v.Height <= height)
                .OrderByDescending(v => v.HInclination).ThenBy(v => v.VInclination).ThenByDescending(v => v.Height).Take(100).ToList();
            var lowerHiUpperViUpperHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination <= hInclination && v.VInclination >= vInclination && v.Height >= height)
                .OrderByDescending(v => v.HInclination).ThenBy(v => v.VInclination).ThenBy(v => v.Height).Take(100).ToList();
            var upperHiLowerViLowerHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination >= hInclination && v.VInclination <= vInclination && v.Height <= height)
                .OrderBy(v => v.HInclination).ThenByDescending(v => v.VInclination).ThenByDescending(v => v.Height).Take(100).ToList();
            var upperHiLowerViUpperHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination >= hInclination && v.VInclination <= vInclination && v.Height >= height)
                .OrderBy(v => v.HInclination).ThenByDescending(v => v.VInclination).ThenBy(v => v.Height).Take(100).ToList();
            var upperHiUpperViLowerHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination >= hInclination && v.VInclination >= vInclination && v.Height <= height)
                .OrderBy(v => v.HInclination).ThenBy(v => v.VInclination).ThenByDescending(v => v.Height).Take(100).ToList();
            var upperHiUpperViUpperHItems = source.Where(v => v.TankName.Equals(tankName) && v.HInclination >= hInclination && v.VInclination >= vInclination && v.Height >= height)
                .OrderBy(v => v.HInclination).ThenBy(v => v.VInclination).ThenBy(v => v.Height).Take(100).ToList();

            OilVolume lowerHiLowerViLowerH = null;
            OilVolume lowerHiLowerViUpperH = null;
            OilVolume lowerHiUpperViLowerH = null;
            OilVolume lowerHiUpperViUpperH = null;
            OilVolume upperHiLowerViLowerH = null;
            OilVolume upperHiLowerViUpperH = null;
            OilVolume upperHiUpperViLowerH = null;
            OilVolume upperHiUpperViUpperH = null;

            if (
                lowerHiLowerViLowerHItems.Any() && lowerHiLowerViLowerHItems.Any()
                &&
                    (
                        !lowerHiLowerViUpperHItems.Any()
                        || !lowerHiUpperViLowerHItems.Any()
                        || !lowerHiUpperViUpperHItems.Any()
                        || !upperHiLowerViLowerHItems.Any()
                        || !upperHiLowerViUpperHItems.Any()
                        || !upperHiUpperViLowerHItems.Any()
                    )
                )
            {
                lowerHiLowerViLowerH = lowerHiLowerViLowerHItems[0];
                upperHiUpperViUpperH = upperHiUpperViUpperHItems[0];

                decimal valueBetweenLowerAndUpper = Interpolate(
                    new[]
                    {
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerViLowerH.HInclination, hInclination, upperHiUpperViUpperH.HInclination),
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerViLowerH.VInclination, vInclination, upperHiUpperViUpperH.VInclination),
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerViLowerH.Height, height, upperHiUpperViUpperH.Height)
                    },
                    lowerHiLowerViLowerH.Volume, upperHiUpperViUpperH.Volume);

                return valueBetweenLowerAndUpper;
            }

            for (int i = 0, count = lowerHiLowerViLowerHItems.Count; i < count; i++)
            {
                lowerHiLowerViLowerH = lowerHiLowerViLowerHItems[i];
                lowerHiLowerViUpperH = lowerHiLowerViUpperHItems.FirstOrDefault(v => v.HInclination.Equals(lowerHiLowerViLowerH.HInclination) && v.VInclination.Equals(lowerHiLowerViLowerH.VInclination));
                lowerHiUpperViLowerH = lowerHiUpperViLowerHItems.FirstOrDefault(v => v.HInclination.Equals(lowerHiLowerViLowerH.HInclination) && v.Height.Equals(lowerHiLowerViLowerH.Height));
                lowerHiUpperViUpperH = lowerHiUpperViUpperHItems.FirstOrDefault(v => v.HInclination.Equals(lowerHiLowerViLowerH.HInclination) && v.VInclination.Equals(lowerHiUpperViLowerH.VInclination) && v.Height.Equals(lowerHiLowerViUpperH.Height));
                upperHiLowerViLowerH = upperHiLowerViLowerHItems.FirstOrDefault(v => v.VInclination.Equals(lowerHiLowerViLowerH.VInclination) && v.Height.Equals(lowerHiLowerViLowerH.Height));
                upperHiLowerViUpperH = upperHiLowerViUpperHItems.FirstOrDefault(v => v.HInclination.Equals(upperHiLowerViLowerH.HInclination) && v.VInclination.Equals(lowerHiLowerViUpperH.VInclination) && v.Height.Equals(lowerHiLowerViUpperH.Height));
                upperHiUpperViLowerH = upperHiUpperViLowerHItems.FirstOrDefault(v => v.HInclination.Equals(upperHiLowerViLowerH.HInclination) && v.VInclination.Equals(lowerHiUpperViLowerH.VInclination) && v.Height.Equals(lowerHiUpperViLowerH.Height));
                upperHiUpperViUpperH = upperHiUpperViUpperHItems.FirstOrDefault(v => v.HInclination.Equals(upperHiLowerViLowerH.HInclination) && v.VInclination.Equals(lowerHiUpperViUpperH.VInclination) && v.Height.Equals(lowerHiUpperViUpperH.Height));

                if (lowerHiLowerViUpperH != null && lowerHiUpperViLowerH != null && lowerHiUpperViUpperH != null && upperHiLowerViLowerH != null && upperHiLowerViUpperH != null && upperHiUpperViLowerH != null && upperHiUpperViUpperH != null)
                {
                    break;
                }
            }

            if (lowerHiLowerViLowerH == null || lowerHiLowerViUpperH == null || lowerHiUpperViLowerH == null || lowerHiUpperViUpperH == null
                || upperHiLowerViLowerH == null || upperHiLowerViUpperH == null || upperHiUpperViLowerH == null || upperHiUpperViUpperH == null)
            {
                throw new Exception(String.Format("OilVolume record not found. TankName = {0}, HInclination = {1}, VInclination = {2}, Height = {3}", tankName, hInclination, vInclination, height));
            }

            decimal inputHiLowerViLowerH = Interpolate(hInclination, lowerHiLowerViLowerH.HInclination, upperHiLowerViLowerH.HInclination, lowerHiLowerViLowerH.Volume, upperHiLowerViLowerH.Volume);
            decimal inputHiLowerViUpperH = Interpolate(hInclination, lowerHiLowerViUpperH.HInclination, upperHiLowerViUpperH.HInclination, lowerHiLowerViUpperH.Volume, upperHiLowerViUpperH.Volume);
            decimal inputHiUpperViLowerH = Interpolate(hInclination, lowerHiUpperViLowerH.HInclination, upperHiUpperViLowerH.HInclination, lowerHiUpperViLowerH.Volume, upperHiUpperViLowerH.Volume);
            decimal inputHiUpperViUpperH = Interpolate(hInclination, lowerHiUpperViUpperH.HInclination, upperHiUpperViUpperH.HInclination, lowerHiUpperViUpperH.Volume, upperHiUpperViUpperH.Volume);

            decimal inputHiInputViLowerH = Interpolate(vInclination, lowerHiLowerViLowerH.VInclination, lowerHiUpperViLowerH.VInclination, inputHiLowerViLowerH, inputHiUpperViLowerH);
            decimal inputHiInputViUpperH = Interpolate(vInclination, lowerHiLowerViLowerH.VInclination, lowerHiUpperViLowerH.VInclination, inputHiLowerViUpperH, inputHiUpperViUpperH);

            decimal inputHiInputViInputH = Interpolate(height, lowerHiLowerViLowerH.Height, lowerHiLowerViUpperH.Height, inputHiInputViLowerH, inputHiInputViUpperH);

            return inputHiInputViInputH;

            #endregion
        }

        public static decimal GetValue(this IEnumerable<ListingHeightCorrection> source, string tankName, decimal height, decimal hInclination)
        {
            var item = source.FirstOrDefault(c => c.Height == height
                && c.HInclination == hInclination
                && c.TankName.Equals(tankName));
            if (item != null)
            {
                return item.Correction;
            }

            #region record not found, interpolate a value

            var lowerHiLowerHItems = source
                .Where(c => c.Height <= height && c.HInclination <= hInclination)
                .OrderByDescending(c => c.Height)
                .ThenByDescending(c => c.HInclination);
            var lowerHiUpperHItems = source
                .Where(c => c.Height <= height && c.HInclination >= hInclination)
                .OrderByDescending(c => c.Height)
                .ThenBy(c => c.HInclination);
            var upperHiLowerHItems = source
                .Where(c => c.Height >= height && c.HInclination <= hInclination)
                .OrderBy(c => c.Height)
                .ThenByDescending(c => c.HInclination);
            var upperHiUpperHItems = source
                .Where(c => c.Height >= height && c.HInclination >= hInclination)
                .OrderBy(c => c.Height)
                .ThenBy(c => c.Height);

            ListingHeightCorrection lowerHiLowerH = null;
            ListingHeightCorrection lowerHiUpperH = null;
            ListingHeightCorrection upperHiLowerH = null;
            ListingHeightCorrection upperHiUpperH = null;

            if (lowerHiLowerHItems.Any() && !lowerHiUpperHItems.Any() && !upperHiLowerHItems.Any() && upperHiUpperHItems.Any())
            {
                lowerHiLowerH = lowerHiLowerHItems.First();
                upperHiUpperH = upperHiUpperHItems.First();

                var valueBetweenLowerAndUpper = Interpolate(
                    new[]
                    {
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerH.Height, height, upperHiUpperH.Height),
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerH.HInclination, hInclination, upperHiUpperH.HInclination)
                    },
                    lowerHiLowerH.Correction, upperHiUpperH.Correction);

                return valueBetweenLowerAndUpper;
            }

            foreach (var i in lowerHiLowerHItems)
            {
                lowerHiLowerH = i;
                lowerHiUpperH = lowerHiUpperHItems.FirstOrDefault(c => c.Height == lowerHiLowerH.Height);
                upperHiLowerH = upperHiLowerHItems.FirstOrDefault(c => c.HInclination == lowerHiLowerH.HInclination);
                upperHiUpperH = upperHiUpperHItems.FirstOrDefault(c => c.Height == upperHiLowerH.Height && c.HInclination == lowerHiUpperH.HInclination);

                if (lowerHiLowerH != null && lowerHiUpperH != null && upperHiLowerH != null && upperHiUpperH != null)
                {
                    break;
                }
            }

            if (lowerHiLowerH == null || lowerHiUpperH == null || upperHiLowerH == null || upperHiUpperH == null)
            {
                throw new Exception(String.Format("Listing height correction record not found. Height = {0}, HInclination = {1}", height, hInclination));
            }

            var inputHiLowerH = Interpolate(height, lowerHiLowerH.Height, upperHiLowerH.Height, lowerHiLowerH.Correction, upperHiLowerH.Correction);
            var inputHiUpperH = Interpolate(height, lowerHiUpperH.Height, upperHiUpperH.Height, lowerHiUpperH.Correction, upperHiUpperH.Correction);
            var inputHiInputH = Interpolate(hInclination, lowerHiLowerH.HInclination, lowerHiUpperH.HInclination, inputHiLowerH, inputHiUpperH);

            return inputHiInputH;

            #endregion
        }

        public static decimal GetValue(this IEnumerable<TrimmingHeightCorrection> source, string tankName, decimal height, decimal vInclination)
        {
            var item = source.FirstOrDefault(c => c.Height == height
                && c.VInclination == vInclination
                && c.TankName.Equals(tankName));
            if (item != null)
            {
                return item.Correction;
            }

            #region record not found, interpolate a value

            var lowerHiLowerVItems = source
                .Where(c => c.Height <= height && c.VInclination <= vInclination)
                .OrderByDescending(c => c.Height)
                .ThenByDescending(c => c.VInclination);
            var lowerHiUpperVItems = source
                .Where(c => c.Height <= height && c.VInclination >= vInclination)
                .OrderByDescending(c => c.Height)
                .ThenBy(c => c.VInclination);
            var upperHiLowerVItems = source
                .Where(c => c.Height >= height && c.VInclination <= vInclination)
                .OrderBy(c => c.Height)
                .ThenByDescending(c => c.VInclination);
            var upperHiUpperVItems = source
                .Where(c => c.Height >= height && c.VInclination >= vInclination)
                .OrderBy(c => c.Height)
                .ThenBy(c => c.Height);

            TrimmingHeightCorrection lowerHiLowerV = null;
            TrimmingHeightCorrection lowerHiUpperV = null;
            TrimmingHeightCorrection upperHiLowerV = null;
            TrimmingHeightCorrection upperHiUpperV = null;

            if (lowerHiLowerVItems.Any() && !lowerHiUpperVItems.Any() && !upperHiLowerVItems.Any() && upperHiUpperVItems.Any())
            {
                lowerHiLowerV = lowerHiLowerVItems.First();
                upperHiUpperV = upperHiUpperVItems.First();

                var valueBetweenLowerAndUpper = Interpolate(
                    new[]
                    {
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerV.Height, height, upperHiUpperV.Height),
                        new Tuple<decimal, decimal, decimal>(lowerHiLowerV.VInclination, vInclination, upperHiUpperV.VInclination)
                    },
                    lowerHiLowerV.Correction, upperHiUpperV.Correction);

                return valueBetweenLowerAndUpper;
            }

            foreach (var i in lowerHiLowerVItems)
            {
                lowerHiLowerV = i;
                lowerHiUpperV = lowerHiUpperVItems.FirstOrDefault(c => c.Height == lowerHiLowerV.Height);
                upperHiLowerV = upperHiLowerVItems.FirstOrDefault(c => c.VInclination == lowerHiLowerV.VInclination);
                upperHiUpperV = upperHiUpperVItems.FirstOrDefault(c => c.Height == upperHiLowerV.Height && c.VInclination == lowerHiUpperV.VInclination);

                if (lowerHiLowerV != null && lowerHiUpperV != null && upperHiLowerV != null && upperHiUpperV != null)
                {
                    break;
                }
            }

            if (lowerHiLowerV == null || lowerHiUpperV == null || upperHiLowerV == null || upperHiUpperV == null)
            {
                throw new Exception(String.Format("Trimming height correction record not found. Height = {0}, VInclination = {1}", height, vInclination));
            }

            var inputHiLowerV = Interpolate(height, lowerHiLowerV.Height, upperHiLowerV.Height, lowerHiLowerV.Correction, upperHiLowerV.Correction);
            var inputHiUpperV = Interpolate(height, lowerHiUpperV.Height, upperHiUpperV.Height, lowerHiUpperV.Correction, upperHiUpperV.Correction);
            var inputHiInputV = Interpolate(vInclination, lowerHiLowerV.VInclination, lowerHiUpperV.VInclination, inputHiLowerV, inputHiUpperV);

            return inputHiInputV;

            #endregion
        }

        public static decimal GetValue(this IEnumerable<Volume> source, string tankName, decimal height)
        {
            var item = source.FirstOrDefault(v => v.Height == height
                && v.TankName.Equals(tankName));
            if (item != null)
            {
                return item.Value;
            }

            #region record not found, interpolate a value

            var lowerHi = source
                .Where(v => v.Height <= height)
                .OrderByDescending(v => v.Height)
                .FirstOrDefault();
            if (lowerHi == null)
            {
                throw new Exception(String.Format("Volume record not found. Height = {0}", height));
            }

            var upperHi = source
                .Where(v => v.Height >= height)
                .OrderBy(v => v.Height)
                .FirstOrDefault();

            if (upperHi == null)
            {
                throw new Exception(String.Format("Volume record not found. Height = {0}", height));
            }

            return Interpolate(height, lowerHi.Height, upperHi.Height, lowerHi.Value, upperHi.Value);

            #endregion
        }

        /// <summary>
        /// returns a value between lowerValue and upperValues
        /// </summary>
        private static decimal Interpolate(decimal input, decimal lower, decimal upper, decimal lowerValue, decimal upperValue)
        {
            return
                ((upper == lower) ? 0m : (input - lower) / (upper - lower) * (upperValue - lowerValue))
                + lowerValue;
        }

        /// <summary>
        /// returns a value between lowerValue and upperValues
        /// </summary>
        private static decimal Interpolate(Tuple<decimal, decimal, decimal>[] tuples, decimal lowerValue, decimal upperValue)
        {
            /// each tuple item contains three values, they are Item1 is lower, Item2 is input, Item3 is upper.
            /// get the average position of each Item2 between each Item1 and Item3, name the average value as Weight.
            /// the returning value is what we want, it is the position between lowerValue and upperValue, calculated with Weight.

            decimal totalPos = tuples.Aggregate(0m, (posTotal, t) => posTotal + ((t.Item3 == t.Item1) ? 0.5m : (t.Item2 - t.Item1) / (t.Item3 - t.Item1)));
            decimal posWeight = totalPos / tuples.Length;
            decimal valueBetweenLowerValueAndUpperValue = lowerValue + (upperValue - lowerValue) * posWeight;

            return valueBetweenLowerValueAndUpperValue;
        }


    }
}
