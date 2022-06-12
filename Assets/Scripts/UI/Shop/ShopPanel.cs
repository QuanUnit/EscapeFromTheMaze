using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private List<ShopProduct> _products;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        UpdateSkinsState();
    }

    private void UpdateSkinsState()
    {
        int index = SkinsStorage.Instance.CurrentSkinIndex;

        foreach (var product in _products)
        {
            int skinIndex = product.SkinIndex;

            bool buyingState = SkinsStorage.Instance.GetBuyingState(skinIndex);
            if (buyingState == true) product.Unlock();

            if (skinIndex == index)
                product.Toggle.isOn = true;
        }
    }
}
