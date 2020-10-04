using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this could be an interface instead of a component. not sure which is best but either will do
public class Interactable : MonoBehaviour
{
    public Action<Item> OnInteract; //container, item, levers can subscribe to this
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Item Interact(Item item)
    {
        print("shouldn't be here!");

        // OnInteract?.Invoke(item);
        return null;
    }

    private void OnDestroy()
    {
        OnInteract = null;
    }
}
