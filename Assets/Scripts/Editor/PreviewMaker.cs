using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CrazyGames
{
    public class PreviewMaker : EditorWindow
    {
        private GameObject _target = null;
        private Vector2Int _size = Vector2Int.zero;

        [MenuItem("Window/Preview maker")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PreviewMaker));
        }

        private void OnGUI()
        {
            _target = (GameObject)EditorGUILayout.ObjectField("", _target, typeof(GameObject), false);
            _size = EditorGUILayout.Vector2IntField("", _size);

            if (GUILayout.Button("Make preview"))
            {
                if (_target != null)
                {
                    //_target = Resources.Load("temp.prefab");
                    //var preview = AssetPreview.GetMiniThumbnail(_target);

                    RuntimePreviewGenerator.PreviewDirection = new Vector3(0, 0, 1);
                    RuntimePreviewGenerator.OrthographicMode = true;
                    RuntimePreviewGenerator.BackgroundColor = new Color(0, 0, 0, 0);
                    RuntimePreviewGenerator.MarkTextureNonReadable = false;

                    var preview = RuntimePreviewGenerator.GenerateModelPreview(_target.transform, _size.x, _size.y, true);
                    var image = preview.EncodeToPNG();
                    System.IO.File.WriteAllBytes(@"D:\Install\Projects\UnityProjects\Logires\Assets\Sprites\Temp.png", image);
                }
            }
        }
    }
}