using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class Generator : Block, IHaveOutputs
    {
        [SerializeField] private BooleanPin _output = null;

        private static event Action OnSwitch = null;
        private static Coroutine _mainCoroutine = null;
        private static bool _currentValue = false;

        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void Start()
        {
            if (_mainCoroutine == null)
            {
                _mainCoroutine = GlobalTicker.Instance.StartDelayed(0.25f, () =>
                {
                    _currentValue = !_currentValue;
                    OnSwitch?.Invoke();
                });
            }
        }

        private void OnEnable()
        {
            OnSwitch += SwitchValue;
        }

        private void OnDisable()
        {
            OnSwitch -= SwitchValue;
        }

        public static void StopTicker()
        {
            if (_mainCoroutine != null)
            {
                GlobalTicker.Instance.StopDelayed(_mainCoroutine);
                _mainCoroutine = null;
            }
        }

        private void SwitchValue()
        {
            try
            {
                _output.Value = _currentValue;
            }
            catch { }
        }

        public override int GetID() => 5;
    }
}