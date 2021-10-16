using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class ScorePoint : MonoBehaviour
{
    private Trigger _trigger;

    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
    }

    private void OnEnable() => _trigger.OnTriggerEnter += OnEnterHandler;
    private void OnDisable() => _trigger.OnTriggerEnter -= OnEnterHandler;

    private void OnEnterHandler(Collider2D other, Trigger sender)
    {
        if (other.TryGetComponent<Wallet>(out var wallet) == false)
            return;

        wallet.Add(1);
        Destroy(gameObject);
    }
}
