using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CrazyGames.Logires.Utils
{
    [RequireComponent(typeof(Image))]
    public class GridMaterialWrapper : MonoBehaviour
    {
        [SerializeField] private Material _material = null;
        [SerializeField] private bool _useTweening = false;
        [SerializeField] private float _duration = 1;
        [SerializeField] private Ease _easeFunction = Ease.Unset;

        private Material _privateMaterial = null;
        private Image _image = null;
        private Vector2 _offset = Vector2.zero;
        private Color _backgroundColor = Color.white;
        private Color _lineColor = Color.black;

        private void OnEnable()
        {
            _image = GetComponent<Image>();
            _privateMaterial = _image.material = Instantiate(_material);

            _offset.x = _privateMaterial.GetFloat("GridOffsetX");
            _offset.y = _privateMaterial.GetFloat("GridOffsetY");
            _backgroundColor = _privateMaterial.GetColor("BackGround");
            _lineColor = _privateMaterial.GetColor("LineColor");
        }

        public Vector2 GetOffset()
        {
            return _offset;
        }

        public Color GetBackgroundColor()
        {
            return _backgroundColor;
        }

        public Color GetLineColor()
        {
            return _lineColor;
        }

        public Tweener SetOffsetX(float newOffset)
        {
            return SetOffsetX(newOffset, _duration);
        }
        public Tweener SetOffsetX(float newOffset, float duration)
        {
            _offset.x = newOffset;

            if (_useTweening)
            {
                return _privateMaterial.DOFloat(newOffset, "GridOffsetX", duration).SetEase(_easeFunction);
            }
            else
            {
                _privateMaterial.SetFloat("GridOffsetX", newOffset);
            }

            return null;
        }

        public Tweener SetOffsetY(float newOffset)
        {
            return SetOffsetY(newOffset, _duration);
        }
        public Tweener SetOffsetY(float newOffset, float duration)
        {
            _offset.y = newOffset;

            if (_useTweening)
            {
                return _privateMaterial.DOFloat(newOffset, "GridOffsetY", duration).SetEase(_easeFunction);
            }
            else
            {
                _privateMaterial.SetFloat("GridOffsetY", newOffset);
            }

            return null;
        }

        public Tweener SetBackgroundColor(Color newColor)
        {
            return SetBackgroundColor(newColor, _duration);
        }
        public Tweener SetBackgroundColor(Color newColor, float duration)
        {
            _backgroundColor = newColor;

            if (_useTweening)
            {
                return _privateMaterial.DOColor(newColor, "BackGround", duration).SetEase(_easeFunction);
            }
            else
            {
                _privateMaterial.SetColor("BackGround", newColor);
            }

            return null;
        }

        public Tweener SetLineColor(Color newColor)
        {
            return SetLineColor(newColor, _duration);
        }
        public Tweener SetLineColor(Color newColor, float duration)
        {
            _lineColor = newColor;

            if (_useTweening)
            {
                return _privateMaterial.DOColor(newColor, "LineColor", duration).SetEase(_easeFunction);
            }
            else
            {
                _privateMaterial.SetColor("LineColor", newColor);
            }

            return null;
        }
    }
}