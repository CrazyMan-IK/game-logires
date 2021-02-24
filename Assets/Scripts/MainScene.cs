using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires
{
    public class MainScene : MonoBehaviour
    {
        [SerializeField] private Transform _savesRoot = null;
        [SerializeField] private SchemeSaveUI _schemePrefab = null;
        
        private void Awake()
        {
            var saves = Saver.Instance.GetAllSaves();

            while (saves.MoveNext())
            {
                var saveIndex = saves.Current;
                var save = Saver.Instance.Load(saveIndex);

                var ui = Instantiate(_schemePrefab, _savesRoot);
                ui.Title = save.Title;
                ui.SaveNumber = saveIndex;
                ui.OnRequestRename += OpenRenameDialog;
            }

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
                Application.Quit();
            }
        }

        private void OpenRenameDialog(SchemeSaveUI sender)
        {
            Saver.Instance.CurrentLevelMetadata.Title = "New";
            sender.Title = "New";
        }

        public void LoadNewSave()
        {
            StartCoroutine(Saver.Instance.LoadScene(-1));
        }
    }
}
