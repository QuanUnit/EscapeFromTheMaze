using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningWall : SimpleWall
{
    public event Action OnOpened;

    public void Open()
    {
        gameObject.SetActive(false);
        OnOpened?.Invoke();
    }
}
