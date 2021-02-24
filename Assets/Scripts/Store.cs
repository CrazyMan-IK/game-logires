using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace CrazyGames.Logires
{
    public class Store : MonoBehaviour, IStoreListener
    {
        private const string _ProductIDSystemSet = "system_set";

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        private void Start()
        {
            if (_storeController == null)
            {
                InitializePurchasing();
            }
        }

        private void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                Product product = _storeController.products.WithID(productId);

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

        private void RestorePurchasesCallback(bool result)
        {
            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore");
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct(_ProductIDSystemSet, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        public void BuySystemSet()
        {
            BuyProductID(_ProductIDSystemSet);
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
            if (string.Equals(purchaseEvent.purchasedProduct.definition.id, _ProductIDSystemSet, StringComparison.Ordinal))
            {
                Debug.Log($"ProcessPurchase: PASS. Product: '{purchaseEvent.purchasedProduct.definition.id}'");

                Utils.AndroidNativeWrapper.CallMethod("ShowToast", new object[] { purchaseEvent.purchasedProduct.definition.id, 0 });
            }
            else
            {
                Debug.Log($"ProcessPurchase: FAIL. Unrecognized product: '{purchaseEvent.purchasedProduct.definition.id}'");
            }

            return PurchaseProcessingResult.Complete;
        }
    }
}
