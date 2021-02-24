using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    public static class ConvertExtensions
    {
        public static List<bool> BoolToBits(this bool value)
        {
            return new List<bool>() { value };
        }
        public static int BoolToInt32(this bool value)
        {
            return value ? 1 : 0;
        }
        public static double BoolToDouble(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool BitsToBool(this List<bool> value)
        {
            return value.Any(x => x);
        }
        public static int BitsToInt32(this List<bool> value)
        {
            var bits = value.Select(x => x ? "1" : "0").Aggregate((x, y) => x + y).PadLeft(32, '0').Split(8).Select(x => Convert.ToByte(x, 2)).Reverse().ToArray();

            if (bits.Length > 0)
            {
                //Debug.Log(bits.Select(x => x.ToString()).Aggregate((x, y) => x + " | " + y));

                return BitConverter.ToInt32(bits, 0);
            }

            return 0;
        }
        public static double BitsToDouble(this List<bool> value)
        {
            var bits = value.Select(x => x ? "1" : "0").Aggregate((x, y) => x + y).PadLeft(64, '0').Split(8).Select(x => Convert.ToByte(x, 2)).Reverse().ToArray();

            if (bits.Length > 0)
            {
                return BitConverter.ToDouble(bits, 0);
            }

            return 0;
        }

        public static bool Int32ToBool(this int value)
        {
            return value != 0;
        }
        public static List<bool> Int32ToBits(this int value)
        {
            var bits = BitConverter.GetBytes(value).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Reverse().Aggregate((x, y) => x + y).Select(x => x != '0');

            return bits.ToList();
        }
        public static double Int32ToDouble(this int value)
        {
            return Convert.ToDouble(value);
        }

        public static bool DoubleToBool(this double value)
        {
            return value != 0;
        }
        public static List<bool> DoubleToBits(this double value)
        {
            var bits = BitConverter.GetBytes(value).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Reverse().Aggregate((x, y) => x + y).Select(x => x != '0');

            return bits.ToList();
        }
        public static int DoubleToInt32(this double value)
        {
            return Convert.ToInt32(value);
        }
    }
}