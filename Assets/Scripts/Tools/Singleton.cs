using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if(Instance == null)
            {
                _isntance = FindObjectOfType<T>();
                if (_isntance == null)
                    throw new MissingComponentException(nameof(T));
            }

            return _isntance;
        }
    }

    private static T _isntance;
}
