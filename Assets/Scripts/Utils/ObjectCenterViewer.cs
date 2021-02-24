using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    [ExecuteAlways]
    public class ObjectCenterViewer : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.zero;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.left * _size.x);
            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.up * _size.y);
            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.forward * _size.z);
            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.right * _size.x);
            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.down * _size.y);
            Gizmos.DrawLine(_transform.position, _transform.position + Vector3.back * _size.z);
        }
    }
}