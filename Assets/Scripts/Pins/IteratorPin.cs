using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class IteratorPin : Pin<LinkedListNode<object>>
    {
        private void OnEnable()
        {
            _linkedPins.CollectionChanged += OnPinsCollectionChanged;
            ValueChanged += OnThisValueChanged;

            _value = _defaultValue;
            OnValueChanged(_defaultValue, _defaultValue);
        }

        private void OnDisable()
        {
            _linkedPins.CollectionChanged -= OnPinsCollectionChanged;
            ValueChanged -= OnThisValueChanged;
        }

        private void OnThisValueChanged(Pin sender, LinkedListNode<object> oldValue, LinkedListNode<object> newValue)
        {
            var currentColor = GetCurrentColor();

            if (!IsInput)
            {
                object currentValue = Value?.Value ?? false;

                foreach (var linkedPin in _linkedPins)
                {
                    if (currentValue is bool val1 && linkedPin is BooleanPin pin1)
                    {
                        pin1.Value = val1;
                    }
                    else if (currentValue is int && linkedPin is BitPin pin2)
                    {
                        int val = Convert.ToInt32(currentValue);

                        pin2.Value = val.Int32ToBits();
                    }
                    else if (currentValue is int val2 && linkedPin is IntegerPin pin3)
                    {
                        pin3.Value = val2;
                    }
                    else if (currentValue is double val3 && linkedPin is DoublePin pin4)
                    {
                        pin4.Value = val3;
                    }
                    else if (linkedPin is IteratorPin pin5)
                    {
                        pin5.Value = Value;
                    }

                    GetLineRendererOf(linkedPin).endColor = currentColor;
                }
            }

            _renderer.color = currentColor;
            GetLineRendererOf(this).startColor = currentColor;
        }

        private void OnPinsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                OnValueChanged(Value, Value);
            }

            if (IsInput && _linkedPins.Count == 0)
            {
                Value = _defaultValue;
            }
        }

        protected override void OnUpdate()
        {
            var currentColor = GetCurrentColor();

            bool isConnected = _linkedPins.Count != 0;

            if (!IsInput)
            {
                foreach (var linkedPin in _linkedPins)
                {
                    GetLineRendererOf(linkedPin).endColor = currentColor;
                }
            }
            else if (IsInput && !isConnected)
            {
                Value = _defaultValue;
            }

            _renderer.color = currentColor;
            GetLineRendererOf(this).startColor = currentColor;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetCurrentColor();
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        public override Color GetCurrentColor()
        {
            return Value == null ? Color.red : Color.magenta;
        }
        public override bool CanConnect(Pin other)
        {
            return !IsInput && base.CanConnect(other);
        }

        public override LinkedListNode<object> GetBaseValue()
        {
            return null;
        }
        public override bool Compare(LinkedListNode<object> x, LinkedListNode<object> y)
        {
            return x == y;
        }
    }
}