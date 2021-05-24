using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CrazyGames.Logires.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class CustomAspectRatioFitter : UIBehaviour, ILayoutSelfController, ILayoutElement
    {
        public enum AspectMode
        {
            None,
            WidthControlsHeight,
            HeightControlsWidth,
            FitInParent,
            EnvelopeParent
        }

        [SerializeField] private AspectMode _aspectMode = AspectMode.None;
        [SerializeField] private float _aspectRatio = 1;

        [NonSerialized] private RectTransform _rect;

        private bool _delayedSetDirty = false;
        private bool _doesParentExist = false;

        private float _minWidth = -1;
        private float _minHeight = -1;
        private float _preferredWidth = -1;
        private float _preferredHeight = -1;
        private float _flexibleWidth = -1;
        private float _flexibleHeight = -1;
        private int _layoutPriority = 1;

        private DrivenRectTransformTracker _tracker = default;

        public AspectMode aspectMode 
        { 
            get { return _aspectMode; } 
            set 
            {
                if (_aspectMode != value)
                {
                    _aspectMode = value;
                    SetDirty();
                }
            } 
        }

        public float aspectRatio 
        { 
            get { return _aspectRatio; } 
            set
            {
                if (_aspectRatio != value)
                {
                    _aspectRatio = value;
                    SetDirty();
                }
            }
        }

        public float minWidth { get { return _minWidth; } }
        public float minHeight { get { return _minHeight; } }
        public float preferredWidth { get { return _preferredWidth; } }
        public float preferredHeight { get { return _preferredHeight; } }
        public float flexibleWidth { get { return _flexibleWidth; } }
        public float flexibleHeight { get { return _flexibleHeight; } }
        public int layoutPriority { get { return _layoutPriority; } }

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

        protected CustomAspectRatioFitter() { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
            _doesParentExist = rectTransform.parent ? true : false;
        }

        protected override void Start()
        {
            base.Start();
            if (!IsComponentValidOnObject() || !IsAspectModeValid())
            {
                enabled = false;
            }
        }

        protected override void OnDisable()
        {
            _tracker.Clear();
            SetDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            _doesParentExist = rectTransform.parent ? true : false;
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        protected virtual void Update()
        {
            if (_delayedSetDirty)
            {
                _delayedSetDirty = false;
                SetDirty();
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsComponentValidOnObject())
            {
                return;
            }

            _tracker.Clear();

            switch (_aspectMode)
            {
#if UNITY_EDITOR
                case AspectMode.None:
                    {
                        if (!Application.isPlaying)
                            _aspectRatio = Mathf.Clamp(rectTransform.rect.width / rectTransform.rect.height, 0.001f, 1000f);

                        break;
                    }
#endif
                case AspectMode.HeightControlsWidth:
                    {
                        _tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
                        _preferredWidth = rectTransform.rect.height * _aspectRatio;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _preferredWidth);
                        break;
                    }
                case AspectMode.WidthControlsHeight:
                    {
                        _tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
                        _preferredHeight = rectTransform.rect.width * _aspectRatio;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _preferredHeight);
                        break;
                    }
                case AspectMode.FitInParent:
                case AspectMode.EnvelopeParent:
                    {
                        if (!DoesParentExists())
                            break;

                        _tracker.Add(this, rectTransform,
                            DrivenTransformProperties.Anchors |
                            DrivenTransformProperties.AnchoredPosition |
                            DrivenTransformProperties.SizeDeltaX |
                            DrivenTransformProperties.SizeDeltaY);

                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchoredPosition = Vector2.zero;

                        Vector2 sizeDelta = Vector2.zero;
                        Vector2 parentSize = GetParentSize();
                        if ((parentSize.y * aspectRatio < parentSize.x) ^ (_aspectMode == AspectMode.FitInParent))
                        {
                            sizeDelta.y = GetSizeDeltaToProduceSize(parentSize.x / aspectRatio, 1);
                        }
                        else
                        {
                            sizeDelta.x = GetSizeDeltaToProduceSize(parentSize.y * aspectRatio, 0);
                        }
                        rectTransform.sizeDelta = sizeDelta;

                        break;
                    }
            }
        }

        private float GetSizeDeltaToProduceSize(float size, int axis)
        {
            return size - GetParentSize()[axis] * (rectTransform.anchorMax[axis] - rectTransform.anchorMin[axis]);
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            return !parent ? Vector2.zero : parent.rect.size;
        }

        public virtual void SetLayoutHorizontal() { }

        public virtual void SetLayoutVertical() { }

        public virtual void CalculateLayoutInputHorizontal() { }

        public virtual void CalculateLayoutInputVertical() { }

        protected void SetDirty()
        {
            if (!IsActive())
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            Canvas.ForceUpdateCanvases();
            UpdateRect();
        }

        public bool IsComponentValidOnObject()
        {
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas && canvas.renderMode != RenderMode.WorldSpace)
            {
                return false;
            }
            return true;
        }

        public bool IsAspectModeValid()
        {
            if (!DoesParentExists() && (aspectMode == AspectMode.EnvelopeParent || aspectMode == AspectMode.FitInParent))
                return false;

            return true;
        }

        private bool DoesParentExists()
        {
            return _doesParentExist;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _aspectRatio = Mathf.Clamp(_aspectRatio, 0.001f, 1000f);
            _delayedSetDirty = true;
        }
#endif
    }
}
