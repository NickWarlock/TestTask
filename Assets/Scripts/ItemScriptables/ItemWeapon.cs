using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon")]
public class ItemWeapon : Item
{
    public float fireRate=1f;
    public int damage;
    public ItemAmmo ammoType;
    public int magazine = 10;
    public float reloadTime = 1f;
}