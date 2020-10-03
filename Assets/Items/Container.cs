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
        gameObject.layer = 8; //set layer to interactable
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

    void Open()
    {
        isOpen = true;
        spriteRenderer.sprite = containerDef.openedSprite;

        StartCoroutine(ThrowInventory());
    }

    IEnumerator Jiggle()
    {
        transform.Translate(new Vector3(0.02f, 0.02f));
        yield return new WaitForSeconds(0.1f);
        transform.Translate(new Vector3(-0.04f, -0.04f));
        yield return new WaitForSeconds(0.1f);
        transform.Translate(new Vector3(0.02f, 0.02f));
    }

    IEnumerator ThrowInventory()
    {
        //TODO
        foreach (ItemDef i in inventory)
        {
            Item item = Instantiate(Resources.Load<Item>("Prefabs/Item Prefab"), transform.position, Quaternion.identity);
            item.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f)));
            yield return new WaitForSeconds(0.25f);
        }
        //inventory.Clear(); don't need to clear inventory, unless later on the player can choose items to withdraw
        yield return null;
    }

    public void ResetState()
    {
        isOpen = false;
        spriteRenderer.sprite = containerDef.defaultSprite;
    }
}
