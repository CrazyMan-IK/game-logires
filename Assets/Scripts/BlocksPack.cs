using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires
{
    [CreateAssetMenu(fileName = "NewPack", menuName = "Logires/BlocksPack", order = 1000)]
    public class BlocksPack : ScriptableObject
    {
        [SerializeField] private List<GameObject> _components = new List<GameObject>();

        public IReadOnlyList<Block> Blocks { get => _components.Select(x => x.GetComponent<Block>()).ToList(); }
    }
}