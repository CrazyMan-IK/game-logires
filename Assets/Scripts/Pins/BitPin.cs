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

        protected override void OnUpdate()
        {
            var currentColor = GetCurrentColor();

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

                    GetLineRendererOf(linkedPin).endColor = currentColor;
                }
            }
            else if (IsInput && !isConnected)
            {
                Value = _defaultValue.GetCopy();
            }

            _renderer.color = currentColor;
            GetLineRendererOf(this).startColor = currentColor;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = (Application.isPlaying ? Value.Any(x => x) : _defaultValue.Any(x => x)) ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        public override Color GetCurrentColor()
        {
            //if (Value.Where(x => x).Take(2).Count() == 2) return Color.green;
            //else if (Value.Where(x => x).Take(2).Count() == 1) return Color.yellow;
            //return Color.red;
            //return Value.Any(x => x) ? Color.green : Color.red;
            var allCount = Mathf.Clamp(Value.Count, 1, int.MaxValue);
            var enabledCount = Value.Count(x => x);
            var val1 = 100.0f / allCount;
            var val2 = val1 * enabledCount;
            var val3 = val2 / 100.0f;
            var val4 = Mathf.Clamp(val3, 0, 0.5f).Remap(0, 0.5f, 0, 1);
            var val5 = Mathf.Clamp(val3, 0.5f, 1).Remap(0.5f, 1, 0, 1);
            return Color.Lerp(Color.green, Color.Lerp(Color.blue, Color.red, 1 - val4), 1 - val5);
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