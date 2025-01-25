using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Item : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite itemIcon;
    public ItemType itemType;
}

public enum ItemType
{
    Weapon,
    Armor,
    Ammo
}
