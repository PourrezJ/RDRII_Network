using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Shared
{

    public static class Extensions
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        public static int FromArgb(byte a, byte r, byte g, byte b)
        {
            return b | g << 8 | r << 16 | a << 24;
        }

        public static void ToArgb(int argb, out byte a, out byte r, out byte g, out byte b)
        {
            b = (byte)(argb & 0xFF);
            g = (byte)((argb & 0xFF00) >> 8);
            r = (byte)((argb & 0xFF0000) >> 16);
            a = (byte)((argb & 0xFF000000) >> 24);
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return default(TValue);
            }
        }

        public static int Get(this IDictionary<int, int> dict, int key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return -1;
            }
        }
    }
}