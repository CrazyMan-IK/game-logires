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
            Vector3 oldPos1 = _line.GetPosition(0);
            Vector3 oldPos2 = _line.GetPosition(1);

            Vector3 pos1, pos2;

            if (_target1 == null)
            {
                pos1 = _transform.position + _position1;
            }
            else
            {
                pos1 = _target1.position;
            }

            if (_target2 == null)
            {
                pos2 = _transform.position + _position2;
            }
            else
            {
                pos2 = _target2.position;
            }

            _line.SetPosition(0, Vector3.Lerp(oldPos1, pos1, Time.deltaTime * 50));
            _line.SetPosition(1, Vector3.Lerp(oldPos2, pos2, Time.deltaTime * 50));
        }
    }
}