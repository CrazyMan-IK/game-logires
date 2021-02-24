using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires
{
    public class BlocksRoot : MonoBehaviour
    {
        private static BlocksRoot _instance = null;

        private List<Block> _blocks = new List<Block>();

        public IReadOnlyList<Block> Blocks { get => _blocks; }

        public static BlocksRoot Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void OnBlockDestroyed(Block block)
        {
            _blocks.Remove(block);
        }

        public void AddBlock(Block block)
        {
            block.OnDestroyed += OnBlockDestroyed;
            _blocks.Add(block);
        }
    }
}
