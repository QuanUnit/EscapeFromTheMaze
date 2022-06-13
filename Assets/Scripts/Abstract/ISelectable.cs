using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    event Action Selected; 
    bool IsSelected { get; }
    void Select();
    void Unselect();
}
