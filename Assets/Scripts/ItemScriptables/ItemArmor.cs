using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Inventory/Armor")]
public class ItemArmor : Item
{
    public ArmorType armorType;
}

public enum ArmorType
{
    Legs,
    Body,
    Back,
    Head
}
