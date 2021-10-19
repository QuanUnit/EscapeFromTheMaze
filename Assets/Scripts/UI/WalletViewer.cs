using TMPro;
using UnityEngine;

public class WalletViewer : MonoBehaviour
{
    [SerializeField] string _prefix;
    [SerializeField ]private TextMeshProUGUI _text;

    private Wallet _wallet;
    private int _value;

    public void Initialize(Wallet wallet)
    {
        _wallet = wallet;
        _wallet.OnChanged += ChangeHandler;
    }

    private void ChangeHandler(int value)
    {
        _value = value;
        _text.text = _prefix + _value.ToString();
    }
}
