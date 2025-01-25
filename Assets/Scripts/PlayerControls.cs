using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private Transform collisionCenter;
    [SerializeField] private int health;
    [SerializeField] private int maxhealth;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Animator animator;

    [SerializeField] int buffer;

    public int GetHP()
    {
        return health;
    }

    public void LoadInfo(int hp, Vector3 loc)
    {
        health = hp;
        transform.position = loc;
        hpBar.updateBar(health * 1.0f / maxhealth * 1.0f);
    }

    void FixedUpdate()
    {
        Vector2 moveDir = moveAction.action.ReadValue<Vector2>();

        Vector2 targetPosition = rb.position + moveAction.action.ReadValue<Vector2>() * speed * Time.fixedDeltaTime;

        RaycastHit2D hit = Physics2D.Raycast(collisionCenter.position, moveDir, speed * Time.fixedDeltaTime * buffer, collisionMask);

        if (hit.collider == null)
        {
            rb.MovePosition(targetPosition);
        }
        bool isMoving = moveDir.magnitude > 0;
        animator.SetBool("walking", isMoving);
    }

    //HEALTH

    public void TakeDamage(int damage)
    {
        health -= damage;
        hpBar.updateBar(health * 1.0f / maxhealth * 1.0f);
        if (health <= 0)
        {
            GameController.Instance.TriggerGameOver();
        }
    }

    //INVENTORY
    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        ItemPile groundItem = other.GetComponent<ItemPile>();
        if (groundItem != null)
        {
            TryPickUpItem(groundItem);
        }
    }
    
    private void TryPickUpItem(ItemPile groundItem)
    {
        int remainingAmount = groundItem.itemStack.quantity;

        while (remainingAmount > 0)
        {
            int itemsToAdd = Mathf.Min(remainingAmount, inventory.maxStackSize);

            if (inventory.AddItem(groundItem.itemStack.item, itemsToAdd))
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
            groundItem.ReduceQuantity(groundItem.itemStack.quantity - remainingAmount);
        }
        else
        {
            Destroy(groundItem.gameObject); // Destroy the ground item if fully picked up
        }

        Debug.Log($"Picked up {groundItem.itemStack.quantity - remainingAmount}x {groundItem.itemStack.item.itemName}. Remaining in pile: {remainingAmount}");
    }*/
}
