using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Models;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public class WIFI_IN : WIFI, IHaveInputs
    {
        [SerializeField] private BitPin _input = null;

        public IReadOnlyList<Pin> Inputs { get => new[] { _input }; }

        private void OnEnable()
        {
            _input.ValueChanged += OnInputValueChanged;
        }

        private void OnDisable()
        {
            _input.ValueChanged -= OnInputValueChanged;
        }

        private void OnInputValueChanged(Pin sender, List<bool> oldValue, List<bool> newValue)
        {
            Multicaster.Instance.BroadcastMessage(new Message() { ID = _id, Value = newValue });
        }

        public override int GetID() => 12;
    }
}