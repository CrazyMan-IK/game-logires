using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using DG.Tweening;
using CrazyGames.Logires.UI;
using CrazyGames.Logires.Utils;
using CrazyGames.Logires.Interfaces;

namespace CrazyGames.Logires
{
    public abstract class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, IEndDragHandler
    {
        private const float SNAP_GRID_SIZE = 1;
        private readonly Color GHOST_COLOR = new Color(0.8f, 0, 0);

        protected Transform _transform = null;
        protected Collider2D _collider = null;
        protected SpriteRenderer _renderer = null;
        protected Camera _mainCamera = null;

        private Color _defaultColor = Color.white;
        private Vector2? _lastFreePos = null;
        private Sprite _preview = null;

        private Dictionary<int, int> _lastSortingOrders = new Dictionary<int, int>();

        public event Action<Block> OnDestroyed;

        public Sprite Preview 
        {
            get
            {
                if (_preview == null)
                {
                    _preview = GetPreview();
                }    
                return _preview;
            }
        }

        private void Awake()
        {
            _transform = transform;
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;

            RuntimePreviewGenerator.PreviewDirection = new Vector3(0, 0, 1);
            RuntimePreviewGenerator.OrthographicMode = true;
            RuntimePreviewGenerator.BackgroundColor = new Color(0, 0, 0, 0);
            RuntimePreviewGenerator.MarkTextureNonReadable = false;

            _defaultColor = _renderer.color;

            //SendMessageUpwards("OnBlockAdded", this);
        }

        protected void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        public abstract int GetID();

        public Sprite GetPreview()
        {
            IHaveInputs haveInputs = null;
            IHaveOutputs haveOutputs = null;

            if (this is IHaveInputs)
            {
                haveInputs = (IHaveInputs)this;
                foreach (var input in haveInputs.Inputs)
                {
                    input.GetComponent<LineRenderer>().enabled = false;
                }
            }
            if (this is IHaveOutputs)
            {
                haveOutputs = (IHaveOutputs)this;
                foreach (var output in haveOutputs.Outputs)
                {
                    output.GetComponent<LineRenderer>().enabled = false;
                }
            }

            var preview = RuntimePreviewGenerator.GenerateModelPreview(transform, 512, 512, true);
#if UNITY_EDITOR
            var image = preview.EncodeToPNG();
            System.IO.File.WriteAllBytes($@"D:\Install\Projects\UnityProjects\Logires\Assets\Sprites\Temp\{name}.png", image);
#endif

            if (this is IHaveInputs)
            {
                foreach (var input in haveInputs.Inputs)
                {
                    input.GetComponent<LineRenderer>().enabled = true;
                }
            }
            if (this is IHaveOutputs)
            {
                foreach (var output in haveOutputs.Outputs)
                {
                    output.GetComponent<LineRenderer>().enabled = true;
                }
            }

            return Sprite.Create(preview, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            var canvases = GetComponentsInChildren<Canvas>(true);
            foreach (var renderer in renderers)
            {
                _lastSortingOrders[renderer.GetInstanceID()] = renderer.sortingOrder;
                renderer.sortingOrder += 15000;
            }
            foreach (var canvas in canvases)
            {
                _lastSortingOrders[canvas.GetInstanceID()] = canvas.sortingOrder;
                canvas.overrideSorting = true;
                canvas.sortingOrder += 15000;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount == 1 || Input.touchCount == 0)
            {
                var viewPos = (_mainCamera.ScreenToViewportPoint(eventData.position) - new Vector3(0.5f, 0.5f)) * 2;
                if (Mathf.Abs(viewPos.x) < -0.85 || Mathf.Abs(viewPos.x) > 0.85)
                {
                    _mainCamera.transform.position += new Vector3(viewPos.x * 30 * Time.deltaTime, 0);
                }
                if (Mathf.Abs(viewPos.y) < -0.85 || Mathf.Abs(viewPos.y) > 0.85)
                {
                    _mainCamera.transform.position += new Vector3(0, viewPos.y * 30 * Time.deltaTime);
                }
                var tempPos = _mainCamera.ScreenToWorldPoint(eventData.position);

                tempPos.x = (float)Math.Round(tempPos.x / SNAP_GRID_SIZE, MidpointRounding.AwayFromZero) * SNAP_GRID_SIZE;
                tempPos.y = (float)Math.Round(tempPos.y / SNAP_GRID_SIZE, MidpointRounding.AwayFromZero) * SNAP_GRID_SIZE;

                var newPos = new Vector3(tempPos.x, tempPos.y, _transform.position.z);

                var nearestComponents = Physics2D.OverlapBoxAll(newPos, new Vector2(_collider.bounds.size.x, _collider.bounds.size.y), 0).Where(x => !x.transform.IsChildOf(_transform)).ToArray();

                _transform.position = newPos;
                if (nearestComponents.Length == 0)
                {
                    _renderer.color = _defaultColor;
                    _lastFreePos = newPos;
                }
                else
                {
                    _renderer.color = GHOST_COLOR;
                }
            }
        }
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var result = EventSystem.current.RaycastUI(eventData).FirstOrDefault();

            if (_lastFreePos != null && result.gameObject != null && (!result.gameObject.CompareTag("Inventory") && !result.gameObject.CompareTag("GateUI")))
            {
                if (_lastSortingOrders.Count > 0)
                {
                    var renderers = GetComponentsInChildren<SpriteRenderer>(true);
                    var canvases = GetComponentsInChildren<Canvas>(true);
                    foreach (var renderer in renderers)
                    {
                        var id = renderer.GetInstanceID();
                        if (_lastSortingOrders.ContainsKey(id))
                        {
                            renderer.sortingOrder = _lastSortingOrders[id];
                        }
                    }
                    foreach (var canvas in canvases)
                    {
                        var id = canvas.GetInstanceID();
                        if (_lastSortingOrders.ContainsKey(id))
                        {
                            canvas.sortingOrder = _lastSortingOrders[id];
                            canvas.overrideSorting = false;
                        }
                    }

                    _lastSortingOrders.Clear();
                }

                _transform.position = _lastFreePos.Value;

                Analytics.CustomEvent("block_pasted", new Dictionary<string, object>
                {
                    { "type_name", GetType().Name },
                    { "id", GetID() }
                });
            }
            else
            {
                var curType = GetType();
                var getter = Inventory.Instance.GetType().GetMethod(nameof(Inventory.GetBlockUI)).MakeGenericMethod(curType);
                var ui = (BlockUI)getter?.Invoke(Inventory.Instance, null);

                if (this is IHaveInputs haveInputs)
                {
                    foreach (var input in haveInputs.Inputs)
                    {
                        input.Disconnect();
                    }
                }
                if (this is IHaveOutputs haveOutputs)
                {
                    foreach (var output in haveOutputs.Outputs)
                    {
                        output.Disconnect();
                    }
                }

                if (ui != null)
                {
                    var elemTrans = ui.ElementImage.GetComponent<RectTransform>();

                    _transform.SetParent(ui.transform);
                    var canvases = GetComponentsInChildren<Canvas>(true);
                    foreach (var canvas in canvases)
                    {
                        _lastSortingOrders[canvas.GetInstanceID()] = canvas.sortingOrder;
                        canvas.overrideSorting = true;
                    }

                    var tween1 = _transform.DOScale(elemTrans.localScale, 0.5f);
                    var tween2 = _transform.DOLocalMove(elemTrans.localPosition, 0.5f);

                    var seq = DOTween.Sequence().Join(tween1).Join(tween2).OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            _renderer.color = _defaultColor;
        }
    }
}
