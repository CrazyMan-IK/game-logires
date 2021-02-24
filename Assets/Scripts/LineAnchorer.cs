using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteAlways]
    public class LineAnchorer : MonoBehaviour
    {
        [SerializeField] private Transform _target1 = null;
        [SerializeField] private Transform _target2 = null;
        [SerializeField] private Vector3 _position1 = Vector3.zero;
        [SerializeField] private Vector3 _position2 = Vector3.zero;

        private Transform _transform = null;
        private LineRenderer _line = null;

        public Transform Target1 { get => _target1; set => _target1 = value; }
        public Transform Target2 { get => _target2; set => _target2 = value; }
        public Vector3 Position1 { get => _position1; set => _position1 = value; }
        public Vector3 Position2 { get => _position2; set => _position2 = value; }

        private void Awake()
        {
            _transform = transform;
            _line = GetComponent<LineRenderer>();

            _line.positionCount = 2;
            UpdatePositions();
        }

        private void Update()
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            if (_target1 == null)
            {
                _line.SetPosition(0, transform.position + _position1);
            }
            else
            {
                _line.SetPosition(0, _target1.position);
            }

            if (_target2 == null)
            {
                _line.SetPosition(1, transform.position + _position2);
            }
            else
            {
                _line.SetPosition(1, _target2.position);
            }
        }
    }
}