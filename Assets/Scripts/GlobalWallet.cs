using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalWallet
{
    public static event Action<int> ValueChanged;

    private static string MoneyKey = nameof(MoneyKey);

    public static int Value
    {
        get => PlayerPrefs.GetInt(MoneyKey, 0);
        set
        {
            if (value < 0) return;
            PlayerPrefs.SetInt(MoneyKey, value);
            ValueChanged?.Invoke(value);
        }
    }
}
