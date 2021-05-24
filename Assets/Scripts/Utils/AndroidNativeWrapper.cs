using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    public static class AndroidNativeWrapper
    {
        private static AndroidJavaObject _androidNativeObject = null;
        private static CustomEventsHandler handler = new CustomEventsHandler();

        public delegate void DoubleChangedHandler(double value);
        public delegate void BooleanChangedHandler(bool value);
        public static event DoubleChangedHandler OnVolumeChanged;
        public static event DoubleChangedHandler OnBatteryLevelChanged;
        public static event BooleanChangedHandler OnBatteryChargingStateChanged;

        private class CustomEventsHandler : AndroidJavaProxy
        {
            public CustomEventsHandler() : base("com.crazy_games.AndroidNative.CustomEventsHandler") { }

            public void VolumeChanged(double value)
            {
                OnVolumeChanged?.Invoke(value);
            }
            public void BatteryLevelChanged(double value)
            {
                OnBatteryLevelChanged?.Invoke(value);
            }
            public void BatteryChargingStateChanged(bool value)
            {
                OnBatteryChargingStateChanged?.Invoke(value);
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        _androidNativeObject = new AndroidJavaObject("com.crazy_games.AndroidNative.AndroidNative", currentActivity, handler);
                    }
                }
            }

            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            if (_androidNativeObject != null)
            {
                _androidNativeObject.Dispose();
            }
        }

        public static void CallMethod(string name, params object[] arguments)
        {
#if UNITY_EDITOR
            Debug.Log($"Native method '{name}', called with arguments [{string.Join(", ", arguments.Select(x => "(" + x + ")"))}]");
#endif
            if (_androidNativeObject != null)
            {
                _androidNativeObject.Call(name, arguments);
            }
        }

        public static T CallMethod<T>(string name, T defaultValue, params object[] arguments)
        {
#if UNITY_EDITOR
            Debug.Log($"Native method '{name}', called with arguments [{string.Join(", ", arguments.Select(x => "(" + x + ")"))}] and default value '{defaultValue}'");
#endif

            if (_androidNativeObject != null)
            {
                return _androidNativeObject.Call<T>(name, arguments);
            }

            return defaultValue;
        }
    }
}