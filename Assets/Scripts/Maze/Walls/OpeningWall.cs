using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OpeningWall : SimpleWall
{
    public event Action OnOpened;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        _animator.SetTrigger("Open");
        //gameObject.SetActive(false);
        OnOpened?.Invoke();
    }
}
