using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Trigger), typeof(Animator))]
public class MazeButton : MonoBehaviour
{
    public UnityEvent OnClick;

    [SerializeField] private int _reward;
    
    private Trigger _trigger;
    private Animator _animator;

    private bool _canUse = true;
        
    private void OnEnable() => _trigger.OnTriggerEnter.AddListener(OnEnterHandler);
    private void OnDisable() => _trigger.OnTriggerEnter.RemoveListener(OnEnterHandler);

    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnterHandler(Collider2D other, Trigger sender)
    {
        if (other.TryGetComponent<PlayerController>(out var player) == true &&
            other.TryGetComponent<Wallet>(out var wallet) == true)
        {
            if (_canUse)
            {
                Use();
                wallet.Add(_reward);
            }
        }
    }

    private void Use()
    {
        _canUse = false;
        _animator.SetTrigger("Click");
        OnClick?.Invoke();
    }
}