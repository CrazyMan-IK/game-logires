using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class Generator : Block, IHaveOutputs
    {
        [SerializeField] private BooleanPin _output = null;

        private static event Action OnSwitch;
        private static Coroutine _mainCoroutine = null;

        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void Start()
        {
            if (_mainCoroutine == null)
            {
                _mainCoroutine = StartCoroutine(MainCoroutine());
            }
        }

        private void OnApplicationQuit()
        {
            if (_mainCoroutine != null)
            {
                StopCoroutine(_mainCoroutine);
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

        private static IEnumerator MainCoroutine()
        {
            while (true)
            {
                OnSwitch?.Invoke();
                yield return new WaitForSeconds(0.25f);
            }
        }

        private void SwitchValue()
        {
            _output.Value = !_output.Value;
        }

        public override int GetID() => 5;
    }
}