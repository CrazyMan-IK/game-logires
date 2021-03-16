using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CrazyGames.Logires.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class CustomContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [SerializeField] private RectTransform _fallback = null;
        [SerializeField] private float _minWidth = -1;
        [SerializeField] private float _minHeight = -1;
        [SerializeField] private FitMode _horizontalFit = FitMode.Unconstrained;
        [SerializeField] private FitMode _verticalFit = FitMode.Unconstrained;

        [NonSerialized] private RectTransform _rect = null;
        private DrivenRectTransformTracker _tracker = default;

        public float MinWidth
        {
            get { return _minWidth; }
            set
            {
                if (_minWidth != value)
                {
                    _minWidth = value;
                    SetDirty();
                }
            }
        }

        public float MinHeight
        {
            get { return _minHeight; }
            set
            {
                if (_minHeight != value)
                {
                    _minHeight = value;
                    SetDirty();
                }
            }
        }

        public FitMode HorizontalFit 
        { 
            get { return _horizontalFit; } 
            set 
            {
                if (_horizontalFit != value)
                {
                    _horizontalFit = value;
                    SetDirty();
                }
            } 
        }

        public FitMode VerticalFit 
        { 
            get { return _verticalFit; } 
            set
            {
                if (_verticalFit != value)
                {
                    _verticalFit = value;
                    SetDirty();
                }
            } 
        }

        private RectTransform rectTransform
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                }
                return _rect;
            }
        }

        private CustomContentSizeFitter() { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            _tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode fitting = (axis == 0 ? HorizontalFit : VerticalFit);
            if (fitting == FitMode.Unconstrained)
            {
                _tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                return;
            }

            _tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            float size;

            if (fitting == FitMode.MinSize)
            {
                size = LayoutUtility.GetMinSize(_rect, axis);
            }
            else
            {
                size = LayoutUtility.GetPreferredSize(_rect, axis);
            }

            var sWidth = _fallback?.rect.width;
            var sHeight = _fallback?.rect.height;

            if (axis == 0)
            {
                if (MinWidth < 0 && size < sWidth)
                {
                    size = sWidth.Value;
                }
                else if (size < MinWidth)
                {
                    size = MinWidth;
                }
            }
            else
            {
                if (MinHeight < 0 && size < sHeight)
                {
                    size = sHeight.Value;
                }
                else if (size < MinHeight)
                {
                    size = MinHeight;
                }
            }

            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size);
        }

        /// <summary>
        /// Calculate and apply the horizontal component of the size to the RectTransform
        /// </summary>
        public void SetLayoutHorizontal()
        {
            _tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        private void SetDirty()
        {
            if (!IsActive())
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}
