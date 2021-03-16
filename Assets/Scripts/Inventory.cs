using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CrazyGames.Logires.UI;

namespace CrazyGames.Logires
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance = null;

        [SerializeField] private ScrollRect _scroller = null;
        [SerializeField] private GameObject _containerPrefab = null;
        [SerializeField] private Transform _contentHolder = null;
        [SerializeField] private List<BlocksPack> _packs = new List<BlocksPack>();

        public List<BlocksPack> Packs { get => _packs; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }
            Instance = this;
        }

        private void Start()
        {
            foreach (var pack in _packs)
            {
                foreach (var block in pack.Blocks)
                {
                    var tempObject = Instantiate(_containerPrefab, _contentHolder);
                    tempObject.name = "ElementUI";

                    var blockUI = tempObject.GetComponent<BlockUI>();

                    blockUI.OnEBeginDrag += _scroller.OnBeginDrag;
                    blockUI.OnEDrag += _scroller.OnDrag;
                    blockUI.OnEEndDrag += _scroller.OnEndDrag;
                    
                    blockUI.transform.localPosition = (Vector2)blockUI.transform.localPosition;

                    blockUI.Block = block;
                }
            }
        }

        public Block GetBlock(int id)
        {
            return _packs.SelectMany(x => x.Blocks).FirstOrDefault(x => x.GetID() == id);
        }

        public BlockUI GetBlockUI<T>() where T : Block
        {
            return _scroller.content.GetComponentsInChildren<BlockUI>().FirstOrDefault(x => x.Block.GetType() == typeof(T));
        }
    }
}