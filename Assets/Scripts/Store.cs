using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Purchasing;
using DG.Tweening;
using CrazyGames.Logires.UI;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Store : MonoBehaviour, IStoreListener
    {
        [SerializeField] private Transform _itemsHolder = null;
        [SerializeField] private PurchaseItemUI _purchaseItemPrefab = null;
        [SerializeField] private List<BlocksPack> _packs = new List<BlocksPack>();

        private IStoreController _storeController = null;
        private IExtensionProvider _storeExtensionProvider = null;
        private CanvasGroup _canvasGroup = null;

        private List<string> _consumables = new List<string>();
        private List<string> _nonConsumables = new List<string>();
        private bool _isOpened = false;

        private IReadOnlyList<string> _AllProducts => _consumables.Concat(_nonConsumables).ToList();

        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                if (_isOpened != value)
                {
                    _isOpened = value;

                    _canvasGroup.DOFade(Convert.ToInt32(_isOpened), 0.5f);
                    _canvasGroup.interactable = _isOpened;
                    _canvasGroup.blocksRaycasts = _isOpened;
                }
            }
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _consumables.AddRange(_packs.Select(x => x.Name + "_set"));
        }

        private void Start()
        {
            if (_storeController == null)
            {
                InitializePurchasing();
            }
        }

        private void RestorePurchasesCallback(bool result)
        {
            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore");
        }

        public void BuyProduct(Product product)
        {
            if (IsInitialized())
            {
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized");
            }
        }

        public void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                Product product = _storeController.products.WithID(productId);
                BuyProduct(product);
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized");
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in _consumables)
            {
                builder.AddProduct(product, ProductType.Consumable);
            }
            foreach (var product in _nonConsumables)
            {
                builder.AddProduct(product, ProductType.NonConsumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized");
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("RestorePurchases started ...");

                var android = _storeExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();
                android.RestoreTransactions(RestorePurchasesCallback);
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");

            _storeController = controller;
            _storeExtensionProvider = extensions;

            var isSetExp = new Regex(@"(\w+)_set$");

            foreach (var product in controller.products.set)
            {
                var match = isSetExp.Match(product.definition.id);
                var prefab = _purchaseItemPrefab;

                if (match.Success && match.Groups[1].Success)
                {
                    var pack = _packs.Find(x => match.Groups[1].Value == x.Name);
                    if (pack.CustomPrefab != null)
                    {
                        prefab = pack.CustomPrefab;
                    }
                }

                var item = Instantiate(prefab, _itemsHolder);
                item.Product = product;
                item.PurchaseRequest += BuyProduct;
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason: " + error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (_AllProducts.Contains(purchaseEvent.purchasedProduct.definition.id, StringComparer.Ordinal))
            {
                Debug.Log($"ProcessPurchase: PASS. Product: '{purchaseEvent.purchasedProduct.definition.id}'");

                var match = Regex.Match(purchaseEvent.purchasedProduct.definition.id, @"(\w+)_set$");

                if (match.Success && match.Groups[1].Success)
                {
                    AndroidNativeWrapper.CallMethod("ShowToast", new object[] { purchaseEvent.purchasedProduct.definition.id, 0 });
                    EncryptedGlobalPreferences.SetPrimitive($"{match.Value}_activated", true);
                    EncryptedGlobalPreferences.SetPrimitive($"any_set_activated", true);
                }
                else
                {
                    /*
                    switch (purchaseEvent.purchasedProduct.definition.id)
                    {
                        
                    }
                    */
                }
            }
            else
            {
                Debug.Log($"ProcessPurchase: FAIL. Unrecognized product: '{purchaseEvent.purchasedProduct.definition.id}'");
            }

            return PurchaseProcessingResult.Complete;
        }
    }
}
