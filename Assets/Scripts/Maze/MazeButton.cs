using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class MazeButton : MonoBehaviour
{
    public event Action OnClick;
    private Trigger _trigger;

    private bool _canUse = true;
        
    private void OnEnable() => _trigger.OnTriggerEnter += OnEnterHandler;
    private void OnDisable() => _trigger.OnTriggerEnter -= OnEnterHandler;

    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
    }

    private void OnEnterHandler(Collider2D other, Trigger requester)
    {
        if (_canUse) Use();
    }

    private void Use()
    {
        _canUse = false;
        OnClick?.Invoke();
    }
}