﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    NONE,
    KEY,
    SCREWDRIVER,

}

public class ItemDef : ScriptableObject
{
    public string itemName;
    public Sprite defaultSprite;
    public Sprite damagedSprite;

    public ToolType toolType;
    public int maxUses;
}