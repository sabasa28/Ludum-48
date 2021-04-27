using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player: ")]
    [SerializeField] Vector2 playerInitialPosition = Vector2.zero;
    [SerializeField] Transform playerTransform = null;

    [Header("Enemies: ")]
    [SerializeField] float minEnemySpawnInterval;
    [SerializeField] float maxEnemySpawnInterval;
    [SerializeField] float enemySpawnY;
    [SerializeField] float minEnemySpawnX;
    [SerializeField] float maxEnemySpawnX;

    [SerializeField] int baseEnemyAmount = 3;
    [SerializeField] Transform enemyContainer;
    int enemyAmount;

    [SerializeField] GameObject[] enemyPrefabs = null;
    List<Enemy> enemies;

    void Awake()
    {
        enemyAmount = baseEnemyAmount;

        Enemy.OnDeath += DestroyEnemy;
    }

    void Start()
    {
        SetNewLevel();

        AudioManager.Get().PlayMusic(AudioManager.Songs.Gameplay);
    }

    void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
        enemies.Remove(enemy);

        if (enemies.Count == 0) EndLevel();
    }

    void SetNewLevel()
    {
        playerTransform.position = playerInitialPosition;

        enemies = new List<Enemy>();
        StartCoroutine(SpawnEnemies());
    }

    void EndLevel()
    {
        enemyAmount++;
        SetNewLevel();
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            yield return new WaitForSeconds(Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval));

            Vector2 position = new Vector2(Random.Range(minEnemySpawnX, maxEnemySpawnX), enemySpawnY);
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            Enemy enemy = Instantiate(prefab, position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
            enemy.playerTransform = playerTransform;
            enemies.Add(enemy);
        }
    }
}