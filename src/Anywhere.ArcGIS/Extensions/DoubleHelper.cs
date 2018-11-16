using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace Anywhere.ArcGIS.Extensions
{
    public static class DoubleHelper
    {
        private const double DoubleEpsilon = 2.2204460492503131e-016;
        private const double SingleEpsilon = 1.192092896e-07f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Or(this float value, float defaultValue) => value.IsInfinityOrNaN() || value.IsZero() ? defaultValue : value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Or(this double value, double defaultValue) => value.IsInfinityOrNaN() || value.IsZero() ? defaultValue : value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Or(this float? value, float defaultValue) => !value.HasValue || value.Value.IsInfinityOrNaN() || value.Value.IsZero() ? defaultValue : value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Or(this double? value, double defaultValue) => !value.HasValue || value.Value.IsInfinityOrNaN() || value.Value.IsZero() ? defaultValue : value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this float a, double b) => AreClose(a, (float)b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double a, float b) => AreClose((float)a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double? a, double? b)
        {
            return a == b || (a.HasValue && b.HasValue && AreClose(a.Value, b.Value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double? a, double b)
        {
            return a == b || (a.HasValue && AreClose(a.Value, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double a, double? b)
        {
            return a == b || (b.HasValue && AreClose(a, b.Value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? Diff(this double current, double original)
        {
            return current.AreClose(original) ? (double?)null : current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? Diff(this double? current, double? original)
        {
            return current.AreClose(original) ? null : current;
        }

        /// <summary>
        /// AreClose - Returns whether or not two floats are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="a"> The first floats to compare. </param>
        /// <param name="b"> The second floats to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this float a, float b)
        {
            //in case they are Infinities (then epsilon check does not work)
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < SingleEpsilon
            double eps = (Math.Abs(a) + Math.Abs(b) + 10.0) * SingleEpsilon;
            double delta = a - b;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="a"> The first double to compare. </param>
        /// <param name="b"> The second double to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double a, double b)
        {
            //in case they are Infinities (then epsilon check does not work)
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DblEpsilon
            double eps = (Math.Abs(a) + Math.Abs(b) + 10.0) * DoubleEpsilon;
            double delta = a - b;
            return (-eps < delta) && (eps > delta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreClose(this double a, double b, double deviation) => !a.IsInfinityOrNaN() && b * (1d - deviation) < a && a < b * (1d + deviation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreCloseAbs(this double a, double b, double epsilon) => !a.IsInfinityOrNaN() && Math.Abs(a-b) <= epsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreCloseAsSingle(this double a, double b) => AreClose((float)a, (float)b);

        public static string GetNumericFormat(this decimal value)
        {
            string str = "#";
            var num = Math.Abs(value);
            if (num > 999999)
            {
                return "0.00E+00";
            }

            if (num > 999)
            {
                return "#####0";
            }

            if (num > 99.9m)
            {
                return "####0.0";
            }

            if (num > 9.99m)
            {
                return "#0.0#";
            }

            if (num > 0.000999m)
            {
                return "0.0##";
            }

            if (num > 1E-35m)
            {
                return "0.00E+00";
            }

            if (num <= 1E-35m)
            {
                return "0";
            }

            return str;
        }

        public static string GetNumericFormat(this double value)
        {
            string str = "#";
            double num = Math.Abs(value);
            if (num > 999999)
            {
                return "0.00E+00";
            }

            if (num > 999)
            {
                return "#####0";
            }

            if (num > 99.9)
            {
                return "####0.0";
            }

            if (num > 9.99)
            {
                return "#0.0#";
            }

            if (num > 0.000999)
            {
                return "0.0##";
            }

            if (num > 1E-35)
            {
                return "0.00E+00";
            }

            if (num <= 1E-35)
            {
                return "0";
            }

            return str;
        }

        /// <summary>
        /// GreaterThan - Returns whether or not the first double is greater than the second double.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThan(this double value1, double value2) => (value1 > value2) && !AreClose(value1, value2);

        /// <summary>
        /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
        /// the second double.  That is, whether or not the first is strictly greater than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThanOrClose(this double value1, double value2) => (value1 > value2) || AreClose(value1, value2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this double value, double min, double max) => !value.IsInfinityOrNaN() && min < value && value < max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeOrMax(this double value, double min, double max) => !value.IsInfinityOrNaN() && min < value && value <= max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeOrMin(this double value, double min, double max) => !value.IsInfinityOrNaN() && min <= value && value < max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeOrMinMax(this double value, double min, double max) => !value.IsInfinityOrNaN() && min <= value && value <= max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRangeOrMinMax(this double value, double? min, double? max) => !value.IsInfinityOrNaN() && (!min.HasValue || min <= value) && (!max.HasValue || value <= max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBetweenZeroAndOne(double val) => GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinityOrNaN(this double value) => double.IsInfinity(value) || double.IsNaN(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinityOrNaN(this float value) => float.IsInfinity(value) || float.IsNaN(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeNumber(this double value) => value < 0d && !double.IsNegativeInfinity(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this double value) => !double.IsInfinity(value) && !double.IsNaN(value);

        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOne(this double value) => Math.Abs(value - 1.0) < 10.0 * DoubleEpsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveNumber(this double value) => 0d < value && !double.IsPositiveInfinity(value);

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this double value) => Math.Abs(value) < (10.0 * DoubleEpsilon);

        /// <summary>
        /// IsZero - Returns whether or not the single is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The single to compare to 0. </param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float value) => Math.Abs(value) < (10.0 * SingleEpsilon);

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZeroOrNull(this double? value) => !value.HasValue || Math.Abs(value.Value) < (10.0 * DoubleEpsilon);

        /// <summary>
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThan(this double value1, double value2) => (value1 < value2) && !AreClose(value1, value2);

        /// <summary>
        /// LessThanOrClose - Returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThanOrClose(this double value1, double value2) => (value1 < value2) || AreClose(value1, value2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OutOfRange(this double value, double min, double max) => value.IsInfinityOrNaN() || value < min || max < value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OutOfRangeOrMax(this double value, double min, double max) => value.IsInfinityOrNaN() || value < min || max <= value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OutOfRangeOrMin(this double value, double min, double max) => value.IsInfinityOrNaN() || value <= min || max < value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OutOfRangeOrMinMax(this double value, double min, double max) => value.IsInfinityOrNaN() || value <= min || max <= value;

        public static string ToDisplayString(this float value) => value.ToString(GetNumericFormat(value)).Trim();

        public static string ToDisplayString(this double value) => value.ToString(GetNumericFormat(value)).Trim();

        public static string ToDisplayString(this double value, bool twoMoreDigits) => value.ToString(GetNumericFormat(value, twoMoreDigits)).Trim();

        public static string ToStringAdaptive(this TimeSpan span)
        {
            var s = new StringBuilder();
            var abs = span.Duration();
            if (abs.Days > 0)
            {
                s.Append(span.Days.ToString("0"));
            }
            if (abs.Hours > 0)
            {
                s.Append(span.Hours.ToString("00"));
            }
            if (abs.Minutes > 0)
            {
                s.Append(span.Minutes.ToString("00"));
            }
            if (abs.Seconds > 0)
            {
                s.Append(span.Seconds.ToString("00"));
            }

            return s.Length > 0 ? "0" : s.ToString();
        }

        private static string GetNumericFormat(double value, bool twoMoreDigits)
        {
            string str;
            if (!twoMoreDigits)
            {
                str = GetNumericFormat(value);
            }
            else
            {
                string str1 = "#";
                double num = Math.Abs(value);
                if (num <= 1E-35)
                {
                    str1 = "0";
                }
                if (num > 1E-35)
                {
                    str1 = "0.0000E+00";
                }
                if (num > 0.000999)
                {
                    str1 = "0.0####";
                }
                if (num > 9.99)
                {
                    str1 = "#0.0###";
                }
                if (num > 99.9)
                {
                    str1 = "#####0.##";
                }
                if (num > 999999)
                {
                    str1 = "0.0000E+00";
                }
                str = str1;
            }
            return str;
        }
    }
}
