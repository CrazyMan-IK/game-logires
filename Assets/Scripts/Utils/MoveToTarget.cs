using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    [ExecuteAlways]
    public class MoveToTarget : MonoBehaviour
    {
        [SerializeField] private Transform _target = null;

        [SerializeField] private bool _smoothedP = false;
        [SerializeField] private float _smoothSpeedP = 0;
        [SerializeField] private bool _lockPX = false;
        [SerializeField] private bool _lockPY = false;
        [SerializeField] private bool _lockPZ = false;
        [SerializeField] private Vector3 _offsetPos = Vector3.zero;

        private Transform _transform = null;

        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_target != null)
            {
                var px = _lockPX ? _transform.position.x : _target.position.x + _offsetPos.x;
                var py = _lockPY ? _transform.position.y : _target.position.y + _offsetPos.y;
                var pz = _lockPZ ? _transform.position.z : _target.position.z + _offsetPos.z;

                if (_smoothedP)
                {
                    _transform.position = Vector3.Lerp(_transform.position, new Vector3(px, py, pz), Time.deltaTime * _smoothSpeedP);
                }
                else
                {
                    transform.position = new Vector3(px, py, pz);
                }
            }
        }
    }
}