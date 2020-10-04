﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Interactable))]
public class Machine : MonoBehaviour
{
    public ContainerDef containerDef;
    public ItemDef transmutedItemDef;
    Item storedItem;

    public SpriteRenderer spriteRenderer;
    public Interactable interactable;
    public ParticleSystem damageParticle;

    public Vector2 minThrowRange;
    public Vector2 maxThrowRange;
    
    public bool doNotReset;
    public int currentHP;

    bool isActive;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    // Start is called before the first frame update
    void Start()
    {
        interactable.OnInteract += Interact;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
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

    void Interact(Item item)
    {
        if (item == null)
        {
            if (storedItem != null)
            {
                RetrieveItem();
                Deactivate();
            }
        }
        else
        {
            if (item.itemDef == containerDef.keyItem)
            {
                if (isActive)
                {
                    return;
                }

                if (containerDef.consumesItem)
                {
                    item.Consume();
                }
                else
                {
                    StoreItem(item);
                }
                Activate();
            }
        }
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            spriteRenderer.sprite = containerDef.openedSprite;
            OnActivate?.Invoke();
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            spriteRenderer.sprite = containerDef.openedSprite;
            OnDeactivate?.Invoke();
        }
    }

    public void Transmute()
    {
        if (storedItem != null)
        {
            storedItem.Consume();
            //storedItem.SetDef(transmutedItemDef);
            storedItem = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position, Quaternion.identity);            
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
        storedItem.Stored();
        //TODO: item would be removed from player

        if (doNotReset)
        {
            storedItem.doNotReset = true;
        }
    }

    public void RetrieveItem()
    {
        storedItem.Retrieved();
        storedItem.doNotReset = false;

        //TODO: item would be handed to player
        storedItem = null;
    }

    public void DropInventory()
    {
        StartCoroutine(ThrowInventory());
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
            storedItem.Retrieved();
            yield return new WaitForEndOfFrame();
            storedItem.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(minThrowRange.x, maxThrowRange.x), UnityEngine.Random.Range(minThrowRange.y, maxThrowRange.y)));
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
            if (storedItem != null)
            {
                storedItem.Retrieved();
                storedItem = null;
            }
        }
    }
}
