using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CrazyGames.Logires
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private float _refreshRate = 1f;

        private float _timer = 0;

        private void Update()
        {
            if (Time.unscaledTime > _timer)
            {
                _text.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
                _timer = Time.unscaledTime + _refreshRate;
            }
        }
    }
}
