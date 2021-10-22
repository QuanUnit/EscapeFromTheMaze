using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    public GameObject[] ContactsGameObjects => contactsGameObjects.ToArray();

    public UnityEvent<Collider2D, Trigger> OnTriggerEnter;
    public UnityEvent<Collider2D, Trigger> OnTriggerExit;
    public UnityEvent<Collider2D, Trigger> OnTriggerStay;

    [SerializeField] private List<GameObject> contactsGameObjects;

    Collider2D ownerCollider;
    private void Awake()
    {
        contactsGameObjects = new List<GameObject>();
        ownerCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (contactsGameObjects.Contains(collision.gameObject) == false && ownerCollider.attachedRigidbody != collision.attachedRigidbody)
            contactsGameObjects.Add(collision.gameObject);

        OnTriggerEnter?.Invoke(collision, this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (contactsGameObjects.Contains(collision.gameObject) == true)
            contactsGameObjects.Remove(collision.gameObject);

        OnTriggerExit?.Invoke(collision, this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (contactsGameObjects.Contains(collision.gameObject) == false && ownerCollider.attachedRigidbody != collision.attachedRigidbody)
            contactsGameObjects.Add(collision.gameObject);

        OnTriggerStay?.Invoke(collision, this);
    }
}
