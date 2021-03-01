using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class RangeGetter : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private IntegerPin _inputA = null;
        [SerializeField] private IntegerPin _inputB = null;
        [SerializeField] private IteratorPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new Pin[] { _inputA, _inputB }; }
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

        private void OnInputValueChanged(Pin sender, int oldValue, int newValue)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            var min = Mathf.Min(_inputA.Value, _inputB.Value);
            var max = Mathf.Max(_inputA.Value, _inputB.Value);

            _output.Value = new LinkedList<object>(Enumerable.Range(min, max - min + 1).Select(x => (object)x)).First;
        }

        public override int GetID() => 15;
    }
}