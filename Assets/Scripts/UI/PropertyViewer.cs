using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class PropertyViewer : MonoBehaviour
{
    [SerializeField] private string _prefix;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private MonoBehaviour _sourceMonoBehaviour;

    private IPropertyChangeNotifier _source;
    
    private void Awake()
    {
        if(_sourceMonoBehaviour == null) return;
        
        if (_sourceMonoBehaviour.TryGetComponent<IPropertyChangeNotifier>(out var source) == false)
        {
            Debug.LogError("IPropertyChangeNotifier not found");
            return;
        }
        
        Initialize(source);
    }

    public void Initialize(IPropertyChangeNotifier source)
    {
        if (_source != null) _source.PropertyOnChanged -= ChangeHandler;
        _source = source;
        _source.PropertyOnChanged += ChangeHandler;
    }

    private void ChangeHandler(object property)
    {
        _text.text = _prefix + property.ToString();
    }
}
