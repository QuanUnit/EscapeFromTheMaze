using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ShopProduct : MonoBehaviour, IPointerDownHandler, ISelectable
{
    public int SkinIndex => _skinIndex;
    public bool IsBuying { get; private set; }

    public event Action Selected;

    public bool IsSelected => _isSelected;

    [SerializeField] private int _skinIndex;
    [SerializeField] private int _cost;
    [SerializeField] private GameObject _locker;
    [SerializeField] private GameObject _selector;
    [SerializeField] private TMP_Text _costText;

    private bool _isSelected;
    private Animator _animator;

    private void Awake()
    {
        _costText.text = _cost.ToString();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_locker.activeInHierarchy == false) Unlock();
    }

    public void UnlockImmediate()
    {
        IsBuying = true;
        SkinsStorage.Instance.SetBuyingState(_skinIndex, true);
        _locker.SetActive(false);
    }

    public void Unlock()
    {
        IsBuying = true;
        _animator.SetTrigger("Unlock");
        SkinsStorage.Instance.SetBuyingState(_skinIndex, true);
    }

    private void Buy()
    {
        if (IsBuying) return;

        GlobalWallet.Value -= _cost;
        Unlock();
        Select();
    }

    public void Select()
    {
        _isSelected = true;
        SkinsStorage.Instance.CurrentSkinIndex = _skinIndex;
        _selector.SetActive(true);
        Selected.Invoke();
    }

    public void Unselect()
    {
        _selector.SetActive(false);
        _isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsBuying == false)
        {
            int currentMoney = GlobalWallet.Value;

            if (currentMoney < _cost)
            {
                _animator.SetTrigger("NotMoney");
                return;
            }

            Buy();
            return;
        }

        if(_isSelected == false) Select();
    }
}
