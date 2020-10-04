using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalToolbox : Interactable
{
    public ContainerDef containerDef;
    Item storedItem;

    public SpriteRenderer spriteRenderer;
    public Interactable interactable;
    public ParticleSystem damageParticle;
    public Vector2 minThrowRange;
    public Vector2 maxThrowRange;
    // Start is called before the first frame update
    void Start()
    {
        
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
            //currentHP = containerDef.maxHP;
        }
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
        if (gameObject.layer != 8)
        {
            gameObject.layer = 8; //set layer to interactable
        }
    }

    public override Item Interact(Item item)
    {
        print("TODO");
        return null;
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

        storedItem.doNotReset = true;
    }

    public void RetrieveItem()
    {
        storedItem.Retrieved();
        storedItem.doNotReset = false;

        //TODO: item would be handed to player
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
            storedItem.Retrieved();
            storedItem.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(minThrowRange.x, maxThrowRange.x), UnityEngine.Random.Range(minThrowRange.y, maxThrowRange.y)));
        }

        yield return null;
    }
}
