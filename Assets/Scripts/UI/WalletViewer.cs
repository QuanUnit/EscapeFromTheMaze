using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WalletViewer : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Wallet _wallet;


    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(Wallet wallet)
    {
        _wallet = wallet;
        _wallet.OnChanged += ChangeHandler;
    }

    private void ChangeHandler(int value)
    {
        _text.text = value.ToString();
    }
}
