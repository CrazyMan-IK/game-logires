using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using CrazyGames.Logires.Utils;

namespace CrazyGames.Logires.UI
{
    public class PurchaseItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _priceText = null;
        [SerializeField] private TextMeshProUGUI _descriptionText = null;

        private Product _product = null;
        private LocalizedString _title = new LocalizedString();
        private LocalizedString _description = new LocalizedString();

        public event Action<Product> PurchaseRequest;

        public Product Product 
        {
            get { return _product; } 
            set
            {
                if (_product != value)
                {
                    _product = value;

                    if (_product != null)
                    {
                        //_titleText.text = _product.metadata.localizedTitle;

                        if (_titleText.text.EndsWith(" (Logires)"))
                        {
                            var suffixIndex = _titleText.text.LastIndexOf(" (Logires)");
                            _titleText.text = _titleText.text.Remove(suffixIndex);
                        }

                        _priceText.text = _product.metadata.localizedPriceString;
                        //_descriptionText.text = _product.metadata.localizedDescription;

                        _title.SetReference("Base", $"{_product.definition.id}_title");
                        _description.SetReference("Base", $"{_product.definition.id}_description");
                    }
                }
            }
        }

        private void OnEnable()
        {
            _title.StringChanged += TitleChanged;
            _description.StringChanged += DescriptionChanged;
        }

        private void OnDisable()
        {
            _title.StringChanged -= TitleChanged;
            _description.StringChanged -= DescriptionChanged;
        }

        private void TitleChanged(string value)
        {
            _titleText.text = value;
        }

        private void DescriptionChanged(string value)
        {
            _descriptionText.text = value;
        }

        public void BuyProduct()
        {
            PurchaseRequest?.Invoke(Product);
        }

        public void ShowDetails()
        {
            var oper = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Base", "store_purchase_warning");
            oper.Completed += (oper) =>
            {
                AndroidNativeWrapper.CallMethod("ShowToast", new object[] { oper.Result, 0 });
            };
        }
    }
}
