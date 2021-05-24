using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;
using CrazyGames.Logires.UI;

namespace CrazyGames.Logires
{
    [CreateAssetMenu(fileName = "NewPack", menuName = "Logires/BlocksPack", order = 1000)]
    public class BlocksPack : ScriptableObject
    {
        [SerializeField] private string _name = "<Unknown>";
        [SerializeField] private bool _forceActivated = false;
        [SerializeField] private bool _buyAnyPackToActivate = false;
        [SerializeField] private PurchaseItemUI _customPurchaseItemPrefab = null;
        [SerializeField] private List<GameObject> _components = new List<GameObject>();

        public string Name => _name;

        public IReadOnlyList<Block> Blocks => _components.Select(x => x.GetComponent<Block>()).ToList();
        
        public PurchaseItemUI CustomPrefab => _customPurchaseItemPrefab;

        public virtual bool IsActivated()
        {
#if UNITY_EDITOR
            return true;
#else
            var anyActivated = EncryptedGlobalPreferences.GetPrimitive($"any_set_activated", false);

            var result = EncryptedGlobalPreferences.GetPrimitive($"{_name}_set_activated", false);

            return _forceActivated || (_buyAnyPackToActivate && anyActivated.Value) || result.Value;
#endif
        }
    }
}