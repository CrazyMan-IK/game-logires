using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace CrazyGames.Logires
{
    public class SchemeSaveUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _titleText = null;

        private string _title = "Unknown";

        public event Action<SchemeSaveUI> OnRequestRename;

        public string Title 
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    _titleText?.SetText(_title);
                }
            }
        }

        public int SaveNumber { get; set; }

        private void Awake()
        {
            _titleText?.SetText(_title);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(Saver.Instance.LoadScene(SaveNumber));
        }

        public void DeleteSave()
        {
            Saver.Instance.DeleteSave(SaveNumber);
            Destroy(gameObject);
        }

        public void RequestRename()
        {
            OnRequestRename?.Invoke(this);
        }
    }
}
