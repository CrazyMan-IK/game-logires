using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrazyGames.Logires.Utils
{
    public static class GlobalPreferences
    {
        private static string _SettingsFile = "&EMPTY&";

        static GlobalPreferences()
        {
#if UNITY_EDITOR
            Initialize(Application.dataPath + "/../preferences.json");
#elif UNITY_ANDROID
            var path = AndroidNativeWrapper.CallMethod<string>("GetDataDir", null);
            if (path != null)
            {
                Initialize(path + "/preferences.json");
            }
#endif
        }

        public static void Initialize(string settingsFile = "")
        {
            if (string.IsNullOrEmpty(settingsFile))
            {
                settingsFile = "&EMPTY&";
            }

            _SettingsFile = settingsFile;
        }

        public static void Set<T>(string key, T value) where T : class
        {
            if (StaticUtils.IsPathValid(_SettingsFile, true))
            {
                JToken oldData;

                using (FileStream file = new FileStream(_SettingsFile, FileMode.OpenOrCreate, FileAccess.Read))
                using (StreamReader fileReader = new StreamReader(file))
                using (JsonReader jsonReader = new JsonTextReader(fileReader))
                {
                    try
                    {
                        oldData = JToken.ReadFrom(jsonReader);
                    }
                    catch
                    {
                        oldData = JToken.Parse("{}");
                    }
                }

                oldData[key] = new JValue(value);
                File.WriteAllText(_SettingsFile, String.Empty);

                using (FileStream file = new FileStream(_SettingsFile, FileMode.OpenOrCreate, FileAccess.Write))
                using (StreamWriter fileWriter = new StreamWriter(file))
                using (JsonWriter jsonWriter = new JsonTextWriter(fileWriter))
                {
                    oldData.WriteTo(jsonWriter);
                }
            }
            else
            {
                throw new ArgumentException("Invalid settings file path");
            }
        }

        public static void SetPrimitive<T>(string key, T value) where T : struct
        {
            Set(key, new ValueWrapper<T>(value));
        }

        public static object Get(string key)
        {
            return Get<object>(key, null);
        }

        public static T Get<T>(string key) where T : class
        {
            if (StaticUtils.IsPathValid(_SettingsFile, true))
            {
                using (FileStream file = new FileStream(_SettingsFile, FileMode.OpenOrCreate))
                using (StreamReader fileReader = new StreamReader(file))
                using (JsonReader jsonReader = new JsonTextReader(fileReader))
                {
                    JToken data;
                    try
                    {
                        data = JToken.ReadFrom(jsonReader);
                    }
                    catch
                    {
                        data = JToken.Parse("{}");
                    }
                    var value = data[key];

                    return value?.Value<T>();
                }
            }
            else
            {
                throw new ArgumentException("Invalid settings file path");
            }
        }

        public static T Get<T>(string key, T defaultValue) where T : class
        {
            var result = Get<T>(key);

            if (result != null)
            {
                return result;
            }

            return defaultValue;
        }

        public static ValueWrapper<T> GetPrimitive<T>(string key) where T : struct
        {
            return Get<ValueWrapper<T>>(key);
        }

        public static ValueWrapper<T> GetPrimitive<T>(string key, T defaultValue) where T : struct
        {
            return Get<ValueWrapper<T>>(key, defaultValue);
        }
    }
}