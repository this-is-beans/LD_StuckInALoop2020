using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Interactable))]
public class Container : MonoBehaviour
{
    public ContainerDef containerDef;
    public List<ItemDef> inventory;

    public SpriteRenderer spriteRenderer;
    public Interactable interactable;
    public ParticleSystem damageParticle;

    public Vector2 minThrowRange;
    public Vector2 maxThrowRange;

    public bool hasLock;
    public bool isOpen;
    public int currentHP;

    public Action OnOpen;

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

    public void Interact(Item item)
    {
        if (isOpen)
        {
            return;
        }

        if (hasLock)
        {
            if (CheckItemKey(item.itemDef))
            {
                Open();
            }
            else
            {
                return;
            }
        }
        else //no lock
        {
            Open();
        }
    }

    bool CheckItemKey(ItemDef itemDef)
    {
        if (containerDef.keyItem == itemDef)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Bash(int dmg)
    {
        if (isOpen || containerDef.isInvincible)
        {
            return;
        }

        currentHP -= dmg;

        currentHP = Mathf.Clamp(currentHP, 0, containerDef.maxHP);

        if (currentHP <= 0)
        {
            ForceOpen();
        }
        else
        {
            if (dmg > 0)
            {
                StartCoroutine(Jiggle());
            }
            //other effects
        }
    }

    public void ForceOpen()
    {
        if (isOpen)
        {
            return;
        }

        StartCoroutine(Jiggle());
        Open();
    }

    void Open()
    {
        isOpen = true;
        spriteRenderer.sprite = containerDef.openedSprite;

        StartCoroutine(ThrowInventory());
    }

    IEnumerator Jiggle()
    {
        transform.Translate(new Vector3(0.1f, 0.1f));
        yield return new WaitForSeconds(0.1f);
        transform.Translate(new Vector3(-0.2f, -0.2f));
        yield return new WaitForSeconds(0.1f);
        transform.Translate(new Vector3(0.1f, 0.1f));
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
        foreach (ItemDef i in inventory)
        {
            Item item = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position, Quaternion.identity);
            yield return new WaitForEndOfFrame();

            item.SetDef(i);
            item.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(minThrowRange.x, maxThrowRange.x), UnityEngine.Random.Range(minThrowRange.y, maxThrowRange.y)));
            yield return new WaitForSeconds(0.15f);
        }

        //inventory.Clear(); don't need to clear inventory, unless later on the player can choose items to withdraw

        yield return null;
    }

    public void ResetState()
    {
        isOpen = false;
        currentHP = containerDef.maxHP;
        spriteRenderer.sprite = containerDef.defaultSprite;
    }
}
