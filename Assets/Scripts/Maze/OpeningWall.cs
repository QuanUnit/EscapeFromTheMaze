using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningWall : MonoBehaviour, IInteractive
{
    public void Interact()
    {
        gameObject.SetActive(false);
    }
}
