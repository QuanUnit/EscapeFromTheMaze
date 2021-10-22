using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    public GameObject[] ContactsGameObjects => contactsGameObjects.ToArray();

    [Header("Trigger")]
    public UnityEvent<Collider2D, Trigger> OnTriggerEnter;
    public UnityEvent<Collider2D, Trigger> OnTriggerStay;
    public UnityEvent<Collider2D, Trigger> OnTriggerExit;
    
    [Header("Collision")]
    public UnityEvent<Collider2D, Trigger> OnCollisionEnter;
    public UnityEvent<Collider2D, Trigger> OnCollisionStay;
    public UnityEvent<Collider2D, Trigger> OnCollisionExit;

    
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (contactsGameObjects.Contains(other.gameObject) == false && ownerCollider.attachedRigidbody != other.collider.attachedRigidbody)
            contactsGameObjects.Add(other.gameObject);
        
        OnCollisionEnter?.Invoke(other.collider, this);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (contactsGameObjects.Contains(other.gameObject) == false && ownerCollider.attachedRigidbody != other.collider.attachedRigidbody)
            contactsGameObjects.Add(other.gameObject);
        
        OnCollisionStay?.Invoke(other.collider, this);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (contactsGameObjects.Contains(other.gameObject) == true)
            contactsGameObjects.Remove(other.gameObject);
        
        OnCollisionExit?.Invoke(other.collider, this);
    }
}
