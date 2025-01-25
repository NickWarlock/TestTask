using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<Loot> lootItems = new List<Loot>();

    public List<ItemStack> GetDroppedLoot()
    {
        List<ItemStack> droppedLoot = new List<ItemStack>();

        foreach (Loot loot in lootItems)
        {
            if (Random.value <= loot.dropChance) // Roll for drop chance
            {
                int quantity = Random.Range(loot.minQuantity, loot.maxQuantity + 1);
                droppedLoot.Add(new ItemStack(loot.item, quantity));
            }
        }

        return droppedLoot;
    }
}


[System.Serializable]
public class Loot
{
    public Item item; // The item to drop
    public int minQuantity = 1; // Minimum quantity
    public int maxQuantity = 1; // Maximum quantity
    [Range(0f, 1f)]
    public float dropChance = 1.0f; // Chance to drop (1 = 100%)
}