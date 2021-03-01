using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class NotGate : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private BitPin _input = null;
        [SerializeField] private BitPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new[] { _input }; }
        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            UpdateOutput();
            _input.ValueChanged += OnInputValueChanged;
        }

        private void OnDisable()
        {
            _input.ValueChanged -= OnInputValueChanged;
        }

        private void OnInputValueChanged(Pin sender, List<bool> oldValue, List<bool> newValue)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            _output.Value = _input.Value.Select(x => !x).ToList();
        }

        public override int GetID() => 1;
    }
}