using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{

    public ItemDef itemDef;

    public SpriteRenderer itemSpriteRenderer;

    public int currentUses;
    bool isBroken;
    public bool doNotReset;

    Vector2 originalPosition;
    bool isOriginal;
    private BoxCollider2D boxCollider;

    public Action<Item> OnTaken;

    void Start()
    {
        // interactable.OnInteract += Interact;
        originalPosition = transform.position;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
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

    public override Item Interact(Item item)
    {
        print("interacting with item: " + itemDef.itemName);

        //if player is holding item, check if combinable, else swap
        if (item)
        {
            //check if items can be combined
            for (int i = 0; i < itemDef.combinableList.Count; i++)
            {
                ItemDef combineItemDef = itemDef.combinableList[i];
                if (combineItemDef == item.itemDef)
                {
                    // check if compatible
                    // in some sort of item map lookup
                    Item newItem = Instantiate(
                        Resources.Load<Item>("Prefabs/Item Prefab"),
                        transform.position, Quaternion.identity
                    );
                    newItem.SetDef(itemDef.combinableListTarget[i]);

                    Hide();
                    item.Hide();
                    newItem.Dropped();
                    return null;
                }
            }

            item.Dropped();
        }

        return this;
    }

    public void Dropped()
    {
        gameObject.layer = 8; //set layer to interactable
        enabled = true;
        doNotReset = false;
        transform.SetParent(null);
        StartCoroutine(DropCoroutine());
    }

    IEnumerator DropCoroutine()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(0f, 0f), UnityEngine.Random.Range(0f, 0f)));
    }

    public void Stored()
    {
        gameObject.layer = 2; //set layer to ignore raycast
        foreach (Transform t in transform)
        {
            //t.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.layer = 2; //set layer to ignore raycast
        foreach (Transform t in transform)
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
        //Drop(); //drop the item gives player feedback that it is broken
    }

    public void Consume()
    {
        if (isOriginal)
        {
            doNotReset = false;
            Hide();
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
        currentUses = itemDef.maxUses;
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
            transform.SetParent(null);
            gameObject.layer = 8; //set layer to interactable        
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }

            transform.position = originalPosition;
            currentUses = itemDef.maxUses;
            isBroken = false;
            itemSpriteRenderer.sprite = itemDef.defaultSprite;
            itemSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}