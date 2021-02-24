using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class EditScene : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = -1;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SaveLevel();
                SceneManager.LoadScene(0);
            }
        }

        public void SaveLevel()
        {
            Saver.Instance.SaveCurrentLevel();
            AndroidNativeWrapper.CallMethod("ShowToast", new object[] { "Level saved", 0 });

            EncryptedGlobalPreferences.Set("Test", 123);
        }
    }
}
