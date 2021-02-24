using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CrazyGames.Logires.Utils
{
    public static class StaticUtils
    {
        public static int Remap(this int value, int minFrom, int maxFrom, int minTo, int maxTo)
        {
            return (int)Remap((double)value, minFrom, maxFrom, minTo, maxTo);
        }
        public static float Remap(this float value, float minFrom, float maxFrom, float minTo, float maxTo)
        {
            return (float)Remap((double)value, minFrom, maxFrom, minTo, maxTo);
        }
        public static double Remap(this double value, double minFrom, double maxFrom, double minTo, double maxTo)
        {
            return (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;
        }

        public static IEnumerable<string> Split(this string value, int length)
        {
            if (value.Length < length)
            {
                return Enumerable.Repeat(value, 1);
            }
            return Enumerable.Range(0, value.Length / length).Select(x => value.Substring(x * length, length));
        }

        public static List<T> GetCopy<T>(this IEnumerable<T> enumerable)
        {
            return new List<T>(enumerable);
        }

        public static bool PointIsVisibleToCamera(this Vector2 point, Camera camera)
        {
            if (camera.WorldToViewportPoint(point).x < 0 ||
                camera.WorldToViewportPoint(point).x > 1 ||
                camera.WorldToViewportPoint(point).y > 1 ||
                camera.WorldToViewportPoint(point).y < 0)
            {
                return false;
            }

            return true;
        }

        public static void MoveTo2D(this Transform target, Vector2 position)
        {
            Vector3 newCameraPos = new Vector3(position.x, position.y, target.position.z);
            target.position = newCameraPos;
        }

        public static Vector3 Add(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y + value, vector.z + value);
        }

        public static Vector2 Add(this Vector2 vector, float value)
        {
            return ((Vector3)vector).Add(value);
        }

        public static Vector3 Subtract(this Vector3 vector, float value)
        {
            return vector.Add(-value);
        }

        public static Vector2 Subtract(this Vector2 vector, float value)
        {
            return vector.Add(-value);
        }

        public static IEnumerable<TSecond> CustomZip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TSecond> resultSelector)
        {
            return first.Reverse().Zip(second.Reverse(), resultSelector).Concat(second.Take(Math.Abs(first.Count() - second.Count())).Reverse()).Reverse();
        }

        public static List<RaycastResult> RaycastUI(this EventSystem eventSystem, PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);

            return results.Where(x => x.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();
        }

        public static bool IsPathValid(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }
    }
}