﻿using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrazyGames.Logires.Utils
{
    public class FreeCamera : MonoBehaviour
    {
        [SerializeField] private InputHandler _viewInput = null;

        [SerializeField] private float _minZoom = 3;
        [SerializeField] private float _maxZoom = 17;

        //[SerializeField] private float _moveSpeed = 1;
        //[SerializeField] private float _zoomSpeed = 1;

        //[SerializeField] private float _transitionsSpeed = 0.4f;

        private Tween _movingTween = null;
        private Camera _mainCamera = null;
        private Transform _mainCameraTransform = null;

        public Vector3 TargetPoint { get; set; } = Vector3.zero;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _mainCameraTransform = _mainCamera.transform;
            TargetPoint = _mainCameraTransform.position;
        }

        private void OnEnable()
        {
            _viewInput.OnMoved += OnMoved;
            _viewInput.OnZoomed += OnZoomed;
        }
        private void OnDisable()
        {
            _viewInput.OnMoved -= OnMoved;
            _viewInput.OnZoomed -= OnZoomed;
        }

#if UNITY_EDITOR
        private void Update()
        {
            var vert = Input.GetAxis("Vertical");
            var hor = Input.GetAxis("Horizontal");
            OnMoved(new Vector2(-hor, -vert), CurrentState.Change);

            var zoomDelta = Input.mouseScrollDelta / 10;
            OnZoomed(zoomDelta.y, Input.mousePosition);
        }
#endif

        private void LateUpdate()
        {
            var result = Vector3.Lerp(_mainCameraTransform.position, TargetPoint, Time.deltaTime * 500);
            result.x = Mathf.Round(result.x * 100) / 100;
            result.y = Mathf.Round(result.y * 100) / 100;
            result.z = Mathf.Round(result.z * 100) / 100;

            _mainCameraTransform.position = result;
        }

        private void OnMoved(Vector2 delta, CurrentState state)
        {
            var endDelta = delta / 15;
            delta /= 2f;

            endDelta *= _mainCamera.orthographicSize.Remap(3, 12, 1, 3);
            delta *= _mainCamera.orthographicSize.Remap(3, 12, 1, 3);

            if (state == CurrentState.Start)
            {
                _movingTween?.Pause().Kill();
            }
            else if (state == CurrentState.Change)
            {
                TargetPoint -= (Vector3)delta * Time.deltaTime;
            }
            else if (state == CurrentState.End && endDelta.magnitude > 3.5f)
            {
                _movingTween = DOTween.To(() => TargetPoint, value => TargetPoint = value, TargetPoint - (Vector3)endDelta, 1.0f).SetEase(Ease.OutCubic);
            }
        }

        private void OnZoomed(float delta, Vector2 point)
        {
            //delta /= 50;
            delta *= 4;

            if (_mainCamera.orthographicSize + delta < _minZoom)
            {
                _mainCamera.orthographicSize = _minZoom;
            }
            else if (_mainCamera.orthographicSize + delta > _maxZoom)
            {
                _mainCamera.orthographicSize = _maxZoom;
            }
            else
            {
                _mainCamera.orthographicSize += delta;
            }
        }

        public void StopMoving()
        {
            _movingTween?.Pause().Kill();
        }
    }
}