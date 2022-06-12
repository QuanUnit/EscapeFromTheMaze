using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopProduct : MonoBehaviour
{
    public Toggle Toggle => _toggle;
    public int SkinIndex => _skinIndex;
    public bool IsBuying { get; private set; }

    [SerializeField] private int _skinIndex;
    [SerializeField] private int _cost;
    [SerializeField] private GameObject _locker;
    [SerializeField] private Toggle _toggle;

    private void OnEnable() => _toggle.onValueChanged.AddListener(ToggleChangeValueHandle);

    private void OnDisable() => _toggle.onValueChanged.RemoveListener(ToggleChangeValueHandle);

    private void Start()
    {
        if (_locker.activeInHierarchy == false) Unlock();
    }

    public void Unlock()
    {
        IsBuying = true;
        _locker.SetActive(false);
        SkinsStorage.Instance.SetBuyingState(_skinIndex, true);
    }

    private void Buy()
    {
        if (IsBuying) return;

        Unlock();
        Select();
    }

    private void Select()
    {
        SkinsStorage.Instance.CurrentSkinIndex = _skinIndex;
    }

    private void ToggleChangeValueHandle(bool value)
    {
        if(value == true)
        {
            if (IsBuying)
            {
                Select();
            }
            else
            {
                Buy();
            }
        }
    }
}
