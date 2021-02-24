using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class BitMerger : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private BitPin _inputA = null;
        [SerializeField] private BitPin _inputB = null;
        [SerializeField] private BitPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new[] { _inputA, _inputB }; }
        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            _output.Value = _inputA.Value.Concat(_inputB.Value).ToList();
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
            _output.Value = _inputA.Value.Concat(_inputB.Value).ToList();
        }

        public override int GetID() => 6;
    }
}