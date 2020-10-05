using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Machine : Interactable {
    public ContainerDef containerDef;
    public ItemDef transmutedItemDef;
    Item storedItem;
    public List<RecipeDef> recipeDefs;
    public List<ItemDef> consumedItems;

    public SpriteRenderer spriteRenderer;
    public ParticleSystem damageParticle;

    public GameObject targetThrowLocation;
    public float minThrowRange;
    public float maxThrowRange;

    public bool doNotReset;
    public int currentHP;

    bool isActive;
    
    // used in win game
    public bool isBroken;
    public RecipeDef fixingRecipe;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnValidate() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (containerDef != null) {
            spriteRenderer.sprite = containerDef.defaultSprite;
            currentHP = containerDef.maxHP;
        }

        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
        if (gameObject.layer != 8) {
            gameObject.layer = 8; //set layer to interactable
        }
    }

    public override Item Interact(Item item) {
        print("interacting with: " + containerDef.name);
        if (!item) {
            print("no item given");
            if (!containerDef.keyItem) {
                if (!isActive) {
                    TryActivate();
                }
                else {
                    TryDeactivate();
                }
            }

            if (storedItem) {
                Item returnItem = storedItem;
                RetrieveItem();
                TryDeactivate();
                return returnItem;
            }
        }
        else {
            if (item.itemDef == containerDef.keyItem) {
                if (isActive) {
                    return item;
                }
            }
            else if (containerDef.consumesItem) {
                print("feeding " + containerDef.name + item.itemDef.name);
                Consume(item);
                TryActivate();
                return null;
            }
            else {
                StoreItem(item);
                //TryActivate();
                return null;
            }
        }

        return item;
    }

    public void Consume(Item item) {
        consumedItems.Add(item.itemDef);
        item.Consume();
    }

    public void TryActivate() {
        print("Activated: " + containerDef.name);
        if (isBroken) {
            if (fixingRecipe) {
                bool consumedAllFixingItems = true;
                foreach (ItemDef requiredItem in fixingRecipe.requiredItems) {
                    if (!consumedItems.Contains(requiredItem)) {
                        consumedAllFixingItems = false;
                        break;
                    }
                }

                if (consumedAllFixingItems) isBroken = false;
            }
        }
        
        foreach (RecipeDef recipeDef in recipeDefs) {
            bool consumedAllRequiredItems = true;
            foreach (ItemDef requiredItem in recipeDef.requiredItems) {
                if (!consumedItems.Contains(requiredItem)) {
                    consumedAllRequiredItems = false;
                    break;
                }
            }

            if (consumedAllRequiredItems) {
                StartCoroutine(MakeItems(recipeDef));
            }
        }

        // not sure what this does --blynxy
        if (!isActive) {
            isActive = true;
            spriteRenderer.sprite = containerDef.openedSprite;
            OnActivate?.Invoke();
        }
    }

    public void TryDeactivate() {
        print("Deactivated: " + containerDef.name);

        if (isActive) {
            isActive = false;
            spriteRenderer.sprite = containerDef.defaultSprite;
            OnDeactivate?.Invoke();
        }
    }

    IEnumerator MakeItems(RecipeDef recipeDef) {
        foreach (ItemDef itemDef in recipeDef.returnedItems) {
            print("Making item: " + itemDef.name);
            Item item = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position,
                Quaternion.identity);
            yield return new WaitForEndOfFrame();

            item.SetDef(itemDef);
            item.GetComponent<BounceBehaviour>().Throw(targetThrowLocation.transform.position * Random.Range(minThrowRange, maxThrowRange));
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void Transmute() {
        if (storedItem != null) {
            storedItem.Consume();
            //storedItem.SetDef(transmutedItemDef);
            storedItem = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position,
                Quaternion.identity);
            storedItem.SetDef(transmutedItemDef);
            StartCoroutine(ThrowInventory());
        }
    }

    public void StoreItem(Item item) {
        if (storedItem != null) {
            StartCoroutine(ThrowInventory());
        }

        storedItem = item;
        storedItem.transform.SetParent(transform);
        storedItem.Stored();
        storedItem.itemSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        storedItem.transform.localPosition = Vector2.zero;
        //TODO: item would be removed from player

        if (doNotReset) {
            storedItem.doNotReset = true;
        }
        else {
            storedItem.doNotReset = false;
        }
    }

    public void RetrieveItem() {
        storedItem.Retrieved();
        storedItem.doNotReset = false;

        //TODO: item would be handed to player
        storedItem = null;
    }

    public void DropInventory() {
        StartCoroutine(ThrowInventory());
    }

    IEnumerator ThrowInventory() {
        if (damageParticle != null) {
            if (!damageParticle.isPlaying) {
                damageParticle.Play();
            }
        }

        if (storedItem != null) {
            storedItem.doNotReset = false;
            storedItem.Retrieved();
            //yield return new WaitForEndOfFrame();
            storedItem.GetComponent<BounceBehaviour>().Throw( targetThrowLocation.transform.position *
                UnityEngine.Random.Range(minThrowRange, maxThrowRange));
            storedItem = null;
        }

        yield return null;
    }

    public void ResetState() {
        if (doNotReset) {
            return;
        }
        else {
            isActive = false;
            currentHP = containerDef.maxHP;
            spriteRenderer.sprite = containerDef.defaultSprite;
            consumedItems = new List<ItemDef>();
            if (storedItem != null) {
                storedItem.Retrieved();
                storedItem = null;
            }
        }
    }
}