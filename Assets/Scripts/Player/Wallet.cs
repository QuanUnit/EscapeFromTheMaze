using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, IPropertyChangeNotifier
{
    public event Action<object> PropertyOnChanged;

    private int _amount;

    public void Add(int value)
    {
        _amount += value;
        PropertyOnChanged?.Invoke(_amount);
    }

    public void Reset()
    {
        _amount = 0;
        PropertyOnChanged?.Invoke(_amount);
    }
}
