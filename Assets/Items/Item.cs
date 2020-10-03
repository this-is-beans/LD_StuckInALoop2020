using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Item : MonoBehaviour
{
    public ItemDef itemDef;

    public SpriteRenderer itemSpriteRenderer;
    public Interactable interactable;

    public int currentUses;
    bool isBroken;
    public bool doNotReset;

    Vector2 originalPosition;
    bool isOriginal;
    // Start is called before the first frame update
    void Start()
    {
        interactable.OnInteract += Interact;
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

    public void Interact(Item item)
    {
        //check if items can be combined

        //if player is holding item swap them
        if (item != null)
        {
            item.Drop();
        }
    }

    public void Drop()
    {

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
