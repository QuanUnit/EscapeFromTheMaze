using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class MazeButton : MonoBehaviour
{
    private IInteractive _interactiveTarget;
    private Trigger _trigger;
    
    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
    }

    public void Initialize(IInteractive target)
    {
        _interactiveTarget = target;
        _trigger.OnTriggerEnter += OnEnterHandler;
    }

    private void OnEnterHandler(Collider2D other, Trigger requester)
    {
        _interactiveTarget.Interact();
        _trigger.OnTriggerEnter -= OnEnterHandler;
    }
}