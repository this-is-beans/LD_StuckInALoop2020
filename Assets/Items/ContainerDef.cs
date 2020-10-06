using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ContainerDef : ScriptableObject
{
    public string containerName;
    public string description;
    public Sprite defaultSprite;
    public Sprite openedSprite;

    [FormerlySerializedAs("keyItem")]
    public ItemDef acceptedItem;
    public List<ItemDef> acceptedItems = new List<ItemDef>();

    [FormerlySerializedAs("destroyItem")]
    public ItemDef destructiveItem;
    public List<ItemDef> destructiveItems = new List<ItemDef>();

    public bool consumesItem;
    public bool requiresOutsideActivation;

    public int maxHP;
    public bool isInvincible;
}
