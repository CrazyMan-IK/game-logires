using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class OrGate : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private BitPin _inputA = null;
        [SerializeField] private BitPin _inputB = null;
        [SerializeField] private BitPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new[] { _inputA, _inputB }; }
        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            UpdateOutput();
            _inputA.ValueChanged += OnInputValueChanged;
            _inputB.ValueChanged += OnInputValueChanged;
        }

        private void OnDisable()
        {
            _inputA.ValueChanged -= OnInputValueChanged;
            _inputB.ValueChanged -= OnInputValueChanged;
        }

        private void OnInputValueChanged(Pin sender, List<bool> oldValue, List<bool> newValue)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            _output.Value = _inputA.Value.CustomZip(_inputB.Value, (a, b) => a || b).ToList();
        }

        public override int GetID() => 3;
    }
}