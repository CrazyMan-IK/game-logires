using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class BatteryLevelGetter : Block, IHaveOutputs
    {
        [SerializeField] private DoublePin _output = null;
#if UNITY_EDITOR
        [SerializeField] private double _debugValue = 50;
#endif

        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void Start()
        {
#if UNITY_EDITOR
            _output.Value = _debugValue;
#endif
        }

        private void OnEnable()
        {
            AndroidNativeWrapper.OnBatteryLevelChanged += OnBatteryLevelChanged;
        }

        private void OnDisable()
        {
            AndroidNativeWrapper.OnBatteryLevelChanged -= OnBatteryLevelChanged;
        }

        private void OnBatteryLevelChanged(double value)
        {
#if UNITY_EDITOR
            _output.Value = _debugValue;
#else
            _output.Value = value;
#endif
        }

        public override int GetID() => 10;
    }
}