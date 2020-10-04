using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Item : Interactable
{
    public ItemDef itemDef;

    public SpriteRenderer itemSpriteRenderer;
    public Interactable interactable;

    public int currentUses;
    bool isBroken;
    public bool doNotReset;

    Vector2 originalPosition;
    bool isOriginal;

    public Item(ItemDef itemDef) {
        this.itemDef = itemDef;
    }
    // Start is called before the first frame update
    void Start()
    {
        // interactable.OnInteract += Interact;
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //might move this out of update for optimzation
        itemSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
    }

    private void OnValidate()
    {
        if (itemDef != null)
        {
            currentUses = itemDef.maxUses;
            gameObject.name = itemDef.itemName;

            if (itemSpriteRenderer != null)
            {
                itemSpriteRenderer.sprite = itemDef.defaultSprite;
                itemSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
            }
        }
        //spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        if (gameObject.layer != 8)
        {
            gameObject.layer = 8; //set layer to interactable
        }
        isOriginal = true; //items placed in the world in the editor won't be destroyed on reset
    }

    public void SetDef(ItemDef itemDef)
    {
        this.itemDef = itemDef;
        itemSpriteRenderer.sprite = itemDef.defaultSprite;
        gameObject.name = itemDef.itemName;
        currentUses = itemDef.maxUses;
    }

    public Item Interact(Item item)
    {
        //check if items can be combined
        int i = 0;
        foreach (ItemDef combineItemDef in itemDef.combinableList) {
            if (combineItemDef == item.itemDef ) {
                // check if compatible
                // in some sort of item map lookup
                Item newItem = Instantiate(
                    Resources.Load<Item>("Prefabs/Item Prefab"),
                    transform.position, Quaternion.identity
                );
                newItem.SetDef(itemDef.combinableListTarget[i]);
                item.GetComponent<BounceBehaviour>().Throw(new Vector2(0, 0));

                return newItem;
            }

            i++;
        }

        //if player is holding item swap them
        if (item != null)
        {
            item.Drop();
        }

        return this;
    }

    public void Drop()
    {

    }

    public void Stored()
    {
        gameObject.layer = 2; //set layer to ignore raycast
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void Retrieved()
    {
        transform.SetParent(null);
        gameObject.layer = 8; //set layer to interactable        
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void Deplete(int amt)
    {
        currentUses -= amt;
        currentUses = Mathf.Clamp(currentUses, 0, itemDef.maxUses);

        if (currentUses <= 0)
        {
            Broken();
        }
    }

    void Broken()
    {
        isBroken = true;
        itemSpriteRenderer.sprite = itemDef.damagedSprite;
        Drop(); //drop the item gives player feedback that it is broken
    }

    public void Consume()
    {
        if (isOriginal)
        {
            doNotReset = false;
            Stored();
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    public void Charge(int amt)
    {
        currentUses += amt;
        currentUses = Mathf.Clamp(currentUses, 0, itemDef.maxUses);

        if (currentUses > 0)
        {
            Fix();
        }
    }

    void Fix()
    {
        isBroken = false;
        itemSpriteRenderer.sprite = itemDef.defaultSprite;
    }

    public void ResetState()
    {
        if (doNotReset)
        {
            return;
        }        
        else if (isOriginal)
        {
            gameObject.layer = 8; //set layer to interactable        
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }
            transform.position = originalPosition;
            currentUses = itemDef.maxUses;
            isBroken = false;
            itemSpriteRenderer.sprite = itemDef.defaultSprite;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
