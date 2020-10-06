using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action<Item> OnInteract; //container, item, levers can subscribe to this

    public virtual Item Interact(Item item)
    {
        Debug.LogError("You shouldn't be here!");

        // OnInteract?.Invoke(item);
        return null;
    }

    private void OnDestroy()
    {
        OnInteract = null;
    }
}
