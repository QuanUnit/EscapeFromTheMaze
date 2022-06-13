using System.Collections;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private float _deltaTime = 0.04f;

    private Coroutine _coroutine;

    public void Type(string text)
    {
        _textField.text = "";

        if (_coroutine != null) StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        foreach (var ch in text)
        {
            _textField.text += ch;
            yield return new WaitForSeconds(_deltaTime);
        }
    }
}