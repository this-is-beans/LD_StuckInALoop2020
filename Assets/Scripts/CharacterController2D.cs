using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterController2D : MonoBehaviour {
    // blynxy just copied catboy, cause wtf is this unity stuff
    public SpriteRenderer characterSpriteRenderer;

    
    
    // movement
    [SerializeField]
    private Vector2 moveVec2;
    
    [SerializeField]
    private float speed;

    [SerializeField] private bool enableBounce;
    [SerializeField]
    private float bounceSpeed;
    [SerializeField]
    private float bounceHeight;
    [SerializeField]
    private bool needBounce;
    [SerializeField]
    private bool bounceUp;


    // player hands and interactions

    [SerializeField] 
    private Vector2 interactableArea;
    private Collider2D[] interactables;

    [SerializeField]
    private GameObject hands;
    private Item heldItem;

    
    // UI junk
    private GameObject ui_interactDescription;
    private GameObject ui_interactLabel;
    public Text ui_heldLabel;


    /** space to think
    
 smol space, to think of things
    
    ** end space to think **/

    void Start() {
        ui_interactDescription = GameObject.Find("UI_InteractDescription");
        ui_interactLabel = GameObject.Find("UI_InteractLabel");
        needBounce = true;
    }

    // Update is called once per frame
    void Update() {
        characterSpriteRenderer.sortingOrder = (Mathf.RoundToInt(transform.position.y * 16f) * -1);
        if (heldItem != null)
        {
            heldItem.itemSpriteRenderer.sortingOrder = characterSpriteRenderer.sortingOrder + 1;
        }

        // get nearby interactables
        interactables = Physics2D.OverlapBoxAll(transform.position,
            interactableArea, 0,
            LayerMask.GetMask("Interactable"));

        Interactable targetItem = null;
        if (interactables.Length > 0)
        {
            foreach (BoxCollider2D i in interactables)
            {                
                if (i.TryGetComponent(out Interactable interactable))
                {
                    //print(interactables[0].name);

                    targetItem = interactable;

                    if (targetItem.TryGetComponent(out Item item))
                    {
                        ui_interactDescription.GetComponent<Text>().text = item.itemDef.itemDescription;
                        ui_interactLabel.GetComponent<Text>().text = item.itemDef.itemName;
                    }
                    break;
                }
            }
        }
        else
        {
            ui_interactDescription.GetComponent<Text>().text = "";
            ui_interactLabel.GetComponent<Text>().text = "";
        }

        moveVec2 = new Vector2();
        // movement: left right up down
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            moveVec2 += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            moveVec2 += Vector2.right;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            moveVec2 += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            moveVec2 += Vector2.down;
        }
        
        // action key
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            // do action maybe
            if(targetItem != null) Interact(targetItem);
        }
        
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha0)) {
            // do action maybe
            if (heldItem) {
                DropItem();
            }
        }

        if (enableBounce) {
            // check if moving and not currently bouncing
            if (moveVec2 != Vector2.zero && needBounce) {
                bounceHeight = Random.Range(.4f, 1f);
                needBounce = false;
                bounceUp = true;
            }
            
           // bounce if bouncing
            if(!needBounce) {
                if (bounceUp) {
                    characterSpriteRenderer.transform.localPosition = new Vector3(characterSpriteRenderer.transform.localPosition.x,Math.Min(bounceHeight,
                        characterSpriteRenderer.transform.localPosition.y + Time.deltaTime * (speed+1)),0);
                    if (characterSpriteRenderer.transform.localPosition.y == bounceHeight)
                        bounceUp = false;
                }
                else {
                    characterSpriteRenderer.transform.localPosition = new Vector3(characterSpriteRenderer.transform.localPosition.x,Math.Max(0,
                                    characterSpriteRenderer.transform.localPosition.y - Time.deltaTime * (speed+1)),0);
                }
                
            }
            
            if (characterSpriteRenderer.transform.localPosition.y == 0) {
                needBounce = true;
            }
        }
        
        
        transform.position = new Vector3(
            transform.position.x + moveVec2.x * Time.deltaTime * speed,
            transform.position.y + moveVec2.y * Time.deltaTime * speed);
        
    }

    void Interact(Interactable target) {
        // do some stupid thing with things that are connected

        Item returnedItem = target.Interact(heldItem);
        
        if (returnedItem == null)
        {
            heldItem = null;
            //item was taken, nothing returned
            ui_heldLabel.text = "";
        }
        else if (returnedItem == heldItem)
        {
            //item was not taken
        }
        else if (returnedItem != heldItem)
        {
            //item was taken and new item given
            if (heldItem != null)
            {
                if (heldItem.transform.parent != hands.transform)
                { //someone else adopted the held item
                    heldItem = null;
                }
                else
                {
                    //item was orphaned, in theory
                    DropItem();
                }
            }
            AddItem(returnedItem);
        } 
        



    }

    void DropItem() {
        ui_heldLabel.text = "";
        heldItem.enabled = true;
        heldItem.doNotReset = false;
        heldItem.transform.SetParent(null);
        heldItem.Drop();
        heldItem = null;
    }
    void AddItem(Item item) {

        if (heldItem != null)
        {
            DropItem();
        }

        heldItem = item;
        heldItem.enabled = false;
        heldItem.doNotReset = true;
        ui_heldLabel.text = heldItem.itemDef.itemName;
        heldItem.gameObject.layer = 2; //set layer to ignore raycast
        heldItem.transform.SetParent(hands.transform);
        heldItem.transform.localPosition = Vector3.zero;
        item.itemSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
    }
    
    
}