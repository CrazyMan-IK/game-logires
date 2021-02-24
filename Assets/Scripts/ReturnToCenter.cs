using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(Button))]
    public class ReturnToCenter : MonoBehaviour
    {
        private Camera _mainCamera = null;
        private Button _button = null;
        private Tween _lastTween = null;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _button = GetComponent<Button>();

            _button.onClick.AddListener(OnClicked);
        }

        public void OnClicked()
        {
            if (_lastTween == null || _lastTween.IsComplete())
            {
                _mainCamera.GetComponent<FreeCamera>().StopMoving();

                var cameraTransform = _mainCamera.transform;
                _lastTween = cameraTransform.DOMove(new Vector3(0, 0, cameraTransform.position.z), 0.2f).OnKill(() => _lastTween = null);
            }
        }
    }
}
