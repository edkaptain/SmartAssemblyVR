using System;
using UnityEngine;
using UnityEngine.Events;

public class GhostItem : MonoBehaviour
{
    public UnityEvent onAction; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item")){

            onAction?.Invoke();
            Destroy(gameObject);
        };
    }
}
