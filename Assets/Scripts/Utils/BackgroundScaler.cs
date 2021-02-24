using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    [ExecuteAlways]
    public class BackgroundScaler : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Vector2 _originalSize = Vector2.zero;

        private void Update()
        {
            float width = _camera.pixelWidth;

            var bottomLeft = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            var bottomRight = _camera.ScreenToWorldPoint(new Vector2(width, 0));

            var w = new Vector2(bottomRight.x - bottomLeft.x, bottomRight.y - bottomLeft.y);

            var ScaleX = w.magnitude;

            var ScaleY = ScaleX * Screen.height / Screen.width;

            transform.localScale = new Vector3(ScaleX / (_originalSize.x / 100), ScaleY / (_originalSize.y / 100), 1f);
        }
    }
}