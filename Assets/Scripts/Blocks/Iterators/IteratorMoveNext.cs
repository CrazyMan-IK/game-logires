using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class IteratorMoveNext : Block, IHaveInputs, IHaveOutputs
    {
        [SerializeField] private IteratorPin _input = null;
        [SerializeField] private IteratorPin _output = null;

        public IReadOnlyList<Pin> Inputs { get => new Pin[] { _input }; }
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

        private void OnInputValueChanged(Pin sender, LinkedListNode<object> oldValue, LinkedListNode<object> newValue)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            _output.Value = _input.Value?.Next;
        }

        public override int GetID() => 14;
    }
}