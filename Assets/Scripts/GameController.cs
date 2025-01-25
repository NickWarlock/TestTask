using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] GameObject gameOverUI;

    [Header("Enemy Spawning Settings")]
    [SerializeField] private List<GameObject> enemies;
    public int enemyCount = 3;
    public int enemyIndex = 0;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    public float spawnClearRadius = 1.5f;
    public float playerAvoidRadius = 3f;
    public float enemyAvoidRadius = 1.5f;
    public LayerMask obstacleLayer;
    public Transform playerTransform;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SummonEnemies();
    }

    public void SaveGame()
    {
        SaveData sd = new SaveData();
        sd.items = Inventory.Instance.GetInventory();
        sd.playerHealth = playerTransform.gameObject.GetComponent<PlayerControls>().GetHP();
        sd.playerPosition = playerTransform.position;
        DataSaver.Instance.SaveGame(sd);
    }

    public void LoadGame()
    {
        SaveData sd = DataSaver.Instance.LoadGame();
        Inventory.Instance.SetInventory(sd.items);
        playerTransform.gameObject.GetComponent<PlayerControls>().LoadInfo(sd.playerHealth, sd.playerPosition);
    }

    private void SummonEnemies()
    {
        List<Vector2> spawnedPositions = new List<Vector2>();

        int spawnedEnemies = 0;

        while (spawnedEnemies < enemyCount)
        {
            // Generate a random position within the spawn area
            Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // Check for obstacles
            if (Physics2D.OverlapCircle(randomPosition, spawnClearRadius, obstacleLayer))
                continue;

            // Check distance from player
            if (playerTransform != null && Vector2.Distance(randomPosition, playerTransform.position) < playerAvoidRadius)
                continue;

            bool tooCloseToOtherEnemies = false;
            foreach (Vector2 position in spawnedPositions)
            {
                if (Vector2.Distance(randomPosition, position) < enemyAvoidRadius)
                {
                    tooCloseToOtherEnemies = true;
                    break;
                }
            }

            if (tooCloseToOtherEnemies)
                continue;



            // Randomly select an enemy prefab
            GameObject enemyPrefab = enemies[enemyIndex];

            // Instantiate the enemy at the valid position
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

            spawnedEnemies++;
        }
    }

    public void TriggerGameOver()
    {
        Time.timeScale = 0;

        gameOverUI.SetActive(true);
    }

}
