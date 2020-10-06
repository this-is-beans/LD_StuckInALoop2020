using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Storage : Interactable
{
    public ContainerDef containerDef;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem damageParticle;
    public Vector2 minThrowRange;
    public Vector2 maxThrowRange;
    public bool doNotReset;

    int currentHP;

    Item storedItem;

    void Start()
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

    void Update()
    {
        
    }

    public override Item Interact(Item item)
    {
        print("interacting with: " + containerDef.name);

        if (!item)
        {
            if (storedItem)
            {
                Item returnItem = storedItem;
                RetrieveItem();
                return returnItem;
            }
        }
        else
        {
            StoreItem(item);
            return null;
        }    

        return item;
    }

    public void StoreItem(Item item)
    {
        if (storedItem != null)
        {
            ThrowInventory();
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

    void ThrowInventory()
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
            storedItem.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(minThrowRange.x, maxThrowRange.x), UnityEngine.Random.Range(minThrowRange.y, maxThrowRange.y)));
            storedItem = null;
        }
    }

    public void ResetState()
    {
        if (doNotReset)
        {
            return;
        }
        else
        {
            currentHP = containerDef.maxHP;
            spriteRenderer.sprite = containerDef.defaultSprite;
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
        Gizmos.DrawCube((minThrowRange + maxThrowRange * 0.5f), size);
    }
}
