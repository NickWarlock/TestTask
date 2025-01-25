using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEngine.WSA;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventorySlotEmpty;
    [SerializeField] private GameObject inventorySlotRoot;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject deleteButton;
    private bool isInventoryOpen = false;

    private int selectedSlotIndex;

    [SerializeField] List<ItemStack> slots = new List<ItemStack>();

    [SerializeField] public int maxStackSize = 99;
    [SerializeField] int inventorySize = 12;

    public int CurrentSlotCount => slots.Count;

    public List<ItemStack> GetInventory()
    {
        return slots;
    }

    public void SetInventory(List<ItemStack> inventory)
    {
        slots = inventory;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        inventoryUI.SetActive(false);
    }
    /*
    public List<ItemStack> AddItems(List<ItemStack> items)
    {
        List<ItemStack> remainingItems = new List<ItemStack>();
        Debug.Log(remainingItems.Count);
        foreach (var slot in items)
        {

            int remainingQuantity = slot.quantity;

            // Try adding the item to the inventory slot by slot
            for (int i = 0; i < slots.Count; i++)
            {
                var currentSlot = slots[i]; // Get the current slot
                if (currentSlot.item == slot.item || currentSlot.item == null) // Match item or empty slot
                {
                    int spaceAvailable = currentSlot.item == null
                        ? maxStackSize
                        : maxStackSize - currentSlot.quantity;

                    if (spaceAvailable > 0)
                    {
                        int quantityToAdd = Mathf.Min(remainingQuantity, spaceAvailable);

                        // Create a new slot with the updated quantity
                        if (currentSlot.item == null)
                        {
                            slots[i] = new ItemStack(slot.item, quantityToAdd); // Add item to empty slot
                        }
                        else
                        {
                            slots[i] = new ItemStack(currentSlot.item, currentSlot.quantity + quantityToAdd); // Update existing slot
                        }

                        remainingQuantity -= quantityToAdd;

                        if (remainingQuantity <= 0)
                            break; // All items added, no need to check further
                    }
                }
            }

            if (remainingQuantity > 0)
            {
                if (slots.Count<inventorySize)
                {
                    int toAdd = Mathf.Min(amount, maxStackSize);
                    slots.Add(new ItemStack(item, toAdd));
                    Debug.Log($"Added {toAdd}x {item.itemName} to a new slot.");

                    if (amount > maxStackSize)
                    {
                        return AddItem(item, amount - maxStackSize);
                    }
                }
                // Add remaining items to the list of items that couldn't fit
                remainingItems.Add(new ItemStack(slot.item, remainingQuantity));
            }
        }

        return remainingItems; // Return the list of items that could not be added
    }
    */

    public bool AddItem(ItemStack stack) => AddItem(stack.item, stack.quantity);
    public bool AddItem(Item item, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item && slots[i].quantity < maxStackSize)
            {
                int newQuantity = slots[i].quantity + amount;

                if (newQuantity <= maxStackSize)
                {
                    slots[i] = new ItemStack(item, newQuantity);
                    Debug.Log($"Added {amount}x {item.itemName} to existing stack in slot {i}.");
                    return true;
                }
                else
                {
                    int overflow = newQuantity - maxStackSize;
                    slots[i] = new ItemStack(item, maxStackSize);
                    return AddItem(item, overflow);
                }
            }
        }
        if (slots.Count < inventorySize)
        {
            int toAdd = Mathf.Min(amount, maxStackSize);
            slots.Add(new ItemStack(item, toAdd));
            Debug.Log($"Added {toAdd}x {item.itemName} to a new slot.");

            if (amount > maxStackSize)
            {
                return AddItem(item, amount - maxStackSize);
            }

            return true;
        }

        Debug.LogWarning("Inventory full! Cannot add more items.");
        return false;
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(true);
            mainUI.SetActive(false);
            PopulateInventory();
            Time.timeScale = 0f;
        }
        else
        {
            inventoryUI.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            selectedSlotIndex = -1;
            mainUI.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    public bool HasAmmo(ItemAmmo ammoType)
    {
        foreach (ItemStack slot in slots)
        {
            if (slot.item == ammoType)
            {
                return true;
            }
        }
        return false;
    }

    public void ConsumeAmmo(ItemAmmo ammoType)
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].item == ammoType)
            {
                ItemStack slot = slots[i];

                slot.quantity--;

                if (slot.quantity <= 0)
                {
                    slots.RemoveAt(i);
                }

                slots[i] = slot;
                return;
            }
        }
    }

    void PopulateInventory()
    {
        foreach (Transform child in inventorySlotRoot.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotRoot.transform);
            UpdateSlot(slot, slots[i].item, slots[i].quantity, i);
        }
        if (slots.Count < 12)
        {
            for (int i = 0; i < 12 - slots.Count; i++)
            {
                GameObject slot = Instantiate(inventorySlotEmpty, inventorySlotRoot.transform);
            }
        }
    }

    void UpdateSlot(GameObject slot, Item item, int quantity, int id)
    {
        Image icon = slot.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = item.itemIcon;

        TMP_Text quantityText = slot.transform.Find("Quantity").GetComponent<TMP_Text>();
        quantityText.text = quantity > 1 ? quantity.ToString() : "";

        Button button = slot.GetComponent<Button>();
        button.onClick.AddListener(() => OnSlotClicked(id));
    }

    void OnSlotClicked(int id)
    {
        Debug.Log($"Clicked on item: {slots[id].item.name}");
        selectedSlotIndex = id;
        deleteButton.gameObject.SetActive(true);
    }

    public void OnDeleteButtonClicked()
    {
        if (selectedSlotIndex >= 0)
        {
            slots.RemoveAt(selectedSlotIndex);
            PopulateInventory();
            deleteButton.gameObject.SetActive(false);
            selectedSlotIndex = -1;
        }
    }

}

[System.Serializable]
public struct ItemStack
{
    public Item item;
    public int quantity;

    public ItemStack(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public bool IsEmpty => item == null || quantity <= 0;
}