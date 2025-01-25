using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPile : MonoBehaviour
{
    public ItemStack itemStack;
    public void Initialize(ItemStack item) => Initialize(item.item, item.quantity);
    public void Initialize(Item item, int quantity)
    {
        itemStack = new ItemStack(item, quantity);
    }

    public void ReduceQuantity(int amount)
    {
        itemStack.quantity -= amount;

        if (itemStack.quantity <= 0)
        {
            Destroy(gameObject); // Destroy the object if no items are left
        }
    }

    private void TryPickUpItem()
    {
        int remainingAmount = itemStack.quantity;

        while (remainingAmount > 0)
        {
            int itemsToAdd = Mathf.Min(remainingAmount, Inventory.Instance.maxStackSize);

            if (Inventory.Instance.AddItem(itemStack.item, itemsToAdd))
            {
                remainingAmount -= itemsToAdd;
            }
            else
            {
                Debug.LogWarning("Inventory full! Could not pick up all items.");
                break; // Exit the loop if the inventory is full
            }

        }
        if (remainingAmount > 0)
        {
            ReduceQuantity(itemStack.quantity - remainingAmount);
        }
        else
        {
            Destroy(gameObject); // Destroy the ground item if fully picked up
        }
    }

    /*
    public List<ItemStack> items = new List<ItemStack>();

    public void Initialize(List<ItemStack> loot)
    {
        items = loot;
    }
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickUpItem();
        }
    }
}