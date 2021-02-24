using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    public class BlockUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SVGImage _elementImage = null;

        private Block _block = null;
        private Camera _mainCamera = null;
        private Block _lastCreated = null;

        public event Action<PointerEventData> OnEBeginDrag;
        public event Action<PointerEventData> OnEDrag;
        public event Action<PointerEventData> OnEEndDrag;

        public Block Block 
        { 
            get => _block; 
            set
            {
                if (_block != value)
                {
                    _block = value;
                    if (_block != null)
                    {
                        _elementImage.sprite = _block.Preview;

                        //var compSprite = _component.GetComponent<SpriteRenderer>();
                        //_elementImage.color = compSprite.color;
                        //_elementImage.sprite = compSprite.sprite;
                    }
                }
            }
        }

        public Transform ElementImage { get => _elementImage.transform; }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnEBeginDrag?.Invoke(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (_lastCreated != null)
            {
                _lastCreated.OnDrag(eventData);
            }
            else
            {
                OnEDrag?.Invoke(eventData);

                var result = EventSystem.current.RaycastUI(eventData).FirstOrDefault();

                if (result.gameObject == null || result.gameObject.CompareTag("InputPanel"))
                {
                    _lastCreated = Instantiate(_block.gameObject, (Vector2)_mainCamera.ScreenToWorldPoint(eventData.position), _block.transform.rotation, BlocksRoot.Instance.transform).GetComponent<Block>();

                    BlocksRoot.Instance.AddBlock(_lastCreated);

                    _lastCreated.transform.localScale = _block.transform.localScale;
                    _lastCreated.name = _lastCreated.name.Replace("(Clone)", "");

                    EventSystem.current.SetSelectedGameObject(_lastCreated.gameObject);

                    _lastCreated.OnBeginDrag(eventData);
                    OnEEndDrag?.Invoke(eventData);
                }

                //Debug.Log(result.gameObject);

                //UnityEngine.UI.GraphicRaycaster.ra.Raycast(_mainCamera.ScreenToWorldPoint(eventData.position), Vector2.zero, 100, LayerMask.GetMask("UI"));
                //Debug.Log(raycast.collider);

                //var tempPos = _mainCamera.ScreenToWorldPoint(eventData.position);
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_lastCreated != null)
            {
                _lastCreated.OnEndDrag(eventData);
                _lastCreated = null;
            }
            OnEEndDrag?.Invoke(eventData);
        }
    }
}