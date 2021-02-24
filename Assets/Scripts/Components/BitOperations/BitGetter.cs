using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class BitGetter : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private BitPin _inputA = null;
        [SerializeField] private IntegerPin _inputB = null;
        [SerializeField] private BooleanPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new Pin[] { _inputA, _inputB }; }
        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            UpdateOutput();
            _inputA.ValueChanged += OnInputAValueChanged;
            _inputB.ValueChanged += OnInputBValueChanged;
        }

        private void OnDisable()
        {
            _inputA.ValueChanged -= OnInputAValueChanged;
            _inputB.ValueChanged -= OnInputBValueChanged;
        }

        private void OnInputAValueChanged(Pin sender, List<bool> oldValue, List<bool> newValue)
        {
            UpdateOutput();
        }

        private void OnInputBValueChanged(Pin sender, int oldValue, int newValue)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            _output.Value = _inputA.Value.ElementAtOrDefault(_inputB.Value);
        }

        public override int GetID() => 7;
    }
}