using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockType
{
    NONE,
    ITEM,
    TRIGGER
}

public class ContainerDef : ScriptableObject
{
    public string containerName;
    public string description;
    public Sprite defaultSprite;
    public Sprite openedSprite;

    public LockType lockType;
    public ItemDef keyItem;
    public bool consumesItem;
    public bool requiresOutsideActivation;
    public ItemDef destroyItem;

    public int maxHP;
    public bool isInvincible;
}
