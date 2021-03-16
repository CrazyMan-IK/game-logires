using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    public class GlobalTicker : MonoBehaviour
    {
        private static GlobalTicker _instance = null;

        public static GlobalTicker Instance
        {
            get
            {
                if (_instance == null)
                {
                    var ticker = new GameObject();
                    _instance = ticker.AddComponent<GlobalTicker>();
                }
                return _instance;
            }
        }

        public Coroutine StartDelayed(float delay, Action method)
        {
            return StartCoroutine(Routine(delay, method));
        }

        public void StopDelayed(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public IEnumerator Routine(float delay, Action method)
        {
            while (true)
            {
                method?.Invoke();
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
