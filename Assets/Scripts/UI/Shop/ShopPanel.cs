using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private List<ShopProduct> _products;

    private ISelectable _activeSelect;

    private void Awake()
    {
        GlobalWallet.ValueChanged += MoneyChangedHandle;

        foreach (var product in _products)
        {
            product.Selected += () => ProductSelectionHandle(product);
        }
    }

    private void Start()
    {
        Init();
        _moneyText.text = GlobalWallet.Value.ToString();
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
            if (buyingState == true) product.UnlockImmediate();

            if (skinIndex == index)
                product.Select();
        }
    }

    private void MoneyChangedHandle(int newValue)
    {
        _moneyText.text = newValue.ToString();
    }

    private void ProductSelectionHandle(ShopProduct product)
    {
        if (_activeSelect != null) _activeSelect.Unselect();

        _activeSelect = product;
    }
}
