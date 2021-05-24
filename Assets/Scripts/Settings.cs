using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Localization.Settings;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Settings : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Dropdown _locale = null;

        private CanvasGroup _canvasGroup = null;

        private bool _isOpened = false;

        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                if (_isOpened != value)
                {
                    _isOpened = value;

                    _canvasGroup.DOFade(Convert.ToInt32(_isOpened), 0.5f);
                    _canvasGroup.interactable = _isOpened;
                    _canvasGroup.blocksRaycasts = _isOpened;
                }
            }
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _isOpened = _canvasGroup.alpha > 0;
        }

        private void OnLocaleInitialized(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<LocalizationSettings> oper)
        {
            _locale.AddOptions(LocalizationSettings.AvailableLocales.Locales.Select(x => {
                var result = x.Identifier.CultureInfo.NativeName;
                var firstLetter = Char.ToUpper(result[0]);
                result = result.Remove(0, 1);
                result = firstLetter + result;
                
                return result;
            }).ToList());

            string currentLocaleCode = GlobalPreferences.Get<string>("current_locale", null);
            if (currentLocaleCode != null)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == currentLocaleCode);
            }

            LocalizationChanged(LocalizationSettings.SelectedLocale);
        }

        private void OnEnable()
        {
            _locale.onValueChanged.AddListener(LocalizationChanged);

            LocalizationSettings.InitializationOperation.Completed += OnLocaleInitialized;
            LocalizationSettings.SelectedLocaleChanged += LocalizationChanged;
        }

        private void OnDisable()
        {
            _locale.onValueChanged.RemoveListener(LocalizationChanged);

            LocalizationSettings.InitializationOperation.Completed -= OnLocaleInitialized;
            LocalizationSettings.SelectedLocaleChanged -= LocalizationChanged;
        }

        private void LocalizationChanged(UnityEngine.Localization.Locale newLocale)
        {
            _locale.value = LocalizationSettings.AvailableLocales.Locales.FindIndex(x => x == newLocale);
            GlobalPreferences.Set("current_locale", newLocale.Identifier.Code);
        }

        private void LocalizationChanged(int newValue)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[newValue];
        }
    }
}
