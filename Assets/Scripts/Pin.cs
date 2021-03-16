using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(LineAnchorer))]
    [RequireComponent(typeof(LineRenderer))]
    public abstract class Pin : MonoBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] protected bool _isInput = false;

        protected readonly Color _defaultColor = new Color(96.0f / 255, 96.0f / 255, 96.0f / 255);
        protected Transform _transform = null;
        protected SpriteRenderer _renderer = null;
        protected LineAnchorer _line = null;
        protected Camera _mainCamera = null;
        protected ObservableCollection<Pin> _linkedPins = new ObservableCollection<Pin>();

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

        protected void Update()
        {
            if (!IsInput)
            {
                var errorPins = _linkedPins.Where(x => !x._linkedPins.Contains(this)).ToList();

                foreach (var pin in errorPins)
                {
                    Disconnect(pin, true);
                }
            }
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

        public virtual bool CanConnect(Pin other)
        {
            return true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsInput)
            {
                Disconnect();
            }
            SetPosition(_mainCamera.ScreenToWorldPoint(eventData.position) - _transform.position);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetPosition(_mainCamera.ScreenToWorldPoint(eventData.position) - _transform.position);
        }
        
        private Block CheckRecursion(Pin destination)
        {
            if (destination == null)
            {
                return null;
            }

            Block lastBlock = destination.GetComponentInParent<Block>();

            while (lastBlock != null)
            {
                if (lastBlock == GetComponentInParent<Block>())
                {
                    break;
                }

                if (IsInput)
                {
                    if (lastBlock is IHaveInputs haveInputs)
                    {
                        if (haveInputs.Inputs.Count > 0)
                        {
                            foreach (var input in haveInputs.Inputs)
                            {
                                var inPin = input._linkedPins.FirstOrDefault();
                                lastBlock = CheckRecursion(inPin);
                            }

                            continue;
                        }
                    }
                }
                else
                {
                    if (lastBlock is IHaveOutputs haveOutputs)
                    {
                        if (haveOutputs.Outputs.Count > 0)
                        {
                            foreach (var output in haveOutputs.Outputs)
                            {
                                if (output._linkedPins.Count > 0)
                                {
                                    Block temp = null;

                                    foreach (var outPin in output._linkedPins)
                                    {
                                        temp ??= CheckRecursion(outPin);
                                    }

                                    lastBlock = temp;
                                }
                                else
                                {
                                    lastBlock = null;
                                }
                            }

                            continue;
                        }
                    }
                }

                lastBlock = null;
            }

            return lastBlock;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var pin1 = eventData.pointerDrag.GetComponent<Pin>();
            var pin2 = eventData.pointerEnter?.GetComponent<Pin>();

            if (pin2 != null &&
                pin1.CanConnect(pin2) &&
                pin2.CanConnect(pin1) &&
                eventData.pointerDrag != eventData.pointerEnter &&
                eventData.pointerDrag.transform.parent != eventData.pointerEnter.transform.parent &&
                pin1.IsInput != pin2.IsInput)
            {
                Block temp = CheckRecursion(pin2);

                if (temp == null)
                {
                    if (IsInput)
                    {
                        Disconnect();
                        _linkedPins.Add(pin2);
                        SetTarget(pin2.transform);
                    }
                    else
                    {
                        _linkedPins.Add(pin2);
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
                pin1.CanConnect(pin2) &&
                pin2.CanConnect(pin1) &&
                eventData.pointerDrag != eventData.pointerEnter &&
                eventData.pointerDrag.transform.parent != eventData.pointerEnter.transform.parent &&
                pin1.IsInput != pin2.IsInput)
            {
                Block temp = CheckRecursion(pin1);

                if (temp == null)
                {
                    if (IsInput)
                    {
                        Disconnect();
                        _linkedPins.Add(pin1);
                        SetTarget(eventData.pointerDrag.transform);
                    }
                    else
                    {
                        _linkedPins.Add(pin1);
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
    public abstract class Pin<T> : Pin
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