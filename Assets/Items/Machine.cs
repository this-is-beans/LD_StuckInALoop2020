using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Machine : Interactable
{
    public ContainerDef containerDef;
    public ItemDef transmutedItemDef;
    Item storedItem;
    public List<RecipeDef> recipeDefs;
    List<ItemDef> consumedItems;

    public SpriteRenderer spriteRenderer;
    public ParticleSystem damageParticle;

    //public GameObject targetThrowLocation; //this is redundant you could have set throw position with the vector2s I had before
    public Vector2 minThrowRange;
    public Vector2 maxThrowRange;

    public bool doNotReset;
    public int currentHP;

    bool isActive;

    // used in win game
    public bool isBroken;
    public RecipeDef fixingRecipe;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    // Start is called before the first frame update
    void Start()
    {
        if (containerDef == null)
        {
            Debug.LogError("Machine has no def: " + name);
        }

        consumedItems = new List<ItemDef>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (containerDef != null)
        {
            spriteRenderer.sprite = containerDef.defaultSprite;
            currentHP = containerDef.maxHP;
        }

        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
        if (gameObject.layer != 8)
        {
            gameObject.layer = 8; //set layer to interactable
        }
    }

    bool CheckIfAcceptedItem(Item item)
    {
        if (containerDef.acceptedItem == item.itemDef || containerDef.acceptedItems.Contains(item.itemDef))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckIfDestructiveItem(Item item)
    {
        if (containerDef.destructiveItem == item.itemDef || containerDef.destructiveItems.Contains(item.itemDef))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override Item Interact(Item item)
    {
        print("interacting with: " + containerDef.name);
        if (!item)
        {
            print("no item given");
            if (!containerDef.acceptedItem && containerDef.acceptedItems.Count == 0)
            {
                if (!isActive)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }

            if (storedItem)
            {
                Item returnItem = storedItem;
                RetrieveItem();
                Deactivate();
                return returnItem;
            }
        }
        else
        {
            if (CheckIfAcceptedItem(item))
            {
                if (consumedItems.Contains(item.itemDef))
                {
                    print("I have already consumed: " + item.itemDef.itemName);
                    return item;
                }
                else if (containerDef.consumesItem)
                {
                    print("feeding " + containerDef.name + item.itemDef.name);
                    Consume(item);
                    if (!containerDef.requiresOutsideActivation)
                    {
                        Activate();
                    }
                    return null;
                }
                else
                {
                    StoreItem(item);
                    if (!containerDef.requiresOutsideActivation)
                    {
                        Activate();
                    }
                    return null;
                }
            }
            else if (CheckIfDestructiveItem(item))
            {
                //Bash(item.itemDef.damageStrength);
                //item.Deplete(1);
                return item;
            }
            else
            {
                print("You tried to give me an incompatible item, if you wanted to store any item use a Storage object instead.");
                return item;
            }
        }

        return item;
    }

    void Consume(Item item)
    {
        consumedItems.Add(item.itemDef);
        item.Consume();
    }

    public void Activate()
    {
        print("Activated: " + containerDef.name);

        // not sure what this does --blynxy

        isActive = true;
        spriteRenderer.sprite = containerDef.openedSprite;
        OnActivate?.Invoke();

    }

    public void TryRepair()
    {
        if (isBroken)
        {
            if (fixingRecipe)
            {
                bool consumedAllFixingItems = true;
                foreach (ItemDef requiredItem in fixingRecipe.requiredItems)
                {
                    if (!consumedItems.Contains(requiredItem))
                    {
                        consumedAllFixingItems = false;
                        break;
                    }
                }

                if (consumedAllFixingItems) isBroken = false;
            }
        }
    }

    public void TryCrafting()
    {
        foreach (RecipeDef recipeDef in recipeDefs)
        {
            bool consumedAllRequiredItems = true;
            foreach (ItemDef requiredItem in recipeDef.requiredItems)
            {
                if (!consumedItems.Contains(requiredItem))
                {
                    print("missing: " + requiredItem.itemName);
                    consumedAllRequiredItems = false;
                    break;
                }
            }

            if (consumedAllRequiredItems)
            {
                StartCoroutine(MakeItems(recipeDef));
            }
        }
    }

    public void Deactivate()
    {
        print("Deactivated: " + containerDef.name);

        if (isActive)
        {
            isActive = false;
            spriteRenderer.sprite = containerDef.defaultSprite;
            OnDeactivate?.Invoke();
        }
    }

    IEnumerator MakeItems(RecipeDef recipeDef)
    {
        foreach (ItemDef itemDef in recipeDef.returnedItems)
        {
            print("Making item: " + itemDef.name);
            Item item = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position,
                Quaternion.identity);
            yield return new WaitForEndOfFrame();

            item.SetDef(itemDef);
            item.GetComponent<BounceBehaviour>().Throw(new Vector2(Random.Range(minThrowRange.x, maxThrowRange.x), Random.Range(minThrowRange.y, maxThrowRange.y)));
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void Transmute()
    {
        if (storedItem != null)
        {
            storedItem.Consume();
            //storedItem.SetDef(transmutedItemDef);
            storedItem = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position,
                Quaternion.identity);
            storedItem.SetDef(transmutedItemDef);
            StartCoroutine(ThrowInventory());
        }
    }

    public void StoreItem(Item item)
    {
        if (storedItem != null)
        {
            StartCoroutine(ThrowInventory());
        }

        storedItem = item;
        storedItem.transform.SetParent(transform);
        storedItem.Stored();
        storedItem.itemSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        storedItem.transform.localPosition = Vector2.zero;

        if (doNotReset)
        {
            storedItem.doNotReset = true;
        }
        else
        {
            storedItem.doNotReset = false;
        }
    }

    public void RetrieveItem()
    {
        storedItem.Retrieved();
        storedItem.doNotReset = false;
        storedItem = null;
    }

    IEnumerator ThrowInventory()
    {
        if (damageParticle != null)
        {
            if (!damageParticle.isPlaying)
            {
                damageParticle.Play();
            }
        }

        if (storedItem != null)
        {
            storedItem.doNotReset = false;
            storedItem.Retrieved();
            //yield return new WaitForEndOfFrame();
            storedItem.GetComponent<BounceBehaviour>().Throw(new Vector2(Random.Range(minThrowRange.x, maxThrowRange.x), Random.Range(minThrowRange.y, maxThrowRange.y)));
            storedItem = null;
        }

        yield return null;
    }

    public void ResetState()
    {
        if (doNotReset)
        {
            return;
        }
        else
        {
            isActive = false;
            currentHP = containerDef.maxHP;
            spriteRenderer.sprite = containerDef.defaultSprite;
            consumedItems = new List<ItemDef>();
            if (storedItem != null)
            {
                storedItem.Retrieved();
                storedItem = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 size = maxThrowRange - minThrowRange;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        Gizmos.DrawCube(((Vector2)transform.position + (minThrowRange + maxThrowRange * 0.5f)), size);
    }
}