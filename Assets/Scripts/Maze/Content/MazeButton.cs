using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger), typeof(Animator))]
public class MazeButton : MonoBehaviour
{
    public event Action OnClick;

    [SerializeField] private int _reward;
    
    private Trigger _trigger;
    private Animator _animator;

    private bool _canUse = true;
        
    private void OnEnable() => _trigger.OnTriggerEnter += OnEnterHandler;
    private void OnDisable() => _trigger.OnTriggerEnter -= OnEnterHandler;

    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnterHandler(Collider2D other, Trigger sender)
    {
        if(other.TryGetComponent<Wallet>(out var wallet) == true)
            if(_canUse) wallet.Add(_reward);
        
        if (other.TryGetComponent<PlayerController>(out var player) == true)
            if (_canUse) Use();
    }

    private void Use()
    {
        _canUse = false;
        _animator.SetTrigger("Click");
        OnClick?.Invoke();
    }
}