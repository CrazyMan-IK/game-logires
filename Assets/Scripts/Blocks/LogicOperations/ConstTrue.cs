using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class ConstTrue : Block, IHaveOutputs
    {
        [SerializeField] private BooleanPin _output = null;

        public IReadOnlyList<Pin> Outputs { get => new[] { _output }; }

        private void OnEnable()
        {
            _output.Value = true;
        }

        public override int GetID() => 0;
    }
}