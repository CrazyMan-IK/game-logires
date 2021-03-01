using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Interfaces;
using CrazyGames.Logires.Utils;
using System;

namespace CrazyGames.Logires
{
    public class Logger : Block, IHaveInputs
    {
        [SerializeField] private BitPin _input = null;
        [SerializeField] private TMPro.TMP_Text _booleanText = null;
        [SerializeField] private TMPro.TMP_Text _bitText = null;
        [SerializeField] private TMPro.TMP_Text _integerText = null;
        [SerializeField] private TMPro.TMP_Text _doubleText = null;

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
            _booleanText.text = newValue.BitsToBool() ? "1" : "0";
            _bitText.text = newValue.Select(x => x ? "1" : "0").Aggregate((x, y) => x + "\u00AD" + y);
            _integerText.text = newValue.BitsToInt32().ToString();
            _doubleText.text = newValue.BitsToDouble().ToString();

#if UNITY_EDITOR
            string message = "------------\n" +
                $"{(newValue.BitsToBool() ? "1" : "0")}\n" +
                $"{newValue.Select(x => (x ? 1 : 0).ToString()).Aggregate((x, y) => x + y)}\n" +
                $"{newValue.BitsToInt32()}\n" +
                $"{newValue.BitsToDouble()}\n" +
                "------------";

            Debug.Log(message);
#endif
        }

        public override int GetID() => 8;
    }
}