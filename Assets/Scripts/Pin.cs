using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CrazyGames.Logires.Interfaces;
using System.Collections.ObjectModel;
using System;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(LineAnchorer))]
    [RequireComponent(typeof(LineRenderer))]
    public abstract class Pin : MonoBehaviour
    {
        [SerializeField] protected bool _isInput = false;

        protected readonly Color _defaultColor = new Color(96.0f / 255, 96.0f / 255, 96.0f / 255);
        protected Transform _transform = null;
        protected SpriteRenderer _renderer = null;
        protected LineAnchorer _line = null;
        protected Camera _mainCamera = null;
        protected List<Pin> _linkedPins = new List<Pin>();

        private LineRenderer _lineRenderer = null;

        public bool IsInput { get => _isInput; }
        public IReadOnlyList<Pin> LinkedPins { get => _linkedPins; }

        protected virtual void Awake()
        {
            _transform = transform;
            _renderer = GetComponent<SpriteRenderer>();
            _line = GetComponent<LineAnchorer>();
            _lineRenderer = GetComponent<LineRenderer>();

            _mainCamera = Camera.main;
        }

        private void SetPosition(Vector2 position)
        {
            //Vector2 target = ;
            //target.x *= 1 / _transform.lossyScale.x;
            //target.y *= 1 / _transform.lossyScale.y;

            _line.Position2 = position;
        }

        private void SetTarget(Transform target)
        {
            _line.Target2 = target;
        }

        protected LineRenderer GetLineRendererOf(Pin pin)
        {
            return pin._lineRenderer;
        }

        public void Disconnect(Pin pin = null, bool remote = false)
        {
            if (!remote)
            {
                foreach (var linkedPin in _linkedPins)
                {
                    linkedPin.Disconnect(this, true);
                }
            }

            _lineRenderer.endColor = _defaultColor;
            SetTarget(null);
            SetPosition(Vector2.zero);

            if (pin == null)
            {
                _linkedPins.Clear();
            }
            else
            {
                _linkedPins.Remove(pin);
            }
        }

        public void ConnectTo(Pin pin, bool remote = false)
        {
            if (pin != null && IsInput != pin.IsInput)
            {
                if (IsInput)
                {
                    Disconnect();
                    _linkedPins.Add(pin);
                    SetTarget(pin.transform);
                }
                else
                {
                    _linkedPins.Add(pin);
                }

                if (!remote)
                {
                    pin.ConnectTo(this, true);
                }
            }
        }

        public abstract Color GetCurrentColor();

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsInput)
            {
                Disconnect();
            }
            SetPosition(_mainCamera.ScreenToWorldPoint(eventData.position) - _transform.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetPosition(_mainCamera.ScreenToWorldPoint(eventData.position) - _transform.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var pin1 = eventData.pointerDrag.GetComponent<Pin>();
            var pin2 = eventData.pointerEnter?.GetComponent<Pin>();

            if (eventData.pointerEnter != null &&
                pin2 != null &&
                eventData.pointerDrag != eventData.pointerEnter &&
                eventData.pointerDrag.transform.parent != eventData.pointerEnter.transform.parent &&
                pin1.IsInput != pin2.IsInput)
            {
                //SetTarget(eventData.pointerEnter.transform);
                if (pin1.IsInput)
                {
                    pin1.Disconnect();
                    _linkedPins.Add(pin2);
                    SetTarget(pin2.transform);
                }
                else
                {
                    //if (_linkedPins.Count == 0)
                    {
                        _linkedPins.Add(pin2);
                    }
                    //else
                    {
                        //_linkedPins[0] = pin2;
                    }
                }
            }

            SetPosition(Vector2.zero);
        }

        
        public void OnDrop(PointerEventData eventData)
        {
            var pin1 = eventData.pointerDrag?.GetComponent<Pin>();
            var pin2 = eventData.pointerEnter?.GetComponent<Pin>();

            if (pin1 != null && pin2 != null &&
                eventData.pointerDrag != eventData.pointerEnter &&
                eventData.pointerDrag.transform.parent != eventData.pointerEnter.transform.parent &&
                pin1.IsInput != pin2.IsInput)
            {
                if (pin2.IsInput)
                {
                    pin2.Disconnect();
                    _linkedPins.Add(pin1);
                    SetTarget(eventData.pointerDrag.transform);
                }
                else
                {
                    //if (_linkedPins.Count == 0)
                    {
                        _linkedPins.Add(pin1);
                    }
                    //else
                    {
                        //_linkedPins[0] = pin1;
                    }
                }
            }
            else
            {
                SetPosition(Vector2.zero);
            }
        }
    }

    [DefaultExecutionOrder(-5)]
    public abstract class Pin<T> : Pin, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] protected T _defaultValue = default;

        [SerializeField] protected T _value = default;
        protected Func<T, T, bool> _comparer = null;

        public delegate void ValueChangedHandler(Pin sender, T oldValue, T newValue);
        public event ValueChangedHandler ValueChanged;

        public T Value 
        {
            get
            {
                if (EqualityComparer<object>.Default.Equals(_value, null))
                {
                    _value = GetBaseValue();
                }

                return _value;
            }
            set
            {
                if (!Compare(_value, value))
                {
                    var oldValue = _value;
                    _value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _value = _defaultValue;
            _renderer.color = GetCurrentColor();
        }

        private void FixedUpdate()
        {
            OnUpdate();
        }

        protected abstract void OnUpdate();

        protected void OnValueChanged(T oldValue, T newValue)
        {
            ValueChanged?.Invoke(this, oldValue, newValue);
        }

        public abstract T GetBaseValue();
        public abstract bool Compare(T x, T y);
    }
}