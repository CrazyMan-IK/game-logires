using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrazyGames.Logires
{
    [ExecuteAlways]
    public class FlexibleGridLayout : MonoBehaviour
    {
        [SerializeField] private float _cellHeight = 100;

        private RectTransform _transform = null;
        private GridLayoutGroup _layout = null;
        private Vector2 _currentSize = Vector2.zero;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _layout = GetComponent<GridLayoutGroup>();

            _currentSize.y = _cellHeight;
        }

        private void Update()
        {
            _currentSize.x = _layout.cellSize.x - _transform.sizeDelta.x / _layout.constraintCount;
            _layout.cellSize = _currentSize;
        }
    }
}
