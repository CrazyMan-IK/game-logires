using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class BooleanPin : Pin<bool>
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

        protected override void OnUpdate()
        {
            bool isConnected = _linkedPins.Count != 0;

            if (!IsInput)
            {
                foreach (var linkedPin in _linkedPins)
                {
                    if (linkedPin is BooleanPin pin1)
                    {
                        pin1.Value = Value;
                    }
                    else if (linkedPin is BitPin pin2)
                    {
                        pin2.Value = Value.BoolToBits();
                    }
                    else if (linkedPin is IntegerPin pin3)
                    {
                        pin3.Value = Value.BoolToInt32();
                    }
                    else if (linkedPin is DoublePin pin4)
                    {
                        pin4.Value = Value.BoolToDouble();
                    }
                }
            }
            else if (IsInput && !isConnected)
            {
                Value = _defaultValue;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = (Application.isPlaying ? Value : _defaultValue) ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        protected override void OnValueChangedHandler(bool oldValue, bool newValue)
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
            return Value ? Color.green : Color.red;
        }

        public override bool GetBaseValue()
        {
            return false;
        }
        public override bool Compare(bool x, bool y)
        {
            return x == y;
        }
    }
}