using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPropertyChangeNotifier
{
    event System.Action<object> PropertyOnChanged;
}
