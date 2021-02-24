using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class IntegerPin : Pin<int>
    {
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
                        pin1.Value = Value.Int32ToBool();
                    }
                    else if (linkedPin is BitPin pin2)
                    {
                        pin2.Value = Value.Int32ToBits();
                    }
                    else if (linkedPin is IntegerPin pin3)
                    {
                        pin3.Value = Value;
                    }
                    else if (linkedPin is DoublePin pin4)
                    {
                        pin4.Value = Value;
                    }

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
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        public override Color GetCurrentColor()
        {
            return Color.cyan;
        }

        public override int GetBaseValue()
        {
            return 0;
        }
        public override bool Compare(int x, int y)
        {
            return x == y;
        }
    }
}