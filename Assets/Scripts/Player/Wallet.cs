using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public event Action<int> OnChanged;

    private int _amount;

    public void Add(int value)
    {
        _amount += value;
        OnChanged?.Invoke(_amount);
    }

    public void Reset()
    {
        _amount = 0;
        OnChanged?.Invoke(_amount);
    }
}
