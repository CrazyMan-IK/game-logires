using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class BitPin : Pin<List<bool>>
    {
        /*private void Start()
        {
            _linkedPins.CollectionChanged += OnPinsCollectionChanged;
            //_linkedPins.OnCollectionCleared += OnPinsCollectionCleared;
            ValueChanged += OnThisValueChanged;

            _value = _defaultValue;
            OnValueChanged(_defaultValue, _defaultValue);
        }

        private void OnThisValueChanged(Interfaces.IPin sender, bool oldValue, bool newValue)
        {
            foreach (var linkedPin in _linkedPins)
            {
                if (linkedPin is BoolPin pin)
                {
                    if (!IsInput)
                    {
                        pin.Value = newValue;
                        pin._lineRenderer.endColor = newValue ? Color.green : Color.red;
                    }
                }
            }

            _renderer.color = newValue ? Color.green : Color.red;
            _lineRenderer.startColor = newValue ? Color.green : Color.red;
        }

        private void OnLinkedValueChanged(Interfaces.IPin sender, bool oldValue, bool newValue)
        {
            if (IsInput)
            {
                Value = newValue;
            }
        }

        private void OnPinsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var changedItems = e.NewItems.Cast<BoolPin>();

                foreach (var item in changedItems)
                {
                    if (IsInput)
                    {
                        _lineRenderer.endColor = item.Value ? Color.green : Color.red;
                        Value = item.Value;
                    }
                    else
                    {

                    }
                    item.ValueChanged += OnLinkedValueChanged;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var changedItems = e.OldItems.Cast<BoolPin>();

                foreach (var item in changedItems)
                {
                    item.ValueChanged -= OnLinkedValueChanged;
                }
            }

            if (_linkedPins.Count == 0 && IsInput)
            {
                Value = _defaultValue;
            }
        }*/
        private void OnEnable()
        {
            _linkedPins.CollectionChanged += OnPinsCollectionChanged;

            _value = _defaultValue;
            OnValueChanged(_defaultValue, _defaultValue);
        }

        private void OnDisable()
        {
            _linkedPins.CollectionChanged -= OnPinsCollectionChanged;
        }

        private void OnPinsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (IsInput)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    OnValueChanged(Value, Value);
                }
            }
        }

        protected override void OnUpdate()
        {
            bool isConnected = _linkedPins.Count != 0;

            if (!IsInput)
            {
                foreach (var linkedPin in _linkedPins)
                {
                    if (linkedPin is BooleanPin pin1)
                    {
                        pin1.Value = Value.BitsToBool();
                    }
                    else if (linkedPin is BitPin pin2)
                    {
                        pin2.Value = Value.GetCopy();
                    }
                    else if (linkedPin is IntegerPin pin3)
                    {
                        pin3.Value = Value.BitsToInt32();
                    }
                    else if (linkedPin is DoublePin pin4)
                    {
                        pin4.Value = Value.BitsToDouble();
                    }
                }
            }
            else if (IsInput && !isConnected)
            {
                Value = _defaultValue.GetCopy();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = (Application.isPlaying ? Value.Any(x => x) : _defaultValue.Any(x => x)) ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        protected override void OnValueChangedHandler(List<bool> oldValue, List<bool> newValue)
        {
            var currentColor = GetCurrentColor();

            if (!IsInput)
            {
                foreach (var linkedPin in _linkedPins)
                {
                    GetLineRendererOf(linkedPin).endColor = currentColor;
                }
            }

            _renderer.color = currentColor;
            GetLineRendererOf(this).startColor = currentColor;
        }

        public override Color GetCurrentColor()
        {
            if (Value.Count == 1)
            {
                return Value[0] ? Color.green : Color.red;
            }

            return Color.blue;
        }

        public override List<bool> GetBaseValue()
        {
            return new List<bool>() { false };
        }
        public override bool Compare(List<bool> x, List<bool> y)
        {
            if (x == null)
            {
                return y == null;
            }
            else
            {
                if (y == null) return false;
            }

            return x.Count == y.Count && x.SequenceEqual(y);
        }
    }
}