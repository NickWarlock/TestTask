using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private Transform collisionCenter;
    [SerializeField] private int health=20;
    [SerializeField] private int maxhealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform player;            // Reference to the player's position
    private bool isPlayerDetected = false;
    private LayerMask collisionMask;

    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float lastAttackTime =0f;

    [SerializeField] private LootTable lootTable;
    [SerializeField] private GameObject itemPilePrefab;

    private void Awake()
    {
        collisionMask = LayerMask.GetMask("Terrain");
    }

    public void TakeDamage(int damage)
    {
        health-=damage;
        hpBar.updateBar(health*1.0f / maxhealth*1.0f);
        if (health <= 0)
        {
            DropLoot();
            Destroy(this.gameObject);
        }
    }

    private void DropLoot()
    {
        if (lootTable == null || itemPilePrefab == null) return;

        // Get dropped loot
        var loot = lootTable.GetDroppedLoot();

        foreach (var item in loot) 
        {
            GameObject itemPile = Instantiate(itemPilePrefab, transform.position, Quaternion.identity);

            ItemPile pileScript = itemPile.GetComponent<ItemPile>();
            pileScript.Initialize(item);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerDetected = true;
            player = collision.transform;
        }

    }

    private Pathfinding pathfinding;
    private List<Node> path;
    private int currentPathIndex;

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    void Update()
    {

        if (isPlayerDetected)
        {

            path = pathfinding.FindPath(transform.position, player.position);
        }
        if (path != null && path.Count > 0)
        {
            Vector2 targetPosition = path[currentPathIndex].worldPosition;
            Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;

            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                    currentPathIndex = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.WakeUp();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackRate)
            {
                PlayerControls playerHealth = collision.gameObject.GetComponent<PlayerControls>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage); // Apply damage
                    lastAttackTime = Time.time; // Update last attack time
                }
            }
        }
    }
}