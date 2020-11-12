using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    public Text ui_interactDescription;
    public Text ui_interactLabel;
    public Text ui_heldLabel;

    void Start()
    {
        var player = FindObjectOfType<CharacterController2D>();
        player.OnReceiveItem += RefreshItemDisplay;
        player.OnRemoveItem += ClearItemDisplay;
        player.OnInteractableProximity += RefreshInteractable;
        player.OnInteractableClear += ClearInteractable;

        //best not to rely on finding an object by name
        //ui_interactDescription = GameObject.Find("UI_InteractDescription").GetComponent<Text>();
        //ui_interactLabel = GameObject.Find("UI_InteractLabel").GetComponent<Text>();

        ui_interactDescription.text = "";
        ui_interactLabel.text = "";
        ui_heldLabel.text = "";
    }

    void Update()
    {

    }

    void RefreshInteractable(Interactable interactable)
    {
        if (interactable.TryGetComponent(out Item item))
        {
            ui_interactDescription.text = item.itemDef.itemDescription;
            ui_interactLabel.text = item.itemDef.itemName;
        }
        else if (interactable.TryGetComponent(out Machine machine))
        {
            ui_interactDescription.text = machine.containerDef.description;
            ui_interactLabel.text = machine.containerDef.containerName;
        }
    }

    void ClearInteractable()
    {
        ui_interactDescription.text = "";
        ui_interactLabel.text = "";
    }

    void RefreshItemDisplay(Item item)
    {
        ui_heldLabel.text = item.itemDef.itemName;
    }

    void ClearItemDisplay()
    {
        ui_heldLabel.text = "";
    }
}
