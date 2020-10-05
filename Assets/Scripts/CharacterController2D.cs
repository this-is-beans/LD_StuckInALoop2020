using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterController2D : MonoBehaviour {
    // blynxy just copied catboy, cause wtf is this unity stuff
    public SpriteRenderer characterSpriteRenderer;
    public Rigidbody2D rbody;

    // movement
    [SerializeField] private Vector2 moveVec2;

    [SerializeField] private float speed;

    [SerializeField] private bool enableBounce;
    [SerializeField] private float bounceSpeed;
    private float _bounceHeight;
    [SerializeField] private float bounceMaxHeight;
    [SerializeField] private float bounceMinHeight;
    [SerializeField] private bool isBouncing;
    [SerializeField] private bool bounceUp;


    // player hands and interactions

    [SerializeField] private float interactableAreaRadius;
    private Collider2D[] interactables;

    [SerializeField] private GameObject hands;
    private Item heldItem;


    // UI junk
    private Text ui_interactDescription;
    private Text ui_interactLabel;
    public Text ui_heldLabel;

    // Time Reset Stuff
    private bool isFrozen;
    private Vector3 startPosition;


    // ANIMATION

    public Animator animator;

    /** space to think
    
 smol space, to think of things
    
    ** end space to think **/
    void OnDrawGizmosSelected() {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(gameObject.transform.position, gameObject.transform.forward, interactableAreaRadius);
    }

    void Start() {
        animator = gameObject.GetComponent<Animator>();
        rbody = gameObject.GetComponent<Rigidbody2D>();
        ui_interactDescription = GameObject.Find("UI_InteractDescription").GetComponent<Text>();
        ui_interactLabel = GameObject.Find("UI_InteractLabel").GetComponent<Text>();
        ui_heldLabel = GameObject.Find("UI_HeldLabel").GetComponent<Text>();
        ui_heldLabel.text = "";
        startPosition = gameObject.transform.position;
        isBouncing = false;
        isFrozen = false;
        enableBounce = true;
        speed = 5;
        bounceSpeed = (speed <= 0) ? 5 : speed / 2;
        bounceMaxHeight = .2f;
        bounceMinHeight = .05f;
    }

    // Update is called once per frame
    void Update() {
        if (!isFrozen) {
            characterSpriteRenderer.sortingOrder = (Mathf.RoundToInt(transform.position.y * 16f) * -1);
            if (heldItem != null) {
                heldItem.itemSpriteRenderer.sortingOrder = characterSpriteRenderer.sortingOrder + 1;
            }

            // get nearby interactables
            interactables = Physics2D.OverlapCircleAll(
                transform.position,
                interactableAreaRadius,
                LayerMask.GetMask("Interactable"));

            Interactable targetItem = null;
            if (interactables.Length > 0) {
                foreach (Collider2D i in interactables) {
                    if (i.TryGetComponent(out Interactable interactable)) {
                        targetItem = interactable;

                        if (targetItem.TryGetComponent(out Item item)) {
                            ui_interactDescription.text = item.itemDef.itemDescription;
                            ui_interactLabel.text = item.itemDef.itemName;
                        }

                        break;
                    }
                }
            }
            else {
                ui_interactDescription.text = "";
                ui_interactLabel.text = "";
            }


            /***
             * MOVEMENT SECTION
             */
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

            /***
             * INTERACTION SECTION
             */

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if (targetItem) {
                    Interact(targetItem);
                }
                else {
                    if (heldItem) DropItem();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha0)) {
                // do action maybe
                if (heldItem) {
                    DropItem();
                }
            }

            /***
             * ANIMATION SECTION
             */
            if (enableBounce) {
                // if moving & not already bouncing, initiate a random bounce
                if (moveVec2 != Vector2.zero && !isBouncing) {
                    _bounceHeight = Random.Range(bounceMinHeight, bounceMaxHeight);
                    isBouncing = true;
                    bounceUp = true;
                }

                // if bounce initiated, process this frame's bounce
                if (isBouncing) {
                    Vector3 oldVector3 = characterSpriteRenderer.transform.localPosition;
                    // bounce direction up
                    if (bounceUp) {
                        characterSpriteRenderer.transform.localPosition = new Vector3(
                            oldVector3.x,
                            Math.Min(_bounceHeight + .3f, oldVector3.y + Time.deltaTime * bounceSpeed),
                            0);
                        if (characterSpriteRenderer.transform.localPosition.y >= _bounceHeight)
                            bounceUp = false;
                    } // else bounce direction down
                    else {
                        characterSpriteRenderer.transform.localPosition = new Vector3(
                            oldVector3.x,
                            Math.Max(0, oldVector3.y - Time.deltaTime * bounceSpeed),
                            0);
                    }
                }

                if (characterSpriteRenderer.transform.localPosition.y == 0) {
                    isBouncing = false;
                }
            }


            //transform.position = new Vector3(
            //transform.position.x + moveVec2.x * Time.deltaTime * speed,
            //transform.position.y + moveVec2.y * Time.deltaTime * speed);
        }
    }

    private void FixedUpdate() {
        if (isFrozen) {
            rbody.velocity = Vector2.zero;
        }
        else {
            rbody.MovePosition(rbody.position + moveVec2 * speed * Time.deltaTime);
        }
    }

    void Interact(Interactable target) {
        // do some stupid thing with things that are connected

        Item returnedItem = target.Interact(heldItem);

        if (!returnedItem) {
            heldItem = null;
            //item was taken, nothing returned
            ui_heldLabel.text = "";
        }
        else if (returnedItem == heldItem) {
            //item was not taken
        }
        else if (returnedItem != heldItem) {
            //item was taken and new item given
            if (heldItem != null) {
                if (heldItem.transform.parent != hands.transform) {
                    //someone else adopted the held item
                    heldItem = null;
                }
                else {
                    //item was orphaned, in theory
                    DropItem();
                }
            }

            AddItem(returnedItem);
        }
    }

    void DropItem() {
        heldItem.EnableBoxCollider2D();
        ui_heldLabel.text = "";
        heldItem.enabled = true;
        heldItem.doNotReset = false;
        heldItem.transform.SetParent(null);
        heldItem.Drop();
        heldItem = null;
    }

    void AddItem(Item item) {
        if (heldItem != null) {
            DropItem();
        }
        
        heldItem = item;
        heldItem.DisableBoxCollider2D();
        heldItem.enabled = false;
        heldItem.doNotReset = true;
        ui_heldLabel.text = heldItem.itemDef.itemName;
        heldItem.gameObject.layer = 2; //set layer to ignore raycast
        heldItem.transform.SetParent(hands.transform);
        heldItem.transform.localPosition = Vector3.zero;
        item.itemSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 16f) * -1;
    }

    public void Freeze() {
        this.isFrozen = true;
    }

    public void UnFreeze() {
        this.isFrozen = false;
    }

    public void ResetState() {
        gameObject.transform.localPosition = startPosition;
    }
}