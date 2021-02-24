using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class ChargingChecker : Block, IHaveOutputs
    {
        [SerializeField] private BooleanPin _output = null;
#if UNITY_EDITOR
        [SerializeField] private bool _debugValue = false;
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
            AndroidNativeWrapper.OnBatteryChargingStateChanged += OnBatteryChargingStateChanged;
        }

        private void OnDisable()
        {
            AndroidNativeWrapper.OnBatteryChargingStateChanged -= OnBatteryChargingStateChanged;
        }

        private void OnBatteryChargingStateChanged(bool value)
        {
#if UNITY_EDITOR
            _output.Value = _debugValue;
#else
            _output.Value = value;
#endif
        }

        public override int GetID() => 11;
    }
}