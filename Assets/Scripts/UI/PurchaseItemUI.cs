using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

namespace CrazyGames.Logires.UI
{
    public class PurchaseItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _priceText = null;
        [SerializeField] private TextMeshProUGUI _descriptionText = null;

        private Product _product = null;

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
                        _titleText.text = _product.metadata.localizedTitle;

                        if (_titleText.text.EndsWith(" (Logires)"))
                        {
                            var suffixIndex = _titleText.text.LastIndexOf(" (Logires)");
                            _titleText.text = _titleText.text.Remove(suffixIndex);
                        }

                        _priceText.text = _product.metadata.localizedPriceString;
                        _descriptionText.text = _product.metadata.localizedDescription;
                    }
                }
            }
        }

        public void BuyProduct()
        {
            PurchaseRequest?.Invoke(Product);
        }
    }
}
