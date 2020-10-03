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
    public Sprite defaultSprite;
    public Sprite openedSprite;

    public LockType lockType;
    public ItemDef keyItem;

    public int maxHP;
    public bool isInvincible;
}
