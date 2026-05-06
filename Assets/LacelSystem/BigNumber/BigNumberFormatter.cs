using System;

namespace LacelSDK
{
    public static class BigNumberFormatter
    {
        // List of suffixes
        private static readonly string[] Suffixes =
        {
        "", "k", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd"
    };

        public static string Format(double value)
        {
            if (double.IsInfinity(value)) return "Infinity";
            if (value == 0) return "0";

            bool isNegative = value < 0;
            value = Math.Abs(value);

            if (value < 1000)
                return (isNegative ? "-" : "") + value.ToString("F1").TrimEnd('0').TrimEnd('.');

            int index = 0;
            while (value >= 1000 && index < Suffixes.Length - 1)
            {
                value /= 1000.0;
                index++;
            }

            // Trả về số với 2 chữ số thập phân (ví dụ: 1.25B)
            string formatted = value.ToString("F2").TrimEnd('0').TrimEnd('.');
            return (isNegative ? "-" : "") + formatted + Suffixes[index];
        }
    }
}
